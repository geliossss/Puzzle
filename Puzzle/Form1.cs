using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Puzzle
{
    public partial class PuzzleForm : Form
    {
        private Bitmap selectedImage;
        private string[] imagePaths;
        private int gridSize = 4;
        private Button startGameBtn;
        private FlowLayoutPanel imagesFlowPanel;
        private Label titleLabel;

        public PuzzleForm()
        {
            InitializeComponent();
            InitializeMainMenu();
        }

        private void InitializeMainMenu()
        {
            this.Text = "Пазл - Выбор изображения и сложности";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;

            LoadImagesFromFolder();
            SetupMainMenuUI();
        }

        private void SetupMainMenuUI()
        {
            titleLabel = new Label
            {
                Text = "Выберите изображение и сложность",
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };

            var difficultyPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50
            };

            var difficultyLabel = new Label
            {
                Text = "Сложность:",
                Font = new Font("Arial", 10),
                Location = new Point(20, 15),
                AutoSize = true
            };

            difficultyComboBox = new ComboBox
            {
                Location = new Point(120, 12),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 10)
            };
            difficultyComboBox.Items.AddRange(new object[] { "Легко", "Средне", "Сложно" });
            difficultyComboBox.SelectedIndex = 0;
            difficultyComboBox.SelectedIndexChanged += DifficultyComboBox_SelectedIndexChanged;

            difficultyPanel.Controls.Add(difficultyLabel);
            difficultyPanel.Controls.Add(difficultyComboBox);

            // Кнопка начала игры
            startGameBtn = new Button
            {
                Text = "Начать игру",
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.LightGreen,
                Size = new Size(200, 40),
                Dock = DockStyle.Bottom,
                Margin = new Padding(20)
            };
            startGameBtn.Click += StartGameBtn_Click;

            // Панель с изображениями
            imagesFlowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20),
                BackColor = SystemColors.ControlLight
            };

            LoadImagesToPanel();

            // Добавляем элементы на форму
            this.Controls.Add(imagesFlowPanel);
            this.Controls.Add(startGameBtn);
            this.Controls.Add(difficultyPanel);
            this.Controls.Add(titleLabel);
        }

        private void DifficultyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (difficultyComboBox.SelectedItem.ToString())
            {
                case "Легко": gridSize = 4; break;
                case "Средне": gridSize = 7; break;
                case "Сложно": gridSize = 10; break;
                default: gridSize = 4; break;
            }
        }

        private void LoadImagesFromFolder()
        {
            string imageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Puzzle", "Assets", "Images");

            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
                return;
            }

            string[] extensions = { "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.gif" };
            var imageFiles = extensions.SelectMany(ext => Directory.GetFiles(imageFolder, ext)).ToArray();
            imagePaths = imageFiles;
        }

        private void LoadImagesToPanel()
        {
            imagesFlowPanel.Controls.Clear();

            if (imagePaths != null && imagePaths.Length > 0)
            {
                foreach (string imagePath in imagePaths)
                {
                    var imagePanel = CreateImagePanel(imagePath);
                    imagesFlowPanel.Controls.Add(imagePanel);
                }
            }
            else
            {
                var label = new Label
                {
                    Text = "Нет доступных изображений. Добавьте изображения в папку Assets/Images",
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Font = new Font("Arial", 12)
                };
                imagesFlowPanel.Controls.Add(label);
            }
        }

        private Panel CreateImagePanel(string imagePath)
        {
            var panel = new Panel
            {
                Size = new Size(180, 200),
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand,
                BackColor = Color.White
            };

            try
            {
                using (var originalImage = Image.FromFile(imagePath))
                {
                    var thumbnail = new Bitmap(originalImage, new Size(160, 140));

                    var pictureBox = new PictureBox
                    {
                        Image = new Bitmap(thumbnail),
                        Size = new Size(160, 140),
                        Location = new Point(10, 10),
                        SizeMode = PictureBoxSizeMode.Zoom
                    };

                    var label = new Label
                    {
                        Text = Path.GetFileNameWithoutExtension(imagePath),
                        Location = new Point(10, 155),
                        Size = new Size(160, 35),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Arial", 9)
                    };

                    // Выделение выбранного изображения
                    panel.Click += (s, e) => SelectImage(panel, imagePath);
                    pictureBox.Click += (s, e) => SelectImage(panel, imagePath);
                    label.Click += (s, e) => SelectImage(panel, imagePath);

                    panel.Controls.Add(pictureBox);
                    panel.Controls.Add(label);
                }
            }
            catch (Exception ex)
            {
                panel.Controls.Add(new Label
                {
                    Text = "Ошибка загрузки",
                    Location = new Point(10, 10),
                    Size = new Size(160, 180),
                    TextAlign = ContentAlignment.MiddleCenter
                });
            }

            return panel;
        }

        private void SelectImage(Panel selectedPanel, string imagePath)
        {
            // Снимаем выделение со всех панелей
            foreach (Control control in imagesFlowPanel.Controls)
            {
                if (control is Panel panel)
                {
                    panel.BackColor = Color.White;
                    panel.BorderStyle = BorderStyle.FixedSingle;
                }
            }

            // Выделяем выбранную панель
            selectedPanel.BackColor = Color.LightBlue;
            selectedPanel.BorderStyle = BorderStyle.Fixed3D;

            try
            {
                selectedImage?.Dispose();
                using (var originalImage = Image.FromFile(imagePath))
                {
                    selectedImage = new Bitmap(originalImage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
            }
        }

        private void StartGameBtn_Click(object sender, EventArgs e)
        {
            if (selectedImage == null)
            {
                MessageBox.Show("Пожалуйста, выберите изображение для пазла!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var gameForm = new GameForm(selectedImage, gridSize);
            gameForm.Show();
            // this.Hide(); // Можно скрыть главную форму, если нужно
        }
    }
}