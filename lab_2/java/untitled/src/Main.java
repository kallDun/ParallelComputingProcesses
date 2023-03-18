import javax.xml.crypto.dsig.keyinfo.KeyValue;
import java.util.Random;

public class Main {

    private int threadIndexDone = 0;
    private final int threadsCount = 8;
    private final int maxArraySize = 100000000;

    private int minIndex = -1;
    private int minElement = Integer.MAX_VALUE;
    private ComputeThread[] threads;

    public static void main(String[] args) {
        Main main = new Main();
        main.EntryPoint();
    }

    public void EntryPoint(){
        System.out.println("Threads count: " + threadsCount);
        int[] array = new int[100000000];
        RandomizeArray(array);

        long startTime = System.nanoTime();

        threads = new ComputeThread[threadsCount];
        for (int i = 0; i < threadsCount; i++) {
            threads[i] = new ComputeThread(array, i, threadsCount, this);
            threads[i].start();
        }
        WaitForThreads();

        long endTime = System.nanoTime();

        System.out.println("Min index: " + minIndex + ", min element: " + array[minIndex]);
        System.out.println("Time: " + (endTime - startTime) + " nanoseconds");
    }

    synchronized private void WaitForThreads(){
        while (threadsCount > threadIndexDone){
            try {
                wait();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }

    synchronized public void incrementThreadIndexDone(int threadIndex){
        System.out.println("Thread " + threadIndex + " done with min index: " + threads[threadIndex].getMinIndex() + ", with value: " + threads[threadIndex].getMinElement());

        if (minIndex == -1) {
            minIndex = threads[threadIndex].getMinIndex();
            minElement = threads[threadIndex].getMinElement();
        }
        else if (threads[threadIndex].getMinElement() < minElement) {
            minIndex = threads[threadIndex].getMinIndex();
            minElement = threads[threadIndex].getMinElement();
        }

        threadIndexDone++;
        notify();
    }

    private void RandomizeArray(int[] array) {
        Random random = new Random();
        int randomIndex = random.nextInt(array.length);
        int randomValue = random.nextInt(Integer.MIN_VALUE, 0);
        array[randomIndex] = randomValue;
        System.out.println("Randomize min index in: " + randomIndex + ", with value: " + randomValue);
    }
}

class ComputeThread extends Thread{

    private final Main owner;
    private final int[] array;
    private final int threadIndex;
    private final int threadsCount;

    private int minIndex;
    private int minElement;


    public ComputeThread(int[] array, int threadIndex, int threadsCount, Main owner) {
        this.array = array;
        this.threadIndex = threadIndex;
        this.threadsCount = threadsCount;
        this.owner = owner;
    }

    @Override
    public void run() {
        int firstIndex = threadIndex * array.length / threadsCount;
        int lastIndex = (threadIndex + 1) * array.length / threadsCount;
        minIndex = firstIndex;
        minElement = array[firstIndex];
        for (int i = firstIndex; i < lastIndex; i++) {
            if (array[i] < minElement) {
                minElement = array[i];
                minIndex = i;
            }
        }
        owner.incrementThreadIndexDone(threadIndex);
    }

    public int getMinIndex() {
        return minIndex;
    }
    public int getMinElement() {
        return minElement;
    }
}