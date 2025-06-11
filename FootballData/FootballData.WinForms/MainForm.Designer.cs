using System.Windows.Forms;

namespace FootballData.WinForms
{
    partial class MainForm
    {
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnTeams;
        private System.Windows.Forms.Button btnMatches;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.ComboBox comboTeams;
        private System.Windows.Forms.Button btnSaveTeam;
        private System.Windows.Forms.Label lblSelectTeam;
        private System.Windows.Forms.Panel panelAllPlayers;
        private System.Windows.Forms.Panel panelFavoritePlayers;
        private System.Windows.Forms.Label lblPlayers;
        private System.Windows.Forms.Label lblFavoritePlayers;
        private System.Windows.Forms.Button btnRankPlayers;
        private System.Windows.Forms.Button btnShowMatchData;
        private System.Windows.Forms.Button btnExportToPDF;

        private void InitializeComponent()
        {
            lblTitle = new Label();
            btnTeams = new Button();
            btnMatches = new Button();
            btnSettings = new Button();
            dataGridView = new DataGridView();
            comboTeams = new ComboBox();
            btnSaveTeam = new Button();
            lblSelectTeam = new Label();
            panelAllPlayers = new Panel();
            panelFavoritePlayers = new Panel();
            lblPlayers = new Label();
            lblFavoritePlayers = new Label();
            btnRankPlayers = new Button();
            btnShowMatchData = new Button();
            btnExportToPDF = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Algerian", 19.8000011F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(766, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(388, 38);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "FIFA World Cup 2018";
            // 
            // btnTeams
            // 
            btnTeams.Font = new Font("Algerian", 10.2F);
            btnTeams.Location = new Point(71, 213);
            btnTeams.Name = "btnTeams";
            btnTeams.Size = new Size(176, 51);
            btnTeams.TabIndex = 1;
            btnTeams.Text = "Show All Teams";
            btnTeams.UseVisualStyleBackColor = true;
            btnTeams.Click += btnTeams_Click;
            // 
            // btnMatches
            // 
            btnMatches.Font = new Font("Algerian", 10.2F);
            btnMatches.Location = new Point(264, 213);
            btnMatches.Name = "btnMatches";
            btnMatches.Size = new Size(176, 51);
            btnMatches.TabIndex = 2;
            btnMatches.Text = "Show All Matches";
            btnMatches.UseVisualStyleBackColor = true;
            btnMatches.Click += btnMatches_Click;
            // 
            // btnSettings
            // 
            btnSettings.Font = new Font("Algerian", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnSettings.Location = new Point(46, 18);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(120, 40);
            btnSettings.TabIndex = 3;
            btnSettings.Text = "Settings";
            btnSettings.UseVisualStyleBackColor = true;
            btnSettings.Click += btnSettings_Click;
            // 
            // dataGridView
            // 
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Font = new Font("Algerian", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridView.Location = new Point(46, 282);
            dataGridView.Name = "dataGridView";
            dataGridView.RowHeadersWidth = 51;
            dataGridView.Size = new Size(973, 545);
            dataGridView.TabIndex = 11;
            // 
            // comboTeams
            // 
            comboTeams.DropDownStyle = ComboBoxStyle.DropDownList;
            comboTeams.Font = new Font("Algerian", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboTeams.Location = new Point(218, 137);
            comboTeams.Name = "comboTeams";
            comboTeams.Size = new Size(228, 27);
            comboTeams.TabIndex = 5;
            // 
            // btnSaveTeam
            // 
            btnSaveTeam.Font = new Font("Algerian", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnSaveTeam.Location = new Point(468, 137);
            btnSaveTeam.Name = "btnSaveTeam";
            btnSaveTeam.Size = new Size(120, 30);
            btnSaveTeam.TabIndex = 6;
            btnSaveTeam.Text = "Save";
            btnSaveTeam.UseVisualStyleBackColor = true;
            btnSaveTeam.Click += btnSaveTeam_Click;
            // 
            // lblSelectTeam
            // 
            lblSelectTeam.AutoSize = true;
            lblSelectTeam.Font = new Font("Algerian", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSelectTeam.Location = new Point(46, 140);
            lblSelectTeam.Name = "lblSelectTeam";
            lblSelectTeam.Size = new Size(166, 19);
            lblSelectTeam.TabIndex = 4;
            lblSelectTeam.Text = "Select Your Team";
            // 
            // panelAllPlayers
            // 
            panelAllPlayers.AllowDrop = true;
            panelAllPlayers.AutoScroll = true;
            panelAllPlayers.BorderStyle = BorderStyle.FixedSingle;
            panelAllPlayers.Font = new Font("Algerian", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            panelAllPlayers.Location = new Point(1055, 145);
            panelAllPlayers.Name = "panelAllPlayers";
            panelAllPlayers.Size = new Size(387, 682);
            panelAllPlayers.TabIndex = 7;
            panelAllPlayers.DragDrop += panelAllPlayers_DragDrop;
            panelAllPlayers.DragEnter += panelAllPlayers_DragEnter;
            panelAllPlayers.MouseWheel += panelAllPlayers_MouseWheel;
            // 
            // panelFavoritePlayers
            // 
            panelFavoritePlayers.AllowDrop = true;
            panelFavoritePlayers.AutoScroll = true;
            panelFavoritePlayers.BorderStyle = BorderStyle.FixedSingle;
            panelFavoritePlayers.Font = new Font("Algerian", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            panelFavoritePlayers.Location = new Point(1513, 145);
            panelFavoritePlayers.Name = "panelFavoritePlayers";
            panelFavoritePlayers.Size = new Size(383, 396);
            panelFavoritePlayers.TabIndex = 8;
            panelFavoritePlayers.DragDrop += panelFavoritePlayers_DragDrop;
            panelFavoritePlayers.DragEnter += panelAllPlayers_DragEnter;
            // 
            // lblPlayers
            // 
            lblPlayers.AutoSize = true;
            lblPlayers.Font = new Font("Algerian", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPlayers.Location = new Point(1201, 104);
            lblPlayers.Name = "lblPlayers";
            lblPlayers.Size = new Size(104, 22);
            lblPlayers.TabIndex = 9;
            lblPlayers.Text = "Players";
            // 
            // lblFavoritePlayers
            // 
            lblFavoritePlayers.AutoSize = true;
            lblFavoritePlayers.Font = new Font("Algerian", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblFavoritePlayers.Location = new Point(1597, 104);
            lblFavoritePlayers.Name = "lblFavoritePlayers";
            lblFavoritePlayers.Size = new Size(211, 22);
            lblFavoritePlayers.TabIndex = 10;
            lblFavoritePlayers.Text = "Favorite Players";
            // 
            // btnRankPlayers
            // 
            btnRankPlayers.Font = new Font("Algerian", 10.2F);
            btnRankPlayers.Location = new Point(468, 213);
            btnRankPlayers.Name = "btnRankPlayers";
            btnRankPlayers.Size = new Size(176, 51);
            btnRankPlayers.TabIndex = 0;
            btnRankPlayers.Text = "Players Ranking";
            btnRankPlayers.UseVisualStyleBackColor = true;
            btnRankPlayers.Click += BtnRankPlayers_Click;
            // 
            // btnShowMatchData
            // 
            btnShowMatchData.Font = new Font("Algerian", 10.2F);
            btnShowMatchData.Location = new Point(665, 213);
            btnShowMatchData.Name = "btnShowMatchData";
            btnShowMatchData.Size = new Size(176, 51);
            btnShowMatchData.TabIndex = 12;
            btnShowMatchData.Text = "Show Match Data";
            btnShowMatchData.UseVisualStyleBackColor = true;
            btnShowMatchData.Click += btnShowAttendanceRanking_Click;
            // 
            // btnExportToPDF
            // 
            btnExportToPDF.Font = new Font("Algerian", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnExportToPDF.Location = new Point(172, 18);
            btnExportToPDF.Name = "btnExportToPDF";
            btnExportToPDF.Size = new Size(120, 40);
            btnExportToPDF.TabIndex = 13;
            btnExportToPDF.Text = "PDF Export";
            btnExportToPDF.Click += btnExportToPDF_Click;
            // 
            // MainForm
            // 
            AutoScroll = true;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(1965, 848);
            Controls.Add(btnShowMatchData);
            Controls.Add(btnExportToPDF);
            Controls.Add(btnRankPlayers);
            Controls.Add(lblTitle);
            Controls.Add(btnTeams);
            Controls.Add(btnMatches);
            Controls.Add(btnSettings);
            Controls.Add(lblSelectTeam);
            Controls.Add(comboTeams);
            Controls.Add(btnSaveTeam);
            Controls.Add(panelAllPlayers);
            Controls.Add(panelFavoritePlayers);
            Controls.Add(lblPlayers);
            Controls.Add(lblFavoritePlayers);
            Controls.Add(dataGridView);
            Name = "MainForm";
            Text = "FIFA World Cup";
            FormClosing += MainForm_FormClosing;
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
