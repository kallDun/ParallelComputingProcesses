using System;
using System.Collections.Generic;
using System.Threading;

namespace sharp
{
    public class Program
    {
        static void Main()
        {
            List<Fork> forks = new List<Fork>();
            List<Philosopher> philosophers = new List<Philosopher>();
            for (int i = 0; i < 5; i++)
            {
                forks.Add(new Fork() { Id = i });
            }
            Waiter waiter = new Waiter { Forks = forks };
            for (int i = 0; i < 5; i++)
            {
                philosophers.Add(new Philosopher(i, forks[i], forks[(i + 1) % 5], waiter));
            }
            foreach (var philosopher in philosophers)
            {
                new Thread(philosopher.Eat).Start();
            }
            Console.ReadKey();
        }
    }

    public class Philosopher
    {
        private int _id;
        private Fork _forkLeft;
        private Fork _forkRight;
        private Waiter _waiter;

        public Philosopher(int id ,Fork forkLeft, Fork forkRight, Waiter waiter)
        {
            _id = id;
            _forkLeft = forkLeft;
            _forkRight = forkRight;
            _waiter = waiter;
        }

        public void Eat()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Philosopher {0} thinking time {1}", _id, i);
                while (!_waiter.CanEat()) { }
                _forkLeft.Access.Wait();
                Console.WriteLine("Philosopher {0} took fork {1}", _id, _forkLeft.Id);
                _forkRight.Access.Wait();
                Console.WriteLine("Philosopher {0} took fork {1}", _id, _forkRight.Id);
                Console.WriteLine("Philosopher {0} eating time {1}", _id, i);
                _forkRight.Access.Release();
                Console.WriteLine("Philosopher {0} put fork {1}", _id, _forkRight.Id);
                _forkLeft.Access.Release();
                Console.WriteLine("Philosopher {0} put fork {1}", _id, _forkLeft.Id);
                _waiter.StopEat();
            }
        }
    }

    public class Fork
    {
        public int Id { get; set; }

        public SemaphoreSlim Access { get; set; } = new SemaphoreSlim(1, 1);
    }

    public class Waiter
    {
        private object locker = new object();
        private int count = 0;
        public List<Fork> Forks { get; set; }
        
        public bool CanEat()
        {
            lock (locker)
            {
                if (count < 4)
                {
                    count++;
                    return true;
                }
                return false;
            }
        }
        public void StopEat()
        {
            lock (locker)
            {
                count--;
            }
        }
    }
}
