using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForms_v1
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Включение визуальных стилей для приложения, чтобы использовать более современные элементы управления.
            Application.EnableVisualStyles();

            // Устанавливаем параметр для совместимости с текстовым рендерингом по умолчанию. False означает использование стандартного рендеринга.
            Application.SetCompatibleTextRenderingDefault(false);

            // Запуск основного окна формы приложения (MainForm), которое будет отображаться пользователю.
            Application.Run(new MainForm());
        }
    }
}
