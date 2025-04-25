using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using PluginInterface;

namespace WinForms_v1
{
    public partial class PluginsDialog : Form
    {
        private DockPanel dockPanel;  // Ссылка на dockPanel

        public PluginsDialog(DockPanel dockPanel)
        {
            InitializeComponent();
            this.dockPanel = dockPanel;  // Инициализируем ссылку на dockPanel
        }

        private void PluginsDialog_Load(object sender, EventArgs e)
        {
            // Добавляем плагины в список
            foreach (var plugin in MainForm.plugins)
            {
                pluginsListBox.Items.Add(plugin.Key);  // Добавляем имя плагина в ListBox
            }

            // Устанавливаем первый элемент как выбранный по умолчанию
            if (pluginsListBox.Items.Count > 0)
                pluginsListBox.SelectedIndex = 0;
        }

        private void pluginsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPluginName = pluginsListBox.SelectedItem.ToString();
            if (MainForm.plugins.TryGetValue(selectedPluginName, out IPlugin plugin))
            {
                // Применяем выбранный плагин к активному документу
                if (dockPanel.ActiveDocument is FormDocument activeDocument)
                {
                    plugin.PluginMethod(activeDocument.GetImage());
                    activeDocument.UpdateAfterPlugin();  // Обновляем отображение после применения плагина
                }

                // Отображаем информацию о плагине (имя и автор)
                DisplayPluginInfo(plugin);
            }
        }

        // Метод для отображения информации о плагине
        private void DisplayPluginInfo(IPlugin plugin)
        {
            // Извлекаем имя плагина и автора из атрибутов
            var pluginType = plugin.GetType();
            var versionAttr = (VersionAttribute)Attribute.GetCustomAttribute(pluginType, typeof(VersionAttribute));

            // Отображаем имя плагина и автора
            pluginNameLabel.Text = $"Имя плагина: {plugin.Name}";
            pluginAuthorLabel.Text = $"Автор: {plugin.Author}";

            // Если атрибут Version существует, отображаем его
            if (versionAttr != null)
            {
                pluginVersionLabel.Text = $"Версия: {versionAttr.Major}.{versionAttr.Minor}";
            }
            else
            {
                pluginVersionLabel.Text = "Версия: Не указана";
            }
        }

        // Закрытие окна
        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
