using System;
using System.IO;
using System.Threading;

namespace SystemProgramming
{
    /*
    class Async1
    {
        // Метод для обычного фонового потока
        static void ShowTime()
        {
            while (true)
            {
                Console.WriteLine($"[Мониторинг] Текущее время: {DateTime.Now:HH:mm:ss}");
                Thread.Sleep(1000);
            }
        }

        // Метод для задачи пула
        static void PoolTask(object state)
        {
            int number = (int)state; 
            Console.WriteLine($"Задача №{number} выполняется в потоке {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(2000);
            Console.WriteLine($"Задача №{number} завершена.");
        }

        static void Main()
        {
            // 1. Обычный поток, фоновый
            Thread monitor = new Thread(ShowTime);
            monitor.IsBackground = true;   // не помешает программе закрыться
            monitor.Start();

            // 2. Три короткие задачи в пул 
            ThreadPool.QueueUserWorkItem(PoolTask, 1);
            ThreadPool.QueueUserWorkItem(PoolTask, 2);
            ThreadPool.QueueUserWorkItem(PoolTask, 3);

            // 3. Главный поток ждёт 5 секунд и завершается
            Thread.Sleep(5000);
            Console.WriteLine("Главный поток завершил работу. Программа закрывается.");
        }
    }
    */

    /*
    // делегат под тяжелый метод
    delegate double CalcDelegate(double x);

    class Async2
    {
        // Тяжелый метод: думает 3 секунды, возвращает квадратный корень
        static double HeavySqrt(double x)
        {
            Thread.Sleep(3000);
            return Math.Sqrt(x);
        }

        // Callback
        static void CalcCallback(IAsyncResult ar)
        {
            CalcDelegate del = (CalcDelegate)ar.AsyncState;
            // результат тяжелого метода
            double result = del.EndInvoke(ar);

            Console.WriteLine($"\n[Callback] Расчет завершен! Результат: {result}");
            Console.WriteLine($"[Callback] Выполнялось в потоке {Thread.CurrentThread.ManagedThreadId}");
        }

        static void Main()
        {
            Console.Write("Введите число для расчета: ");
            double n = double.Parse(Console.ReadLine());

            // Экземпляр делегата
            CalcDelegate calc = HeavySqrt;

            // Асинхронный запуск
            calc.BeginInvoke(n, new AsyncCallback(CalcCallback), calc);

            // Это сообщение видно мгновенно
            Console.WriteLine("Расчет запущен. Главный поток свободен.");

            Console.ReadLine();
        }
    }
    */

    /*
    class Async3
    {
        static int tickets = 100;
        static readonly object locker = new object();

        static void SellTickets(object cashierObj)
        {
            string name = (string)cashierObj;
            while (true)
            {

                lock (locker)
                {
                    if (tickets <= 0) break;    // билетов нет — кассир уходит
                    tickets--;
                    Console.WriteLine($"{name} продал билет. Осталось: {tickets}");
                }

                Thread.Sleep(10);  
            }
        }

        static void Main()
        {
            for (int i = 1; i <= 5; i++)
            {
                Thread t = new Thread(SellTickets);
                t.Start($"Кассир {i}");
            }

            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
    */

    // Делегат для APM-расчета
    delegate string TrajectoryDelegate();

    class Async4
    {
        static int fuelLevel = 0;   // общий счетчик топлива

        // Мониторинг: фоновый поток
        static void Telemetry()
        {
            while (true)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Телеметрия в норме...");
            }
        }

        // APM: тяжелый метод
        static string CalculateTrajectory()
        {
            Thread.Sleep(4000);
            return "Траектория рассчитана! К запуску готов.";
        }

        static void TrajectoryCallback(IAsyncResult ar)
        {
            TrajectoryDelegate del = (TrajectoryDelegate)ar.AsyncState;
            string message = del.EndInvoke(ar);
            Console.WriteLine($"\n[ЦУП] {message}");
        }

        // Топливные насосы: задача для пула потоков
        static void FuelPumps(object state)
        {
            for (int i = 0; i < 10; i++)
            {
                Interlocked.Increment(ref fuelLevel);  // атомарно +1
                Thread.Sleep(500);                     // интервал 500 мс
            }
        }

        static void Main()
        {
            Console.WriteLine("=== Подготовка к запуску ракеты ===\n");

            // Фоновый мониторинг
            Thread telemetry = new Thread(Telemetry);
            telemetry.IsBackground = true;   // не помешает закрытию программы
            telemetry.Start();

            // Асинхронный расчет траектории
            TrajectoryDelegate calc = CalculateTrajectory;
            calc.BeginInvoke(new AsyncCallback(TrajectoryCallback), calc);

            // Насосы — в пул потоков
            ThreadPool.QueueUserWorkItem(FuelPumps, null);

            // Главный поток ждет 5 секунд
            Thread.Sleep(5000);

            // Безопасное чтение итогового уровня топлива
            int fuel = Interlocked.CompareExchange(ref fuelLevel, 0, 0);
            Console.WriteLine($"\n[Главный] Итоговый уровень топлива: {fuel}");
            Console.WriteLine("[Главный] Цикл завершен. Программа завершается.");
        }
    }
}