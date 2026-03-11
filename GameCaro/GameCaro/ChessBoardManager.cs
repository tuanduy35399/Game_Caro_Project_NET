using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

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

    public class ChessBoardManager
    {
        private readonly Panel _chessBoard;
        private readonly List<Player> _players;
        private List<List<Button>> _matrix;
        private bool _isGameOver;

        public event EventHandler<TurnChangedEventArgs> TurnChanged;
        public event EventHandler<GameEndedEventArgs> GameEnded;

        public IReadOnlyList<Player> Players
        {
            get { return _players; }
        }

        public int CurrentPlayerIndex { get; private set; }
        public int MoveCount { get; private set; }

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
            _players = new List<Player>
            {
                new Player(playerXName, Properties.Resources.x),
                new Player(playerOName, Properties.Resources.o)
            };

            CurrentPlayerIndex = 0;
            MoveCount = 0;
        }

        public void DrawChessBoard()
        {
            _chessBoard.Enabled = true;
            _chessBoard.Controls.Clear();
            _matrix = new List<List<Button>>();
            _isGameOver = false;
            MoveCount = 0;
            CurrentPlayerIndex = 0;

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
            if (_isGameOver)
            {
                return;
            }

            var button = sender as Button;
            if (button == null || button.BackgroundImage != null)
            {
                return;
            }

            button.BackgroundImage = _players[CurrentPlayerIndex].Mark;
            MoveCount++;

            if (IsEndGame(button))
            {
                _isGameOver = true;
                _chessBoard.Enabled = false;
                var winner = _players[CurrentPlayerIndex];
                GameEnded?.Invoke(this, new GameEndedEventArgs(winner.Name, CurrentPlayerIndex, MoveCount));
                return;
            }

            if (MoveCount >= Cons.CHESS_BOARD_WIDTH * Cons.CHESS_BOARD_HEIGHT)
            {
                _isGameOver = true;
                _chessBoard.Enabled = false;
                GameEnded?.Invoke(this, new GameEndedEventArgs(null, -1, MoveCount));
                return;
            }

            CurrentPlayerIndex = CurrentPlayerIndex == 0 ? 1 : 0;
            RaiseTurnChanged();
        }

        private void RaiseTurnChanged()
        {
            var current = _players[CurrentPlayerIndex];
            TurnChanged?.Invoke(this, new TurnChangedEventArgs(current.Name, current.Mark, CurrentPlayerIndex));
        }

        private bool IsEndGame(Button btn)
        {
            return CountHorizontal(btn) >= 5 ||
                   CountVertical(btn) >= 5 ||
                   CountPrimaryDiagonal(btn) >= 5 ||
                   CountSecondaryDiagonal(btn) >= 5;
        }

        private Point GetChessPoint(Button btn)
        {
            return (Point)btn.Tag;
        }

        private int CountHorizontal(Button btn)
        {
            Point point = GetChessPoint(btn);
            return CountDirection(point, -1, 0, btn) + CountDirection(point, 1, 0, btn) - 1;
        }

        private int CountVertical(Button btn)
        {
            Point point = GetChessPoint(btn);
            return CountDirection(point, 0, -1, btn) + CountDirection(point, 0, 1, btn) - 1;
        }

        private int CountPrimaryDiagonal(Button btn)
        {
            Point point = GetChessPoint(btn);
            return CountDirection(point, -1, -1, btn) + CountDirection(point, 1, 1, btn) - 1;
        }

        private int CountSecondaryDiagonal(Button btn)
        {
            Point point = GetChessPoint(btn);
            return CountDirection(point, 1, -1, btn) + CountDirection(point, -1, 1, btn) - 1;
        }

        private int CountDirection(Point start, int dx, int dy, Button sourceButton)
        {
            int count = 0;
            int x = start.X;
            int y = start.Y;

            while (x >= 0 && x < Cons.CHESS_BOARD_WIDTH && y >= 0 && y < Cons.CHESS_BOARD_HEIGHT)
            {
                if (_matrix[y][x].BackgroundImage == sourceButton.BackgroundImage)
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