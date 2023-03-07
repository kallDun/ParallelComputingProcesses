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
            computeThreads[i] = new ComputeThread(threadsCount, breakerThread);
            computeThreads[i].start();
        }
        breakerThread.start();

        for (int i = 0; i < threadsCount; i++) {
            try {
                computeThreads[i].join();
                System.out.println("Thread " + i + " sum: " + computeThreads[i].getSum() + " elements: " + computeThreads[i].getElements());
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }
}


class ComputeThread extends Thread {
    private long sum = 0;
    private long elements = 0;
    private final int step;
    private final BreakerThread breakerThread;

    public ComputeThread(int step, BreakerThread breakerThread) {
        this.step = step;
        this.breakerThread = breakerThread;
    }

    @Override
    public void run(){
        for (sum = 0; breakerThread.isWorking; sum += step, elements++);
    }

    public long getSum() {
        return sum;
    }
    public long getElements() { return elements; }
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

