using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.IO;

namespace SystemProgramming
{
    /*
    class ChildProcess1
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--child")
            {
                ChildMode();
                return;
            }
            ParentMode(); 
        }

        static void ChildMode()
        {
            Console.WriteLine($"[РЕБЁНОК] Я дочерний процесс, PID = {Process.GetCurrentProcess().Id}");
            Console.WriteLine("[РЕБЁНОК] Работаю 3 секунды...");
            Thread.Sleep(3000);
            Console.WriteLine("[РЕБЁНОК] Завершаюсь с кодом 42.");
            Environment.ExitCode = 42;   // код возврата для родителя
        }

        static void ParentMode()
        {
            string exePath = Assembly.GetEntryAssembly().Location; // путь к собственному .exe

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = "--child",     // запуск копии в роли ребёнка
                UseShellExecute = true     // у ребёнка будет своё окно консоли
            };

            using (Process child = Process.Start(psi))
            {
                Console.WriteLine($"[РОДИТЕЛЬ] Дочерний процесс запущен, PID = {child.Id}");
                Console.WriteLine("[РОДИТЕЛЬ] Ожидаю его завершения (WaitForExit)...");

                child.WaitForExit();       // заблокировались, пока ребёнок не умрёт

                Console.WriteLine($"[РОДИТЕЛЬ] Ребёнок завершился. Код завершения: {child.ExitCode}");
            }

            Console.ReadKey();
        }
    }
    */

    /*
    class ChildProcess2
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--child")
            {
                ChildMode();
                return;
            }
            ParentMode();
        }

        static void ChildMode()
        {
            Console.WriteLine($"[РЕБЁНОК] PID = {Process.GetCurrentProcess().Id}. Буду работать 30 секунд.");
            for (int i = 30; i > 0; i--)
            {
                Console.WriteLine($"[РЕБЁНОК] Осталось секунд: {i}");
                Thread.Sleep(1000);
            }
            Console.WriteLine("[РЕБЁНОК] Завершился сам, код 0.");
            Environment.ExitCode = 0;
        }

        static void ParentMode()
        {
            string exePath = Assembly.GetEntryAssembly().Location;

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = "--child",
                UseShellExecute = true
            };

            using (Process child = Process.Start(psi))
            {
                Console.WriteLine($"[РОДИТЕЛЬ] Ребёнок запущен, PID = {child.Id}");
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("  1 — дождаться завершения и показать код завершения");
                Console.WriteLine("  2 — принудительно завершить (Kill)");
                Console.Write("Ваш выбор: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.WriteLine("[РОДИТЕЛЬ] Ожидаю завершения...");
                    child.WaitForExit();
                    Console.WriteLine($"[РОДИТЕЛЬ] Ребёнок завершился САМ. Код: {child.ExitCode}");
                }
                else
                {
                    Console.WriteLine("[РОДИТЕЛЬ] Принудительно завершаю ребёнка...");
                    child.Kill();
                    child.WaitForExit();  // гарантия, что ОС освободила ресурсы
                    Console.WriteLine($"[РОДИТЕЛЬ] Ребёнок УБИТ. Код: {child.ExitCode}");
                }
            }

            Console.ReadKey();
        }
    }
    */

