using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Puzzle
{
    public partial class GameForm : Form
    {
        private PuzzleGame game;
        private Bitmap puzzleImage;
        private Timer gameTimer;
        private int gridSize;


        private void InitializeGameForm()
        {
            this.DoubleBuffered = true;
            this.Text = "Пазл";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            SetupGameUI();
            StartNewGame();
        }

        public GameForm(Bitmap image, int size)
        {
            // Ресайз изображения
            puzzleImage = ResizeImage(image);
            gridSize = size;
            InitializeGameForm();
        }

        private Bitmap ResizeImage(Image image)
        {
            int targetHeight = 500;
            // Рассчитываем ширину пропорционально высоте
            double ratio = (double)image.Width / image.Height;
            int targetWidth = (int)(targetHeight * ratio);

            var resizedImage = new Bitmap(targetWidth, targetHeight);

            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, targetWidth, targetHeight);
            }

            return resizedImage;
        }

        private void SetupGameUI()
        {
            // Создаем игровое поле с учетом размеров изображения (после ресайза)
            game = new PuzzleGame(puzzleImage, gridSize);

            // Рассчитываем размер формы пропорционально ресайзнутому изображению
            // Добавляем отступы для краев формы
            int formWidth = game.Cols * game.PieceWidth * 2;
            int formHeight = game.Rows * game.PieceHeight + 200;

            // Устанавливаем минимальный размер формы
            formWidth = Math.Max(formWidth, 800);
            formHeight = Math.Max(formHeight, 700);

            this.ClientSize = new Size(formWidth, formHeight);

            // Таймер для обновления состояния игры
            gameTimer = new Timer { Interval = 100 };
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            // Обработчики событий мыши
            this.MouseDown += GameForm_MouseDown;
            this.MouseMove += GameForm_MouseMove;
            this.MouseUp += GameForm_MouseUp;

            // Обработчик клавиатуры
            this.KeyPreview = true;
            this.KeyDown += GameForm_KeyDown;
        }
        private void StartNewGame()
        {
            if (puzzleImage != null)
            {
                game = new PuzzleGame(puzzleImage, gridSize);
                this.Invalidate();
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (game == null) return;

            if (game.IsCompleted)
            {
                this.Text = "Пазл собран! Поздравляем!";
                gameTimer.Stop();
            }
            else
            {
                int correctPieces = game.Pieces.Count(p => p.IsPlacedCorrectly);
                int totalPieces = game.Cols * game.Rows;
                this.Text = $"Пазл - Собрано: {correctPieces}/{totalPieces}";
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (game == null) return;

            // Центрируем игровое поле
            int offsetX = (this.ClientSize.Width - game.Cols * game.PieceWidth) / 2;
            int offsetY = (this.ClientSize.Height - game.Rows * game.PieceHeight) / 2;

            e.Graphics.TranslateTransform(offsetX, offsetY);
            game.Draw(e.Graphics);
        }

        private void GameForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (game == null) return;

            if (e.Button == MouseButtons.Left)
            {
                int offsetX = (this.ClientSize.Width - game.Cols * game.PieceWidth) / 2;
                int offsetY = (this.ClientSize.Height - game.Rows * game.PieceHeight) / 2;

                Point gamePoint = new Point(e.X - offsetX, e.Y - offsetY);
                game.SelectPiece(gamePoint);
                this.Invalidate();
            }
        }

        private void GameForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (game == null) return;

            if (e.Button == MouseButtons.Left)
            {
                int offsetX = (this.ClientSize.Width - game.Cols * game.PieceWidth) / 2;
                int offsetY = (this.ClientSize.Height - game.Rows * game.PieceHeight) / 2;

                Point gamePoint = new Point(e.X - offsetX, e.Y - offsetY);
                game.MoveSelectedPiece(gamePoint);
                this.Invalidate();
            }
        }

        private void GameForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (game == null) return;

            if (e.Button == MouseButtons.Left)
            {
                game.DropPiece();
                this.Invalidate();
            }
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (game == null) return;

            if (e.KeyCode == Keys.R)
            {
                StartNewGame();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            gameTimer?.Stop();
            puzzleImage?.Dispose();
        }

       
    }
}