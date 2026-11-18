using System;
using System.IO;
using System.Threading;

namespace SystemProgramming
{
    /*
    class Thread1
    {
        // Метод потока
        static void PrintNumbers()
        {
            for (int i = 0; i <= 50; i++)
            {
                Console.WriteLine($"[Поток] {i}");
                Thread.Sleep(20);   // микропауза, чтобы вывод не мелькал мгновенно
            }
        }

        static void Main()
        {
            Console.WriteLine("Главный поток запустил поток-работник");

            Thread t = new Thread(PrintNumbers);
            t.Start();

            t.Join();   // ожидание
            Console.WriteLine("Готово!");
            Console.ReadKey();
        }
    }
    */

    /*
    class Thread2
    {
        static void PrintRange(object rangeObj)
        {
            int[] range = (int[])rangeObj; 
            for (int i = range[0]; i <= range[1]; i++)
            {
                Console.WriteLine($"[Поток] {i}");
                Thread.Sleep(10);
            }
        }

        static void Main()
        {
            Console.Write("Начало диапазона: ");
            int start = int.Parse(Console.ReadLine());
            Console.Write("Конец диапазона: ");
            int end = int.Parse(Console.ReadLine());

            Thread t = new Thread(new ParameterizedThreadStart(PrintRange));
            t.Start(new int[] { start, end });   // параметр улетает в object

            t.Join();
            Console.WriteLine("Готово!");
            Console.ReadKey();
        }
    }
    */

    /*
    class Thread3
    {
        static void PrintPart(object partObj)
        {
            int[] part = (int[])partObj;   // {номер потока, от, до}
            for (int i = part[1]; i <= part[2]; i++)
            {
                Console.WriteLine($"Поток #{part[0]}: {i}");
                Thread.Sleep(5);
            }
        }

        static void Main()
        {
            Console.Write("Начало диапазона: ");
            int start = int.Parse(Console.ReadLine());
            Console.Write("Конец диапазона: ");
            int end = int.Parse(Console.ReadLine());
            Console.Write("Количество потоков: ");
            int count = int.Parse(Console.ReadLine());

            Thread[] threads = new Thread[count];

            int total = end - start + 1;     // сколько всего чисел
            int perThread = total / count;   // сколько чисел на поток
            int current = start;

            for (int i = 0; i < count; i++)
            {
                int from = current;
                int to = (i == count - 1) ? end : current + perThread - 1; // остаток — последнему
                threads[i] = new Thread(PrintPart);
                threads[i].Start(new int[] { i + 1, from, to });
                current = to + 1;
            }

            foreach (Thread t in threads)
                t.Join();

            Console.WriteLine("Все потоки завершили работу!");
            Console.ReadKey();
        }
    }
    */

    class Thread4
    {
        static int[] numbers = new int[10000];
        static int maxVal, minVal;
        static double avgVal;

        static void FindMax()
        {
            int m = numbers[0];
            foreach (int x in numbers) if (x > m) m = x;
            maxVal = m;
        }

        static void FindMin()
        {
            int m = numbers[0];
            foreach (int x in numbers) if (x < m) m = x;
            minVal = m;
        }

        static void FindAvg()
        {
            long sum = 0;
            foreach (int x in numbers) sum += x;
            avgVal = (double)sum / numbers.Length;
        }

        static void WriteToFile()
        {
            using (StreamWriter sw = new StreamWriter("result.txt"))
            {
                sw.WriteLine("Набор чисел:");
                for (int i = 0; i < numbers.Length; i++)
                    sw.Write(numbers[i] + " ");
                sw.WriteLine();
                sw.WriteLine($"Максимум: {maxVal}");
                sw.WriteLine($"Минимум:  {minVal}");
                sw.WriteLine($"Среднее:  {avgVal:F2}");
            }
        }

        static void Main()
        {
            Random rnd = new Random();
            for (int i = 0; i < numbers.Length; i++)
                numbers[i] = rnd.Next(0, 1001);   // набор из 10000 чисел

            Thread tMax = new Thread(FindMax);    // каждому вычислению — свой поток
            Thread tMin = new Thread(FindMin);
            Thread tAvg = new Thread(FindAvg);

            tMax.Start(); tMin.Start(); tAvg.Start();

            tMax.Join(); tMin.Join(); tAvg.Join();

            Thread tWrite = new Thread(WriteToFile);
            tWrite.Start();
            tWrite.Join();
            Console.WriteLine("Данные сохранены в result.txt (папка bin\\Debug проекта)");

            Console.WriteLine($"Максимум: {maxVal}");
            Console.WriteLine($"Минимум:  {minVal}");
            Console.WriteLine($"Среднее:  {avgVal:F2}");
            Console.ReadKey();
        }
    }
}
