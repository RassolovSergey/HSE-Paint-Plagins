using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Reflection;
using PluginInterface;

namespace WinForms_v1
{
    public partial class MainForm : Form
    {
        // Статические свойства для настройки текущего цвета, размера и типа инструмента.
        public static Color CurrentColor { get; set; }      // Цвет кисти
        public static int CurrentWidth { get; set; }        // Размер кисти
        public static Tools CurrentTool { get; set; }       // Тип инструмента
        public static bool IsFilled { get; set; } = false;  // Заполненность фигуры (по умолчанию false)

        // Словарь для хранения путей к изображениям курсоров для разных инструментов
        public static readonly Dictionary<Tools, string> ToolCursors = new Dictionary<Tools, string>
        {
            { Tools.Pen, @"C:\Users\NPC\Documents\Code - Projects\Software Design\WinForms_v1\WinForms_v1\Assets\Cursor\Pen.png" },         // Курсор для инструмента "Кисть"
            { Tools.Eraser, @"C:\Users\NPC\Documents\Code - Projects\Software Design\WinForms_v1\WinForms_v1\Assets\Cursor\Eraser.png" },   // Курсор для инструмента "Ластик"
            { Tools.Line, @"C:\Users\NPC\Documents\Code - Projects\Software Design\WinForms_v1\WinForms_v1\Assets\Line.png" },              // Курсор для инструмента "Линия"
            { Tools.Circle, @"C:\Users\NPC\Documents\Code - Projects\Software Design\WinForms_v1\WinForms_v1\Assets\Cursor\Circle.png" },   // Курсор для инструмента "Круг"
            { Tools.Heart, @"C:\Users\NPC\Documents\Code - Projects\Software Design\WinForms_v1\WinForms_v1\Assets\Heart.png" },            // Курсор для инструмента "Сердце"
            { Tools.Text, @"C:\Users\NPC\Documents\Code - Projects\Software Design\WinForms_v1\WinForms_v1\Assets\Cursor\Text.png" },       // Курсор для инструмента "Текст"
            { Tools.Fill, @"C:\Users\NPC\Documents\Code - Projects\Software Design\WinForms_v1\WinForms_v1\Assets\Cursor\ColorBucket.png" } // Курсор для инструмента "Заливка"
        };


        public static Dictionary<string, IPlugin> plugins = new Dictionary<string, IPlugin>();

        // Панель для отображения окон (документов), поддерживающая Dock-свойства.
        private DockPanel dockPanel;

        // Конструктор формы MainForm
        public MainForm()
        {
            InitializeComponent();

            // Создаём DockPanel, который будет заполнять всё доступное пространство.
            dockPanel = new DockPanel
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(dockPanel);  // Добавляем панель в контролы формы

            // Инициализация начальных значений для кисти
            CurrentColor = Color.Black; // Цвет кисти по умолчанию — чёрный
            CurrentWidth = 5;           // Размер кисти по умолчанию — 5px

            FindPlugins();         // Поиск и загрузка плагинов
            CreatePluginsMenu();   // Создание меню с пунктами для каждого плагина
        }

