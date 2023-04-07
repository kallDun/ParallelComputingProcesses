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
            Init(storageSize: 4, itemNumbers: 5, producersCount: 4, consumersCount: 3);
            Console.ReadKey();
        }

        private static void Init(int storageSize, int itemNumbers, int producersCount, int consumersCount)
        {
            Storage storage = new Storage(storageSize, producersCount, consumersCount);
            List<WorkingThread> producers = new List<WorkingThread>();
            List<WorkingThread> consumers = new List<WorkingThread>();

            for (int i = 0; i < producersCount; i++)
            {
                producers.Add(new Producer(i, storage, itemNumbers));
            }
            for (int i = 0; i < consumersCount; i++)
            {
                consumers.Add(new Consumer(i, storage, itemNumbers));
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
        public Storage(int storageSize, int producersCount, int consumersCount)
        {
            Access = new Semaphore(1, 1);
            Full = new Semaphore(storageSize, storageSize);
            Empty = new Semaphore(0, storageSize);
            Size = storageSize;
        }
    }

    public abstract class WorkingThread
    {
        protected static volatile Random random = new Random();
        protected volatile int itemNumbers;
        protected volatile Storage storage;
        protected volatile int index;
        private Thread thread;

        protected WorkingThread(int index, Storage storage, int itemNumbers)
        {
            this.index = index;
            this.itemNumbers = itemNumbers;
            this.storage = storage;
            thread = new Thread(DoWork);
            thread.Start();
        }

        public abstract void DoWork();
    }

    public class Producer : WorkingThread
    {
        private static volatile int currentIndex = 0;

        public Producer(int index, Storage storage, int itemNumbers) : base(index, storage, itemNumbers)
        {
        }

        public override void DoWork()
        {
            while (currentIndex < itemNumbers)
            {
                storage.Full.WaitOne();
                Thread.Sleep(random.Next(0, 100));
                storage.Access.WaitOne();
                if (currentIndex >= itemNumbers)
                {
                    storage.Access.Release();
                    return;
                }

                storage.List.Add($"item {currentIndex}");
                Console.WriteLine($"Producer {currentIndex} added item {currentIndex}" +
                    $"\t| storage: {storage.List.Count}/{storage.Size} \t| work: {currentIndex + 1}/{itemNumbers} |");
                currentIndex++;

                storage.Access.Release();
                storage.Empty.Release();
            }
        }
    }

    public class Consumer : WorkingThread
    {
        private static volatile int currentIndex = 0;

        public Consumer(int index, Storage storage, int itemNumbers) : base(index, storage, itemNumbers)
        {
        }

        public override void DoWork()
        {
            while (currentIndex < itemNumbers)
            {
                storage.Empty.WaitOne();
                Thread.Sleep(random.Next(0, 100));
                storage.Access.WaitOne();
                if (currentIndex >= itemNumbers)
                {
                    storage.Access.Release();
                    return;
                }

                string item = storage.List.ElementAt(0);
                storage.List.RemoveAt(0);
                Console.WriteLine($"Consumer {index} took {item}" +
                    $"\t| storage: {storage.List.Count}/{storage.Size} \t| work: {currentIndex + 1}/{itemNumbers} |");
                currentIndex++;

                storage.Access.Release();
                storage.Full.Release();
            }
        }
    }
}
