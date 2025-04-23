using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using WeifenLuo.WinFormsUI.Docking;

namespace WinForms_v1
{
    public partial class FormDocument : DockContent // Наследуемся от DockContent
    {
        private int x, y;
        private Bitmap bmp;
        private Bitmap bmpTemp;
        private string currentFilePath = null;
        private float scaleFactor = 1.0f; // Коэффициент масштабирования
        private Bitmap _image;  // Поле для хранения изображения


        private Stack<Bitmap> undoStack = new Stack<Bitmap>(); // История назад
        private Stack<Bitmap> redoStack = new Stack<Bitmap>(); // История вперёд

        public FormDocument()
        {
            InitializeComponent();
            bmp = new Bitmap(ClientSize.Width, ClientSize.Height);
            bmpTemp = bmp;
            SaveState(); // Сохраняем начальное состояние
        }


        public FormDocument(Bitmap bmp)
        {
            InitializeComponent();
            this.bmp = new Bitmap(bmp);
            this.bmpTemp = new Bitmap(bmp);
            this.BackgroundImage = this.bmp;
            SaveState();
        }
        private void FormDocument_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var pen = new Pen(MainForm.CurrentColor, MainForm.CurrentWidth);
                var eraser = new Pen(Color.White, MainForm.CurrentWidth); // Ластик использует белый цвет (или цвет фона)

