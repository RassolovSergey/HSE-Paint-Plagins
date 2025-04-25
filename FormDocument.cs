using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;

namespace WinForms_v1
{
    // Класс FormDocument наследуется от DockContent, что позволяет использовать его как дочернюю форму в DockPanel
    public partial class FormDocument : DockContent
    {
        private int x, y;           // Переменные для хранения текущих координат мыши
        private Bitmap bmp;         // Основное изображение, которое редактируется
        private Bitmap bmpTemp;     // Временная копия изображения для инструментов, таких как "Ластик", "Круг" и т.д.
        private string currentFilePath = null;  // Путь к текущему файлу (если файл был сохранён)
        private float scaleFactor = 1.0f;       // Коэффициент масштабирования

        // Стек для отмены изменений (Undo)
        private Stack<Bitmap> undoStack = new Stack<Bitmap>();

        // Стек для возврата изменений (Redo)
        private Stack<Bitmap> redoStack = new Stack<Bitmap>();

                            // КОНСТРУКТОРЫ

        // НОВОЕ
        // Конструктор, который добавляет обработчики для перетаскивания
        public FormDocument()
        {
            InitializeComponent();
            bmp = new Bitmap(ClientSize.Width, ClientSize.Height);  // Убедитесь, что bmp инициализирован
            bmpTemp = bmp;  // Инициализируем bmpTemp с bmp
            SaveState();  // Сохраняем начальное состояние изображения
        }



        // Конструктор с параметром Bitmap для загрузки уже существующего изображения
        public FormDocument(Bitmap bmp)
        {   
            InitializeComponent();          // Инициализация компонентов формы
            this.bmp = new Bitmap(bmp);     // Копируем переданное изображение
            this.bmpTemp = new Bitmap(bmp);     // Создаем временную копию
            this.BackgroundImage = this.bmp;    // Устанавливаем изображение в качестве фона
            SaveState(); // Сохраняем начальное состояние
        }



                            // ОБРАБОТЧИКИ

        // Обработчик события движения мыши
        private void FormDocument_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Если нажата левая кнопка мыши
            {
                var pen = new Pen(MainForm.CurrentColor, MainForm.CurrentWidth); // Создаём перо с текущим цветом и шириной
                var eraser = new Pen(Color.White, MainForm.CurrentWidth); // Ластик использует белый цвет

                // В зависимости от выбранного инструмента выполняем различные действия
                switch (MainForm.CurrentTool)
                {
                    case Tools.Pen: // Инструмент "Кисть"
                        var g = Graphics.FromImage(bmp); // Создаём графический объект для рисования
                        g.DrawLine(pen, x, y, e.X, e.Y); // Рисуем линию
                        x = e.X; // Обновляем координаты
                        y = e.Y;
                        bmpTemp = bmp; // Обновляем временную копию изображения
                        Invalidate(); // Перерисовываем форму
                        break;
                    case Tools.Eraser: // Инструмент "Ластик"
                        g = Graphics.FromImage(bmp);
                        using (SolidBrush eraserBrush = new SolidBrush(Color.White)) // Используем белый цвет для ластика
                        {
                            g.FillEllipse(eraserBrush, e.X - MainForm.CurrentWidth / 2, e.Y - MainForm.CurrentWidth / 2,
                                          MainForm.CurrentWidth, MainForm.CurrentWidth); // Рисуем круглый ластик
                        }
                        x = e.X;
                        y = e.Y;
                        bmpTemp = bmp;
                        Invalidate();
                        break;
                    case Tools.Circle: // Инструмент "Круг"
                        bmpTemp = (Bitmap)bmp.Clone(); // Создаём копию изображения
                        g = Graphics.FromImage(bmpTemp);
                        g.DrawEllipse(pen, new Rectangle(x, y, e.X - x, e.Y - y)); // Рисуем круг
                        Invalidate();
                        break;
                    case Tools.Line: // Инструмент "Линия"
                        bmpTemp = (Bitmap)bmp.Clone();
                        g = Graphics.FromImage(bmpTemp);
                        g.DrawLine(pen, x, y, e.X, e.Y); // Рисуем линию
                        Invalidate();
                        break;
                    case Tools.Heart: // Инструмент "Сердце"
                        bmpTemp = (Bitmap)bmp.Clone();
                        g = Graphics.FromImage(bmpTemp);
                        DrawHeart(g, pen, x, y, e.X, e.Y); // Рисуем сердце
                        Invalidate();
                        break;
                }
            }
        }

