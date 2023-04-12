import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.concurrent.Semaphore;

public class Main {
    public static void main(String[] args) {
        Init(4, 20, 4, 6);
    }
    private static void Init(int storageSize, int workTarget, int producers, int consumers) {
        Storage storage = new Storage(storageSize, workTarget);

        for (int i = 0; i < consumers; i++) {
            new Consumer(i, storage).start();
        }

        for (int i = 0; i < producers; i++) {
            new Producer(i, storage).start();
        }
    }
}

class Storage{
    public int size;
    public Semaphore access;
    public Semaphore empty;
    public Semaphore full;
    public List<String> buffer;
    public volatile int workTarget;
    public volatile int workDone;

    private int lastIndex = 0;

    public Storage(int size, int workTarget){
        this.size = size;
        this.access = new Semaphore(1);
        this.empty = new Semaphore(0);
        this.full = new Semaphore(size);
        this.workTarget = workTarget;
        this.workDone = 0;
        buffer = new ArrayList<>();
    }

    public int put(){
        buffer.add("item " + lastIndex);
        lastIndex++;
        return lastIndex - 1;
    }
}

abstract class WorkingThread extends Thread{
    protected final Storage storage;
    protected final int index;

    protected Random random = new Random();

    public WorkingThread(int index, Storage storage){
        this.storage = storage;
        this.index = index;
    }

    @Override
    public abstract void run();
}

class Producer extends WorkingThread{
    public Producer(int index, Storage storage){
        super(index, storage);
    }

    @Override
    public void run() {
        while (storage.workDone < storage.workTarget){
            try {
                storage.full.acquire();
                Thread.sleep(random.nextInt(0, 100));
                storage.access.acquire();

                if (storage.workDone >= storage.workTarget){
                    storage.access.release();
                    return;
                }

                int itemIndex = storage.put();
                System.out.println("Producer " + index + " added " + (itemIndex));
                storage.workDone++;

                storage.access.release();
                storage.empty.release();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }
}

class Consumer extends WorkingThread{
    public Consumer(int index, Storage storage){
        super(index, storage);
    }

    @Override
    public void run() {
        while (storage.workDone < storage.workTarget){
            try {
                storage.empty.acquire();
                Thread.sleep(random.nextInt(0, 100));
                storage.access.acquire();

                if (storage.workDone >= storage.workTarget){
                    storage.access.release();
                    return;
                }

                String item = storage.buffer.remove(0);
                System.out.println("Consumer " + index + " took " + (item));
                storage.workDone++;

                storage.access.release();
                storage.full.release();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }
}