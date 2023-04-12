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
            for (int i = 0; i < 5; i++)
            {
                philosophers.Add(new Philosopher(i, forks[i], forks[(i + 1) % 5]));
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

        public Philosopher(int id ,Fork forkLeft, Fork forkRight)
        {
            _id = id;
            _forkLeft = forkLeft;
            _forkRight = forkRight;
        }

        public void Eat()
        {
            var minFork = _forkLeft.Id < _forkRight.Id ? _forkLeft : _forkRight;
            var maxFork = _forkLeft.Id > _forkRight.Id ? _forkLeft : _forkRight;

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Philosopher {0} thinking time {1}", _id, i);
                minFork.Access.WaitOne();
                Console.WriteLine("Philosopher {0} took fork {1}", _id, minFork.Id);
                maxFork.Access.WaitOne();
                Console.WriteLine("Philosopher {0} took fork {1}", _id, maxFork.Id);
                Console.WriteLine("Philosopher {0} eating time {1}", _id, i);
                maxFork.Access.Release();
                Console.WriteLine("Philosopher {0} put fork {1}", _id, maxFork.Id);
                minFork.Access.Release();
                Console.WriteLine("Philosopher {0} put fork {1}", _id, minFork.Id);
            }
        }
    }

    public class Fork
    {
        public int Id { get; set; }

        public Semaphore Access { get; set; } = new Semaphore(1, 1);
    }
}
