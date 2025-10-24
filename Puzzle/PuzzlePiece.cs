using System;
using System.Drawing;

namespace Puzzle
{
    public class PuzzlePiece
    {
        public int Id { get; set; }
        public Image Image { get; set; }
        public Rectangle SourceRect { get; set; }
        public Point CurrentPosition { get; set; }
        public Point CorrectPosition { get; set; }
        public bool IsPlacedCorrectly { get; set; }
        public bool IsSelected { get; set; }
        public bool IsLocked { get; set; }
        public Point DrawPosition { get; set; }
        public bool IsBeingDragged { get; set; }
        public Point DragOffset { get; set; }

        public PuzzlePiece(int id, Image image, Rectangle sourceRect, Point correctPosition)
        {
            Id = id;
            Image = image;
            SourceRect = sourceRect;
            CorrectPosition = correctPosition;
            CurrentPosition = correctPosition;
            DrawPosition = new Point(correctPosition.X * sourceRect.Width, correctPosition.Y * sourceRect.Height);
            IsLocked = false;
        }

        public void Draw(Graphics g, int pieceWidth, int pieceHeight)
        {
            Rectangle destRect = new Rectangle(DrawPosition, new Size(pieceWidth, pieceHeight));

            // Рисуем тень только если кусочек не на своем месте
            if (!IsPlacedCorrectly)
            {
                Rectangle shadowRect = new Rectangle(
                    DrawPosition.X - 5,
                    DrawPosition.Y - 5,
                    pieceWidth + 10,
                    pieceHeight + 10
                );

                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddRectangle(shadowRect);

                    using (var shadowBrush = new System.Drawing.Drawing2D.PathGradientBrush(path))
                    {
                        shadowBrush.CenterColor = Color.FromArgb(200, 0, 0, 0);
                        Color[] surroundingColors = { Color.FromArgb(0, 0, 0, 0) };
                        shadowBrush.SurroundColors = surroundingColors;
                        shadowBrush.FocusScales = new PointF(0.8f, 0.8f);

                        g.FillRectangle(shadowBrush, shadowRect);
                    }
                }
            }

            // Рисуем изображение кусочка
            g.DrawImage(Image, destRect, SourceRect, GraphicsUnit.Pixel);

            // Зеленая подсветка если кусочек рядом с нужным местом (но не точно на нем)
            if (IsPlacedCorrectly && !IsLocked)
            {
                using (var greenOverlay = new SolidBrush(Color.FromArgb(80, 0, 255, 0)))
                {
                    g.FillRectangle(greenOverlay, destRect);
                }
            }

            g.DrawRectangle(Pens.Black, destRect);
        }

        public bool ContainsPoint(Point point, int pieceWidth, int pieceHeight)
        {
            Rectangle bounds = new Rectangle(DrawPosition, new Size(pieceWidth, pieceHeight));
            return bounds.Contains(point);
        }

        public void UpdateCorrectness(int pieceWidth, int pieceHeight)
        {
            Point correctDrawPos = new Point(
                CorrectPosition.X * pieceWidth,
                CorrectPosition.Y * pieceHeight
            );

            int distance = Math.Abs(DrawPosition.X - correctDrawPos.X) +
                          Math.Abs(DrawPosition.Y - correctDrawPos.Y);

            int avgSize = (pieceWidth + pieceHeight) / 2;
            IsPlacedCorrectly = distance < avgSize / 4;
        }

        public void SnapToGrid(int pieceWidth, int pieceHeight)
        {
            if (IsPlacedCorrectly)
            {
                DrawPosition = new Point(
                    CorrectPosition.X * pieceWidth,
                    CorrectPosition.Y * pieceHeight
                );
                IsBeingDragged = false;
                IsSelected = false;
                IsLocked = true;
            }
        }
    }
}