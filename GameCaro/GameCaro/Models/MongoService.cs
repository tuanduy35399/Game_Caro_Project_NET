using GameCaro.Infrastructure;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameCaro.Models
{
    public enum MongoErrorCode
    {
        None = 0,
        InvalidConfiguration = 1,
        InvalidConnectionString = 2,
        AuthenticationFailed = 3,
        Timeout = 4,
        DnsResolutionFailed = 5,
        NetworkUnreachable = 6,
        IpNotAllowed = 7,
        ServerUnavailable = 8,
        Unknown = 99
    }

    public class ServiceResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string TechnicalDetails { get; set; }
        public MongoErrorCode ErrorCode { get; set; }

        public static ServiceResult Ok()
        {
            return new ServiceResult
            {
                Success = true,
                ErrorCode = MongoErrorCode.None
            };
        }

        public static ServiceResult Fail(string message, MongoErrorCode errorCode, string technicalDetails)
        {
            return new ServiceResult
            {
                Success = false,
                ErrorMessage = message,
                ErrorCode = errorCode,
                TechnicalDetails = technicalDetails
            };
        }
    }

    public class AuthResult : ServiceResult
    {
        public UserModel User { get; set; }
    }

    public class MongoService
    {
        private static readonly ConcurrentDictionary<string, MongoClient> ClientCache =
            new ConcurrentDictionary<string, MongoClient>(StringComparer.Ordinal);

        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<UserModel> _users;
        private readonly IMongoCollection<MatchHistoryModel> _matches;
        private bool _indexesEnsured;

        public MongoService(string connectionString, string databaseName)
        {
            ValidateConnectionString(connectionString);
            ValidateDatabaseName(databaseName);

            var normalizedConnectionString = connectionString.Trim();
            var normalizedDatabaseName = databaseName.Trim();
            var client = ClientCache.GetOrAdd(normalizedConnectionString, CreateMongoClient);

            _database = client.GetDatabase(normalizedDatabaseName);
            _users = _database.GetCollection<UserModel>("users");
            _matches = _database.GetCollection<MatchHistoryModel>("matches");
        }

        private static MongoClient CreateMongoClient(string connectionString)
        {
            var url = new MongoUrl(connectionString);
            var settings = MongoClientSettings.FromUrl(url);

            settings.ApplicationName = "GameCaro";
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(20);
            settings.ConnectTimeout = TimeSpan.FromSeconds(15);
            settings.SocketTimeout = TimeSpan.FromSeconds(30);
            settings.RetryReads = true;
            settings.RetryWrites = true;
            settings.HeartbeatInterval = TimeSpan.FromSeconds(10);
            settings.HeartbeatTimeout = TimeSpan.FromSeconds(10);
            settings.MaxConnectionIdleTime = TimeSpan.FromMinutes(2);
            settings.WaitQueueTimeout = TimeSpan.FromSeconds(10);

            return new MongoClient(settings);
        }

        private static void ValidateDatabaseName(string databaseName)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException("MongoDB database name is empty.");
            }
        }

        private static void ValidateConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("MongoDB connection string is empty.");
            }

            var normalized = connectionString.Trim();
            if (normalized.IndexOf("<db_password>", StringComparison.OrdinalIgnoreCase) >= 0 ||
                normalized.IndexOf("<password>", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new ArgumentException(
                    "MongoDB connection string contains placeholder credentials.");
            }

            if (!normalized.StartsWith("mongodb://", StringComparison.OrdinalIgnoreCase) &&
                !normalized.StartsWith("mongodb+srv://", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "MongoDB connection string must start with 'mongodb://' or 'mongodb+srv://'.");
            }
        }

        public static string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password ?? string.Empty);
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }

        public async Task<ServiceResult> TestConnectionAsync()
        {
            const int maxAttempts = 3;
            Exception lastException = null;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    await _database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
                    await EnsureIndexesAsync();

                    if (attempt > 1)
                    {
                        AppLogger.Info("MongoService.TestConnectionAsync",
                            "Connection recovered on attempt " + attempt + ".");
                    }

                    return ServiceResult.Ok();
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    var mapped = MapErrorCode(ex);
                    bool isRetryable =
                        mapped == MongoErrorCode.Timeout ||
                        mapped == MongoErrorCode.ServerUnavailable ||
                        mapped == MongoErrorCode.NetworkUnreachable ||
                        mapped == MongoErrorCode.DnsResolutionFailed;

                    AppLogger.Error(
                        "MongoService.TestConnectionAsync",
                        "Connection attempt " + attempt + "/" + maxAttempts + " failed.",
                        ex);

                    if (!isRetryable || attempt == maxAttempts)
                    {
                        break;
                    }

                    await Task.Delay(attempt * 1500);
                }
            }

            return BuildConnectionFailureResult(lastException);
        }

        private async Task EnsureIndexesAsync()
        {
            if (_indexesEnsured)
            {
                return;
            }

            var usernameIndex = new CreateIndexModel<UserModel>(
                Builders<UserModel>.IndexKeys.Ascending(x => x.Username),
                new CreateIndexOptions { Unique = true, Name = "ux_username" });

            var matchFinishedIndex = new CreateIndexModel<MatchHistoryModel>(
                Builders<MatchHistoryModel>.IndexKeys.Descending(x => x.FinishedAtUtc),
                new CreateIndexOptions { Name = "ix_finishedAtUtc" });

            await _users.Indexes.CreateOneAsync(usernameIndex);
            await _matches.Indexes.CreateOneAsync(matchFinishedIndex);
            _indexesEnsured = true;
        }

        public async Task<ServiceResult> RegisterAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return ServiceResult.Fail("Ten dang nhap khong duoc de trong.", MongoErrorCode.InvalidConfiguration, null);
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 4)
            {
                return ServiceResult.Fail("Mat khau phai co it nhat 4 ky tu.", MongoErrorCode.InvalidConfiguration, null);
            }

            username = username.Trim();
            await EnsureIndexesAsync();

            var model = new UserModel
            {
                Username = username,
                PasswordHash = HashPassword(password),
                Score = 0,
                TotalGames = 0,
                CreatedAtUtc = DateTime.UtcNow,
                LastLoginAtUtc = null
            };

            try
            {
                await _users.InsertOneAsync(model);
                return ServiceResult.Ok();
            }
            catch (MongoWriteException ex) when (ex.WriteError != null &&
                                                ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return ServiceResult.Fail("Ten dang nhap da ton tai.", MongoErrorCode.InvalidConfiguration, ex.Message);
            }
            catch (Exception ex)
            {
                AppLogger.Error("MongoService.RegisterAsync", "Register failed.", ex);
                var code = MapErrorCode(ex);
                return ServiceResult.Fail(
                    "Dang ky that bai do loi ket noi du lieu. Vui long thu lai.",
                    code,
                    BuildTechnicalDetails(ex));
            }
        }

        public async Task<AuthResult> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Vui long nhap ten dang nhap.",
                    ErrorCode = MongoErrorCode.InvalidConfiguration
                };
            }

            username = username.Trim();
            var hash = HashPassword(password);

            try
            {
                var filter = Builders<UserModel>.Filter.Eq(x => x.Username, username) &
                             Builders<UserModel>.Filter.Eq(x => x.PasswordHash, hash);

                var user = await _users.Find(filter).FirstOrDefaultAsync();
                if (user == null)
                {
                    return new AuthResult
                    {
                        Success = false,
                        ErrorMessage = "Ten dang nhap hoac mat khau khong dung.",
                        ErrorCode = MongoErrorCode.None
                    };
                }

                var now = DateTime.UtcNow;
                var update = Builders<UserModel>.Update.Set(x => x.LastLoginAtUtc, now);
                await _users.UpdateOneAsync(Builders<UserModel>.Filter.Eq(x => x.Id, user.Id), update);
                user.LastLoginAtUtc = now;

                return new AuthResult
                {
                    Success = true,
                    User = user,
                    ErrorCode = MongoErrorCode.None
                };
            }
            catch (Exception ex)
            {
                AppLogger.Error("MongoService.AuthenticateAsync", "Authentication failed.", ex);
                var code = MapErrorCode(ex);
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Dang nhap that bai do loi ket noi du lieu. Vui long thu lai.",
                    ErrorCode = code,
                    TechnicalDetails = BuildTechnicalDetails(ex)
                };
            }
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            var count = await _users.CountDocumentsAsync(
                Builders<UserModel>.Filter.Eq(x => x.Username, username.Trim()));
            return count > 0;
        }

        public async Task SaveMatchAsync(MatchHistoryModel match)
        {
            if (match == null)
            {
                return;
            }

            await _matches.InsertOneAsync(match);

            var participants = new List<string>();
            if (!string.IsNullOrWhiteSpace(match.PlayerX))
            {
                participants.Add(match.PlayerX.Trim());
            }
            if (!string.IsNullOrWhiteSpace(match.PlayerO))
            {
                participants.Add(match.PlayerO.Trim());
            }
            participants = participants.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            foreach (var username in participants)
            {
                var update = Builders<UserModel>.Update.Inc(x => x.TotalGames, 1);
                await _users.UpdateOneAsync(Builders<UserModel>.Filter.Eq(x => x.Username, username), update);
            }

            if (!string.IsNullOrWhiteSpace(match.Winner))
            {
                var scoreUpdate = Builders<UserModel>.Update.Inc(x => x.Score, 1);
                await _users.UpdateOneAsync(
                    Builders<UserModel>.Filter.Eq(x => x.Username, match.Winner.Trim()),
                    scoreUpdate);
            }
        }

        public async Task<List<UserModel>> GetTopScoresAsync(int limit)
        {
            return await _users.Find(FilterDefinition<UserModel>.Empty)
                .SortByDescending(x => x.Score)
                .ThenBy(x => x.Username)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<List<MatchHistoryModel>> GetRecentMatchesAsync(int limit)
        {
            return await _matches.Find(FilterDefinition<MatchHistoryModel>.Empty)
                .SortByDescending(x => x.FinishedAtUtc)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<ServiceResult> DeleteUserAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return ServiceResult.Fail("Username is required.", MongoErrorCode.InvalidConfiguration, null);
            }

            var normalizedUsername = username.Trim();

            try
            {
                var userDeleteResult = await _users.DeleteOneAsync(
                    Builders<UserModel>.Filter.Eq(x => x.Username, normalizedUsername));

                if (userDeleteResult.DeletedCount == 0)
                {
                    return ServiceResult.Fail("Account not found or already deleted.", MongoErrorCode.None, null);
                }

                var matchFilter =
                    Builders<MatchHistoryModel>.Filter.Eq(x => x.PlayerX, normalizedUsername) |
                    Builders<MatchHistoryModel>.Filter.Eq(x => x.PlayerO, normalizedUsername) |
                    Builders<MatchHistoryModel>.Filter.Eq(x => x.Winner, normalizedUsername);

                await _matches.DeleteManyAsync(matchFilter);
                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                AppLogger.Error("MongoService.DeleteUserAsync", "Delete account failed.", ex);
                var code = MapErrorCode(ex);
                return ServiceResult.Fail(
                    "Failed to delete account because database is unreachable.",
                    code,
                    BuildTechnicalDetails(ex));
            }
        }

        private static ServiceResult BuildConnectionFailureResult(Exception ex)
        {
            var code = MapErrorCode(ex);
            var userMessage = BuildUserMessage(code);
            var technicalDetails = BuildTechnicalDetails(ex);

            AppLogger.Error(
                "MongoService.BuildConnectionFailureResult",
                "MongoDB connection check failed. ErrorCode=" + code + ". Details: " + technicalDetails,
                ex);

            return ServiceResult.Fail(userMessage, code, technicalDetails);
        }

        private static MongoErrorCode MapErrorCode(Exception ex)
        {
            if (ex == null)
            {
                return MongoErrorCode.Unknown;
            }

            if (ex is ArgumentException || ex is MongoConfigurationException || ex is FormatException)
            {
                return MongoErrorCode.InvalidConnectionString;
            }

            if (ex is MongoAuthenticationException)
            {
                return MongoErrorCode.AuthenticationFailed;
            }

            if (ex is TimeoutException || ex is MongoExecutionTimeoutException)
            {
                return MongoErrorCode.Timeout;
            }

            var socketException = FindSocketException(ex);
            if (socketException != null)
            {
                switch (socketException.SocketErrorCode)
                {
                    case SocketError.HostNotFound:
                    case SocketError.NoData:
                        return MongoErrorCode.DnsResolutionFailed;

                    case SocketError.NetworkDown:
                    case SocketError.NetworkUnreachable:
                        return MongoErrorCode.NetworkUnreachable;

                    case SocketError.TimedOut:
                        return MongoErrorCode.Timeout;

                    case SocketError.ConnectionRefused:
                        return MongoErrorCode.ServerUnavailable;
                }
            }

            var message = (ex.Message ?? string.Empty).ToLowerInvariant();
            if (message.Contains("whitelist") || message.Contains("network access") ||
                message.Contains("not allowed from this ip"))
            {
                return MongoErrorCode.IpNotAllowed;
            }

            if (message.Contains("authentication failed") || message.Contains("bad auth"))
            {
                return MongoErrorCode.AuthenticationFailed;
            }

            if (message.Contains("no such host is known") || message.Contains("name or service not known"))
            {
                return MongoErrorCode.DnsResolutionFailed;
            }

            if (message.Contains("timed out") || message.Contains("timeout"))
            {
                return MongoErrorCode.Timeout;
            }

            if (message.Contains("actively refused"))
            {
                return MongoErrorCode.ServerUnavailable;
            }

            if (ex is MongoConnectionException)
            {
                return MongoErrorCode.ServerUnavailable;
            }

            return MongoErrorCode.Unknown;
        }

        private static SocketException FindSocketException(Exception ex)
        {
            var current = ex;
            while (current != null)
            {
                var socket = current as SocketException;
                if (socket != null)
                {
                    return socket;
                }
                current = current.InnerException;
            }

            return null;
        }

        private static string BuildUserMessage(MongoErrorCode code)
        {
            switch (code)
            {
                case MongoErrorCode.InvalidConnectionString:
                case MongoErrorCode.InvalidConfiguration:
                    return "Cau hinh MongoDB khong hop le. Hay kiem tra connection string va ten database.";

                case MongoErrorCode.AuthenticationFailed:
                    return "Khong dang nhap duoc MongoDB. Hay kiem tra username/password.";

                case MongoErrorCode.DnsResolutionFailed:
                    return "Khong phan giai duoc dia chi MongoDB. Hay kiem tra DNS, mang va connection string.";

                case MongoErrorCode.NetworkUnreachable:
                    return "Khong the ket noi mang toi MongoDB. Hay kiem tra internet/VPN/firewall.";

                case MongoErrorCode.IpNotAllowed:
                    return "IP hien tai chua duoc phep truy cap MongoDB Atlas. Hay cap nhat Network Access.";

                case MongoErrorCode.Timeout:
                    return "Ket noi MongoDB bi timeout. Hay kiem tra mang va Atlas Network Access.";

                case MongoErrorCode.ServerUnavailable:
                    return "Khong the ket noi den MongoDB server. Hay kiem tra host/port, firewall va trang thai cluster.";

                default:
                    return "Khong the ket noi MongoDB. Vui long thu lai va kiem tra log ky thuat.";
            }
        }

        private static string BuildTechnicalDetails(Exception ex)
        {
            if (ex == null)
            {
                return "No exception details.";
            }

            var details = new StringBuilder();
            var current = ex;
            var level = 0;
            while (current != null && level < 5)
            {
                details.Append("[").Append(level).Append("] ")
                    .Append(current.GetType().Name)
                    .Append(": ")
                    .Append(current.Message)
                    .Append(" ");
                current = current.InnerException;
                level++;
            }

            return details.ToString().Trim();
        }
    }
}
