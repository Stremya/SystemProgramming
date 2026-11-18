using System;
using System.Threading;
using System.Collections.Generic;

/*
class Mutex1
{
    static void Main()
    {
        Mutex mutex = null;

        try
        {
            Mutex.OpenExisting("MY_MUTEX");
            Console.WriteLine("Приложение уже запущено!");
            return;  
        }
        catch (WaitHandleCannotBeOpenedException)
        {
            // Мьютекса с таким именем нет
        }

        mutex = new Mutex(true, "MY_MUTEX");  
        try
        {
            Console.WriteLine("Приложение работает. Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
        finally
        {
            mutex.ReleaseMutex();  
            mutex.Dispose();
        }
    }
}


class MutexCounter
{
    public int count = 0;                       
    private readonly Mutex mutex = new Mutex();   

    public void IncrementMany(int times)
    {
        for (int i = 0; i < times; i++)
        {
            mutex.WaitOne();     
            count++;            
            mutex.ReleaseMutex(); 
        }
    }
}

class Mutex2
{
    static void Main()
    {
        MutexCounter counter = new MutexCounter();

        Thread[] threads = new Thread[5];
        for (int i = 0; i < 5; i++)
        {
            threads[i] = new Thread(() => counter.IncrementMany(1000000));
            threads[i].Start();
        }

        foreach (Thread t in threads) t.Join();

        Console.WriteLine($"Итоговое значение count: {counter.count}");  
        Console.ReadKey();
    }
}
*/

/*
class Munex3
{
    static Semaphore semaphore = new Semaphore(3, 3);  
    static int completed = 0;
    static ManualResetEvent allDone = new ManualResetEvent(false);

    static void Work(object state)
    {
        int taskNumber = (int)state;
        while (true)
        {
            if (semaphore.WaitOne(500))  
            {
                try
                {
                    Console.WriteLine($"[Задача {taskNumber}] заняла место. Работает поток {Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(2000);    
                    Console.WriteLine($"[Задача {taskNumber}] завершила работу, освобождает место.");
                }
                finally
                {
                    semaphore.Release();  
                }
                break;   
            }
            else
            {
                Console.WriteLine($"[Задача {taskNumber}] таймаут: мест нет, повторяю попытку...");
            }
        }

        if (Interlocked.Increment(ref completed) == 6)
            allDone.Set();
    }

    static void Main()
    {
        for (int i = 1; i <= 6; i++)
            ThreadPool.QueueUserWorkItem(Work, i);

        allDone.WaitOne();
        Console.WriteLine("Все 6 задач завершены.");
        Console.ReadKey();
    }
}
*/

/*
class Mutex4
{
    const int TotalItems = 12;                          
    static Queue<int> buffer = new Queue<int>();
    static Semaphore empty = new Semaphore(5, 5);       
    static Semaphore full = new Semaphore(0, 5);      
    static readonly object locker = new object();     
    static int consumed = 0;                           

    static void Producer()
    {
        for (int i = 1; i <= TotalItems; i++)
        {
            empty.WaitOne();          
            lock (locker)
            {
                buffer.Enqueue(i);
                Console.WriteLine($"[Производитель] добавил {i}. В очереди: {buffer.Count}");
            }
            full.Release();          
            Thread.Sleep(150);
        }
        Console.WriteLine("[Производитель] закончил работу.");
    }

    static void Consumer(object nameObj)
    {
        string name = (string)nameObj;
        while (true)
        {
            full.WaitOne();           
            int item, taken;
            lock (locker)
            {
                if (buffer.Count == 0)  
                {
                    full.Release();     
                    return;
                }
                item = buffer.Dequeue();
                taken = ++consumed;
                Console.WriteLine($"{name}: взял элемент {item}. В очереди: {buffer.Count}");
            }
            empty.Release();           
            Thread.Sleep(400);         

            if (taken == TotalItems)    
            {
                full.Release();        
                return;
            }
        }
    }

    static void Main()
    {
        Thread p = new Thread(Producer);
        Thread c1 = new Thread(Consumer); c1.Start("Потребитель 1");
        Thread c2 = new Thread(Consumer); c2.Start("Потребитель 2");
        p.Start();

        p.Join(); c1.Join(); c2.Join();
        Console.WriteLine($"Обработано элементов: {consumed} из {TotalItems}. Потоки завершились корректно.");
        Console.ReadKey();
    }
}
*/

/*
class Mutex5
{
    static ManualResetEvent startEvent = new ManualResetEvent(false); 

    static void Work(object numberObj)
    {
        int number = (int)numberObj;
        startEvent.WaitOne();         
        Console.WriteLine($"Поток {number} стартовал");
    }

    static void Main()
    {
        for (int i = 1; i <= 10; i++)
        {
            Thread t = new Thread(Work);
            t.IsBackground = true;    
            t.Start(i);
        }

        Console.WriteLine("10 потоков созданы и ждут сигнал...");
        Thread.Sleep(2000);

        Console.WriteLine("SET() — сигнал дан, все потоки отпущены!");
        startEvent.Set();
        Thread.Sleep(1000);

        Console.WriteLine("RESET() — ворота закрыты снова. Запускаю контрольный поток 11...");
        startEvent.Reset();
        Thread t11 = new Thread(Work);
        t11.IsBackground = true;
        t11.Start(11);
        Thread.Sleep(1500);
        Console.WriteLine("Поток 11 молчит — он ждёт сигнал (событие закрыто).");

        Console.ReadKey();
    }
}
*/

class Mutex6
{
    static AutoResetEvent event1 = new AutoResetEvent(true);  
    static AutoResetEvent event2 = new AutoResetEvent(false); 

    static void PrintOdd()
    {
        for (int n = 1; n <= 9; n += 2)
        {
            event1.WaitOne();   
            Console.WriteLine(n);
            event2.Set();        
        }
    }

    static void PrintEven()
    {
        for (int n = 2; n <= 10; n += 2)
        {
            event2.WaitOne();
            Console.WriteLine(n);
            event1.Set();     
        }
    }

    static void Main()
    {
        Thread tOdd = new Thread(PrintOdd);
        Thread tEven = new Thread(PrintEven);
        tOdd.Start();
        tEven.Start();
        tOdd.Join();
        tEven.Join();
        Console.WriteLine("Готово: строгий порядок 1..10");
        Console.ReadKey();
    }
}