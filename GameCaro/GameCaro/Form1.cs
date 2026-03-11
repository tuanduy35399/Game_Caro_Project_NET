using GameCaro.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace GameCaro
{
    public partial class Form1 : Form
    {
        private MongoService _mongoService;
        private ChessBoardManager _chessBoard;
        private UserModel _loggedInUser;
        private string _playerOneName;
        private string _playerTwoName;
        private DateTime _gameStartedAtUtc;
        private bool _gameFinished;
        private bool _forceClose;

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

            try
            {
                txtBoxPlayerName.ReadOnly = true;
                txtBoxIP.Text = "Guest";
                progressBarOfPlayer.Step = Cons.COOL_DOWN_STEP;
                progressBarOfPlayer.Maximum = Cons.COOL_DOWN_TIME;
                timerCoolDown.Interval = Cons.COOL_DOWN_INTERVAL;

                // Read configuration using ConfigurationManager (more reliable than XDocument)
                string connectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
                string databaseName = ConfigurationManager.AppSettings["MongoDatabaseName"] ?? "CaroData";

                // Validate connection string
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    MessageBox.Show(
                        "MongoDB connection string is missing from App.config. Please add it.",
                        "Configuration Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    _forceClose = true;
                    Close();
                    return;
                }

                // Check for placeholder passwords
                if (connectionString.Contains("<db_password>") || 
                    connectionString.Contains("<password>") ||
                    connectionString.Contains("PASS") ||
                    connectionString.Contains("PASSWORD"))
                {
                    MessageBox.Show(
                        "MongoDB connection string contains placeholder credentials.\n\n" +
                        "Please open App.config and replace the placeholder with your actual " +
                        "MongoDB Atlas credentials (username and password).\n\n" +
                        "You can find your connection string in MongoDB Atlas:\n" +
                        "1. Log in to https://cloud.mongodb.com\n" +
                        "2. Go to Clusters → Connect → Drivers\n" +
                        "3. Copy the connection string",
                        "Invalid Configuration",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    _forceClose = true;
                    Close();
                    return;
                }

                if (string.IsNullOrWhiteSpace(databaseName))
                {
                    databaseName = "CaroData";
                }

                _mongoService = new MongoService(connectionString, databaseName);
                var connectionResult = await _mongoService.TestConnectionAsync();
                if (!connectionResult.Success)
                {
                    MessageBox.Show(
                        "MongoDB connection failed.\n\n" + connectionResult.ErrorMessage,
                        "Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    _forceClose = true;
                    Close();
                    return;
                }

                using (var loginForm = new LoginForm(_mongoService))
                {
                    if (loginForm.ShowDialog(this) != DialogResult.OK || loginForm.LoggedInUser == null)
                    {
                        _forceClose = true;
                        Close();
                        return;
                    }

                    _loggedInUser = loginForm.LoggedInUser;
                }

                _playerOneName = _loggedInUser.Username;
                _playerTwoName = string.IsNullOrWhiteSpace(txtBoxIP.Text) ? "Guest" : txtBoxIP.Text.Trim();
                StartNewGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể khởi tạo chương trình: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _forceClose = true;
                Close();
            }
        }

        private void StartNewGame()
        {
            _playerTwoName = string.IsNullOrWhiteSpace(txtBoxIP.Text) ? "Guest" : txtBoxIP.Text.Trim();
            _gameFinished = false;
            _gameStartedAtUtc = DateTime.UtcNow;

            if (_chessBoard != null)
            {
                _chessBoard.TurnChanged -= ChessBoard_TurnChanged;
                _chessBoard.GameEnded -= ChessBoard_GameEnded;
            }

            _chessBoard = new ChessBoardManager(panelChessBoard, _playerOneName, _playerTwoName);
            _chessBoard.TurnChanged += ChessBoard_TurnChanged;
            _chessBoard.GameEnded += ChessBoard_GameEnded;
            _chessBoard.DrawChessBoard();

            progressBarOfPlayer.Value = 0;
            panelChessBoard.Enabled = true;
            timerCoolDown.Stop();
            timerCoolDown.Start();
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

        private void ChessBoard_TurnChanged(object sender, TurnChangedEventArgs e)
        {
            txtBoxPlayerName.Text = e.CurrentPlayerName;
            pictureBoxMark.Image = e.CurrentPlayerMark;
            progressBarOfPlayer.Value = 0;
            timerCoolDown.Stop();
            timerCoolDown.Start();
        }

        private async void ChessBoard_GameEnded(object sender, GameEndedEventArgs e)
        {
            if (_gameFinished)
            {
                return;
            }

            _gameFinished = true;
            EndBoardOnly();

            string message;
            if (string.IsNullOrWhiteSpace(e.WinnerName))
            {
                message = "Ván đấu kết thúc với kết quả hòa.";
            }
            else
            {
                message = "Người thắng: " + e.WinnerName;
            }

            try
            {
                await SaveMatchAsync(e.WinnerName, "5 quân liên tiếp", e.MoveCount);
                if (!string.IsNullOrWhiteSpace(e.WinnerName))
                {
                    var topUsers = await _mongoService.GetTopScoresAsync(1);
                    if (topUsers.Count > 0 && string.Equals(topUsers[0].Username, e.WinnerName, StringComparison.OrdinalIgnoreCase))
                    {
                        message += "\n\n" + e.WinnerName + " đang dẫn đầu bảng xếp hạng với " + topUsers[0].Score + " điểm.";
                    }
                }
            }
            catch (Exception ex)
            {
                message += "\n\nLưu lịch sử / điểm thất bại: " + ex.Message;
            }

            MessageBox.Show(this, message, "Kết thúc trận", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task SaveMatchAsync(string winnerName, string reason, int moveCount)
        {
            var match = new MatchHistoryModel
            {
                PlayerX = _playerOneName,
                PlayerO = _playerTwoName,
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
            if (progressBarOfPlayer.Value + progressBarOfPlayer.Step <= progressBarOfPlayer.Maximum)
            {
                progressBarOfPlayer.PerformStep();
            }
            else
            {
                progressBarOfPlayer.Value = progressBarOfPlayer.Maximum;
            }

            if (progressBarOfPlayer.Value < progressBarOfPlayer.Maximum)
            {
                return;
            }

            if (_gameFinished)
            {
                return;
            }

            _gameFinished = true;
            EndBoardOnly();

            string loser = txtBoxPlayerName.Text;
            string winner = string.Equals(loser, _playerOneName, StringComparison.OrdinalIgnoreCase)
                ? _playerTwoName
                : _playerOneName;

            try
            {
                await SaveMatchAsync(winner, "Hết thời gian", _chessBoard != null ? _chessBoard.MoveCount : 0);
                MessageBox.Show(this, loser + " đã hết thời gian. Người thắng: " + winner, "Hết giờ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Không thể lưu kết quả hết giờ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnLAN_Click(object sender, EventArgs e)
        {
            _playerTwoName = string.IsNullOrWhiteSpace(txtBoxIP.Text) ? "Guest" : txtBoxIP.Text.Trim();
            MessageBox.Show(this, "Đối thủ hiện tại: " + _playerTwoName, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show(this, "Không thể tải bảng xếp hạng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(this, "Không thể tải lịch sử trận: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hide();
            try
            {
                using (var loginForm = new LoginForm(_mongoService))
                {
                    if (loginForm.ShowDialog(this) != DialogResult.OK || loginForm.LoggedInUser == null)
                    {
                        _forceClose = true;
                        Close();
                        return;
                    }

                    _loggedInUser = loginForm.LoggedInUser;
                    _playerOneName = _loggedInUser.Username;
                    StartNewGame();
                }
            }
            finally
            {
                Show();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_forceClose)
            {
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn thoát?", "Xác nhận", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                e.Cancel = true;
            }
        }
    }
}