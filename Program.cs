using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace MediaLoader
{
    static class Program
    {
        static public string inPath = "",  // Путь к исходной папке с рекламными роликами
                             outPath = ""; // Путь к папке на флешке
        static public string BlocksPath = "F:\\Блоки\\", MediaPath = "E:\\Клипы\\";
        static public string[] Points = new string[] {
            "XXX",
            "Атланта",
            "Грань",
            "Дом Быта",
            "Кантри-Чикен",
            "кц Красный",
            "кц-пиццерия U-city",
            "Маяк",
            "М-н 19",
            "М-н 70",
            "МС Чикен",
            "Новый Континент",
            "Орленок",
            "Симс кафе(Платинум)",
            "Сингапур",
            "Супер-Гуд",
            "Универсам"
        };
        static public string[] Flashs = new string[] {
            "XX",
            "AT",
            "GR",
            "DB",
            "CC",
            "KR",
            "UC",
            "MA",
            "19",
            "70",
            "MC",
            "NK",
            "OR",
            "SK",
            "SI",
            "SG",
            "UN"
        };

        [STAThread]
        static void Main()
        {
            try
            {
                // Проверяем необходимость обновления программы
                if (Application.ExecutablePath.Length >= 8 && Application.ExecutablePath.Substring(Application.ExecutablePath.Length - 8) == "_new.exe")
                {
                    // Ожидаем завершения предыдущей копии программы
                    Thread.Sleep(5000);
                    // Заменяем файл приложения
                    File.Copy(Application.ExecutablePath, Application.ExecutablePath.Substring(0, Application.ExecutablePath.Length - 8), true);
                    // Запускаем обновленную версию программы
                    Process P = new Process();
                    P.StartInfo.FileName = Application.ExecutablePath.Substring(0, Application.ExecutablePath.Length - 8);
                    P.StartInfo.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                    P.StartInfo.UseShellExecute = true;
                    P.Start();
                    return;
                }
                else
                    File.Delete(Application.ExecutablePath + "_new.exe");
            }
            catch { }

            Control.CheckForIllegalCrossThreadCalls = false;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
