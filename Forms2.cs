using System;
using System.IO;
using System.Threading;

/*
class Program
{
    static int[] numbers = new int[1000];
    static int maxVal, minVal;
    static double avgVal;

    static ManualResetEvent generationDone = new ManualResetEvent(false);

    static void Generate()
    {
        Random rnd = new Random();
        Console.WriteLine("[Генератор] Начал генерацию 1000 чисел (0..5000)...");
        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i] = rnd.Next(0, 5001);
            Thread.Sleep(1);   
        }
        Console.WriteLine("[Генератор] Генерация завершена! Открываю событие аналитикам.");
        generationDone.Set();   
    }

    static void FindMax()
    {
        Console.WriteLine("[Максимум] Ожидаю завершение генерации...");
        generationDone.WaitOne();          
        int m = numbers[0];
        foreach (int x in numbers) if (x > m) m = x;
        maxVal = m;
        Console.WriteLine($"[Максимум] Максимум = {maxVal}");
    }

    static void FindMin()
    {
        Console.WriteLine("[Минимум] Ожидаю завершение генерации...");
        generationDone.WaitOne();
        int m = numbers[0];
        foreach (int x in numbers) if (x < m) m = x;
        minVal = m;
        Console.WriteLine($"[Минимум] Минимум = {minVal}");
    }

    static void FindAvg()
    {
        Console.WriteLine("[Среднее] Ожидаю завершение генерации...");
        generationDone.WaitOne();
        long sum = 0;
        foreach (int x in numbers) sum += x;
        avgVal = (double)sum / numbers.Length;
        Console.WriteLine($"[Среднее] Среднее арифметическое = {avgVal:F2}");
    }

    static void Main()
    {
        Thread tGen = new Thread(Generate);
        Thread tMax = new Thread(FindMax);
        Thread tMin = new Thread(FindMin);
        Thread tAvg = new Thread(FindAvg);

        tMax.Start(); tMin.Start(); tAvg.Start();   
        tGen.Start();                            

        tGen.Join(); tMax.Join(); tMin.Join(); tAvg.Join();

        Console.WriteLine($"\nИтог: макс={maxVal}, мин={minVal}, среднее={avgVal:F2}");
        Console.ReadKey();
    }
}
*/

/*
class Program
{
    static readonly object locker = new object();  

    static void CountSentences(object pathObj)
    {
        string path = (string)pathObj;
        lock (locker)                              
        {
            Console.WriteLine("[Счётчик] Занял критическую секцию, читаю файл...");
            string text = File.ReadAllText(path);
            Thread.Sleep(2000);                    

            int count = 0;
            foreach (char c in text)
                if (c == '.' || c == '!' || c == '?') count++;

            Console.WriteLine($"[Счётчик] Количество предложений в файле: {count}");
            Console.WriteLine("[Счётчик] Освобождаю критическую секцию.");
        }
    }

    static void ModifyFile(object pathObj)
    {
        string path = (string)pathObj;
        Console.WriteLine("[Модификатор] Пытаюсь войти в критическую секцию...");
        lock (locker)                             
        {
            Console.WriteLine("[Модификатор] Секция свободна! Заменяю все '!' на '#'...");
            string text = File.ReadAllText(path);
            text = text.Replace('!', '#');
            File.WriteAllText(path, text);
            Console.WriteLine("[Модификатор] Файл изменён.");
        }
    }

    static void Main()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt");
        File.WriteAllText(path, "Привет мир! Это тестовый файл! Как дела? Отлично.!");

        Thread t1 = new Thread(CountSentences);
        Thread t2 = new Thread(ModifyFile);

        t1.Start(path);
        t2.Start(path);

        t1.Join(); t2.Join();

        Console.WriteLine("\nГотово. Содержимое файла после модификации:");
        Console.WriteLine(File.ReadAllText(path));
        Console.ReadKey();
    }
}
*/

class Program
{
    static readonly object locker = new object();   
    static int[] data;                            

    static void SortArray(object arrObj)
    {
        int[] arr = (int[])arrObj;
        lock (locker)
        {
            Console.WriteLine("[Сортировщик] Занял критическую секцию, сортирую массив...");
            Thread.Sleep(2000);     
            Array.Sort(arr);     
            Console.WriteLine("[Сортировщик] Массив отсортирован. Освобождаю секцию.");
        }
    }

    static void SearchNumber(object numberObj)
    {
        int number = (int)numberObj;
        Console.WriteLine($"[Поисковик] Ищу число {number}. Пытаюсь войти в критическую секцию...");
        lock (locker)                              
        {
            int index = Array.BinarySearch(data, number);  
            if (index >= 0)
                Console.WriteLine($"[Поисковик] Число {number} найдено! Индекс: {index}");
            else
                Console.WriteLine($"[Поисковик] Числа {number} в массиве нет.");
        }
    }

    static void Main()
    {
        Random rnd = new Random();
        data = new int[20];
        Console.Write("Исходный массив: ");
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = rnd.Next(0, 100);
            Console.Write(data[i] + " ");
        }
        Console.WriteLine();

        Console.Write("Введите число для поиска (например, из списка выше): ");
        int number = int.Parse(Console.ReadLine());

        Thread t1 = new Thread(SortArray);
        Thread t2 = new Thread(SearchNumber);

        t1.Start(data);     
        t2.Start(number);  

        t1.Join(); t2.Join();

        Console.Write("Отсортированный массив: ");
        foreach (int x in data) Console.Write(x + " ");
        Console.WriteLine();
        Console.ReadKey();
    }
}