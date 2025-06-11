namespace FootballData.WinForms
{
    partial class PlayerControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblNumber;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.PictureBox playerPicture;
        private System.Windows.Forms.Button btnUploadImage;

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
            lblName = new Label();
            lblNumber = new Label();
            lblPosition = new Label();
            playerPicture = new PictureBox();
            btnUploadImage = new Button();
            ((System.ComponentModel.ISupportInitialize)playerPicture).BeginInit();
            SuspendLayout();
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Font = new Font("Algerian", 10.8F);
            lblName.Location = new Point(115, 10);
            lblName.Name = "lblName";
            lblName.Size = new Size(58, 20);
            lblName.TabIndex = 0;
            lblName.Text = "Name";
            // 
            // lblNumber
            // 
            lblNumber.AutoSize = true;
            lblNumber.Font = new Font("Algerian", 10.8F);
            lblNumber.Location = new Point(115, 40);
            lblNumber.Name = "lblNumber";
            lblNumber.Size = new Size(42, 20);
            lblNumber.TabIndex = 1;
            lblNumber.Text = "#10";
            // 
            // lblPosition
            // 
            lblPosition.AutoSize = true;
            lblPosition.Font = new Font("Algerian", 10.8F);
            lblPosition.Location = new Point(115, 69);
            lblPosition.Name = "lblPosition";
            lblPosition.Size = new Size(85, 20);
            lblPosition.TabIndex = 2;
            lblPosition.Text = "Position";
            // 
            // playerPicture
            // 
            playerPicture.Location = new Point(10, 10);
            playerPicture.Name = "playerPicture";
            playerPicture.Size = new Size(99, 79);
            playerPicture.SizeMode = PictureBoxSizeMode.Zoom;
            playerPicture.TabIndex = 4;
            playerPicture.TabStop = false;
            // 
            // btnUploadImage
            // 
            btnUploadImage.Font = new Font("Algerian", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnUploadImage.Location = new Point(10, 95);
            btnUploadImage.Name = "btnUploadImage";
            btnUploadImage.Size = new Size(99, 26);
            btnUploadImage.TabIndex = 5;
            btnUploadImage.Text = "📷";
            btnUploadImage.Click += btnUploadImage_Click;
            // 
            // PlayerControl
            // 
            Controls.Add(lblName);
            Controls.Add(lblNumber);
            Controls.Add(lblPosition);
            Controls.Add(playerPicture);
            Controls.Add(btnUploadImage);
            Cursor = Cursors.Hand;
            Name = "PlayerControl";
            Size = new Size(341, 127);
            ((System.ComponentModel.ISupportInitialize)playerPicture).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
