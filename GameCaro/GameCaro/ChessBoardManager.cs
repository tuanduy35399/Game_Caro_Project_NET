using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameCaro.Infrastructure;

namespace GameCaro
{
    public class TurnChangedEventArgs : EventArgs
    {
        public string CurrentPlayerName { get; private set; }
        public Image CurrentPlayerMark { get; private set; }
        public int CurrentPlayerIndex { get; private set; }

        public TurnChangedEventArgs(string currentPlayerName, Image currentPlayerMark, int currentPlayerIndex)
        {
            CurrentPlayerName = currentPlayerName;
            CurrentPlayerMark = currentPlayerMark;
            CurrentPlayerIndex = currentPlayerIndex;
        }
    }

    public class GameEndedEventArgs : EventArgs
    {
        public string WinnerName { get; private set; }
        public int WinnerIndex { get; private set; }
        public int MoveCount { get; private set; }

        public GameEndedEventArgs(string winnerName, int winnerIndex, int moveCount)
        {
            WinnerName = winnerName;
            WinnerIndex = winnerIndex;
            MoveCount = moveCount;
        }
    }

    public class MovePlacedEventArgs : EventArgs
    {
        public int Col { get; private set; }
        public int Row { get; private set; }
        public int PlayerIndex { get; private set; }
        public string PlayerName { get; private set; }
        public int MoveCount { get; private set; }
        public bool IsRemoteMove { get; private set; }

        public MovePlacedEventArgs(int col, int row, int playerIndex, string playerName, int moveCount, bool isRemoteMove)
        {
            Col = col;
            Row = row;
            PlayerIndex = playerIndex;
            PlayerName = playerName;
            MoveCount = moveCount;
            IsRemoteMove = isRemoteMove;
        }
    }

    public class ChessBoardManager
    {
        private readonly Panel _chessBoard;
        private readonly List<Player> _players;
        private List<List<Button>> _matrix;
        private int[,] _boardState;
        private bool _isGameOver;

        public event EventHandler<TurnChangedEventArgs> TurnChanged;
        public event EventHandler<GameEndedEventArgs> GameEnded;
        public event EventHandler<MovePlacedEventArgs> MovePlaced;

        public IReadOnlyList<Player> Players
        {
            get { return _players; }
        }

        public int CurrentPlayerIndex { get; private set; }
        public int MoveCount { get; private set; }
        public bool IsGameOver { get { return _isGameOver; } }

        public string CurrentPlayerName
        {
            get { return _players[CurrentPlayerIndex].Name; }
        }

        public string OpponentPlayerName
        {
            get { return _players[CurrentPlayerIndex == 0 ? 1 : 0].Name; }
        }

        public ChessBoardManager(Panel chessBoard, string playerXName, string playerOName)
        {
            _chessBoard = chessBoard;
            var xMark = TryGetMarkImage(true);
            var oMark = TryGetMarkImage(false);
            _players = new List<Player>
            {
                new Player(playerXName, xMark),
                new Player(playerOName, oMark)
            };

            CurrentPlayerIndex = 0;
            MoveCount = 0;
        }

        public void DrawChessBoard()
        {
            _chessBoard.Enabled = true;
            _chessBoard.Controls.Clear();
            _matrix = new List<List<Button>>();
            _boardState = new int[Cons.CHESS_BOARD_HEIGHT, Cons.CHESS_BOARD_WIDTH];
            _isGameOver = false;
            MoveCount = 0;
            CurrentPlayerIndex = 0;

            for (int row = 0; row < Cons.CHESS_BOARD_HEIGHT; row++)
            {
                for (int col = 0; col < Cons.CHESS_BOARD_WIDTH; col++)
                {
                    _boardState[row, col] = -1;
                }
            }

            Button oldButton = new Button
            {
                Width = 0,
                Height = 0,
                Location = new Point(0, 0)
            };

            for (int row = 0; row < Cons.CHESS_BOARD_HEIGHT; row++)
            {
                _matrix.Add(new List<Button>());

                for (int col = 0; col < Cons.CHESS_BOARD_WIDTH; col++)
                {
                    Button btn = new Button
                    {
                        Width = Cons.CHESS_WIDTH,
                        Height = Cons.CHESS_HEIGHT,
                        Location = new Point(oldButton.Location.X + oldButton.Width, oldButton.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = new Point(col, row)
                    };

                    btn.Click += Btn_Click;
                    _chessBoard.Controls.Add(btn);
                    _matrix[row].Add(btn);
                    oldButton = btn;
                }

                oldButton.Location = new Point(0, oldButton.Location.Y + Cons.CHESS_HEIGHT);
                oldButton.Width = 0;
                oldButton.Height = 0;
            }

            RaiseTurnChanged();
        }

        public void DisableBoard()
        {
            _isGameOver = true;
            _chessBoard.Enabled = false;
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null || _isGameOver)
            {
                return;
            }

            var chessPoint = (Point)button.Tag;
            string _;
            TryPlaceMove(chessPoint.X, chessPoint.Y, CurrentPlayerIndex, false, out _);
        }

