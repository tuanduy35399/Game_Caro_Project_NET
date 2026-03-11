using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GameCaro.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameCaro.Models
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public static ServiceResult Ok()
        {
            return new ServiceResult { Success = true };
        }

        public static ServiceResult Fail(string message)
        {
            return new ServiceResult { Success = false, ErrorMessage = message };
        }
    }

    public class AuthResult : ServiceResult
    {
        public UserModel User { get; set; }
    }

    public class MongoService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<UserModel> _users;
        private readonly IMongoCollection<MatchHistoryModel> _matches;
        private bool _indexesEnsured;

        public MongoService(string connectionString, string databaseName)
        {
            ValidateConnectionString(connectionString);
            
            var url = new MongoUrl(connectionString);
            var settings = MongoClientSettings.FromUrl(url);
            
            // MongoDB driver will automatically negotiate the best authentication mechanism
            // Do not force SCRAM-SHA-1 or SCRAM-SHA-256 - let MongoDB Atlas handle it
            
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            settings.ConnectTimeout = TimeSpan.FromSeconds(5);
            settings.RetryReads = true;
            settings.RetryWrites = true;

            var client = new MongoClient(settings);
            _database = client.GetDatabase(databaseName);
            _users = _database.GetCollection<UserModel>("users");
            _matches = _database.GetCollection<MatchHistoryModel>("matches");
        }

        private static void ValidateConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("MongoDB connection string is empty or null.");
            }

            // Detect placeholder passwords
            if (connectionString.Contains("<db_password>") || 
                connectionString.Contains("<password>") ||
                connectionString.Contains("PASS") ||
                connectionString.Contains("PASSWORD"))
            {
                throw new ArgumentException(
                    "MongoDB connection string contains placeholder credentials. " +
                    "Please update App.config with your actual MongoDB Atlas credentials.");
            }

            // Basic format validation
            if (!connectionString.StartsWith("mongodb+srv://") && !connectionString.StartsWith("mongodb://"))
            {
                throw new ArgumentException(
                    "MongoDB connection string must start with 'mongodb://' or 'mongodb+srv://'. " +
                    "Current format appears invalid.");
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
            try
            {
                await _database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
                await EnsureIndexesAsync();
                return ServiceResult.Ok();
            }
            catch (MongoAuthenticationException authEx)
            {
                return ServiceResult.Fail(
                    "MongoDB authentication failed. Please verify your credentials in App.config. " +
                    "Details: " + authEx.Message);
            }
            catch (MongoConnectionException connEx)
            {
                return ServiceResult.Fail(
                    "Cannot connect to MongoDB Atlas. " +
                    "Possible causes: invalid IP whitelist, incorrect connection string, or cluster not running. " +
                    "Details: " + connEx.Message);
            }
            catch (TimeoutException timeoutEx)
            {
                return ServiceResult.Fail(
                    "MongoDB connection timeout. Check your network and IP whitelist in MongoDB Atlas. " +
                    "Details: " + timeoutEx.Message);
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(
                    "MongoDB connection error: " + ex.GetType().Name + " - " + ex.Message);
            }
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
                return ServiceResult.Fail("Tên đăng nhập không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 4)
            {
                return ServiceResult.Fail("Mật khẩu phải có ít nhất 4 ký tự.");
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
            catch (MongoWriteException ex) when (ex.WriteError != null && ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return ServiceResult.Fail("Tên đăng nhập đã tồn tại.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }

        public async Task<AuthResult> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return new AuthResult { Success = false, ErrorMessage = "Vui lòng nhập tên đăng nhập." };
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
                    return new AuthResult { Success = false, ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng." };
                }

                var update = Builders<UserModel>.Update.Set(x => x.LastLoginAtUtc, DateTime.UtcNow);
                await _users.UpdateOneAsync(Builders<UserModel>.Filter.Eq(x => x.Id, user.Id), update);
                user.LastLoginAtUtc = DateTime.UtcNow;

                return new AuthResult { Success = true, User = user };
            }
            catch (Exception ex)
            {
                return new AuthResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            var count = await _users.CountDocumentsAsync(Builders<UserModel>.Filter.Eq(x => x.Username, username.Trim()));
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
            if (!string.IsNullOrWhiteSpace(match.PlayerX)) participants.Add(match.PlayerX.Trim());
            if (!string.IsNullOrWhiteSpace(match.PlayerO)) participants.Add(match.PlayerO.Trim());
            participants = participants.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            foreach (var username in participants)
            {
                var update = Builders<UserModel>.Update.Inc(x => x.TotalGames, 1);
                await _users.UpdateOneAsync(Builders<UserModel>.Filter.Eq(x => x.Username, username), update);
            }

            if (!string.IsNullOrWhiteSpace(match.Winner))
            {
                var scoreUpdate = Builders<UserModel>.Update.Inc(x => x.Score, 1);
                await _users.UpdateOneAsync(Builders<UserModel>.Filter.Eq(x => x.Username, match.Winner.Trim()), scoreUpdate);
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
    }
}