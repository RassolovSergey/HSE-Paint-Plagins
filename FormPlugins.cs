using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PluginInterface;

namespace WinForms_v1
{
    public partial class FormPlugins: Form
    {
        private Bitmap _image;  // Поле для хранения изображения
        Dictionary<string, IPlugin> plugins = new Dictionary<string, IPlugin>();

        public FormPlugins()
        {
            InitializeComponent();

            FindPlugins();
            CreatePluginsMenu();
        }

        // Конструктор, который принимает изображение
        public FormPlugins(Bitmap image)
        {
            InitializeComponent();
            _image = image ?? throw new ArgumentNullException(nameof(image), "Изображение не передано!");
            pictureBox1.Image = _image;  // Показываем изображение на форме
        }

        // Метод для получения обновленного изображения
        public Bitmap GetUpdatedImage()
        {
            return (Bitmap)pictureBox1.Image;  // Возвращаем обновленное изображение
        }

        void FindPlugins()
        {
            // папка с плагинами
            string folder = System.AppDomain.CurrentDomain.BaseDirectory;

            // dll-файлы в этой папке
            string[] files = Directory.GetFiles(folder, "*.dll");

            foreach (string file in files)
                try
                {
                    Assembly assembly = Assembly.LoadFile(file);

                    foreach (Type type in assembly.GetTypes())
                    {
                        Type iface = type.GetInterface("PluginInterface.IPlugin");

                        if (iface != null)
                        {
                            IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                            plugins.Add(plugin.Name, plugin);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки плагина\n" + ex.Message);
                }
        }
        private void CreatePluginsMenu()
        {
            foreach (var p in plugins)
            {
                var item = фильтрыToolStripMenuItem.DropDownItems.Add(p.Value.Name);
                item.Click += OnPluginClick;
            }
        }
        private void OnPluginClick(object sender, EventArgs args)
        {
            IPlugin plugin = plugins[((ToolStripMenuItem)sender).Text];
            plugin.PluginMethod((Bitmap)pictureBox1.Image);
            pictureBox1.Refresh();

        }
    }
}