        public bool TryPlaceMove(int col, int row, int playerIndex, bool isRemoteMove, out string error)
        {
            error = null;

            if (_isGameOver)
            {
                error = "The game has already ended.";
                return false;
            }

            if (col < 0 || col >= Cons.CHESS_BOARD_WIDTH || row < 0 || row >= Cons.CHESS_BOARD_HEIGHT)
            {
                error = "Move is out of board bounds.";
                return false;
            }

            if (playerIndex != CurrentPlayerIndex)
            {
                error = "It is not this player's turn.";
                return false;
            }

            if (_boardState[row, col] != -1)
            {
                error = "Cell is already occupied.";
                return false;
            }

            _boardState[row, col] = playerIndex;
            var button = _matrix[row][col];
            var mark = _players[playerIndex].Mark;
            if (mark != null)
            {
                button.BackgroundImage = mark;
            }
            else
            {
                button.Text = playerIndex == 0 ? "X" : "O";
                button.Font = new Font(SystemFonts.DefaultFont.FontFamily, 12F, FontStyle.Bold);
            }
            MoveCount++;
            MovePlaced?.Invoke(this, new MovePlacedEventArgs(col, row, playerIndex, _players[playerIndex].Name, MoveCount, isRemoteMove));

            if (IsEndGame(col, row, playerIndex))
            {
                _isGameOver = true;
                _chessBoard.Enabled = false;
                var winner = _players[playerIndex];
                GameEnded?.Invoke(this, new GameEndedEventArgs(winner.Name, playerIndex, MoveCount));
                return true;
            }

            if (MoveCount >= Cons.CHESS_BOARD_WIDTH * Cons.CHESS_BOARD_HEIGHT)
            {
                _isGameOver = true;
                _chessBoard.Enabled = false;
                GameEnded?.Invoke(this, new GameEndedEventArgs(null, -1, MoveCount));
                return true;
            }

            CurrentPlayerIndex = playerIndex == 0 ? 1 : 0;
            RaiseTurnChanged();
            return true;
        }

        private static Image TryGetMarkImage(bool isX)
        {
            try
            {
                return isX ? Properties.Resources.x : Properties.Resources.o;
            }
            catch (Exception ex)
            {
                AppLogger.Error(
                    "ChessBoardManager.TryGetMarkImage",
                    "Unable to load embedded mark image '" + (isX ? "x" : "o") + "'. Fallback to text marker.",
                    ex);
                return null;
            }
        }

        private void RaiseTurnChanged()
        {
            var current = _players[CurrentPlayerIndex];
            TurnChanged?.Invoke(this, new TurnChangedEventArgs(current.Name, current.Mark, CurrentPlayerIndex));
        }

        private bool IsEndGame(int col, int row, int playerIndex)
        {
            return CountHorizontal(col, row, playerIndex) >= 5 ||
                   CountVertical(col, row, playerIndex) >= 5 ||
                   CountPrimaryDiagonal(col, row, playerIndex) >= 5 ||
                   CountSecondaryDiagonal(col, row, playerIndex) >= 5;
        }

        private int CountHorizontal(int col, int row, int playerIndex)
        {
            return CountDirection(col, row, -1, 0, playerIndex) + CountDirection(col, row, 1, 0, playerIndex) - 1;
        }

        private int CountVertical(int col, int row, int playerIndex)
        {
            return CountDirection(col, row, 0, -1, playerIndex) + CountDirection(col, row, 0, 1, playerIndex) - 1;
        }

        private int CountPrimaryDiagonal(int col, int row, int playerIndex)
        {
            return CountDirection(col, row, -1, -1, playerIndex) + CountDirection(col, row, 1, 1, playerIndex) - 1;
        }

        private int CountSecondaryDiagonal(int col, int row, int playerIndex)
        {
            return CountDirection(col, row, 1, -1, playerIndex) + CountDirection(col, row, -1, 1, playerIndex) - 1;
        }

        private int CountDirection(int col, int row, int dx, int dy, int playerIndex)
        {
            int count = 0;
            int x = col;
            int y = row;

            while (x >= 0 && x < Cons.CHESS_BOARD_WIDTH && y >= 0 && y < Cons.CHESS_BOARD_HEIGHT)
            {
                if (_boardState[y, x] == playerIndex)
                {
                    count++;
                    x += dx;
                    y += dy;
                }
                else
                {
                    break;
                }
            }

            return count;
        }
    }
}
