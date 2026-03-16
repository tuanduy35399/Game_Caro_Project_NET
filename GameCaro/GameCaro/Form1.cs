using GameCaro.Models;
using GameCaro.Networking;
using GameCaro.Infrastructure;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro
{
    public partial class Form1 : Form
    {
        private MongoService _mongoService;
        private ChessBoardManager _chessBoard;
        private UserModel _loggedInUser;
        private LanGameSession _lanSession;

        private string _playerXName;
        private string _playerOName;
        private DateTime _gameStartedAtUtc;
        private bool _gameFinished;
        private bool _forceClose;
        private bool _isClosing;
        private bool _isLastMoveRemote;

        private bool _isOnlineMode;
        private bool _isLanConnected;
        private bool _isLanHost;
        private int _localPlayerIndex;
        private string _remotePlayerName;

        private ToolStripMenuItem _hostLanMenuItem;
        private ToolStripMenuItem _disconnectLanMenuItem;
        private ToolStripMenuItem _deleteAccountToolStripMenuItem;
        private ToolStripSeparator _accountSeparator;

        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            InitializeRuntimeUi();

            try
            {
                string connectionString = GetSettingOrEnvironment(
                    "MongoConnectionString",
                    "GAMECARO_MONGO_CONNECTION_STRING",
                    null);
                string databaseName = GetSettingOrEnvironment(
                    "MongoDatabaseName",
                    "GAMECARO_MONGO_DATABASE_NAME",
                    "CaroData");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    MessageBox.Show(
                        "Thieu MongoDB connection string.\n" +
                        "Hay cau hinh App.config key 'MongoConnectionString'\n" +
                        "hoac bien moi truong 'GAMECARO_MONGO_CONNECTION_STRING'.",
                        "Configuration Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    _forceClose = true;
                    Close();
                    return;
                }

                _mongoService = new MongoService(connectionString, databaseName);
                var connectionResult = await _mongoService.TestConnectionAsync();
                if (!connectionResult.Success)
                {
                    AppLogger.Error(
                        "Form1_Load",
                        "MongoDB connection failed. Code=" + connectionResult.ErrorCode +
                        ". Details=" + (connectionResult.TechnicalDetails ?? "N/A"));

                    MessageBox.Show(
                        connectionResult.ErrorMessage + "\n\n" +
                        "Log ky thuat: " + AppLogger.LogFilePath + "\n\n" +
                        "Neu van loi, hay kiem tra Atlas Network Access, IP whitelist, DNS va firewall.",
                        "Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    _forceClose = true;
                    Close();
                    return;
                }

                if (!ShowLoginDialog())
                {
                    _forceClose = true;
                    Close();
                    return;
                }

                EnterWaitingToStartState();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Form1_Load", "Initialization failed.", ex);
                MessageBox.Show(
                    "Khoi tao ung dung that bai.\n" +
                    "Xem log ky thuat tai: " + AppLogger.LogFilePath,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _forceClose = true;
                Close();
            }
        }

        private static string GetSettingOrEnvironment(string appSettingKey, string envVarKey, string fallbackValue)
        {
            var envValue = Environment.GetEnvironmentVariable(envVarKey);
            if (!string.IsNullOrWhiteSpace(envValue))
            {
                return envValue.Trim();
            }

            var appSettingValue = ConfigurationManager.AppSettings[appSettingKey];
            if (!string.IsNullOrWhiteSpace(appSettingValue))
            {
                return appSettingValue.Trim();
            }

            return fallbackValue;
        }

        private void InitializeRuntimeUi()
        {
            txtBoxPlayerName.ReadOnly = true;
            txtBoxIP.Text = string.Empty;
            txtLanPort.Text = Cons.LAN_PORT.ToString();
            TryLoadAvatarImage();

            progressBarOfPlayer.Step = Cons.COOL_DOWN_STEP;
            progressBarOfPlayer.Maximum = Cons.COOL_DOWN_TIME;
            timerCoolDown.Interval = Cons.COOL_DOWN_INTERVAL;

            _hostLanMenuItem = new ToolStripMenuItem("Host LAN");
            _hostLanMenuItem.Click += hostLanToolStripMenuItem_Click;
            _disconnectLanMenuItem = new ToolStripMenuItem("Disconnect LAN");
            _disconnectLanMenuItem.Click += disconnectLanToolStripMenuItem_Click;
            _deleteAccountToolStripMenuItem = new ToolStripMenuItem("Delete account");
            _deleteAccountToolStripMenuItem.Click += deleteAccountToolStripMenuItem_Click;
            _accountSeparator = new ToolStripSeparator();

            menuToolStripMenuItem.DropDownItems.Insert(1, _hostLanMenuItem);
            menuToolStripMenuItem.DropDownItems.Insert(2, _disconnectLanMenuItem);
            menuToolStripMenuItem.DropDownItems.Insert(7, _accountSeparator);
            menuToolStripMenuItem.DropDownItems.Insert(8, _deleteAccountToolStripMenuItem);

            SetStatus("Status: Ready.");
            UpdateBoardInteraction();
            CenterChessBoard();
        }

        private void TryLoadAvatarImage()
        {
            if (pictureBoxAvatar == null)
            {
                return;
            }

            try
            {
                pictureBoxAvatar.Image = Properties.Resources.avatar;
            }
            catch (Exception ex)
            {
                pictureBoxAvatar.Image = null;
                AppLogger.Error("TryLoadAvatarImage", "Unable to load embedded avatar resource.", ex);
            }
        }

        private bool ShowLoginDialog()
        {
            using (var loginForm = new LoginForm(_mongoService))
            {
                if (loginForm.ShowDialog(this) != DialogResult.OK || loginForm.LoggedInUser == null)
                {
                    return false;
                }

                _loggedInUser = loginForm.LoggedInUser;
                return true;
            }
        }

        private void StartLocalGame()
        {
            _isOnlineMode = false;
            _isLanConnected = false;
            _isLanHost = false;
            _localPlayerIndex = 0;
            _remotePlayerName = "Guest";

            _playerXName = _loggedInUser != null ? _loggedInUser.Username : "Player X";
            _playerOName = "Guest";

            StartGameCore();
            SetStatus("Local mode. Your turn.");
        }

        private void EnterWaitingToStartState()
        {
            timerCoolDown.Stop();
            progressBarOfPlayer.Value = 0;
            txtBoxPlayerName.Text = string.Empty;
            pictureBoxMark.Image = null;
            _gameFinished = true;
            panelChessBoard.Enabled = false;
            panelChessBoard.BackColor = Color.Gainsboro;
            panelChessBoard.Controls.Clear();
            _playerXName = _loggedInUser != null ? _loggedInUser.Username : "Player X";
            _playerOName = "Guest";
            SetStatus("Dang nhap thanh cong. Bam 'Bat dau choi' de vao tran.");
            CenterChessBoard();
            UpdateActionButtons();
        }

        private void StartOnlineGame(string playerXName, string playerOName)
        {
            _playerXName = string.IsNullOrWhiteSpace(playerXName) ? "Player X" : playerXName.Trim();
            _playerOName = string.IsNullOrWhiteSpace(playerOName) ? "Player O" : playerOName.Trim();

            if (_loggedInUser != null &&
                string.Equals(_loggedInUser.Username, _playerXName, StringComparison.OrdinalIgnoreCase))
            {
                _localPlayerIndex = 0;
            }
            else
            {
                _localPlayerIndex = 1;
            }

            StartGameCore();
        }

        private void StartGameCore()
        {
            _gameFinished = false;
            _isLastMoveRemote = false;
            _gameStartedAtUtc = DateTime.UtcNow;

            if (_chessBoard != null)
            {
                _chessBoard.TurnChanged -= ChessBoard_TurnChanged;
                _chessBoard.GameEnded -= ChessBoard_GameEnded;
                _chessBoard.MovePlaced -= ChessBoard_MovePlaced;
            }

            _chessBoard = new ChessBoardManager(panelChessBoard, _playerXName, _playerOName);
            _chessBoard.TurnChanged += ChessBoard_TurnChanged;
            _chessBoard.GameEnded += ChessBoard_GameEnded;
            _chessBoard.MovePlaced += ChessBoard_MovePlaced;
            _chessBoard.DrawChessBoard();
            CenterChessBoard();

            progressBarOfPlayer.Value = 0;
            if (_isOnlineMode)
            {
                timerCoolDown.Stop();
            }
            else
            {
                timerCoolDown.Stop();
                timerCoolDown.Start();
            }

            UpdateBoardInteraction();
        }

        private void EndBoardOnly()
        {
            timerCoolDown.Stop();
            panelChessBoard.Enabled = false;
            if (_chessBoard != null)
            {
                _chessBoard.DisableBoard();
            }
        }

        private void UpdateBoardInteraction()
        {
            bool enabled = !_gameFinished && _chessBoard != null;
            if (_isOnlineMode)
            {
                enabled = enabled && _isLanConnected && _chessBoard.CurrentPlayerIndex == _localPlayerIndex;
            }

            panelChessBoard.Enabled = enabled;
            panelChessBoard.BackColor = enabled ? SystemColors.Control : Color.Gainsboro;
            UpdateActionButtons();
        }

        private void SetStatus(string message)
        {
            if (lblGameStatus != null)
            {
                lblGameStatus.Text = message;

                string lowered = string.IsNullOrWhiteSpace(message)
                    ? string.Empty
                    : message.ToLowerInvariant();
                if (lowered.Contains("failed") || lowered.Contains("error") || lowered.Contains("lost") || lowered.Contains("disconnected"))
                {
                    lblGameStatus.ForeColor = Color.Maroon;
                }
                else if (lowered.Contains("your turn"))
                {
                    lblGameStatus.ForeColor = Color.DarkGreen;
                }
                else
                {
                    lblGameStatus.ForeColor = Color.DarkBlue;
                }
            }
        }

        private void UpdateActionButtons()
        {
            if (btnStartGame == null || btnHostLan == null || btnLAN == null || txtLanPort == null)
            {
                return;
            }

            bool canConfigureLan = !_isLanConnected;
            btnHostLan.Enabled = canConfigureLan;
            btnLAN.Enabled = canConfigureLan;
            txtBoxIP.Enabled = canConfigureLan;
            txtLanPort.Enabled = canConfigureLan;

            if (_isOnlineMode)
            {
                btnStartGame.Enabled = _isLanConnected && _isLanHost && !string.IsNullOrWhiteSpace(_remotePlayerName);
            }
            else
            {
                btnStartGame.Enabled = _loggedInUser != null;
            }
        }

        private void CenterChessBoard()
        {
            if (panelBoardHost == null || panelChessBoard == null)
            {
                return;
            }

            int x = Math.Max(12, (panelBoardHost.ClientSize.Width - panelChessBoard.Width) / 2);
            int y = Math.Max(12, (panelBoardHost.ClientSize.Height - panelChessBoard.Height) / 2);
            panelChessBoard.Location = new Point(x, y);
        }

        private void panelBoardHost_Resize(object sender, EventArgs e)
        {
            CenterChessBoard();
        }

        private async void ChessBoard_MovePlaced(object sender, MovePlacedEventArgs e)
        {
            _isLastMoveRemote = e.IsRemoteMove;
            if (!_isOnlineMode || !_isLanConnected || _lanSession == null)
            {
                return;
            }

            if (e.IsRemoteMove)
            {
                return;
            }

            try
            {
                int nextPlayerIndex = e.PlayerIndex == 0 ? 1 : 0;
                await _lanSession.SendAsync(LanMessage.Move(e.Col, e.Row, e.PlayerIndex, nextPlayerIndex, e.MoveCount));
            }
            catch (Exception ex)
            {
                SetStatus("Status: Failed to send move. " + ex.Message);
                HandleLanDisconnected("Move synchronization failed.");
            }
        }

        private void ChessBoard_TurnChanged(object sender, TurnChangedEventArgs e)
        {
            txtBoxPlayerName.Text = e.CurrentPlayerName;
            pictureBoxMark.Image = e.CurrentPlayerMark;

            if (!_isOnlineMode)
            {
                progressBarOfPlayer.Value = 0;
                timerCoolDown.Stop();
                timerCoolDown.Start();
                SetStatus("Local mode: " + e.CurrentPlayerName + " turn.");
            }
            else
            {
                progressBarOfPlayer.Value = 0;
                string localName = _loggedInUser != null ? _loggedInUser.Username : "You";
                if (e.CurrentPlayerIndex == _localPlayerIndex)
                {
                    SetStatus("Connected. Your turn (" + localName + ").");
                }
                else
                {
                    SetStatus("Connected. Opponent turn. Waiting...");
                }
            }

            UpdateBoardInteraction();
        }

        private async void ChessBoard_GameEnded(object sender, GameEndedEventArgs e)
        {
            if (_gameFinished)
            {
                return;
            }

            _gameFinished = true;
            EndBoardOnly();

            string winnerName = string.IsNullOrWhiteSpace(e.WinnerName) ? null : e.WinnerName;
            string message = winnerName == null
                ? "Game ended in a draw."
                : "Winner: " + winnerName;

            try
            {
                if (!_isOnlineMode || _isLanHost)
                {
                    await SaveMatchAsync(winnerName, "5 in a row", e.MoveCount);
                }

                if (_isOnlineMode && _isLanConnected && _lanSession != null && !_isLastMoveRemote)
                {
                    await _lanSession.SendAsync(LanMessage.GameOver(winnerName, "5 in a row", e.MoveCount));
                }
            }
            catch (Exception ex)
            {
                message += "\n\nSave failed: " + ex.Message;
            }

            MessageBox.Show(this, message, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SetStatus("Status: Game ended.");
            UpdateBoardInteraction();
        }

        private async Task SaveMatchAsync(string winnerName, string reason, int moveCount)
        {
            var match = new MatchHistoryModel
            {
                PlayerX = _playerXName,
                PlayerO = _playerOName,
                Winner = winnerName,
                Reason = reason,
                MoveCount = moveCount,
                StartedAtUtc = _gameStartedAtUtc,
                FinishedAtUtc = DateTime.UtcNow,
                DurationSeconds = (int)Math.Max(0, (DateTime.UtcNow - _gameStartedAtUtc).TotalSeconds)
            };

            await _mongoService.SaveMatchAsync(match);
        }

        private async void timerCoolDown_Tick(object sender, EventArgs e)
        {
            if (_isOnlineMode)
            {
                timerCoolDown.Stop();
                progressBarOfPlayer.Value = 0;
                return;
            }

            if (progressBarOfPlayer.Value + progressBarOfPlayer.Step <= progressBarOfPlayer.Maximum)
            {
                progressBarOfPlayer.PerformStep();
            }
            else
            {
                progressBarOfPlayer.Value = progressBarOfPlayer.Maximum;
            }

            if (progressBarOfPlayer.Value < progressBarOfPlayer.Maximum || _gameFinished)
            {
                return;
            }

            _gameFinished = true;
            EndBoardOnly();

            string loser = txtBoxPlayerName.Text;
            string winner = string.Equals(loser, _playerXName, StringComparison.OrdinalIgnoreCase)
                ? _playerOName
                : _playerXName;

            try
            {
                await SaveMatchAsync(winner, "Timeout", _chessBoard != null ? _chessBoard.MoveCount : 0);
                MessageBox.Show(this, loser + " ran out of time. Winner: " + winner, "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to save timeout result: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SetStatus("Status: Game ended by timeout.");
            UpdateBoardInteraction();
        }

        private async void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_isOnlineMode && _isLanConnected && _lanSession != null)
            {
                await _lanSession.SendAsync(LanMessage.NewGame(_playerXName, _playerOName));
                StartOnlineGame(_playerXName, _playerOName);
                SetStatus("Connected. New online game started.");
                return;
            }

            StartLocalGame();
        }

        private async void btnStartGame_Click(object sender, EventArgs e)
        {
            if (_loggedInUser == null)
            {
                MessageBox.Show(this, "Please login first.", "Game", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_isOnlineMode && _isLanConnected)
            {
                if (_isLanHost)
                {
                    if (string.IsNullOrWhiteSpace(_remotePlayerName))
                    {
                        MessageBox.Show(this, "Chua co doi thu ket noi vao phong LAN.", "LAN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    _playerXName = _loggedInUser.Username;
                    _playerOName = _remotePlayerName;
                    await _lanSession.SendAsync(LanMessage.NewGame(_playerXName, _playerOName));
                    StartOnlineGame(_playerXName, _playerOName);
                    SetStatus("Connected. New online game started.");
                }
                else
                {
                    MessageBox.Show(this, "Dang cho host bat dau tran.", "LAN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                return;
            }

            StartLocalGame();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void btnLAN_Click(object sender, EventArgs e)
        {
            if (_loggedInUser == null)
            {
                MessageBox.Show(this, "Please login first.", "LAN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_isLanConnected)
            {
                MessageBox.Show(this, "LAN is already connected. Use menu -> Disconnect LAN first.", "LAN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string hostIp = txtBoxIP.Text.Trim();
            if (string.IsNullOrWhiteSpace(hostIp))
            {
                MessageBox.Show(
                    this,
                    "Hãy nhập IP LAN của máy host (ví dụ: 192.168.x.x hoặc 10.x.x.x).\n" +
                    "127.0.0.1 chỉ dùng khi test 2 app trên cùng 1 máy.",
                    "Join LAN",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (IsLoopbackInput(hostIp))
            {
                var localOnlyConfirm = MessageBox.Show(
                    this,
                    "Bạn đang nhập localhost (127.0.0.1).\n" +
                    "Giá trị này chỉ dùng để test trên cùng 1 máy.\n\n" +
                    "Bạn có muốn tiếp tục không?",
                    "Join LAN",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (localOnlyConfirm != DialogResult.Yes)
                {
                    return;
                }
            }

            int port;
            if (!TryGetLanPort(out port))
            {
                return;
            }

            await JoinLanAsync(hostIp, port);
        }

        private async void hostLanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_loggedInUser == null)
            {
                MessageBox.Show(this, "Please login first.", "LAN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_isLanConnected)
            {
                MessageBox.Show(this, "LAN is already connected. Disconnect first to host a new room.", "LAN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int port;
            if (!TryGetLanPort(out port))
            {
                return;
            }

            await HostLanAsync(port);
        }

        private async void disconnectLanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await DisconnectLanAsync("Manual disconnect.", true);
            StartLocalGame();
            SetStatus("Status: LAN disconnected. Back to local mode.");
        }

        private async Task HostLanAsync(int port)
        {
            await DisconnectLanAsync("Reset LAN session.", false);

            _lanSession = new LanGameSession();
            _lanSession.MessageReceived += LanSession_MessageReceived;
            _lanSession.Disconnected += LanSession_Disconnected;

            var localIps = string.Join(", ", LanGameSession.GetLocalIPv4Addresses());
            SetStatus("Hosting LAN at port " + port + ". Waiting for opponent...");
            MessageBox.Show(
                this,
                "Đã mở phòng LAN.\n\nIP để máy khác nhập:\n" + localIps + "\n\nPort: " + port,
                "Host LAN",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            var connectResult = await _lanSession.HostAsync(port, Cons.LAN_HOST_WAIT_TIMEOUT_MS);
            if (!connectResult.Success)
            {
                SetStatus("Status: Host failed. " + connectResult.ErrorMessage);
                MessageBox.Show(this, connectResult.ErrorMessage, "Host LAN failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await DisconnectLanAsync("Host failed.", false);
                return;
            }

            _isOnlineMode = true;
            _isLanConnected = true;
            _isLanHost = true;
            _localPlayerIndex = 0;
            _gameFinished = true;

            await _lanSession.SendAsync(LanMessage.Hello(_loggedInUser.Username));
            SetStatus("Connected. Waiting for opponent profile...");
            UpdateBoardInteraction();
        }

        private async Task JoinLanAsync(string hostIp, int port)
        {
            await DisconnectLanAsync("Reset LAN session.", false);

            _lanSession = new LanGameSession();
            _lanSession.MessageReceived += LanSession_MessageReceived;
            _lanSession.Disconnected += LanSession_Disconnected;

            SetStatus("Connecting to " + hostIp + ":" + port + " ...");
            var connectResult = await _lanSession.JoinAsync(hostIp, port, Cons.LAN_CONNECT_TIMEOUT_MS);
            if (!connectResult.Success)
            {
                SetStatus("Status: Join failed. " + connectResult.ErrorMessage);
                MessageBox.Show(
                    this,
                    BuildJoinFailedMessage(hostIp, port, connectResult.ErrorMessage),
                    "Join LAN failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                await DisconnectLanAsync("Join failed.", false);
                return;
            }

            _isOnlineMode = true;
            _isLanConnected = true;
            _isLanHost = false;
            _localPlayerIndex = 1;
            _gameFinished = true;

            await _lanSession.SendAsync(LanMessage.Hello(_loggedInUser.Username));
            SetStatus("Connected to host. Waiting for game start...");
            UpdateBoardInteraction();
        }

        private async Task DisconnectLanAsync(string reason, bool notifyRemote)
        {
            var session = _lanSession;
            _lanSession = null;

            if (session != null)
            {
                session.MessageReceived -= LanSession_MessageReceived;
                session.Disconnected -= LanSession_Disconnected;

                try
                {
                    if (notifyRemote)
                    {
                        await session.StopAsync(reason);
                    }
                    else
                    {
                        session.Dispose();
                    }
                }
                catch
                {
                    session.Dispose();
                }
            }

            _isLanConnected = false;
            _isLanHost = false;
            _isOnlineMode = false;
            _localPlayerIndex = 0;
            _remotePlayerName = null;
            UpdateBoardInteraction();
        }

        private bool TryGetLanPort(out int port)
        {
            port = 0;
            string rawPort = txtLanPort != null ? txtLanPort.Text.Trim() : string.Empty;
            if (string.IsNullOrWhiteSpace(rawPort))
            {
                rawPort = Cons.LAN_PORT.ToString();
            }

            int parsed;
            if (!int.TryParse(rawPort, out parsed) || parsed < 1 || parsed > 65535)
            {
                MessageBox.Show(this, "Port không hợp lệ. Vui lòng nhập số từ 1 đến 65535.", "LAN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            port = parsed;
            return true;
        }

        private static bool IsLoopbackInput(string hostIp)
        {
            if (string.IsNullOrWhiteSpace(hostIp))
            {
                return false;
            }

            IPAddress parsedIp;
            if (!IPAddress.TryParse(hostIp, out parsedIp))
            {
                return string.Equals(hostIp, "localhost", StringComparison.OrdinalIgnoreCase);
            }

            return IPAddress.IsLoopback(parsedIp);
        }

        private static string BuildJoinFailedMessage(string hostIp, int port, string detail)
        {
            return "Không thể kết nối tới máy host.\n\n" +
                   "Host: " + hostIp + ":" + port + "\n" +
                   "Chi tiết: " + (string.IsNullOrWhiteSpace(detail) ? "Không có." : detail) + "\n\n" +
                   "Hãy kiểm tra:\n" +
                   "- Host đã bấm Host LAN và đang mở phòng\n" +
                   "- IP/port đã nhập đúng\n" +
                   "- Hai máy cùng mạng nội bộ\n" +
                   "- Firewall cho phép cổng này";
        }

        private void LanSession_MessageReceived(object sender, LanMessage message)
        {
            if (_isClosing || IsDisposed || Disposing)
            {
                return;
            }

            if (!IsHandleCreated)
            {
                return;
            }

            BeginInvoke(new Action(async () => await HandleLanMessageAsync(message)));
        }

        private void LanSession_Disconnected(object sender, string reason)
        {
            if (_isClosing || IsDisposed || Disposing)
            {
                return;
            }

            if (!IsHandleCreated)
            {
                return;
            }

            BeginInvoke(new Action(() => HandleLanDisconnected(reason)));
        }

        private async Task HandleLanMessageAsync(LanMessage message)
        {
            if (message == null)
            {
                return;
            }

            switch (message.Type)
            {
                case LanMessageType.Hello:
                    _remotePlayerName = message.PlayerName;
                    if (_isLanHost)
                    {
                        _playerXName = _loggedInUser.Username;
                        _playerOName = _remotePlayerName;
                        SetStatus("Connected: " + _remotePlayerName + ". Bam 'Bat dau choi' de vao tran.");
                    }
                    else
                    {
                        SetStatus("Connected to host: " + _remotePlayerName + ". Waiting for host to start...");
                    }
                    UpdateActionButtons();
                    break;

                case LanMessageType.NewGame:
                    StartOnlineGame(message.PlayerX, message.PlayerO);
                    SetStatus(_chessBoard.CurrentPlayerIndex == _localPlayerIndex
                        ? "Connected. Your turn."
                        : "Connected. Opponent turn. Waiting...");
                    break;

                case LanMessageType.Move:
                    if (_chessBoard == null || _gameFinished)
                    {
                        return;
                    }

                    string error;
                    if (!_chessBoard.TryPlaceMove(message.Col, message.Row, message.PlayerIndex, true, out error))
                    {
                        HandleLanDisconnected("State synchronization error: " + error);
                        return;
                    }

                    if (!_chessBoard.IsGameOver && _chessBoard.CurrentPlayerIndex != message.NextPlayerIndex)
                    {
                        HandleLanDisconnected("Turn synchronization mismatch.");
                    }
                    break;

                case LanMessageType.GameOver:
                    if (_gameFinished)
                    {
                        return;
                    }

                    _gameFinished = true;
                    EndBoardOnly();
                    SetStatus("Status: Game ended by remote side.");
                    MessageBox.Show(
                        this,
                        string.IsNullOrWhiteSpace(message.WinnerName)
                            ? "Remote side reported draw."
                            : "Remote side reported winner: " + message.WinnerName,
                        "Game Over",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    break;

                case LanMessageType.Disconnect:
                    HandleLanDisconnected("Opponent disconnected. " + message.Reason);
                    break;
            }
        }

        private void HandleLanDisconnected(string reason)
        {
            if (_isClosing)
            {
                return;
            }

            bool hadConnection = _isLanConnected;
            _isLanConnected = false;
            _isOnlineMode = false;
            _isLanHost = false;
            _localPlayerIndex = 0;
            _gameFinished = true;

            EndBoardOnly();
            SetStatus("Status: LAN disconnected. " + reason);
            UpdateBoardInteraction();

            var session = _lanSession;
            _lanSession = null;
            if (session != null)
            {
                session.MessageReceived -= LanSession_MessageReceived;
                session.Disconnected -= LanSession_Disconnected;
                session.Dispose();
            }

            if (hadConnection)
            {
                MessageBox.Show(this, "Connection lost: " + reason, "LAN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void topScoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var users = await _mongoService.GetTopScoresAsync(Cons.DEFAULT_TOP_LIMIT);
                using (var form = new TopScoresForm(users))
                {
                    form.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to load top scores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void matchHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var matches = await _mongoService.GetRecentMatchesAsync(Cons.DEFAULT_HISTORY_LIMIT);
                using (var form = new MatchHistoryForm(matches))
                {
                    form.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to load match history: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await LogoutAndReloginAsync(false);
        }

        private async void deleteAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_loggedInUser == null)
            {
                MessageBox.Show(this, "No logged in user.", "Delete account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                this,
                "Are you sure you want to delete account '" + _loggedInUser.Username + "'?\nThis action cannot be undone.",
                "Confirm delete account",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
            {
                return;
            }

            Enabled = false;
            Cursor = Cursors.WaitCursor;
            try
            {
                var deleteResult = await _mongoService.DeleteUserAsync(_loggedInUser.Username);
                if (!deleteResult.Success)
                {
                    MessageBox.Show(this, deleteResult.ErrorMessage ?? "Delete account failed.", "Delete account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SetStatus("Status: Delete account failed.");
                    return;
                }

                MessageBox.Show(this, "Account deleted successfully.", "Delete account", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LogoutAndReloginAsync(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Delete account failed: " + ex.Message, "Delete account", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                Enabled = true;
            }
        }

        private async Task LogoutAndReloginAsync(bool fromDeleteAccount)
        {
            await DisconnectLanAsync("User logged out.", true);

            _loggedInUser = null;
            _gameFinished = true;
            timerCoolDown.Stop();
            EndBoardOnly();

            if (!fromDeleteAccount)
            {
                MessageBox.Show(this, "Logged out successfully.", "Logout", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Hide();
            try
            {
                if (!ShowLoginDialog())
                {
                    _forceClose = true;
                    Close();
                    return;
                }

                EnterWaitingToStartState();
            }
            finally
            {
                if (!IsDisposed && !Disposing)
                {
                    Show();
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_forceClose)
            {
                _isClosing = true;
                return;
            }

            if (MessageBox.Show("Are you sure you want to exit?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                e.Cancel = true;
                return;
            }

            _isClosing = true;
            var session = _lanSession;
            _lanSession = null;
            if (session != null)
            {
                session.MessageReceived -= LanSession_MessageReceived;
                session.Disconnected -= LanSession_Disconnected;
                session.Dispose();
            }
        }
    }
}
