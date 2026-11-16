using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace SystemProgramming
{
    /*
    internal class Program1
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int MessageBox(IntPtr hWnd, string lpText, string lpCaption, uint uType);

        private const uint MB_OK = 0x00000000;
        private const uint MB_ICONINFORMATION = 0x00000040;

        static void Main()
        {
            MessageBox(IntPtr.Zero, "Hello, World!", "Системное программирование",
                       MB_OK | MB_ICONINFORMATION);
        }
    }
    */

    /*
    internal class Program2
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int MessageBox(IntPtr hWnd, string lpText, string lpCaption, uint uType);

        private const uint MB_OK = 0x00000000;
        private const uint MB_YESNO = 0x00000004;
        private const uint MB_ICONQUESTION = 0x00000020;
        private const int IDYES = 6;

        static void Main()
        {
            bool playAgain = true;
            while (playAgain)
            {
                PlayRound();
                int res = MessageBox(IntPtr.Zero, "Сыграем ещё раз?", "Игра «Угадай число»",
                                     MB_YESNO | MB_ICONQUESTION);
                playAgain = (res == IDYES);
            }
        }

        static void PlayRound()
        {
            MessageBox(IntPtr.Zero,
                "Загадайте число от 0 до 100.\nЯ буду задавать вопросы, отвечайте честно!",
                "Игра «Угадай число»", MB_OK | MB_ICONQUESTION);

            int low = 0, high = 100;
            while (low < high)
            {
                int mid = (low + high) / 2;
                int answer = MessageBox(IntPtr.Zero,
                    $"Ваше число больше {mid}?\n\nДа  — больше\nНет — не больше",
                    "Игра «Угадай число»", MB_YESNO | MB_ICONQUESTION);

                if (answer == IDYES) low = mid + 1;
                else high = mid;
            }

            int check = MessageBox(IntPtr.Zero,
                $"Ваше число — {low}!",
                "Игра «Угадай число»", MB_YESNO | MB_ICONQUESTION);

            if (check != IDYES)
                MessageBox(IntPtr.Zero, "Кажется, где-то вы ошиблись в ответах 🙂",
                             "Игра «Угадай число»", MB_OK);
        }
    }
    */

    /*
    class Program3
    {
        // Поиск окна по имени класса и/или заголовку
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Отправка сообщения окну
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_CLOSE = 0x0010;   // «закрыть окно»

        static void Main()
        {
            IntPtr hwndNotepad = FindWindow("Notepad", null);

            if (hwndNotepad == IntPtr.Zero)
            {
                Console.WriteLine("Окно Блокнота не найдено. Запустите Блокнот и повторите.");
                return;
            }

            SendMessage(hwndNotepad, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            Console.WriteLine("Сообщение WM_CLOSE отправлено главному окну Блокнота.");
        }
    }
    */

    class Program4
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll")]
        private static extern bool IsWindow(IntPtr hWnd);

        private const uint WM_SETTEXT = 0x000C;   // установить текст окна (заголовок)

        static void Main()
        {
            IntPtr hwnd = FindWindow("Notepad", null);
            if (hwnd == IntPtr.Zero)
            {
                Console.WriteLine("Блокнот не найден. Запустите его и повторите.");
                return;
            }

            Console.WriteLine("Обновление заголовка Блокнота каждую секунду...");
            Console.WriteLine("Нажмите любую клавишу в этой консоли для выхода.");

            while (true)
            {
                // выход по нажатию клавиши
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    break;
                }

                // если Блокнот закрыли — найти снова
                if (!IsWindow(hwnd))
                {
                    hwnd = FindWindow("Notepad", null);
                    if (hwnd == IntPtr.Zero)
                    {
                        Console.WriteLine("Блокнот закрыт, завершение.");
                        break;
                    }
                }

                string time = DateTime.Now.ToString("HH:mm:ss");
                SendMessage(hwnd, WM_SETTEXT, IntPtr.Zero, time);

                Thread.Sleep(1000);   // пауза 1 секунда
            }
        }
    }
}
