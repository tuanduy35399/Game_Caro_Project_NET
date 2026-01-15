using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;



namespace GameCaro

{
    public class ChessBoardManager
    {
        #region Properties
        private Panel ChessBoard
        {
            get;
            set;
        }

        private List<Player> player;
        public List<Player> Player { get => player; set => player = value; }

        private int currentPlayer;
        public int CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }


        private TextBox playerName;
        public TextBox PlayerName { get => playerName; set => playerName = value; }

        private PictureBox playerMark;
        public PictureBox PlayerMark { get => playerMark; set => playerMark = value; }

        private List<List<Button>> matrix;

        #endregion

        #region Initialize
        public ChessBoardManager(Panel chessBoard, TextBox playerName, PictureBox mark)
        {
            this.ChessBoard = chessBoard;
            this.PlayerName = playerName;
            this.PlayerMark = mark;
            this.Player = new List<Player>()
            {
                new Player("Ronaldo", Properties.Resources.o ),
                new Player("Messi", Properties.Resources.x)
            };
            CurrentPlayer = 0; //Mac dinh la 0

            ChangePlayer();
        }
        #endregion


        #region Methods
        public void DrawChessBar()
        {
            matrix = new List<List<Button>>();

            Button oldButton = new Button()
            {
                Width = 0,
                Location = new Point(0, 0)
            };
            for (int i = 0; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                matrix.Add(new List<Button>());
                for (int j = 0; j < Cons.CHESS_BOARD_WIDTH; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Cons.CHESS_WIDTH,
                        Height = Cons.CHESS_HEIGHT,
                        Location = new Point(oldButton.Location.X + oldButton.Width, oldButton.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString()
                    };
                    btn.Click += btn_Click;

                    ChessBoard.Controls.Add(btn);

                    matrix[i].Add(btn);

                    oldButton = btn;
                }
                oldButton.Location = new Point(0, oldButton.Location.Y + Cons.CHESS_HEIGHT);
                oldButton.Width = 0;
                oldButton.Height = 0;
            }
        }
        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.BackgroundImage != null)
            {
                return;
            }

            //btn.BackgroundImage = Image.FromFile(Application.StartupPath + "\\Resources\\x.png");
            Mark(btn);

            ChangePlayer();

            if(IsEndGame(btn))
            {
                EndGame();
            }
        }

        private void EndGame()
        {
            MessageBox.Show("Ket thuc game");
        }

        private bool IsEndGame(Button btn)
        {

            return IsEndHorizontal(btn) || IsEndVertical(btn) || IsEndPrimaryDiagonal(btn) || IsEndSubDiagonal(btn);
        }

        private Point GetChessPoint(Button btn)
        {
            //Point point = new Point();

            int vertical = Convert.ToInt32(btn.Tag);
            int horizontal = matrix[vertical].IndexOf(btn);
             
            return new Point(horizontal, vertical);
        }
        private bool IsEndHorizontal(Button btn)
        {
            Point point = GetChessPoint(btn);
            int cntLeft = 0;
            int cntRight = 0;
            //Dem ben trai
            for (int i = point.X; i >= 0; i--)
            {
                
                if (matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    cntLeft++;
                }
                else break;
            }

            //Dem ben phai
            for (int i = point.X+1; i < Cons.CHESS_BOARD_WIDTH ; i++)
            {

                if (matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    cntRight++;
                }
                else break;
            }
            return cntLeft+ cntRight==5;
        }
        private bool IsEndVertical(Button btn)
        {
            Point point = GetChessPoint(btn);
            int cntTop = 0;
            int cntBottom = 0;
            for (int i = point.Y; i >= 0; i--)
            {

                if (matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    cntTop++;
                }
                else break;
            }

            //Dem ben phai
            for (int i = point.Y + 1; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {

                if (matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    cntBottom++;
                }
                else break;
            }
            return cntTop + cntBottom == 5;
        }
        private bool IsEndPrimaryDiagonal(Button btn)
        {
            Point point = GetChessPoint(btn);
            int cnt1 = 0;
            int cnt2 = 0;
            for (int i = 0; i <= point.Y; i++) //X hay Y deu duoc 
            {
                if (point.X - i < 0 || point.Y - i < 0) break;
                if (matrix[point.Y - i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    cnt1++;
                }
                else break;
            }

            //Dem ben phai
            for (int i = 1; i <= Cons.CHESS_BOARD_WIDTH - point.Y; i++) //X hay Y deu duoc 
            {
                if (point.X + i >= Cons.CHESS_BOARD_WIDTH || point.Y + i >= Cons.CHESS_BOARD_HEIGHT) break;
                if (matrix[point.Y + i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    cnt2++;
                }
                else break;
            }
            return cnt1 + cnt2 == 5;
        }
        private bool IsEndSubDiagonal(Button btn)
        {
            Point point = GetChessPoint(btn);
            int cnt1 = 0;
            int cnt2 = 0;
            for (int i = 0; i <= point.Y; i++) //X hay Y deu duoc 
            {
                if (point.X + i > Cons.CHESS_BOARD_WIDTH || point.Y - i < 0) break;
                if (matrix[point.Y - i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    cnt1++;
                }
                else break;
            }

            //Dem ben phai
            for (int i = 1; i <= Cons.CHESS_BOARD_WIDTH - point.Y; i++) //X hay Y deu duoc 
            {
                if (point.X - i < 0 || point.Y + i >= Cons.CHESS_BOARD_HEIGHT) break;

                if (matrix[point.Y + i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    cnt2++;
                }
                else break;
            }
            return cnt1 + cnt2 == 5;
        }
        private void Mark(Button btn)
        {
            btn.BackgroundImage = Player[CurrentPlayer].Mark;

            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
        }
        private void ChangePlayer()
        {
            PlayerName.Text = Player[CurrentPlayer].Name;

            PlayerMark.Image = Player[CurrentPlayer].Mark;
        }
        #endregion

    }
}
