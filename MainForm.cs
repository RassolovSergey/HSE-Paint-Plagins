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
        // Словарь для хранения загруженных плагинов (по имени)
        Dictionary<string, IPlugin> plugins = new Dictionary<string, IPlugin>(); 

        // Цвет кисти
        public static Color CurrentColor { get; set; }
        // Размер кисти
        public static int CurrentWidth { get; set; }

        // Тип инструмента
        public static Tools CurrentTool { get; set; }

        public static bool IsFilled { get; set; } = false;

        private DockPanel dockPanel; // Панель для окон

        public MainForm()
        {
            InitializeComponent();

            // Создаём DockPanel
            dockPanel = new DockPanel
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(dockPanel);

            CurrentColor = Color.Black;
            CurrentWidth = 5;

            FindPlugins();         // Поиск и загрузка плагинов
            CreatePluginsMenu();   // Создание меню с пунктами для каждого плагина
        }

        // О Программе
        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new FormAbout();
            frm.ShowDialog();
        }
        // Создать
        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var doc = new FormDocument();
            doc.Show(dockPanel, DockState.Document); // Открываем как вкладку
        }
        // Цвет - Крассный
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Red;
        }
        // Цвет - Синий
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Blue;
        }
        // Цвет - Зелёный
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Green;
        }
        // Цвет - Черный
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            CurrentColor = Color.Black;
        }
        // Цветовая палитра
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            { CurrentColor = dlg.Color; }
        }

        // Размер - 1px
        private void pxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentWidth = 1;
        }
        // Размер - 5px
        private void pxToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CurrentWidth = 5;
        }
        // Размер - 10px
        private void pxToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            CurrentWidth = 10;
        }
        // Размер - 30px
        private void pxToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            CurrentWidth = 30;
        }
        // Выход
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Pen;
            UpdateCursor();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Line;
            UpdateCursor();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Circle;
            UpdateCursor();
        }

        // Окно - О Программе...
        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            var frm = new FormAbout();
            frm.ShowDialog();
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

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                doc.Save();
            }
        }

        private void SaveJPGMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "JPEG Files (*.jpg)|*.jpg";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        doc.SaveAsJPG(saveFileDialog.FileName);
                    }
                }
            }
        }

        private void SaveBMPMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Bitmap Files (*.bmp)|*.bmp";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        doc.SaveAsBMP(saveFileDialog.FileName);
                    }
                }
            }
        }

        private void изображениеВФорматеPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "PNG Files (*.png)|*.png";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        doc.SaveAsPNG(saveFileDialog.FileName);
                    }
                }
            }
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Изображения|*.bmp;*.jpg;*.png";
                openFileDialog.Title = "Открыть изображение";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Bitmap bmp = new Bitmap(openFileDialog.FileName);
                        var doc = new FormDocument(bmp);
                        doc.MdiParent = this;
                        doc.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка загрузки изображения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void импортироватьФайлBMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "BMP Files (*.bmp)|*.bmp";
                    openFileDialog.Title = "Импортировать BMP";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            Bitmap bmp = new Bitmap(openFileDialog.FileName);
                            doc.LoadImage(bmp);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка загрузки BMP: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Нет активного документа для импорта!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void импортироватьФайлJPGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "JPEG Files (*.jpg)|*.jpg";
                    openFileDialog.Title = "Импортировать JPG";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            Bitmap bmp = new Bitmap(openFileDialog.FileName);
                            doc.LoadImage(bmp);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка загрузки JPG: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Нет активного документа для импорта!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void сохранитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                doc.Save();
            }
        }

        private void маштаб10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                doc.ScaleImage(1.1f); // Увеличение масштаба на 10%
            }
        }

        private void маштаб10ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                doc.ScaleImage(0.9f); // Уменьшение масштаба на 10%
            }
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                doc.ScaleImage(0.9f); // Уменьшение масштаба на 10%
            }
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                doc.ScaleImage(1.1f); // Увеличение масштаба на 10%
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                doc.Save();
            }
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Изображения|*.bmp;*.jpg;*.png";
                openFileDialog.Title = "Открыть изображение";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Bitmap bmp = new Bitmap(openFileDialog.FileName);
                        var doc = new FormDocument(bmp);
                        doc.MdiParent = this;
                        doc.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка загрузки изображения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void назадToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                doc.Undo();
            }
        }

        private void вперёдToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormDocument doc)
            {
                doc.Redo();
            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Eraser; // Ластик
            UpdateCursor();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Heart;
            UpdateCursor();
        }

        private void TextButton_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Text;
            UpdateCursor();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            CurrentTool = Tools.Fill; // Заливка
            UpdateCursor();
        }

        // Флаг о том, что фигуры должны быть закрашенными
        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            IsFilled = true;
        }

        // Флаг о том, что фигуры не должны быть закрашенными
        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            IsFilled = false;
        }
        private void UpdateCursor()
        {
            if (ActiveMdiChild is FormDocument doc && ToolCursors.ContainsKey(CurrentTool))
            {
                string cursorPath = ToolCursors[CurrentTool];
                if (System.IO.File.Exists(cursorPath))
                {
                    doc.Cursor = new Cursor(new Bitmap(cursorPath).GetHicon());
                }
                else
                {
                    doc.Cursor = Cursors.Default;
                }
            }
        }


        public static readonly Dictionary<Tools, string> ToolCursors = new Dictionary<Tools, string>
        {
            { Tools.Pen, @"C:\Users\NPC\Documents\Code - Projects\Software Design\WinForms_v1\WinForms_v1\Assets\Cursor\Pen.png" },
            { Tools.Eraser, @"C:\Users\NPC\Documents\Code - Projects\Software Design\WinForms_v1\WinForms_v1\Assets\Cursor\Eraser.png" },
            { Tools.Line, @"C:\Users\NPC\Documents\Code - Projects\Software Design\WinForms_v1\WinForms_v1\Assets\Line.png" },
            { Tools.Circle, @"C:\Users\NPC\Documents\Code - Projects\Software Design\WinForms_v1\WinForms_v1\Assets\Cursor\Circle.png" },
            { Tools.Heart, @"C:\Users\NPC\Documents\Code - Projects\Software Design\WinForms_v1\WinForms_v1\Assets\Heart.png" },
            { Tools.Text, @"C:\Users\NPC\Documents\Code - Projects\Software Design\WinForms_v1\WinForms_v1\Assets\Cursor\Text.png" },
            { Tools.Fill, @"C:\Users\NPC\Documents\Code - Projects\Software Design\WinForms_v1\WinForms_v1\Assets\Cursor\ColorBucket.png" }
        };

        private string layoutFile = "layout.xml"; // Файл конфигурации окон


        private void MainForm_Load(object sender, EventArgs e)
        {
            if (File.Exists(layoutFile))
            {
                dockPanel.LoadFromXml(layoutFile, delegate { return new FormDocument(); });
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            dockPanel.SaveAsXml(layoutFile);
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
    }
}
