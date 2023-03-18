using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

public class Program
{
    public static object MainLock = new object();
    public static int ThreadsFinishedCounter = 0;


    private static readonly int threadsCount = 2;
    private static readonly int maxArraySize = 100000000;

    private static void Main(string[] args)
    {
        Console.WriteLine($"Threads count is {threadsCount}");
        int[] array = new int[maxArraySize];
        RandomizeArray(ref array);

        ComputeThread[] computeThreads = new ComputeThread[threadsCount];
        Thread[] threads = new Thread[threadsCount];

        Stopwatch timer = new Stopwatch();
        timer.Start();

        for (int i = 0; i < threadsCount; i++)
        {
            computeThreads[i] = new ComputeThread(array, i, threadsCount);
            threads[i] = computeThreads[i].StartInSeparateThread();
        }

        lock (MainLock)
        {
            while (ThreadsFinishedCounter < threadsCount)
            {
                Monitor.Wait(MainLock);
            }
        }

        timer.Stop();

        var ArrayMinValue = GetMinValue(computeThreads.Select(x => x.MinArrayValue));
        Console.WriteLine($"Founded element with index: {ArrayMinValue.Key} and value {ArrayMinValue.Value}");
        Console.WriteLine($"Time of execution: {timer.ElapsedMilliseconds} ms");
        Console.ReadKey();
    }

    
    private static KeyValuePair<int, int> GetMinValue(IEnumerable<KeyValuePair<int, int>> enumerable)
    {
        KeyValuePair<int, int> minValue = enumerable.First();
        foreach (var value in enumerable)
        {
            if (value.Value < minValue.Value)
                minValue = value;
        }
        return minValue;
    }

    private static void RandomizeArray(ref int[] array)
    {
        Random random = new Random();
        int index = random.Next(0, array.Length);
        int value = random.Next(int.MinValue, 0);
        array[index] = value;
        Console.WriteLine($"Element with index: {index} replaced with {value}");
    }
}


public class ComputeThread
{
    private int threadIndex;
    private int threadsCount;
    private int[] array;
    private KeyValuePair<int, int> min;
    public KeyValuePair<int, int> MinArrayValue => min;

    public ComputeThread(int[] array, int threadIndex, int threadsCount)
    {
        this.array = array;
        this.threadIndex = threadIndex;
        this.threadsCount = threadsCount;
    }

    public Thread StartInSeparateThread()
    {
        Thread thread = new Thread(new ThreadStart(Run));
        thread.Start();
        return thread;
    }
    
    private void Run()
    {
        int firstIndex = threadIndex * array.Length / threadsCount;
        int lastIndex = (threadIndex + 1) * array.Length / threadsCount;

        min = new KeyValuePair<int, int>(firstIndex, array[firstIndex]);
        
        for (int i = firstIndex; i < lastIndex; i++)
        {
            if (array[i] < min.Value)
            {
                min = new KeyValuePair<int, int>(i, array[i]);
            }
        }

        Console.WriteLine($"Min element in thread {threadIndex} with index {min.Key} is {min.Value}");

        lock (Program.MainLock)
        {
            Program.ThreadsFinishedCounter++;
            Monitor.Pulse(Program.MainLock);
        }
    }
}