                switch (MainForm.CurrentTool)
                {
                    case Tools.Pen:
                        var g = Graphics.FromImage(bmp);
                        g.DrawLine(pen, x, y, e.X, e.Y);
                        x = e.X;
                        y = e.Y;
                        bmpTemp = bmp;
                        Invalidate();
                        break;
                    case Tools.Eraser:
                        g = Graphics.FromImage(bmp);
                        using (SolidBrush eraserBrush = new SolidBrush(Color.White)) // Используем белый цвет (можно изменить)
                        {
                            g.FillEllipse(eraserBrush, e.X - MainForm.CurrentWidth / 2, e.Y - MainForm.CurrentWidth / 2,
                                          MainForm.CurrentWidth, MainForm.CurrentWidth);
                        }
                        x = e.X;
                        y = e.Y;
                        bmpTemp = bmp;
                        Invalidate();
                        break;
                    case Tools.Circle:
                        bmpTemp = (Bitmap)bmp.Clone();
                        g = Graphics.FromImage(bmpTemp);
                        g.DrawEllipse(pen, new Rectangle(x, y, e.X - x, e.Y - y));
                        Invalidate();
                        break;
                    case Tools.Line:
                        bmpTemp = (Bitmap)bmp.Clone();
                        g = Graphics.FromImage(bmpTemp);
                        g.DrawLine(pen, x, y, e.X, e.Y);
                        Invalidate();
                        break;
                    case Tools.Heart:
                        bmpTemp = (Bitmap)bmp.Clone();
                        g = Graphics.FromImage(bmpTemp);
                        DrawHeart(g, pen, x, y, e.X, e.Y);
                        Invalidate();
                        break;
                }
            }
        }

        private void FormDocument_MouseUp(object sender, MouseEventArgs e)
        {
            if (MainForm.CurrentTool == Tools.Circle || MainForm.CurrentTool == Tools.Line || MainForm.CurrentTool == Tools.Heart || MainForm.CurrentTool == Tools.Eraser)
            {
                bmp = bmpTemp;
                SaveState();
                Invalidate();
            }
        }

        private void FormDocument_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                x = e.X;
                y = e.Y;
                SaveState(); // Сохраняем состояние перед изменением

                // Инструмент "Текст"
                if (MainForm.CurrentTool == Tools.Text)
                {
                    using (var inputBox = new TextInputForm()) // Окно ввода текста
                    {
                        if (inputBox.ShowDialog() == DialogResult.OK)
                        {
                            string text = inputBox.InputText;
                            if (!string.IsNullOrEmpty(text))
                            {
                                using (Graphics g = Graphics.FromImage(bmp))
                                {
                                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                                    using (Font font = new Font("Arial", 14, FontStyle.Bold))
                                    using (SolidBrush brush = new SolidBrush(MainForm.CurrentColor))
                                    {
                                        g.DrawString(text, font, brush, new PointF(x, y));
                                    }
                                }
                                bmpTemp = (Bitmap)bmp.Clone();
                                Invalidate();
                            }
                        }
                    }
                }
                // Инструмент "Заливка"
                else if (MainForm.CurrentTool == Tools.Fill)
                {
                    FloodFill(bmp, new Point(x, y), MainForm.CurrentColor);
                    bmpTemp = (Bitmap)bmp.Clone();
                    Invalidate();
                }
            }
        }


        public void LoadImage(Bitmap newBmp)
        {
            if (newBmp != null)
            {
                bmp = new Bitmap(newBmp);
                bmpTemp = new Bitmap(newBmp);
                this.BackgroundImage = bmp;
                SaveState();
                Invalidate();
            }
        }
        public void ScaleImage(float factor)
        {
            scaleFactor *= factor;
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (bmpTemp != null)
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                int newWidth = (int)(bmpTemp.Width * scaleFactor);
                int newHeight = (int)(bmpTemp.Height * scaleFactor);
                e.Graphics.DrawImage(bmpTemp, new Rectangle(0, 0, newWidth, newHeight));
            }
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpg)|*.jpg|Bitmap Files (*.bmp)|*.bmp";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        currentFilePath = saveFileDialog.FileName;
                        SaveToFile(currentFilePath);
                    }
                }
            }
            else
            {
                SaveToFile(currentFilePath);
            }
        }
        private void SaveToFile(string path)
        {
            ImageFormat format = ImageFormat.Png;
            if (path.EndsWith(".jpg"))
                format = ImageFormat.Jpeg;
            else if (path.EndsWith(".bmp"))
                format = ImageFormat.Bmp;

            bmp.Save(path, format);
        }
        public void SaveAsPNG(string path)
        {
            bmp.Save(path, ImageFormat.Png);
        }
        public void SaveAsJPG(string path)
        {
            bmp.Save(path, ImageFormat.Jpeg);
        }
        public void SaveAsBMP(string path)
        {
            bmp.Save(path, ImageFormat.Bmp);
        }

        private void SaveState()
        {
            // Добавляем текущее изображение в стек "Назад"
            undoStack.Push((Bitmap)bmp.Clone());
            // При новом действии сбрасываем "Вперёд"
            redoStack.Clear();
        }
        public void Undo()
        {
            if (undoStack.Count > 1) // Оставляем хотя бы одно состояние
            {
                redoStack.Push(undoStack.Pop()); // Переносим текущее состояние в "Вперёд"
                bmp = (Bitmap)undoStack.Peek().Clone();
                bmpTemp = bmp;
                Invalidate();
            }
        }
        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                bmp = (Bitmap)redoStack.Pop().Clone();
                bmpTemp = bmp;
                undoStack.Push((Bitmap)bmp.Clone()); // Добавляем обратно в "Назад"
                Invalidate();
            }
        }

        private void DrawHeart(Graphics g, Pen pen, int x1, int y1, int x2, int y2)
        {
            int numPoints = 100;
            PointF[] points = new PointF[numPoints];

            // Определяем границы
            int left = Math.Min(x1, x2);
            int right = Math.Max(x1, x2);
            int top = Math.Min(y1, y2);
            int bottom = Math.Max(y1, y2);

            int width = right - left;
            int height = bottom - top;

            if (width < 10 || height < 10) return; // Предотвращаем слишком маленькие размеры

            float scaleX = width / 2f;
            float scaleY = height / 2f;
            float centerX = left + scaleX;
            float centerY = top + scaleY;

            // Вычисляем точки сердца
            for (int i = 0; i < numPoints; i++)
            {
                double t = Math.PI * 2 * i / numPoints;
                float px = (float)(16 * Math.Sin(t) * Math.Sin(t) * Math.Sin(t));
                float py = (float)(13 * Math.Cos(t) - 5 * Math.Cos(2 * t) - 2 * Math.Cos(3 * t) - Math.Cos(4 * t));

                points[i] = new PointF(centerX + px * scaleX / 16, centerY - py * scaleY / 17);
            }

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddClosedCurve(points);
                g.DrawPath(pen, path);
            }
        }
        private void FloodFill(Bitmap bmp, Point pt, Color newColor)
        {
            Color targetColor = bmp.GetPixel(pt.X, pt.Y);
            if (targetColor.ToArgb() == newColor.ToArgb()) return; // Если цвета совпадают, не заливаем

            Stack<Point> pixels = new Stack<Point>();
            pixels.Push(pt);

            while (pixels.Count > 0)
            {
                Point temp = pixels.Pop();
                if (temp.X < 0 || temp.X >= bmp.Width || temp.Y < 0 || temp.Y >= bmp.Height)
                    continue;

                if (bmp.GetPixel(temp.X, temp.Y) == targetColor)
                {
                    bmp.SetPixel(temp.X, temp.Y, newColor);

                    pixels.Push(new Point(temp.X - 1, temp.Y)); // Влево
                    pixels.Push(new Point(temp.X + 1, temp.Y)); // Вправо
                    pixels.Push(new Point(temp.X, temp.Y - 1)); // Вверх
                    pixels.Push(new Point(temp.X, temp.Y + 1)); // Вниз
                }
            }
        }
        private void UpdateCursor()
        {
            if (MainForm.ToolCursors.ContainsKey(MainForm.CurrentTool))
            {
                string cursorPath = MainForm.ToolCursors[MainForm.CurrentTool];
                if (System.IO.File.Exists(cursorPath))
                {
                    this.Cursor = new Cursor(new Bitmap(cursorPath).GetHicon());
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            UpdateCursor();
        }
    }
}
