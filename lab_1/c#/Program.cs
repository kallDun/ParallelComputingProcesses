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
                threads.Add(new ComputeThread(step: threadsCount, breaker));
                threads[i].Thread.Start();
            }
            breaker.Thread.Start();

            for (int i = 0; i < threadsCount; i++)
            {
                threads[i].Thread.Join();
                Console.WriteLine($"Sum of thread #{i} is {threads[i].Sum}, elements count is {threads[i].ElementsCount}");
            }
            Console.ReadKey();
        }
    }

    public class ComputeThread
    {
        public Thread Thread { get; }
        public long Sum => sum;
        public long ElementsCount => elementsCount;

        private readonly int step;
        private readonly BreakerThread breaker;
        private long sum = 0;
        private long elementsCount = 0;

        public ComputeThread(int step, BreakerThread breaker)
        {
            this.step = step;
            this.breaker = breaker;
            Thread = new Thread(Compute);
        }

        private void Compute()
        {
            for (sum = 0; breaker.IsWorking; sum += step, elementsCount++);
        }
    }

    public class BreakerThread
    {
        public Thread Thread { get; }

        private readonly int breakInSec;
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
