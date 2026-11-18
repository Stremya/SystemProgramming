using System;
using System.Diagnostics;
using System.Threading;

namespace SystemProgramming
{
    /*
    class Thread1
    {
        // Метод потока
        static void Work(object nameObj)
        {
            string name = (string)nameObj;   // приведение к строке
            for (int i = 1; i <= 5; i++)
            {
                Console.WriteLine($"{name} Шаг {i}");
                Thread.Sleep(500);   // пауза, чтобы потоки красиво перемежались
            }
        }

        static void Main()
        {
            // Два потока на делегате ParameterizedThreadStart
            Thread t1 = new Thread(new ParameterizedThreadStart(Work));
            Thread t2 = new Thread(new ParameterizedThreadStart(Work));

            t1.Start("Поток А");   // параметр улетает в object-аргумент метода
            t2.Start("Поток Б");

            // Главный поток терпеливо ждёт обоих
            t1.Join();
            t2.Join();

            Console.WriteLine("Все потоки завершили работу");
            Console.ReadKey();
        }
    }
    */

    /*
    class Thread2
    {
        // Метод для таймера
        static void TimerTick(object state)
        {
            Console.WriteLine($"[Таймер] Текущее время: {DateTime.Now:HH:mm:ss}");
        }

        static void BackgroundWork()
        {
            while (true)
            {
                Console.WriteLine("Фоновый поток работает...");
                Thread.Sleep(300);
            }
        }

        static void Main()
        {
            Timer timer = new Timer(TimerTick, null, 1000, 2000);

            Thread bg = new Thread(BackgroundWork);
            bg.IsBackground = true;   // фон - умрёт вместе с главным потоком
            bg.Start();

            Console.WriteLine("Нажмите Enter для выхода");
            Console.ReadLine();
        }
    }
    */

    /*
    class Thread3
    {
        static int counter = 0;
        static readonly object locker = new object();   // "замок"

        static void Increment()
        {
            for (int i = 0; i < 100000; i++)   // 100 000 итераций на поток
            {
                // ВЕРСИЯ 1
                // counter++;

                // ВЕРСИЯ 2
                lock (locker)
                {
                    counter++;
                }
            }
        }

        static void Main()
        {
            Thread[] threads = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                threads[i] = new Thread(Increment);
                threads[i].Start();
            }

            foreach (Thread t in threads)
                t.Join();   // ожидание

            Console.WriteLine($"Итоговое значение counter: {counter}");
            Console.ReadKey();
        }
    }
    */

    class Thread4
    {
        static void Main()
        {
            Console.WriteLine("Приготовьтесь...");
            Console.WriteLine("Как только увидите красное «ЖМИ!» — нажмите любую клавишу!");

            Random rnd = new Random();
            int delay = rnd.Next(2000, 5001);  // от 2000 до 5000 мс
            Thread.Sleep(delay);               // главный поток "замер"

            // Сигнал
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ЖМИ!");
            Console.ResetColor();

            // поток-таймаут на 3 секунды
            CancellationTokenSource cts = new CancellationTokenSource();
            Thread timeoutThread = new Thread(() =>
            {
                Thread.Sleep(3000);
                if (!cts.IsCancellationRequested)   // игрок так и не нажал
                {
                    Console.WriteLine("\nВы не успели! Слишком долго.");
                    Environment.Exit(0);            // завершение игры
                }
            });
            timeoutThread.IsBackground = true;
            timeoutThread.Start();

            // Замер реакции
            Stopwatch sw = Stopwatch.StartNew();
            Console.ReadKey(true);   // ожидание нажатия
            sw.Stop();

            cts.Cancel();            // игрок успел
            Console.WriteLine($"Ваше время реакции: {sw.ElapsedMilliseconds} мс");
            Console.ReadKey();
        }
    }
}
