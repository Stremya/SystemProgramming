using System;
using System.Linq;
using System.Threading.Tasks;

class Program1
{
    static void Main()
    {
        Task t1 = new Task(() => Console.WriteLine($"1) Task.Start:            {DateTime.Now:dd.MM.yyyy HH:mm:ss}"));
        t1.Start();

        Task t2 = Task.Factory.StartNew(() => Console.WriteLine($"2) Task.Factory.StartNew: {DateTime.Now:dd.MM.yyyy HH:mm:ss}"));

        Task t3 = Task.Run(() => Console.WriteLine($"3) Task.Run:              {DateTime.Now:dd.MM.yyyy HH:mm:ss}"));

        Task.WaitAll(t1, t2, t3);
        Console.WriteLine("Все три задачи завершены.");
        Console.ReadKey();
    }
}

class Program2
{
    static bool IsPrime(int n)
    {
        if (n < 2) return false;
        for (int i = 2; i * i <= n; i++)
            if (n % i == 0) return false;
        return true;
    }

    static void Main()
    {
        Task task = Task.Run(() =>
        {
            Console.WriteLine("Простые числа от 0 до 1000:");
            for (int i = 0; i <= 1000; i++)
                if (IsPrime(i))
                    Console.Write(i + " ");
            Console.WriteLine();
        });

        task.Wait();  
        Console.WriteLine("Задача завершила работу.");
        Console.ReadKey();
    }
}

class Program3
{
    static bool IsPrime(int n)
    {
        if (n < 2) return false;
        for (int i = 2; i * i <= n; i++)
            if (n % i == 0) return false;
        return true;
    }

    static void Main()
    {
        Console.Write("Начало диапазона: ");
        int start = int.Parse(Console.ReadLine());
        Console.Write("Конец диапазона: ");
        int end = int.Parse(Console.ReadLine());

        Task<int> task = Task.Run(() =>
        {
            int count = 0;
            for (int i = start; i <= end; i++)
            {
                if (IsPrime(i))
                {
                    Console.Write(i + " ");
                    count++;
                }
            }
            Console.WriteLine();
            return count;    
        });

        task.Wait();  
        Console.WriteLine($"Количество простых чисел: {task.Result}");
        Console.ReadKey();
    }
}

class Program4
{
    static double Min(int[] a) { int m = a[0]; foreach (int x in a) if (x < m) m = x; return m; }
    static double Max(int[] a) { int m = a[0]; foreach (int x in a) if (x > m) m = x; return m; }
    static double Sum(int[] a) { long s = 0; foreach (int x in a) s += x; return s; }
    static double Avg(int[] a) { long s = 0; foreach (int x in a) s += x; return (double)s / a.Length; }

    static void Main()
    {
        Random rnd = new Random();
        int[] numbers = new int[1000];
        for (int i = 0; i < numbers.Length; i++)
            numbers[i] = rnd.Next(-100, 101);

        Task<double>[] tasks = new Task<double>[4];
        tasks[0] = Task.Run(() => Min(numbers));
        tasks[1] = Task.Run(() => Max(numbers));
        tasks[2] = Task.Run(() => Sum(numbers));
        tasks[3] = Task.Run(() => Avg(numbers));

        Task.WaitAll(tasks);   

        Console.WriteLine($"Минимум: {tasks[0].Result}");
        Console.WriteLine($"Максимум: {tasks[1].Result}");
        Console.WriteLine($"Сумма:   {tasks[2].Result}");
        Console.WriteLine($"Среднее: {tasks[3].Result:F2}");
        Console.ReadKey();
    }
}

class Program5
{
    static void Main()
    {
        Random rnd = new Random();
        int[] source = new int[30];
        for (int i = 0; i < source.Length; i++)
            source[i] = rnd.Next(0, 15); 

        Console.WriteLine("Исходный массив: " + string.Join(" ", source));
        Console.Write("Значение для поиска: ");
        int value = int.Parse(Console.ReadLine());

        Task<int[]> removeTask = Task.Run(() =>
        {
            Console.WriteLine("[Шаг 1] Удаляю повторы...");
            return source.Distinct().ToArray();
        });

        Task<int[]> sortTask = removeTask.ContinueWith(t =>
        {
            Console.WriteLine("[Шаг 2] Сортирую массив...");
            int[] arr = t.Result.Clone() as int[];
            Array.Sort(arr);
            return arr;
        });

        Task<int> searchTask = sortTask.ContinueWith(t =>
        {
            Console.WriteLine("[Шаг 3] Бинарный поиск...");
            return Array.BinarySearch(t.Result, value);
        });

        searchTask.Wait();   

        Console.WriteLine("Массив после обработки: " + string.Join(" ", sortTask.Result));
        int index = searchTask.Result;
        if (index >= 0)
            Console.WriteLine($"Значение {value} найдено, индекс: {index}");
        else
            Console.WriteLine($"Значения {value} в массиве нет.");
        Console.ReadKey();
    }
}