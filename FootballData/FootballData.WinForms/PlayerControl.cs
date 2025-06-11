using FootballData.DataLayer.Models;
using System.Drawing.Imaging;

namespace FootballData.WinForms
{
    public partial class PlayerControl : UserControl
    {
        public Player Player { get; private set; }
        private bool _isFavorite;

        private static readonly string ImageFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FootballData", "Players");
        private Settings _settings;

        public PlayerControl(Player player, bool isFavorite = false)
        {
            InitializeComponent();
            _settings = Settings.Load();
            Player = player ?? throw new ArgumentNullException(nameof(player));
            _isFavorite = isFavorite;
            EnsureImageDirectoryExists();
            UpdateDisplay();
        }

        public void SetFavorite(bool isFavorite)
        {
            _isFavorite = isFavorite;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            lblName.Text = _isFavorite ? $"✶ {Player.Name}" : Player.Name;
            lblPosition.Text = Player.Position;
            lblNumber.Text = Player.ShirtNumber > 0 ? (Player.IsCaptain ? $"#{Player.ShirtNumber} ©" : $"#{Player.ShirtNumber}") : "";
            playerPicture.Image = GetPlayerImage();
        }

        private Image GetPlayerImage()
        {
            string playerImagePath = GetPlayerImagePath();
            try
            {
                if (File.Exists(playerImagePath))
                {
                    return LoadImageSafely(playerImagePath, playerPicture.Width, playerPicture.Height);
                }
                else
                {
                    using (var ms = new MemoryStream(Properties.Resources.DefaultPlayerImage))
                    {
                        return Image.FromStream(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading player image: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return new Bitmap(playerPicture.Width, playerPicture.Height);
            }
        }

        private string GetPlayerImagePath()
        {
            return Path.Combine(ImageFolder, $"{Player.Name.Replace(" ", "_")}.jpg");
        }

        private void EnsureImageDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(ImageFolder))
                {
                    Directory.CreateDirectory(ImageFolder);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error creating image directory: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string destinationPath = GetPlayerImagePath();

                    try
                    {
                        Image originalImage = Image.FromFile(openFileDialog.FileName);
                        Image resizedImage = ResizeImage(originalImage, playerPicture.Width, playerPicture.Height);

                        resizedImage.Save(destinationPath, ImageFormat.Jpeg);
                        playerPicture.Image = resizedImage;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Error saving image: {ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        private Image LoadImageSafely(string path, int width, int height)
        {
            try
            {
                using (var img = Image.FromFile(path))
                {
                    return ResizeImage(new Bitmap(img), width, height);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading image from file: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return new Bitmap(width, height);
            }
        }

        private Image ResizeImage(Image img, int width, int height)
        {
            try
            {
                Bitmap newImage = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(img, 0, 0, width, height);
                }
                return newImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error resizing image: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return new Bitmap(width, height);
            }
        }

        private void PlayerControl_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(this, DragDropEffects.Move);
        }
    }
}