                                // ЦВЕТА
        // "Цвет - Крассный"
        // Обработчик события для изменения цвета на красный
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Red;   // Устанавливаем красный цвет кисти
        }
        // "Цвет - Синий"
        // Обработчик события для изменения цвета на синий
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Blue;  // Устанавливаем синий цвет кисти
        }
        // "Цвет - Зелёный"
        // Обработчик события для изменения цвета на зелёный
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Green; // Устанавливаем зелёный цвет кисти
        }
        // "Цвет - Черный"
        // Обработчик события для изменения цвета на чёрный
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Black; // Устанавливаем чёрный цвет кисти
        }
        // "Цветовая палитра"
        // Обработчик события для открытия диалога выбора цвета
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            var сolorDialog = new ColorDialog();                // Создаём диалог для выбора цвета
            if (сolorDialog.ShowDialog() == DialogResult.OK)    // Если пользователь выбрал цвет
            { CurrentColor = сolorDialog.Color; }   // Устанавливаем выбранный цвет кисти
        }


                                 // РАЗМЕРЫ КИСТИ

        // Обработчик события для установки размера кисти 1px
        private void pxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentWidth = 1;  // Устанавливаем размер кисти на 1px
        }

        // Обработчик события для установки размера кисти 5px
        private void pxToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CurrentWidth = 5;  // Устанавливаем размер кисти на 5px
        }

        // Обработчик события для установки размера кисти 10px
        private void pxToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            CurrentWidth = 10;  // Устанавливаем размер кисти на 10px
        }

        // Обработчик события для установки размера кисти 30px
        private void pxToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            CurrentWidth = 30;  // Устанавливаем размер кисти на 30px
        }


                                // ИНСТРУМЕНТЫ

        // Обработчик события для установки инструмента "Кисть"
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Pen;  // Устанавливаем текущий инструмент как кисть
            UpdateCursor();  // Обновляем курсор в зависимости от выбранного инструмента
        }

        // Обработчик события для установки инструмента "Линия"
        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Line;  // Устанавливаем текущий инструмент как линию
            UpdateCursor();  // Обновляем курсор
        }

        // Обработчик события для установки инструмента "Круг"
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Circle;  // Устанавливаем текущий инструмент как круг
            UpdateCursor();  // Обновляем курсор
        }

        // "Ластик"
        // Обработчик события для выбора инструмента "Ластик"
        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            // Устанавливаем текущий инструмент как "Ластик"
            CurrentTool = Tools.Eraser;
            // Обновляем курсор в зависимости от выбранного инструмента
            UpdateCursor();
        }

        // "Сердце"
        // Обработчик события для выбора инструмента "Сердце"
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            // Устанавливаем текущий инструмент как "Сердце"
            CurrentTool = Tools.Heart;
            // Обновляем курсор
            UpdateCursor();
        }

        // "Текст"
        // Обработчик события для выбора инструмента "Текст"
        private void TextButton_Click(object sender, EventArgs e)
        {
            // Устанавливаем текущий инструмент как "Текст"
            CurrentTool = Tools.Text;
            // Обновляем курсор
            UpdateCursor();
        }

        // "Заливка"
        // Обработчик события для выбора инструмента "Заливка"
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            // Устанавливаем текущий инструмент как "Заливка"
            CurrentTool = Tools.Fill;
            // Обновляем курсор
            UpdateCursor();
        }

        // "Закрашивать фигуру"
        // Обработчик события для флага "Закрашивать фигуру"
        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            // Устанавливаем флаг, что фигуры должны быть закрашенными
            IsFilled = true;
        }

        // "Не закрашивать фигуру"
        // Обработчик события для флага "Не закрашивать фигуру"
        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            // Устанавливаем флаг, что фигуры не должны быть закрашенными
            IsFilled = false;
        }



                                // РАСПОЛОЖЕНИЕ ОКОН

        // Упорядочить окна Каскадом
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        // Упорядочить окна по Горизонтали 
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        // Упорядочить окна по Вертикали
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        // Упорядочить окна - Свернутые
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }


                                // СОХАРНЕНИЕ

        // Сохранить
        // Обработчик события для сохранения текущего документа
        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, что активное дочернее окно является экземпляром FormDocument
            if (ActiveMdiChild is FormDocument doc)
            {
                // Вызываем метод Save() для сохранения документа
                doc.Save();
            }
        }

        // "Сохранить как..." - JPG
        // Обработчик события для сохранения документа в формате JPG
        private void SaveJPGMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, что активное дочернее окно является экземпляром FormDocument
            if (ActiveMdiChild is FormDocument doc)
            {
                // Создаём диалоговое окно для сохранения файла
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    // Устанавливаем фильтр для выбора только файлов .jpg
                    saveFileDialog.Filter = "JPEG Files (*.jpg)|*.jpg";
                    // Показываем диалоговое окно и проверяем результат
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Вызываем метод SaveAsJPG() для сохранения документа в формате JPG
                        doc.SaveAsJPG(saveFileDialog.FileName);
                    }
                }
            }
        }

        // "Сохранить как..." - BMP
        // Обработчик события для сохранения документа в формате BMP
        private void SaveBMPMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, что активное дочернее окно является экземпляром FormDocument
            if (ActiveMdiChild is FormDocument doc)
            {
                // Создаём диалоговое окно для сохранения файла
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    // Устанавливаем фильтр для выбора только файлов .bmp
                    saveFileDialog.Filter = "Bitmap Files (*.bmp)|*.bmp";
                    // Показываем диалоговое окно и проверяем результат
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Вызываем метод SaveAsBMP() для сохранения документа в формате BMP
                        doc.SaveAsBMP(saveFileDialog.FileName);
                    }
                }
            }
        }

        // "Сохранить как..." - PNG
        // Обработчик события для сохранения документа в формате PNG
        private void изображениеВФорматеPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, что активное дочернее окно является экземпляром FormDocument
            if (ActiveMdiChild is FormDocument doc)
            {
                // Создаём диалоговое окно для сохранения файла
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    // Устанавливаем фильтр для выбора только файлов .png
                    saveFileDialog.Filter = "PNG Files (*.png)|*.png";
                    // Показываем диалоговое окно и проверяем результат
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Вызываем метод SaveAsPNG() для сохранения документа в формате PNG
                        doc.SaveAsPNG(saveFileDialog.FileName);
                    }
                }
            }
        }

        // "Сохранить" - Документ
        // Обработчик события для сохранения текущего документа
        private void сохранитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Проверяем, что активное дочернее окно является экземпляром FormDocument
            if (ActiveMdiChild is FormDocument doc)
            {
                // Сохраняем текущий документ
                doc.Save();
            }
        }

        // "Сохранить" - Документ
        // Обработчик события для сохранения текущего документа (кнопка на панели инструментов)
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            // Проверяем, что активное дочернее окно является экземпляром FormDocument
            if (ActiveMdiChild is FormDocument doc)
            {
                // Сохраняем текущий документ
                doc.Save();
            }
        }


                                // ДОБАВЛЕНИЕ ИЗРБРАЖЕНИЙ

        // "Откарыть"
        // Обработчик события для открытия файла изображения
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Создаём диалоговое окно для выбора файла изображения
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Устанавливаем фильтр для выбора файлов изображений (.bmp, .jpg, .png)
                openFileDialog.Filter = "Изображения|*.bmp;*.jpg;*.png";
                openFileDialog.Title = "Открыть изображение"; // Устанавливаем заголовок окна

                // Показываем диалоговое окно и проверяем результат
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Загружаем выбранное изображение в объект Bitmap
                        Bitmap bmp = new Bitmap(openFileDialog.FileName);
                        // Создаём новый экземпляр формы для отображения документа
                        var doc = new FormDocument(bmp);
                        // Устанавливаем родительскую форму для нового документа
                        doc.MdiParent = this;
                        // Показываем форму документа как дочернюю
                        doc.Show();
                    }
                    catch (Exception ex)
                    {
                        // Если возникла ошибка при загрузке изображения, выводим сообщение об ошибке
                        MessageBox.Show("Ошибка загрузки изображения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // "Откарыть"
        // Обработчик события для открытия файла изображения (кнопка на панели инструментов)
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            // Создаём диалоговое окно для выбора файла изображения
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Изображения|*.bmp;*.jpg;*.png";  // Устанавливаем фильтр для изображений
                openFileDialog.Title = "Открыть изображение";  // Заголовок окна

                // Показываем диалоговое окно и проверяем результат
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Загружаем выбранное изображение в объект Bitmap
                        Bitmap bmp = new Bitmap(openFileDialog.FileName);
                        // Создаём новый экземпляр формы для отображения документа
                        var doc = new FormDocument(bmp);
                        doc.MdiParent = this;  // Устанавливаем родительскую форму для нового документа
                        doc.Show();  // Показываем форму документа как дочернюю
                    }
                    catch (Exception ex)
                    {
                        // Если возникла ошибка при загрузке изображения, выводим сообщение об ошибке
                        MessageBox.Show("Ошибка загрузки изображения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // "Импорт" - BMP
        // Обработчик события для импорта файла BMP
        private void импортироватьФайлBMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, что активное дочернее окно является экземпляром FormDocument
            if (ActiveMdiChild is FormDocument doc)
            {
                // Создаём диалоговое окно для выбора файла BMP
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "BMP Files (*.bmp)|*.bmp";  // Устанавливаем фильтр для BMP файлов
                    openFileDialog.Title = "Импортировать BMP";  // Заголовок окна

                    // Показываем диалоговое окно и проверяем результат
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            // Загружаем выбранный BMP файл в объект Bitmap
                            Bitmap bmp = new Bitmap(openFileDialog.FileName);
                            // Загружаем изображение в документ
                            doc.LoadImage(bmp);
                        }
                        catch (Exception ex)
                        {
                            // Если возникла ошибка при загрузке BMP, выводим сообщение об ошибке
                            MessageBox.Show("Ошибка загрузки BMP: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                // Если нет активного документа, выводим предупреждение
                MessageBox.Show("Нет активного документа для импорта!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // "Импорт" - JPG
        // Обработчик события для импорта файла JPG
        private void импортироватьФайлJPGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, что активное дочернее окно является экземпляром FormDocument
            if (ActiveMdiChild is FormDocument doc)
            {
                // Создаём диалоговое окно для выбора файла JPG
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "JPEG Files (*.jpg)|*.jpg";  // Устанавливаем фильтр для JPG файлов
                    openFileDialog.Title = "Импортировать JPG";  // Заголовок окна

                    // Показываем диалоговое окно и проверяем результат
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            // Загружаем выбранный JPG файл в объект Bitmap
                            Bitmap bmp = new Bitmap(openFileDialog.FileName);
                            // Загружаем изображение в документ
                            doc.LoadImage(bmp);
                        }
                        catch (Exception ex)
                        {
                            // Если возникла ошибка при загрузке JPG, выводим сообщение об ошибке
                            MessageBox.Show("Ошибка загрузки JPG: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                // Если нет активного документа, выводим предупреждение
                MessageBox.Show("Нет активного документа для импорта!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        // "Создать"
        // Обработчик события для создания нового документа
        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var doc = new FormDocument();            // Создаём новый документ
            doc.Show(dockPanel, DockState.Document); // Открываем как вкладку
        }



                                    // МАШТАБ

        // Обработчик события для увеличения масштаба изображения на 10%
        private void маштаб10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, что активное дочернее окно является экземпляром FormDocument
            if (ActiveMdiChild is FormDocument doc)
            {
                // Увеличиваем масштаб изображения на 10%
                doc.ScaleImage(1.1f);
            }
        }

        // Обработчик события для уменьшения масштаба изображения на 10%
        private void маштаб10ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Проверяем, что активное дочернее окно является экземпляром FormDocument
            if (ActiveMdiChild is FormDocument doc)
            {
                // Уменьшаем масштаб изображения на 10%
                doc.ScaleImage(0.9f);
            }
        }

        // Обработчик события для уменьшения масштаба изображения на 10% (кнопка на панели инструментов)
        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            // Проверяем, что активное дочернее окно является экземпляром FormDocument
            if (ActiveMdiChild is FormDocument doc)
            {
                // Уменьшаем масштаб изображения на 10%
                doc.ScaleImage(0.9f);
            }
        }

        // Обработчик события для увеличения масштаба изображения на 10% (кнопка на панели инструментов)
        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            // Проверяем, что активное дочернее окно является экземпляром FormDocument
            if (ActiveMdiChild is FormDocument doc)
            {
                // Увеличиваем масштаб изображения на 10%
                doc.ScaleImage(1.1f);
            }
        }




        // "О Программе..."
        // Обработчик события для открытия окна "О программе"
        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var FormAbout = new FormAbout();  // Создаём экземпляр формы "О программе"
            FormAbout.ShowDialog();           // Показываем форму как модальное окно
        }


        // "Выход"
        // Обработчик события для выхода из приложения
        // Обработчик события для выхода из приложения
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Завершаем приложение
            Application.Exit();
        }

        // Обработчик события для отмены последнего действия (Undo)
        private void назадToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, что активное дочернее окно является экземпляром FormDocument
            if (ActiveMdiChild is FormDocument doc)
            {
                // Вызываем метод Undo() для отмены последнего действия
                doc.Undo();
            }
        }

        // Обработчик события для возврата последнего отменённого действия (Redo)
        private void вперёдToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем, что активное дочернее окно является экземпляром FormDocument
            if (ActiveMdiChild is FormDocument doc)
            {
                // Вызываем метод Redo() для возврата последнего отменённого действия
                doc.Redo();
            }
        }


                                // КОНФИГ            

        // Путь к файлу конфигурации, в котором сохраняется расположение окон
        private string layoutFile = "layout.xml"; // Файл конфигурации окон

        // Обработчик события загрузки формы
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Проверяем, существует ли файл конфигурации
            if (File.Exists(layoutFile))
            {
                // Загружаем расположение окон из конфигурационного файла
                dockPanel.LoadFromXml(layoutFile, delegate { return new FormDocument(); });
            }
        }
            

                                // ПРОЧЕЕ

        // Обработчик события закрытия формы
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Сохраняем расположение окон в конфигурационный файл при закрытии формы
            dockPanel.SaveAsXml(layoutFile);
        }

        // Обработчик нажатия на пункт меню плагина
        private void OnPluginClick(object sender, EventArgs e)
        {
            if (dockPanel.ActiveDocument is FormDocument activeDocument)
            {
                // Получаем имя плагина из текста пункта меню
                string pluginName = ((ToolStripMenuItem)sender).Text;

                // Проверяем, есть ли такой плагин
                if (plugins.TryGetValue(pluginName, out IPlugin plugin))
                {
                    // Применяем плагин к изображению в активном документе
                    plugin.PluginMethod(activeDocument.GetImage());

                    // Обновляем отображение и сохраняем состояние
                    activeDocument.UpdateAfterPlugin();
                }
            }
        }


                                // МЕТОДЫ            

        // Метод для обновления курсора в зависимости от выбранного инструмента
        private void UpdateCursor()
        {
            // Проверяем, что активное дочернее окно является экземпляром FormDocument и существует курсор для текущего инструмента
            if (ActiveMdiChild is FormDocument doc && ToolCursors.ContainsKey(CurrentTool))
            {
                // Получаем путь к изображению курсора для текущего инструмента
                string cursorPath = ToolCursors[CurrentTool];
                // Проверяем, существует ли файл курсора
                if (System.IO.File.Exists(cursorPath))
                {
                    // Создаём новый курсор из изображения и применяем его к документу
                    doc.Cursor = new Cursor(new Bitmap(cursorPath).GetHicon());
                }
                else
                {
                    // Если файл курсора не найден, устанавливаем курсор по умолчанию
                    doc.Cursor = Cursors.Default;
                }
            }
        }

        // Метод для поиска и загрузки плагинов из текущей директории
        void FindPlugins()
        {
            // Получение базовой директории приложения
            string folder = System.AppDomain.CurrentDomain.BaseDirectory;

            // Получение всех .dll-файлов в директории
            string[] files = Directory.GetFiles(folder, "*.dll");

            foreach (string file in files)
                try
                {
                    // Загрузка сборки (библиотеки) из файла
                    Assembly assembly = Assembly.LoadFile(file);

                    // Поиск всех типов в сборке
                    foreach (Type type in assembly.GetTypes())
                    {
                        // Проверка, реализует ли тип интерфейс IPlugin
                        Type iface = type.GetInterface("PluginInterface.IPlugin");

                        if (iface != null)
                        {
                            // Создание экземпляра плагина и добавление в словарь
                            IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                            plugins.Add(plugin.Name, plugin);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Отображение ошибки в случае неудачной загрузки плагина
                    MessageBox.Show("Ошибка загрузки плагина\n" + ex.Message);
                }
        }

        // Метод для создания меню плагинов на форме
        private void CreatePluginsMenu()
        {
            foreach (var p in plugins)
            {
                // Добавление элемента в подменю "фильтры"
                var item = фильтрыToolStripMenuItem.DropDownItems.Add(p.Value.Name);

                // Подписка на событие клика по пункту меню
                item.Click += OnPluginClick;
            }
        }

    }
}
