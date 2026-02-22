using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ChessBoardManager chessBoard = new ChessBoardManager(panelChessBoard, txtBoxPlayerName, pictureBoxMark);
            chessBoard.EndedGame += ChessBoard_EndedGame;
            chessBoard.PlayerMarked += ChessBoard_PlayerMarked;
            progressBarOfPlayer.Step = Cons.COOL_DOWN_STEP;
            progressBarOfPlayer.Maximum = Cons.COOL_DOWN_TIME;
            progressBarOfPlayer.Value = 0;
            timerCoolDown.Interval = Cons.COOL_DOWN_INTERVEL;

            chessBoard.DrawChessBar();
            timerCoolDown.Start();
        }
        void EndGame()
        {
            timerCoolDown.Stop();
            panelChessBoard.Enabled = false;
            //chessBoard.Enabled = false;
            MessageBox.Show("Kết thúc");
        }

        void ChessBoard_PlayerMarked(object sender, EventArgs e)
        {
            timerCoolDown.Start();
            progressBarOfPlayer.Value = 0;
        }

        void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
        }
        private void timerCoolDown_Tick(object sender, EventArgs e)
        {
            progressBarOfPlayer.PerformStep();
            if(progressBarOfPlayer.Value >= progressBarOfPlayer.Maximum)
            {
                EndGame();
                
            }
        }
    }
}
