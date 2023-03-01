import java.util.InputMismatchException;
import java.util.Scanner;

public class Main {
    public static void main(String[] args) {

        int threadsCount = 0;
        try (Scanner scanner = new Scanner(System.in)) {
            System.out.print("Enter count of threads: ");
            threadsCount = scanner.nextInt();
        } catch (InputMismatchException e) {
            System.out.println("Incorrect input");
            return;
        }


        ComputeThread[] computeThreads = new ComputeThread[threadsCount];
        BreakerThread breakerThread = new BreakerThread(15);

        for (int i = 0; i < threadsCount; i++) {
            computeThreads[i] = new ComputeThread(threadsCount, i, breakerThread);
            computeThreads[i].start();
        }
        breakerThread.start();

        for (int i = 0; i < threadsCount; i++) {
            try {
                computeThreads[i].join();
                System.out.println("Thread " + i + " sum: " + computeThreads[i].getSum());
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }

        System.out.println("Total sum: " + getTotalSum(computeThreads));
    }
    private static long getTotalSum(ComputeThread[] computeThreads) {
        long sum = 0;
        for (ComputeThread computeThread : computeThreads) {
            sum += computeThread.getSum();
        }
        return sum;
    }
}


class ComputeThread extends Thread {
    private long sum = 0;
    private final int threadsCount;
    private final int threadNumber;
    private final BreakerThread breakerThread;

    public ComputeThread(int threadsCount, int threadNumber, BreakerThread breakerThread) {
        this.threadsCount = threadsCount;
        this.threadNumber = threadNumber;
        this.breakerThread = breakerThread;
    }

    @Override
    public void run(){
        for (sum = threadNumber; breakerThread.isWorking; sum += threadsCount);
    }

    public long getSum() {
        return sum;
    }
}

class BreakerThread extends Thread{
    private final int secondsWait;
    public volatile boolean isWorking = true;

    public BreakerThread(int secondsWait) {
        this.secondsWait = secondsWait;
    }

    @Override
    public void run(){
        try {
            sleep(secondsWait * 1000L);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        isWorking = false;
    }
}