        // Обработчик события отпускания кнопки мыши
        private void FormDocument_MouseUp(object sender, MouseEventArgs e)
        {
            // Если был выбран один из инструментов, сохраняем изображение и обновляем форму
            if (MainForm.CurrentTool == Tools.Circle || MainForm.CurrentTool == Tools.Line || MainForm.CurrentTool == Tools.Heart || MainForm.CurrentTool == Tools.Eraser)
            {
                bmp = bmpTemp; // Сохраняем временное изображение в основное
                SaveState(); // Сохраняем состояние
                Invalidate(); // Перерисовываем форму
            }
        }

        // Обработчик события нажатия кнопки мыши
        private void FormDocument_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Если нажата левая кнопка мыши
            {
                x = e.X; // Запоминаем начальную точку
                y = e.Y;
                SaveState(); // Сохраняем состояние перед изменением

                // Если выбран инструмент "Текст", открываем окно ввода
                if (MainForm.CurrentTool == Tools.Text)
                {
                    using (var inputBox = new TextInputForm()) // Окно ввода текста
                    {
                        if (inputBox.ShowDialog() == DialogResult.OK)
                        {
                            string text = inputBox.InputText; // Получаем введённый текст
                            if (!string.IsNullOrEmpty(text))
                            {
                                using (Graphics g = Graphics.FromImage(bmp))
                                {
                                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                                    using (Font font = new Font("Arial", 14, FontStyle.Bold))
                                    using (SolidBrush brush = new SolidBrush(MainForm.CurrentColor))
                                    {
                                        g.DrawString(text, font, brush, new PointF(x, y)); // Рисуем текст
                                    }
                                }
                                bmpTemp = (Bitmap)bmp.Clone(); // Сохраняем копию
                                Invalidate(); // Обновляем форму
                            }
                        }
                    }
                }
                // Если выбран инструмент "Заливка", выполняем заливку
                else if (MainForm.CurrentTool == Tools.Fill)
                {
                    FloodFill(bmp, new Point(x, y), MainForm.CurrentColor); // Заливаем область
                    bmpTemp = (Bitmap)bmp.Clone(); // Сохраняем копию
                    Invalidate(); // Обновляем форму
                }
            }
        }



                        // СОХРАНЕНИЕ ФАЙЛОВ

        // Метод для сохранения изображения
        public void Save()
        {
            if (string.IsNullOrEmpty(currentFilePath)) // Если файл не был сохранён ранее
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog()) // Открываем диалог сохранения
                {
                    saveFileDialog.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpg)|*.jpg|Bitmap Files (*.bmp)|*.bmp"; // Фильтр форматов
                    if (saveFileDialog.ShowDialog() == DialogResult.OK) // Если пользователь выбрал файл
                    {
                        currentFilePath = saveFileDialog.FileName; // Сохраняем путь
                        SaveToFile(currentFilePath); // Сохраняем изображение
                    }
                }
            }
            else
            {
                SaveToFile(currentFilePath); // Если файл уже сохранён, сохраняем по текущему пути
            }
        }

        // Метод для сохранения изображения в файл
        private void SaveToFile(string path)
        {
            ImageFormat format = ImageFormat.Png; // По умолчанию формат PNG
            if (path.EndsWith(".jpg")) // Если расширение файла .jpg
                format = ImageFormat.Jpeg; // Используем формат JPEG
            else if (path.EndsWith(".bmp")) // Если расширение файла .bmp
                format = ImageFormat.Bmp; // Используем формат BMP

            bmp.Save(path, format); // Сохраняем изображение в выбранном формате
        }

        // Метод для сохранения изображения в формате PNG
        public void SaveAsPNG(string path)
        {
            bmp.Save(path, ImageFormat.Png); // Сохраняем изображение в формате PNG
        }

        // Метод для сохранения изображения в формате JPEG
        public void SaveAsJPG(string path)
        {
            bmp.Save(path, ImageFormat.Jpeg); // Сохраняем изображение в формате JPEG
        }

        // Метод для сохранения изображения в формате BMP
        public void SaveAsBMP(string path)
        {
            bmp.Save(path, ImageFormat.Bmp); // Сохраняем изображение в формате BMP
        }


                            // СОСТОЯНИЕ ИЗОБРАЖЕНИЯ

        // Метод для сохранения состояния изображения (для Undo)
        private void SaveState()
        {
            if (bmp != null)
            {
                undoStack.Push((Bitmap)bmp.Clone());  // Сохраняем текущее состояние изображения
                redoStack.Clear();  // Очищаем стек "Вперёд"
            }
            else
            {
                // Обработка случая, если bmp не инициализирован
                MessageBox.Show("Изображение не инициализировано!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для отмены последнего действия (Undo)
        public void Undo()
        {
            if (undoStack.Count > 1) // Оставляем хотя бы одно состояние
            {
                redoStack.Push(undoStack.Pop()); // Переносим текущее состояние в "Вперёд"
                bmp = (Bitmap)undoStack.Peek().Clone(); // Восстанавливаем последнее состояние из стека "Назад"
                bmpTemp = bmp; // Обновляем временную копию
                Invalidate(); // Перерисовываем форму
            }
        }

        // Метод для возврата последнего отменённого действия (Redo)
        public void Redo()
        {
            if (redoStack.Count > 0) // Если есть действия для возврата
            {
                bmp = (Bitmap)redoStack.Pop().Clone(); // Восстанавливаем состояние из стека "Вперёд"
                bmpTemp = bmp; // Обновляем временную копию
                undoStack.Push((Bitmap)bmp.Clone()); // Добавляем обратно в "Назад"
                Invalidate(); // Перерисовываем форму
            }
        }


                        // МЕТОДЫ ДЛЯ ИНСТРУМЕНТОВ

        // Метод для рисования сердца
        private void DrawHeart(Graphics g, Pen pen, int x1, int y1, int x2, int y2)
        {
            int numPoints = 100; // Количество точек для рисования
            PointF[] points = new PointF[numPoints]; // Массив точек для рисования сердца

            // Определяем границы для сердца
            int left = Math.Min(x1, x2);
            int right = Math.Max(x1, x2);
            int top = Math.Min(y1, y2);
            int bottom = Math.Max(y1, y2);

            int width = right - left;
            int height = bottom - top;

            if (width < 10 || height < 10) return; // Если размеры слишком маленькие, не рисуем

            float scaleX = width / 2f;
            float scaleY = height / 2f;
            float centerX = left + scaleX;
            float centerY = top + scaleY;

            // Вычисляем точки для рисования сердца
            for (int i = 0; i < numPoints; i++)
            {
                double t = Math.PI * 2 * i / numPoints;
                float px = (float)(16 * Math.Sin(t) * Math.Sin(t) * Math.Sin(t));
                float py = (float)(13 * Math.Cos(t) - 5 * Math.Cos(2 * t) - 2 * Math.Cos(3 * t) - Math.Cos(4 * t));

                points[i] = new PointF(centerX + px * scaleX / 16, centerY - py * scaleY / 17);
            }

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddClosedCurve(points); // Создаём путь для сердца
                g.DrawPath(pen, path); // Рисуем сердце
            }
        }

        // Метод для заливки области
        private void FloodFill(Bitmap bmp, Point pt, Color newColor)
        {
            Color targetColor = bmp.GetPixel(pt.X, pt.Y); // Получаем исходный цвет
            if (targetColor.ToArgb() == newColor.ToArgb()) return; // Если цвет совпадает, не заливаем

            Stack<Point> pixels = new Stack<Point>(); // Стек для заливки
            pixels.Push(pt);

            while (pixels.Count > 0) // Пока есть точки для заливки
            {
                Point temp = pixels.Pop();
                if (temp.X < 0 || temp.X >= bmp.Width || temp.Y < 0 || temp.Y >= bmp.Height)
                    continue; // Если точка выходит за пределы изображения, пропускаем её

                if (bmp.GetPixel(temp.X, temp.Y) == targetColor) // Если пиксель соответствует целевому цвету
                {
                    bmp.SetPixel(temp.X, temp.Y, newColor); // Заменяем пиксель на новый цвет

                    // Добавляем соседние пиксели в стек
                    pixels.Push(new Point(temp.X - 1, temp.Y)); // Влево
                    pixels.Push(new Point(temp.X + 1, temp.Y)); // Вправо
                    pixels.Push(new Point(temp.X, temp.Y - 1)); // Вверх
                    pixels.Push(new Point(temp.X, temp.Y + 1)); // Вниз
                }
            }
        }

                        // МЕТОДЫ ДЛЯ ИЗОБРАЖЕНИЯ

        // Метод для загрузки изображения
        public void LoadImage(Bitmap newBmp)
        {
            if (newBmp != null)     // Если изображение не пустое
            {
                bmp = new Bitmap(newBmp);       // Копируем новое изображение
                bmpTemp = new Bitmap(newBmp);   // Создаём временную копию
                this.BackgroundImage = bmp;     // Устанавливаем его в качестве фона
                SaveState();    // Сохраняем состояние
                Invalidate();   // Обновляем форму
            }
        }

        // Метод для получения текущего изображения
        public Bitmap GetImage()
        {
            return bmp; // Возвращаем текущее изображение
        }


                        // МЕТОДЫ ДЛЯ КУРСОРА

        // Метод для обновления курсора в зависимости от выбранного инструмента
        private void UpdateCursor()
        {
            if (MainForm.ToolCursors.ContainsKey(MainForm.CurrentTool)) // Проверяем, есть ли курсор для текущего инструмента
            {
                string cursorPath = MainForm.ToolCursors[MainForm.CurrentTool]; // Получаем путь к файлу курсора
                if (System.IO.File.Exists(cursorPath)) // Если файл существует
                {
                    this.Cursor = new Cursor(new Bitmap(cursorPath).GetHicon()); // Устанавливаем новый курсор
                }
                else
                {
                    this.Cursor = Cursors.Default; // Если файл не найден, устанавливаем стандартный курсор
                }
            }
        }

        // Переопределение метода активации формы для обновления курсора
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            UpdateCursor(); // Обновляем курсор при активации формы
        }


                        // МЕТОДЫ ДЛЯ РАБОТЫ С ПЛАГИНАМИ

        // Метод для обновления после использования плагина
        public void UpdateAfterPlugin()
        {
            bmpTemp = (Bitmap)bmp.Clone(); // Создаём копию изображения
            Invalidate(); // Перерисовываем форму
            SaveState(); // Сохраняем состояние
        }

                        // ПРОЧЕЕ

        // Метод для масштабирования изображения
        public void ScaleImage(float factor)
        {
            scaleFactor *= factor; // Увеличиваем или уменьшаем коэффициент масштабирования
            Invalidate(); // Перерисовываем форму, чтобы отобразить изменённый размер
        }

        // Переопределение метода для отрисовки на форме
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Вызываем базовый метод для стандартной отрисовки
            if (bmpTemp != null) // Если временная картинка не пуста
            {
                // Устанавливаем режим интерполяции для качественного масштабирования
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                int newWidth = (int)(bmpTemp.Width * scaleFactor); // Новая ширина с учётом масштаба
                int newHeight = (int)(bmpTemp.Height * scaleFactor); // Новая высота с учётом масштаба
                                                                     // Рисуем изображение с новым размером
                e.Graphics.DrawImage(bmpTemp, new Rectangle(0, 0, newWidth, newHeight));
            }
        }


        // Обработчик для события DragEnter
        private void FormDocument_DragEnter(object sender, DragEventArgs e)
        {
            // Проверяем, поддерживает ли перетаскиваемый объект изображение
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;  // Разрешаем копирование файла
            }
            else
            {
                e.Effect = DragDropEffects.None;  // Отменяем перетаскивание, если тип данных не поддерживается
            }
        }

        // Обработчик для события DragDrop
        private void FormDocument_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                string filePath = files[0];

                // Проверка на допустимые форматы изображений
                string[] validExtensions = { ".jpg", ".jpeg", ".png", ".bmp" };
                string extension = Path.GetExtension(filePath).ToLower();

                if (Array.Exists(validExtensions, ext => ext == extension))
                {
                    try
                    {
                        Bitmap loadedImage = new Bitmap(filePath);
                        this.LoadImage(loadedImage);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при загрузке изображения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Недопустимый формат файла!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

    }
}
