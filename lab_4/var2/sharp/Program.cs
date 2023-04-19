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
            for (int i = 0; i < 10; i++)
            {
                TryAgain:
                Console.WriteLine("Philosopher {0} thinking time {1}", _id, i);
                _forkLeft.Access.WaitOne();
                Console.WriteLine("Philosopher {0} took fork {1}", _id, _forkLeft.Id);
                if (!_forkRight.Access.WaitOne(0))
                {
                    Console.WriteLine("Philosopher {0} put fork {1}", _id, _forkLeft.Id);
                    _forkLeft.Access.Release();
                    goto TryAgain;
                }
                
                Console.WriteLine("Philosopher {0} took fork {1}", _id, _forkRight.Id);
                Console.WriteLine("Philosopher {0} eating time {1}", _id, i);
                _forkRight.Access.Release();
                Console.WriteLine("Philosopher {0} put fork {1}", _id, _forkRight.Id);
                _forkLeft.Access.Release();
                Console.WriteLine("Philosopher {0} put fork {1}", _id, _forkLeft.Id);
            }
        }
    }

    public class Fork
    {
        public int Id { get; set; }

        public Semaphore Access { get; set; } = new Semaphore(1, 1);
    }
}
