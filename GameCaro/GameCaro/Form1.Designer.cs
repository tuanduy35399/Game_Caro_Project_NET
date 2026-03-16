namespace GameCaro
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Timer timerCoolDown;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topScoresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem matchHistoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.Panel panelBoardHost;
        private System.Windows.Forms.Panel panelChessBoard;
        private System.Windows.Forms.TableLayoutPanel rightLayout;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBoxAvatar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupMatchInfo;
        private System.Windows.Forms.TableLayoutPanel tableMatchInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBoxPlayerName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar progressBarOfPlayer;
        private System.Windows.Forms.Label labelStatusTitle;
        private System.Windows.Forms.Label lblGameStatus;
        private System.Windows.Forms.PictureBox pictureBoxMark;
        private System.Windows.Forms.GroupBox groupLan;
        private System.Windows.Forms.TableLayoutPanel tableLan;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBoxIP;
        private System.Windows.Forms.Label labelLanPort;
        private System.Windows.Forms.TextBox txtLanPort;
        private System.Windows.Forms.FlowLayoutPanel flowLanButtons;
        private System.Windows.Forms.Button btnHostLan;
        private System.Windows.Forms.Button btnLAN;
        private System.Windows.Forms.GroupBox groupControls;
        private System.Windows.Forms.TableLayoutPanel tableControls;
        private System.Windows.Forms.Button btnStartGame;

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
            this.timerCoolDown = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topScoresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.matchHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.panelBoardHost = new System.Windows.Forms.Panel();
            this.panelChessBoard = new System.Windows.Forms.Panel();
            this.rightLayout = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBoxAvatar = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupMatchInfo = new System.Windows.Forms.GroupBox();
            this.tableMatchInfo = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBoxPlayerName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.progressBarOfPlayer = new System.Windows.Forms.ProgressBar();
            this.labelStatusTitle = new System.Windows.Forms.Label();
            this.lblGameStatus = new System.Windows.Forms.Label();
            this.pictureBoxMark = new System.Windows.Forms.PictureBox();
            this.groupLan = new System.Windows.Forms.GroupBox();
            this.tableLan = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBoxIP = new System.Windows.Forms.TextBox();
            this.labelLanPort = new System.Windows.Forms.Label();
            this.txtLanPort = new System.Windows.Forms.TextBox();
            this.flowLanButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnHostLan = new System.Windows.Forms.Button();
            this.btnLAN = new System.Windows.Forms.Button();
            this.groupControls = new System.Windows.Forms.GroupBox();
            this.tableControls = new System.Windows.Forms.TableLayoutPanel();
            this.btnStartGame = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.panelBoardHost.SuspendLayout();
            this.rightLayout.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).BeginInit();
            this.groupMatchInfo.SuspendLayout();
            this.tableMatchInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMark)).BeginInit();
            this.groupLan.SuspendLayout();
            this.tableLan.SuspendLayout();
            this.flowLanButtons.SuspendLayout();
            this.groupControls.SuspendLayout();
            this.tableControls.SuspendLayout();
            this.SuspendLayout();
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
            this.menuStrip1.Size = new System.Drawing.Size(1400, 28);
            this.menuStrip1.TabIndex = 0;
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
            this.newGameToolStripMenuItem.Size = new System.Drawing.Size(177, 26);
            this.newGameToolStripMenuItem.Text = "New game";
            this.newGameToolStripMenuItem.Click += new System.EventHandler(this.newGameToolStripMenuItem_Click);
            // 
            // topScoresToolStripMenuItem
            // 
            this.topScoresToolStripMenuItem.Name = "topScoresToolStripMenuItem";
            this.topScoresToolStripMenuItem.Size = new System.Drawing.Size(177, 26);
            this.topScoresToolStripMenuItem.Text = "Top scores";
            this.topScoresToolStripMenuItem.Click += new System.EventHandler(this.topScoresToolStripMenuItem_Click);
            // 
            // matchHistoryToolStripMenuItem
            // 
            this.matchHistoryToolStripMenuItem.Name = "matchHistoryToolStripMenuItem";
            this.matchHistoryToolStripMenuItem.Size = new System.Drawing.Size(177, 26);
            this.matchHistoryToolStripMenuItem.Text = "Match history";
            this.matchHistoryToolStripMenuItem.Click += new System.EventHandler(this.matchHistoryToolStripMenuItem_Click);
            // 
            // logoutToolStripMenuItem
            // 
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(177, 26);
            this.logoutToolStripMenuItem.Text = "Logout";
            this.logoutToolStripMenuItem.Click += new System.EventHandler(this.logoutToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(177, 26);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitMain.Location = new System.Drawing.Point(0, 28);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.panelBoardHost);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.rightLayout);
            this.splitMain.Panel2MinSize = 330;
            this.splitMain.Size = new System.Drawing.Size(1400, 832);
            this.splitMain.SplitterDistance = 1050;
            this.splitMain.TabIndex = 1;
            // 
            // panelBoardHost
            // 
            this.panelBoardHost.AutoScroll = true;
            this.panelBoardHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(239)))), ((int)(((byte)(244)))));
            this.panelBoardHost.Controls.Add(this.panelChessBoard);
            this.panelBoardHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBoardHost.Location = new System.Drawing.Point(0, 0);
            this.panelBoardHost.Name = "panelBoardHost";
            this.panelBoardHost.Size = new System.Drawing.Size(1050, 832);
            this.panelBoardHost.TabIndex = 0;
            this.panelBoardHost.Resize += new System.EventHandler(this.panelBoardHost_Resize);
            // 
            // panelChessBoard
            // 
            this.panelChessBoard.BackColor = System.Drawing.SystemColors.Control;
            this.panelChessBoard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelChessBoard.Location = new System.Drawing.Point(164, 115);
            this.panelChessBoard.Name = "panelChessBoard";
            this.panelChessBoard.Size = new System.Drawing.Size(722, 602);
            this.panelChessBoard.TabIndex = 0;
            // 
            // rightLayout
            // 
            this.rightLayout.ColumnCount = 1;
            this.rightLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightLayout.Controls.Add(this.panel2, 0, 0);
            this.rightLayout.Controls.Add(this.groupMatchInfo, 0, 1);
            this.rightLayout.Controls.Add(this.groupLan, 0, 2);
            this.rightLayout.Controls.Add(this.groupControls, 0, 3);
            this.rightLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightLayout.Location = new System.Drawing.Point(0, 0);
            this.rightLayout.Name = "rightLayout";
            this.rightLayout.Padding = new System.Windows.Forms.Padding(8, 8, 8, 12);
            this.rightLayout.RowCount = 4;
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42F));
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24F));
            this.rightLayout.Size = new System.Drawing.Size(346, 832);
            this.rightLayout.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pictureBoxAvatar);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(11, 11);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(324, 124);
            this.panel2.TabIndex = 0;
            // 
            // pictureBoxAvatar
            // 
            this.pictureBoxAvatar.Image = null;
            this.pictureBoxAvatar.Location = new System.Drawing.Point(8, 10);
            this.pictureBoxAvatar.Name = "pictureBoxAvatar";
            this.pictureBoxAvatar.Size = new System.Drawing.Size(76, 76);
            this.pictureBoxAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxAvatar.TabIndex = 0;
            this.pictureBoxAvatar.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(92, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 54);
            this.label4.TabIndex = 1;
            this.label4.Text = "Caro";
            // 
            // groupMatchInfo
            // 
            this.groupMatchInfo.Controls.Add(this.tableMatchInfo);
            this.groupMatchInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupMatchInfo.Location = new System.Drawing.Point(11, 141);
            this.groupMatchInfo.Name = "groupMatchInfo";
            this.groupMatchInfo.Padding = new System.Windows.Forms.Padding(8);
            this.groupMatchInfo.Size = new System.Drawing.Size(324, 278);
            this.groupMatchInfo.TabIndex = 1;
            this.groupMatchInfo.TabStop = false;
            this.groupMatchInfo.Text = "Thong tin tran dau";
            // 
            // tableMatchInfo
            // 
            this.tableMatchInfo.ColumnCount = 2;
            this.tableMatchInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.tableMatchInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMatchInfo.Controls.Add(this.label1, 0, 0);
            this.tableMatchInfo.Controls.Add(this.txtBoxPlayerName, 1, 0);
            this.tableMatchInfo.Controls.Add(this.label3, 0, 1);
            this.tableMatchInfo.Controls.Add(this.progressBarOfPlayer, 1, 1);
            this.tableMatchInfo.Controls.Add(this.labelStatusTitle, 0, 2);
            this.tableMatchInfo.Controls.Add(this.lblGameStatus, 1, 2);
            this.tableMatchInfo.Controls.Add(this.pictureBoxMark, 0, 3);
            this.tableMatchInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableMatchInfo.Location = new System.Drawing.Point(8, 23);
            this.tableMatchInfo.Name = "tableMatchInfo";
            this.tableMatchInfo.RowCount = 4;
            this.tableMatchInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableMatchInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableMatchInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.tableMatchInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableMatchInfo.Size = new System.Drawing.Size(308, 247);
            this.tableMatchInfo.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Luot choi";
            // 
            // txtBoxPlayerName
            // 
            this.txtBoxPlayerName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxPlayerName.Location = new System.Drawing.Point(85, 5);
            this.txtBoxPlayerName.Name = "txtBoxPlayerName";
            this.txtBoxPlayerName.ReadOnly = true;
            this.txtBoxPlayerName.Size = new System.Drawing.Size(220, 22);
            this.txtBoxPlayerName.TabIndex = 1;
            this.txtBoxPlayerName.TabStop = false;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Thoi gian di";
            // 
            // progressBarOfPlayer
            // 
            this.progressBarOfPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarOfPlayer.Location = new System.Drawing.Point(85, 36);
            this.progressBarOfPlayer.Name = "progressBarOfPlayer";
            this.progressBarOfPlayer.Size = new System.Drawing.Size(220, 24);
            this.progressBarOfPlayer.TabIndex = 3;
            // 
            // labelStatusTitle
            // 
            this.labelStatusTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelStatusTitle.AutoSize = true;
            this.labelStatusTitle.Location = new System.Drawing.Point(3, 98);
            this.labelStatusTitle.Name = "labelStatusTitle";
            this.labelStatusTitle.Size = new System.Drawing.Size(65, 16);
            this.labelStatusTitle.TabIndex = 4;
            this.labelStatusTitle.Text = "Trang thai";
            // 
            // lblGameStatus
            // 
            this.lblGameStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblGameStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGameStatus.Location = new System.Drawing.Point(85, 68);
            this.lblGameStatus.Name = "lblGameStatus";
            this.lblGameStatus.Padding = new System.Windows.Forms.Padding(6);
            this.lblGameStatus.Size = new System.Drawing.Size(220, 84);
            this.lblGameStatus.TabIndex = 5;
            this.lblGameStatus.Text = "Status: Ready.";
            // 
            // pictureBoxMark
            // 
            this.tableMatchInfo.SetColumnSpan(this.pictureBoxMark, 2);
            this.pictureBoxMark.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxMark.Location = new System.Drawing.Point(24, 168);
            this.pictureBoxMark.Margin = new System.Windows.Forms.Padding(24, 16, 24, 8);
            this.pictureBoxMark.Name = "pictureBoxMark";
            this.pictureBoxMark.Size = new System.Drawing.Size(260, 71);
            this.pictureBoxMark.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxMark.TabIndex = 6;
            this.pictureBoxMark.TabStop = false;
            // 
            // groupLan
            // 
            this.groupLan.Controls.Add(this.tableLan);
            this.groupLan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupLan.Location = new System.Drawing.Point(11, 425);
            this.groupLan.Name = "groupLan";
            this.groupLan.Padding = new System.Windows.Forms.Padding(8);
            this.groupLan.Size = new System.Drawing.Size(324, 226);
            this.groupLan.TabIndex = 2;
            this.groupLan.TabStop = false;
            this.groupLan.Text = "Ket noi LAN";
            // 
            // tableLan
            // 
            this.tableLan.ColumnCount = 2;
            this.tableLan.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.tableLan.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLan.Controls.Add(this.label2, 0, 0);
            this.tableLan.Controls.Add(this.txtBoxIP, 1, 0);
            this.tableLan.Controls.Add(this.labelLanPort, 0, 1);
            this.tableLan.Controls.Add(this.txtLanPort, 1, 1);
            this.tableLan.Controls.Add(this.flowLanButtons, 0, 2);
            this.tableLan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLan.Location = new System.Drawing.Point(8, 23);
            this.tableLan.Name = "tableLan";
            this.tableLan.RowCount = 4;
            this.tableLan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tableLan.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLan.Size = new System.Drawing.Size(308, 195);
            this.tableLan.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Host IP";
            // 
            // txtBoxIP
            // 
            this.txtBoxIP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxIP.Location = new System.Drawing.Point(85, 5);
            this.txtBoxIP.Name = "txtBoxIP";
            this.txtBoxIP.Size = new System.Drawing.Size(220, 22);
            this.txtBoxIP.TabIndex = 0;
            // 
            // labelLanPort
            // 
            this.labelLanPort.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelLanPort.AutoSize = true;
            this.labelLanPort.Location = new System.Drawing.Point(3, 40);
            this.labelLanPort.Name = "labelLanPort";
            this.labelLanPort.Size = new System.Drawing.Size(31, 16);
            this.labelLanPort.TabIndex = 2;
            this.labelLanPort.Text = "Port";
            // 
            // txtLanPort
            // 
            this.txtLanPort.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtLanPort.Location = new System.Drawing.Point(85, 37);
            this.txtLanPort.MaxLength = 5;
            this.txtLanPort.Name = "txtLanPort";
            this.txtLanPort.Size = new System.Drawing.Size(94, 22);
            this.txtLanPort.TabIndex = 1;
            this.txtLanPort.Text = "5000";
            // 
            // flowLanButtons
            // 
            this.tableLan.SetColumnSpan(this.flowLanButtons, 2);
            this.flowLanButtons.Controls.Add(this.btnHostLan);
            this.flowLanButtons.Controls.Add(this.btnLAN);
            this.flowLanButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLanButtons.Location = new System.Drawing.Point(3, 67);
            this.flowLanButtons.Name = "flowLanButtons";
            this.flowLanButtons.Size = new System.Drawing.Size(302, 38);
            this.flowLanButtons.TabIndex = 4;
            this.flowLanButtons.WrapContents = false;
            // 
            // btnHostLan
            // 
            this.btnHostLan.Location = new System.Drawing.Point(3, 3);
            this.btnHostLan.Name = "btnHostLan";
            this.btnHostLan.Size = new System.Drawing.Size(120, 30);
            this.btnHostLan.TabIndex = 2;
            this.btnHostLan.Text = "Host LAN";
            this.btnHostLan.UseVisualStyleBackColor = true;
            this.btnHostLan.Click += new System.EventHandler(this.hostLanToolStripMenuItem_Click);
            // 
            // btnLAN
            // 
            this.btnLAN.Location = new System.Drawing.Point(129, 3);
            this.btnLAN.Name = "btnLAN";
            this.btnLAN.Size = new System.Drawing.Size(120, 30);
            this.btnLAN.TabIndex = 3;
            this.btnLAN.Text = "Join LAN";
            this.btnLAN.UseVisualStyleBackColor = true;
            this.btnLAN.Click += new System.EventHandler(this.btnLAN_Click);
            // 
            // groupControls
            // 
            this.groupControls.Controls.Add(this.tableControls);
            this.groupControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControls.Location = new System.Drawing.Point(11, 657);
            this.groupControls.Name = "groupControls";
            this.groupControls.Padding = new System.Windows.Forms.Padding(8);
            this.groupControls.Size = new System.Drawing.Size(324, 160);
            this.groupControls.TabIndex = 3;
            this.groupControls.TabStop = false;
            this.groupControls.Text = "Dieu khien tran dau";
            // 
            // tableControls
            // 
            this.tableControls.ColumnCount = 1;
            this.tableControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableControls.Controls.Add(this.btnStartGame, 0, 0);
            this.tableControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableControls.Location = new System.Drawing.Point(8, 23);
            this.tableControls.Name = "tableControls";
            this.tableControls.RowCount = 2;
            this.tableControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableControls.Size = new System.Drawing.Size(308, 52);
            this.tableControls.TabIndex = 0;
            // 
            // btnStartGame
            // 
            this.btnStartGame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStartGame.Location = new System.Drawing.Point(3, 3);
            this.btnStartGame.Name = "btnStartGame";
            this.btnStartGame.Size = new System.Drawing.Size(302, 34);
            this.btnStartGame.TabIndex = 0;
            this.btnStartGame.Text = "Bat dau choi";
            this.btnStartGame.UseVisualStyleBackColor = true;
            this.btnStartGame.Click += new System.EventHandler(this.btnStartGame_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 860);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1160, 760);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Game Caro";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.panelBoardHost.ResumeLayout(false);
            this.rightLayout.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAvatar)).EndInit();
            this.groupMatchInfo.ResumeLayout(false);
            this.tableMatchInfo.ResumeLayout(false);
            this.tableMatchInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMark)).EndInit();
            this.groupLan.ResumeLayout(false);
            this.tableLan.ResumeLayout(false);
            this.tableLan.PerformLayout();
            this.flowLanButtons.ResumeLayout(false);
            this.groupControls.ResumeLayout(false);
            this.tableControls.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
