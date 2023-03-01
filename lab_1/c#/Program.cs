using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace lab_1
{
    public class Program
    {
        static void Main()
        {
            if (!int.TryParse(Console.ReadLine(), out int threadsCount)) return;

            List<ComputeThread> threads = new List<ComputeThread>();
            BreakerThread breaker = new BreakerThread(breakInSec: 20);

            for (int i = 0; i < threadsCount; i++)
            {
                threads.Add(new ComputeThread(i, threadsCount, breaker));
                threads[i].Thread.Start();
            }
            breaker.Thread.Start();

            for (int i = 0; i < threadsCount; i++)
            {
                threads[i].Thread.Join();
                Console.WriteLine($"Sum of thread #{i} is {threads[i].Sum}");
            }
            Console.WriteLine($"Total sum is {threads.Sum(x => x.Sum)}");
            Console.ReadKey();
        }
    }

    public class ComputeThread
    {
        public Thread Thread { get; }
        public long Sum => sum;

        private int threadsCount;
        private int threadNumber;
        private BreakerThread breaker;
        private long sum;

        public ComputeThread(int threadNumber, int threadsCount, BreakerThread breaker)
        {
            this.threadNumber = threadNumber;
            this.threadsCount = threadsCount;
            this.breaker = breaker;
            Thread = new Thread(Compute);
        }

        private void Compute()
        {
            for (sum = threadNumber; breaker.IsWorking; sum += threadsCount);
        }
    }

    public class BreakerThread
    {
        public Thread Thread { get; }

        private int breakInSec;
        public bool IsWorking { get; private set; } = true;

        public BreakerThread(int breakInSec)
        {
            this.breakInSec = breakInSec;
            Thread = new Thread(Break);
        }

        private void Break()
        {
            Thread.Sleep(breakInSec * 1000);
            IsWorking = false;
        }
    }
}
