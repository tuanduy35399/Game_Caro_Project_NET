using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameCaro.Networking
{
    public class LanConnectResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public static LanConnectResult Ok()
        {
            return new LanConnectResult { Success = true };
        }

        public static LanConnectResult Fail(string error)
        {
            return new LanConnectResult { Success = false, ErrorMessage = error };
        }
    }

    public sealed class LanGameSession : IDisposable
    {
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);
        private TcpListener _listener;
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;
        private CancellationTokenSource _receiveCts;
        private Task _receiveTask;
        private int _disconnectedRaised;

        public bool IsConnected
        {
            get { return _client != null && _client.Connected; }
        }

        public bool IsHost { get; private set; }

        public event EventHandler<LanMessage> MessageReceived;
        public event EventHandler<string> Disconnected;

        public async Task<LanConnectResult> HostAsync(int port, int waitTimeoutMs)
        {
            Cleanup();
            IsHost = true;
            _listener = new TcpListener(IPAddress.Any, port);

            try
            {
                _listener.Start();
                var acceptTask = _listener.AcceptTcpClientAsync();
                var timeoutTask = Task.Delay(waitTimeoutMs);
                var completed = await Task.WhenAny(acceptTask, timeoutTask);
                if (completed != acceptTask)
                {
                    return LanConnectResult.Fail("Timeout waiting for client connection.");
                }

                _client = acceptTask.Result;
                InitializeStreamsAndLoop();
                return LanConnectResult.Ok();
            }
            catch (SocketException ex)
            {
                Cleanup();
                return LanConnectResult.Fail(BuildHostSocketError(ex, port));
            }
            catch (Exception ex)
            {
                Cleanup();
                return LanConnectResult.Fail("Không thể tạo server LAN: " + ex.Message);
            }
            finally
            {
                if (_listener != null)
                {
                    _listener.Stop();
                    _listener = null;
                }
            }
        }

        public async Task<LanConnectResult> JoinAsync(string hostIp, int port, int connectTimeoutMs)
        {
            if (string.IsNullOrWhiteSpace(hostIp))
            {
                return LanConnectResult.Fail("Thiếu địa chỉ IP host.");
            }

            Cleanup();
            IsHost = false;
            _client = new TcpClient();

            try
            {
                var connectTask = _client.ConnectAsync(hostIp, port);
                var timeoutTask = Task.Delay(connectTimeoutMs);
                var completed = await Task.WhenAny(connectTask, timeoutTask);
                if (completed != connectTask)
                {
                    Cleanup();
                    return LanConnectResult.Fail(
                        "Kết nối bị timeout. Host có thể chưa mở phòng hoặc firewall đang chặn cổng " + port + ".");
                }

                await connectTask;
                InitializeStreamsAndLoop();
                return LanConnectResult.Ok();
            }
            catch (SocketException ex)
            {
                Cleanup();
                return LanConnectResult.Fail(BuildJoinSocketError(ex, hostIp, port));
            }
            catch (Exception ex)
            {
                Cleanup();
                return LanConnectResult.Fail("Kết nối LAN thất bại: " + ex.Message);
            }
        }

        public async Task SendAsync(LanMessage message)
        {
            if (!IsConnected || message == null || _writer == null)
            {
                return;
            }

            string payload = LanMessageCodec.Serialize(message);
            await _sendLock.WaitAsync();
            try
            {
                await _writer.WriteLineAsync(payload);
                await _writer.FlushAsync();
            }
            catch (Exception ex)
            {
                RaiseDisconnected("Send failed: " + ex.Message);
                Cleanup();
            }
            finally
            {
                _sendLock.Release();
            }
        }

        public async Task StopAsync(string reason)
        {
            try
            {
                if (IsConnected)
                {
                    await SendAsync(LanMessage.Disconnect(reason));
                }
            }
            catch
            {
            }
            finally
            {
                Cleanup();
            }
        }

        public static List<string> GetLocalIPv4Addresses()
        {
            var list = new List<string>();
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                list.AddRange(host.AddressList
                    .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
                    .Select(ip => ip.ToString()));
            }
            catch
            {
            }

            if (list.Count == 0)
            {
                list.Add("127.0.0.1");
            }

            return list.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        private void InitializeStreamsAndLoop()
        {
            if (_client == null)
            {
                throw new InvalidOperationException("TCP client is not initialized.");
            }

            _disconnectedRaised = 0;
            var stream = _client.GetStream();
            _reader = new StreamReader(stream, Encoding.UTF8);
            _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true, NewLine = "\n" };
            _receiveCts = new CancellationTokenSource();
            _receiveTask = Task.Run(() => ReceiveLoopAsync(_receiveCts.Token));
        }

        private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && _reader != null)
                {
                    string line = await _reader.ReadLineAsync();
                    if (line == null)
                    {
                        RaiseDisconnected("Remote side closed the connection.");
                        break;
                    }

                    LanMessage parsed;
                    if (LanMessageCodec.TryParse(line, out parsed))
                    {
                        MessageReceived?.Invoke(this, parsed);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (IOException ex)
            {
                RaiseDisconnected("Connection lost: " + ex.Message);
            }
            catch (Exception ex)
            {
                RaiseDisconnected("Receive error: " + ex.Message);
            }
            finally
            {
                Cleanup();
            }
        }

        private void RaiseDisconnected(string reason)
        {
            if (Interlocked.Exchange(ref _disconnectedRaised, 1) == 1)
            {
                return;
            }

            Disconnected?.Invoke(this, string.IsNullOrWhiteSpace(reason) ? "Disconnected." : reason);
        }

        private void Cleanup()
        {
            try
            {
                if (_receiveCts != null)
                {
                    _receiveCts.Cancel();
                    _receiveCts.Dispose();
                    _receiveCts = null;
                }
            }
            catch
            {
            }

            try
            {
                if (_reader != null)
                {
                    _reader.Dispose();
                    _reader = null;
                }
            }
            catch
            {
            }

            try
            {
                if (_writer != null)
                {
                    _writer.Dispose();
                    _writer = null;
                }
            }
            catch
            {
            }

            try
            {
                if (_client != null)
                {
                    _client.Close();
                    _client = null;
                }
            }
            catch
            {
            }

            try
            {
                if (_listener != null)
                {
                    _listener.Stop();
                    _listener = null;
                }
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            Cleanup();
            _sendLock.Dispose();
        }

        private static string BuildHostSocketError(SocketException ex, int port)
        {
            if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
            {
                return "Không thể host LAN vì port " + port + " đang được ứng dụng khác sử dụng.";
            }

            if (ex.SocketErrorCode == SocketError.AccessDenied)
            {
                return "Không có quyền mở port " + port + ". Hãy chạy app với quyền phù hợp hoặc đổi port.";
            }

            return "Lỗi khi host LAN (port " + port + "): " + ex.Message;
        }

        private static string BuildJoinSocketError(SocketException ex, string hostIp, int port)
        {
            switch (ex.SocketErrorCode)
            {
                case SocketError.ConnectionRefused:
                    return "Host từ chối kết nối tại " + hostIp + ":" + port +
                           ". Hãy kiểm tra host đã mở phòng (Host LAN) chưa.";

                case SocketError.HostNotFound:
                case SocketError.NoData:
                    return "Không tìm thấy host '" + hostIp + "'. Hãy kiểm tra lại IP.";

                case SocketError.NetworkUnreachable:
                case SocketError.NetworkDown:
                    return "Không thể tới mạng LAN hiện tại. Hãy kiểm tra hai máy có cùng mạng nội bộ.";

                case SocketError.TimedOut:
                    return "Kết nối tới " + hostIp + ":" + port + " bị timeout. Có thể firewall đang chặn.";

                default:
                    return "Kết nối tới " + hostIp + ":" + port + " thất bại: " + ex.Message;
            }
        }
    }
}
