using System.Windows.Forms;

namespace WinForms_v1
{
    partial class PluginsDialog
    {
        private System.ComponentModel.IContainer components = null;
        private ListBox pluginsListBox;
        private Button closeButton;
        private Label pluginNameLabel;
        private Label pluginAuthorLabel;
        private Label pluginVersionLabel;

        /// <summary>
        /// Очистка всех используемых ресурсов.
        /// </summary>
        /// <param name="disposing">Истинно, если управляемые ресурсы должны быть удалены; иначе — ложь.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически сгенерированный конструктором форм

        private void InitializeComponent()
        {
            this.pluginsListBox = new System.Windows.Forms.ListBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.pluginNameLabel = new System.Windows.Forms.Label();
            this.pluginAuthorLabel = new System.Windows.Forms.Label();
            this.pluginVersionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // 
            // pluginsListBox
            // 
            this.pluginsListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.pluginsListBox.FormattingEnabled = true;
            this.pluginsListBox.Location = new System.Drawing.Point(0, 0);
            this.pluginsListBox.Name = "pluginsListBox";
            this.pluginsListBox.Size = new System.Drawing.Size(284, 200);
            this.pluginsListBox.TabIndex = 0;
            this.pluginsListBox.SelectedIndexChanged += new System.EventHandler(this.pluginsListBox_SelectedIndexChanged);

            // 
            // closeButton
            // 
            this.closeButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.closeButton.Location = new System.Drawing.Point(0, 206);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(284, 45);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Закрыть";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);

            // 
            // pluginNameLabel
            // 
            this.pluginNameLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pluginNameLabel.Location = new System.Drawing.Point(0, 200);
            this.pluginNameLabel.Name = "pluginNameLabel";
            this.pluginNameLabel.Size = new System.Drawing.Size(284, 20);
            this.pluginNameLabel.TabIndex = 2;
            this.pluginNameLabel.Text = "Имя плагина: ";

            // 
            // pluginAuthorLabel
            // 
            this.pluginAuthorLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pluginAuthorLabel.Location = new System.Drawing.Point(0, 220);
            this.pluginAuthorLabel.Name = "pluginAuthorLabel";
            this.pluginAuthorLabel.Size = new System.Drawing.Size(284, 20);
            this.pluginAuthorLabel.TabIndex = 3;
            this.pluginAuthorLabel.Text = "Автор: ";

            // 
            // pluginVersionLabel
            // 
            this.pluginVersionLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pluginVersionLabel.Location = new System.Drawing.Point(0, 240);
            this.pluginVersionLabel.Name = "pluginVersionLabel";
            this.pluginVersionLabel.Size = new System.Drawing.Size(284, 20);
            this.pluginVersionLabel.TabIndex = 4;
            this.pluginVersionLabel.Text = "Версия: ";

            // 
            // PluginsDialog
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.pluginVersionLabel);
            this.Controls.Add(this.pluginAuthorLabel);
            this.Controls.Add(this.pluginNameLabel);
            this.Controls.Add(this.pluginsListBox);
            this.Controls.Add(this.closeButton);
            this.Name = "PluginsDialog";
            this.Text = "Менеджер плагинов";
            this.Load += new System.EventHandler(this.PluginsDialog_Load);
            this.ResumeLayout(false);
        }

        #endregion
    }
}


