using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace sharp
{
    public class Program
    {
        static void Main(string[] args)
        {
            Init(storageSize: 4, itemNumbersForWork: 20, producersCount: 4, consumersCount: 6);
            Console.ReadKey();
        }

        private static void Init(int storageSize, int itemNumbersForWork, int producersCount, int consumersCount)
        {
            Storage storage = new Storage(storageSize, itemNumbersForWork);
            List<WorkingThread> producers = new List<WorkingThread>();
            List<WorkingThread> consumers = new List<WorkingThread>();

            for (int i = 0; i < consumersCount; i++)
            {
                consumers.Add(new Consumer(i, storage));
            }
            for (int i = 0; i < producersCount; i++)
            {
                producers.Add(new Producer(i, storage));
            }
        }
    }

    public class Storage
    {
        public int Size { get; private set; }
        public Semaphore Access { get; set; }
        public Semaphore Full { get; set; }
        public Semaphore Empty { get; set; }
        public List<string> List { get; } = new List<string>();
        public int ItemNumbers { get; }
        
        private int lastIndex = 0;
        
        public Storage(int storageSize, int itemNumbersForWork)
        {
            Access = new Semaphore(1, 1);
            Full = new Semaphore(storageSize, storageSize);
            Empty = new Semaphore(0, storageSize);
            Size = storageSize;
            ItemNumbers = itemNumbersForWork;
        }
        
        public int AddNewItemToList()
        {
            List.Add($"item {lastIndex++}");
            return lastIndex - 1;
        }
    }

    public abstract class WorkingThread
    {
        protected static volatile int workIndex = 0;

        protected static volatile Random random = new Random();
        protected volatile Storage storage;
        protected volatile int index;
        private Thread thread;

        protected WorkingThread(int index, Storage storage)
        {
            this.index = index;
            this.storage = storage;
            thread = new Thread(DoWork);
            thread.Start();
        }

        public abstract void DoWork();
    }

    public class Producer : WorkingThread
    {
        public Producer(int index, Storage storage) : base(index, storage)
        {
        }

        public override void DoWork()
        {
            while (workIndex < storage.ItemNumbers)
            {
                storage.Full.WaitOne();
                Thread.Sleep(random.Next(0, 100));
                storage.Access.WaitOne();
                if (workIndex >= storage.ItemNumbers)
                {
                    storage.Access.Release();
                    return;
                }

                int itemIndex = storage.AddNewItemToList();
                Console.WriteLine($"{workIndex + 1}. Producer {index} added item {itemIndex}" +
                    $"      \t| storage: {storage.List.Count}/{storage.Size}");
                workIndex++;

                storage.Access.Release();
                storage.Empty.Release();
            }
        }
    }

    public class Consumer : WorkingThread
    {
        public Consumer(int index, Storage storage) : base(index, storage)
        {
        }

        public override void DoWork()
        {
            while (workIndex < storage.ItemNumbers)
            {
                storage.Empty.WaitOne();
                Thread.Sleep(random.Next(0, 100));
                storage.Access.WaitOne();
                if (workIndex >= storage.ItemNumbers)
                {
                    storage.Access.Release();
                    return;
                }

                string item = storage.List.ElementAt(0);
                storage.List.RemoveAt(0);
                Console.WriteLine($"{workIndex + 1}. Consumer {index} took {item}" +
                    $"       \t| storage: {storage.List.Count}/{storage.Size}");
                workIndex++;

                storage.Access.Release();
                storage.Full.Release();
            }
        }
    }
}
