namespace FootballData.WinForms
{
    partial class SettingsForm
    {
        private System.Windows.Forms.ComboBox comboChampionship;
        private System.Windows.Forms.ComboBox comboLanguage;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblChampionship;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.ComboBox comboDataSource;
        private Label lblData;

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            comboChampionship = new ComboBox();
            comboLanguage = new ComboBox();
            btnSave = new Button();
            lblChampionship = new Label();
            lblLanguage = new Label();
            comboDataSource = new ComboBox();
            lblData = new Label();
            SuspendLayout();
            // 
            // comboChampionship
            // 
            comboChampionship.DropDownStyle = ComboBoxStyle.DropDownList;
            comboChampionship.Font = new Font("Algerian", 10.2F);
            comboChampionship.FormattingEnabled = true;
            comboChampionship.Items.AddRange(new object[] { "men", "women" });
            comboChampionship.Location = new Point(150, 30);
            comboChampionship.Name = "comboChampionship";
            comboChampionship.Size = new Size(200, 27);
            comboChampionship.TabIndex = 0;
            // 
            // comboLanguage
            // 
            comboLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            comboLanguage.Font = new Font("Algerian", 10.2F);
            comboLanguage.FormattingEnabled = true;
            comboLanguage.Items.AddRange(new object[] { "en", "hr" });
            comboLanguage.Location = new Point(150, 80);
            comboLanguage.Name = "comboLanguage";
            comboLanguage.Size = new Size(200, 27);
            comboLanguage.TabIndex = 1;
            // 
            // btnSave
            // 
            btnSave.Font = new Font("Algerian", 10.2F);
            btnSave.Location = new Point(150, 166);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(100, 40);
            btnSave.TabIndex = 2;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // lblChampionship
            // 
            lblChampionship.AutoSize = true;
            lblChampionship.Font = new Font("Algerian", 10.2F);
            lblChampionship.Location = new Point(30, 30);
            lblChampionship.Name = "lblChampionship";
            lblChampionship.Size = new Size(117, 19);
            lblChampionship.TabIndex = 3;
            lblChampionship.Text = "Tournament";
            // 
            // lblLanguage
            // 
            lblLanguage.AutoSize = true;
            lblLanguage.Font = new Font("Algerian", 10.2F);
            lblLanguage.Location = new Point(30, 80);
            lblLanguage.Name = "lblLanguage";
            lblLanguage.Size = new Size(99, 19);
            lblLanguage.TabIndex = 4;
            lblLanguage.Text = "Language";
            // 
            // comboDataSource
            // 
            comboDataSource.DropDownStyle = ComboBoxStyle.DropDownList;
            comboDataSource.Font = new Font("Algerian", 10.2F);
            comboDataSource.FormattingEnabled = true;
            comboDataSource.Items.AddRange(new object[] { "API", "Local JSON" });
            comboDataSource.Location = new Point(150, 125);
            comboDataSource.Name = "comboDataSource";
            comboDataSource.Size = new Size(200, 27);
            comboDataSource.TabIndex = 4;
            // 
            // lblData
            // 
            lblData.AutoSize = true;
            lblData.Font = new Font("Algerian", 10.2F);
            lblData.Location = new Point(30, 128);
            lblData.Name = "lblData";
            lblData.Size = new Size(55, 19);
            lblData.TabIndex = 5;
            lblData.Text = "Data";
            // 
            // SettingsForm
            // 
            ClientSize = new Size(400, 250);
            Controls.Add(lblData);
            Controls.Add(comboDataSource);
            Controls.Add(comboChampionship);
            Controls.Add(comboLanguage);
            Controls.Add(btnSave);
            Controls.Add(lblChampionship);
            Controls.Add(lblLanguage);
            Name = "SettingsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
