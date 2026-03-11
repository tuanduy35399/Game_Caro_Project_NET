namespace GameCaro
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelChessBoard;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBoxAvatar;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PictureBox pictureBoxMark;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar progressBarOfPlayer;
        private System.Windows.Forms.TextBox txtBoxIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLAN;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBoxPlayerName;
        private System.Windows.Forms.Timer timerCoolDown;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topScoresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem matchHistoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelChessBoard = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBoxAvatar = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pictureBoxMark = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.progressBarOfPlayer = new System.Windows.Forms.ProgressBar();
            this.txtBoxIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnLAN = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBoxPlayerName = new System.Windows.Forms.TextBox();
            this.timerCoolDown = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topScoresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.matchHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMark)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelChessBoard
            // 
            this.panelChessBoard.Location = new System.Drawing.Point(12, 34);
            this.panelChessBoard.Name = "panelChessBoard";
            this.panelChessBoard.Size = new System.Drawing.Size(934, 701);
            this.panelChessBoard.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.pictureBoxAvatar);
            this.panel2.Location = new System.Drawing.Point(1007, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 198);
            this.panel2.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe Print", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(42, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 58);
            this.label4.TabIndex = 1;
            this.label4.Text = "Caro";
            // 
            // pictureBoxAvatar
            // 
            this.pictureBoxAvatar.Image = global::GameCaro.Properties.Resources.avatar;
            this.pictureBoxAvatar.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxAvatar.Name = "pictureBoxAvatar";
            this.pictureBoxAvatar.Size = new System.Drawing.Size(200, 199);
            this.pictureBoxAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxAvatar.TabIndex = 0;
            this.pictureBoxAvatar.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.pictureBoxMark);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.progressBarOfPlayer);
            this.panel3.Controls.Add(this.txtBoxIP);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.btnLAN);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.txtBoxPlayerName);
            this.panel3.Location = new System.Drawing.Point(953, 228);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(301, 527);
            this.panel3.TabIndex = 2;
            // 
            // pictureBoxMark
            // 
            this.pictureBoxMark.Location = new System.Drawing.Point(54, 271);
            this.pictureBoxMark.Name = "pictureBoxMark";
            this.pictureBoxMark.Size = new System.Drawing.Size(200, 200);
            this.pictureBoxMark.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxMark.TabIndex = 7;
            this.pictureBoxMark.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 163);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Thời gian đi";
            // 
            // progressBarOfPlayer
            // 
            this.progressBarOfPlayer.Location = new System.Drawing.Point(6, 191);
            this.progressBarOfPlayer.Name = "progressBarOfPlayer";
            this.progressBarOfPlayer.Size = new System.Drawing.Size(289, 23);
            this.progressBarOfPlayer.TabIndex = 5;
            // 
            // txtBoxIP
            // 
            this.txtBoxIP.Location = new System.Drawing.Point(82, 75);
            this.txtBoxIP.Name = "txtBoxIP";
            this.txtBoxIP.Size = new System.Drawing.Size(213, 22);
            this.txtBoxIP.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Đối thủ";
            // 
            // btnLAN
            // 
            this.btnLAN.Location = new System.Drawing.Point(82, 117);
            this.btnLAN.Name = "btnLAN";
            this.btnLAN.Size = new System.Drawing.Size(85, 29);
            this.btnLAN.TabIndex = 2;
            this.btnLAN.Text = "Áp dụng";
            this.btnLAN.UseVisualStyleBackColor = true;
            this.btnLAN.Click += new System.EventHandler(this.btnLAN_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Lượt chơi";
            // 
            // txtBoxPlayerName
            // 
            this.txtBoxPlayerName.Location = new System.Drawing.Point(82, 28);
            this.txtBoxPlayerName.Name = "txtBoxPlayerName";
            this.txtBoxPlayerName.ReadOnly = true;
            this.txtBoxPlayerName.Size = new System.Drawing.Size(213, 22);
            this.txtBoxPlayerName.TabIndex = 0;
            // 
            // timerCoolDown
            // 
            this.timerCoolDown.Tick += new System.EventHandler(this.timerCoolDown_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1260, 28);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGameToolStripMenuItem,
            this.topScoresToolStripMenuItem,
            this.matchHistoryToolStripMenuItem,
            this.logoutToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(60, 24);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // newGameToolStripMenuItem
            // 
            this.newGameToolStripMenuItem.Name = "newGameToolStripMenuItem";
            this.newGameToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.newGameToolStripMenuItem.Text = "New game";
            this.newGameToolStripMenuItem.Click += new System.EventHandler(this.newGameToolStripMenuItem_Click);
            // 
            // topScoresToolStripMenuItem
            // 
            this.topScoresToolStripMenuItem.Name = "topScoresToolStripMenuItem";
            this.topScoresToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.topScoresToolStripMenuItem.Text = "Top scores";
            this.topScoresToolStripMenuItem.Click += new System.EventHandler(this.topScoresToolStripMenuItem_Click);
            // 
            // matchHistoryToolStripMenuItem
            // 
            this.matchHistoryToolStripMenuItem.Name = "matchHistoryToolStripMenuItem";
            this.matchHistoryToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.matchHistoryToolStripMenuItem.Text = "Match history";
            this.matchHistoryToolStripMenuItem.Click += new System.EventHandler(this.matchHistoryToolStripMenuItem_Click);
            // 
            // logoutToolStripMenuItem
            // 
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.logoutToolStripMenuItem.Text = "Logout";
            this.logoutToolStripMenuItem.Click += new System.EventHandler(this.logoutToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1260, 767);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelChessBoard);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Game Caro";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMark)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

