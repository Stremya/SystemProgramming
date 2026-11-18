using System;
using System.Diagnostics;
using System.Linq;

namespace SystemProgramming
{
    /*
    class Diagnostic1
    {
        static long GetMemorySafe(Process p)
        {
            try { return p.WorkingSet64; }
            catch { return 0; }
        }

        static void Main()
        {
            Process current = Process.GetCurrentProcess();

            Console.WriteLine("=== Текущий процесс ===");
            Console.WriteLine($"Имя процесса:        {current.ProcessName}");
            Console.WriteLine($"PID:                 {current.Id}");
            Console.WriteLine($"Время запуска:       {current.StartTime}");
            Console.WriteLine($"Физическая память:   {current.WorkingSet64 / 1048576.0:F1} MB");
            Console.WriteLine($"Количество потоков:  {current.Threads.Count}");

            Console.WriteLine("\n=== Топ-5 процессов по физической памяти ===");
            Process[] all = Process.GetProcesses();

            var top5 = all.OrderByDescending(GetMemorySafe).Take(5);

            int place = 1;
            foreach (Process p in top5)
            {
                Console.WriteLine($"{place}. {p.ProcessName,-25} PID: {p.Id,-6} " +
                                  $"Память: {GetMemorySafe(p) / 1048576.0:F1} MB");
                place++;
            }

            Console.ReadKey();
        }
    }
    */

    /*
    class Diagnostic2
    {
        static void Main()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c ping 8.8.8.8 -n 4",  // /c = выполнить команду и завершиться
                UseShellExecute = false,             // без ОС-оболочки -> можно перенаправлять потоки
                RedirectStandardOutput = true,       // вывод ребёнка читаем МЫ, а не консоль
                CreateNoWindow = true                // не показывать лишнее чёрное окно
            };

            // using = после выхода из блока объект будет корректно освобождён
            using (Process child = Process.Start(psi))
            {
                Console.WriteLine($"Дочерний процесс cmd.exe запущен, PID: {child.Id}");
                Console.WriteLine("Читаем его вывод...\n");

                string output = child.StandardOutput.ReadToEnd(); // читать до конца потока
                child.WaitForExit();                              // дождаться завершения

                Console.WriteLine("=== Вывод команды ping ===");
                Console.WriteLine(output);
            }

            Console.ReadKey();
        }
    }
    */

    /*
    class Diagnostic3
    {
        static void Main()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "notepad.exe",
                UseShellExecute = true   // GUI-приложение
            };

            using (Process child = Process.Start(psi))
            {
                Console.WriteLine($"Процесс запущен (PID {child.Id}), ожидаем 3 секунды...");

                bool exited = child.WaitForExit(3000);   // 3000 мс

                if (exited)
                {
                    Console.WriteLine("Процесс завершился сам за 3 секунды.");
                }
                else
                {
                    Console.WriteLine("ВНИМАНИЕ: процесс не завершился за 3 секунды.");
                    Console.WriteLine("Принудительно завершаем его методом Kill()...");

                    child.Kill();          // принудительное завершение
                    child.WaitForExit();  

                    Console.WriteLine("Процесс принудительно завершён.");
                }
            }

            Console.ReadKey();
        }
    }
    */

    class Diagncostic4
    {
        static void Main()
        {
            AppDomain current = AppDomain.CurrentDomain;
            Console.WriteLine($"Имя текущего домена: {current.FriendlyName}");

            AppDomain newDomain = AppDomain.CreateDomain("TestDomain");
            Console.WriteLine($"Создан новый домен: {newDomain.FriendlyName}");

            AppDomain.Unload(newDomain);
            Console.WriteLine("Домен выгружен.");

            Console.ReadKey();
        }
    }
}