    /*
    class ChildProcess3
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--child")
            {
                ChildMode(args);
                return;
            }
            ParentMode();
        }

        static void ChildMode(string[] args)
        {
            // отобразить аргументы
            Console.WriteLine("[РЕБЁНОК] Полученные аргументы:");
            for (int i = 1; i < args.Length; i++)   // пропуск метки --child
                Console.WriteLine($"  args[{i}] = {args[i]}");

            // аргументы: --child 7 3 +
            double a = double.Parse(args[1]);
            double b = double.Parse(args[2]);
            string op = args[3];

            double result = 0;
            switch (op)
            {
                case "+": result = a + b; break;
                case "-": result = a - b; break;
                case "*": result = a * b; break;
                case "/": result = (b != 0) ? a / b : double.NaN; break;
                default:
                    Console.WriteLine($"[РЕБЁНОК] Неизвестная операция: {op}");
                    Environment.ExitCode = 1;
                    Console.ReadKey();
                    return;
            }

            Console.WriteLine($"[РЕБЁНОК] {a} {op} {b} = {result}");
            // вывести результат сложения 10
            Console.WriteLine($"[РЕБЁНОК] Результат сложения 10: {result + 10}");
            Environment.ExitCode = 0;

            Console.WriteLine("[РЕБЁНОК] Нажмите любую клавишу для закрытия окна...");
            Console.ReadKey();
        }

        static void ParentMode()
        {
            Console.Write("Первое число (Enter — 7): ");
            string s1 = Console.ReadLine();
            Console.Write("Второе число (Enter — 3): ");
            string s2 = Console.ReadLine();
            Console.Write("Операция (+, -, *, /) (Enter — +): ");
            string op = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(s1)) s1 = "7";
            if (string.IsNullOrWhiteSpace(s2)) s2 = "3";
            if (string.IsNullOrWhiteSpace(op)) op = "+";

            string exePath = Assembly.GetEntryAssembly().Location;

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"--child {s1} {s2} {op}",   // процесс передачи аргументов ребёнку
                UseShellExecute = true
            };

            using (Process child = Process.Start(psi))
            {
                Console.WriteLine($"[РОДИТЕЛЬ] Ребёнок запущен с аргументами: {s1} {s2} {op}, PID = {child.Id}");
                child.WaitForExit();
                Console.WriteLine($"[РОДИТЕЛЬ] Ребёнок отработал с кодом: {child.ExitCode}");
            }

            Console.ReadKey();
        }
    }
    */

    class ChildProcess4
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--child")
            {
                ChildMode(args);
                return;
            }
            ParentMode();
        }

        static void ChildMode(string[] args)
        {
            // аргументы: --child "путь к файлу" слово
            string path = args[1];
            string word = args[2];

            Console.WriteLine("[РЕБЁНОК] Полученные аргументы:");
            Console.WriteLine($"  файл: {path}");
            Console.WriteLine($"  слово: {word}");

            if (!File.Exists(path))
            {
                Console.WriteLine("[РЕБЁНОК] Ошибка: файл не найден!");
                Environment.ExitCode = 1;
                Console.ReadKey();
                return;
            }

            string text = File.ReadAllText(path);

            // текст на слова по разделителям
            char[] separators = { ' ', '\t', '\r', '\n', '.', ',', '!', '?', ':', ';',
                              '(', ')', '"', '\'', '/', '\\', '<', '>', '[', ']', '{', '}', '-' };
            string[] words = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            int count = 0;
            foreach (string w in words)
            {
                if (w.Equals(word, StringComparison.OrdinalIgnoreCase)) // без учёта регистра
                    count++;
            }

            Console.WriteLine($"[РЕБЁНОК] Слово \"{word}\" встречается в файле раз: {count}");
            Environment.ExitCode = 0;

            Console.WriteLine("[РЕБЁНОК] Нажмите любую клавишу для закрытия окна...");
            Console.ReadKey();
        }

        static void ParentMode()
        {
            Console.Write("Путь к файлу (Enter — создать тестовый файл): ");
            string path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path))
            {
                // чтобы демонстрация всегда работала — файл-пример
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt");
                File.WriteAllText(path,
                    "I have a bicycle. The bicycle is red. My friend also rides a bicycle. " +
                    "A car is not a bicycle. BICYCLE forever!");
                Console.WriteLine($"[РОДИТЕЛЬ] Создан тестовый файл: {path}");
            }

            Console.Write("Слово для поиска (Enter — bicycle): ");
            string word = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(word)) word = "bicycle";

            string exePath = Assembly.GetEntryAssembly().Location;

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"--child \"{path}\" {word}",
                UseShellExecute = true
            };

            using (Process child = Process.Start(psi))
            {
                Console.WriteLine($"[РОДИТЕЛЬ] Ребёнок запущен, PID = {child.Id}");
                child.WaitForExit();
                Console.WriteLine($"[РОДИТЕЛЬ] Ребёнок отработал с кодом: {child.ExitCode}");
            }

            Console.ReadKey();
        }
    }
}
