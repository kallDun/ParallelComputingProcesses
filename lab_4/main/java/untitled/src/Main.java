import java.util.concurrent.Semaphore;

public class Main {
    public static void main(String[] args) {
        Fork[] forks = new Fork[5];
        for (int i = 0; i < 5; i++) {
            forks[i] = new Fork(i);
        }
        Philosopher[] philosophers = new Philosopher[5];
        for (int i = 0; i < 5; i++) {
            philosophers[i] = new Philosopher(i, forks[i], forks[(i + 1) % 5]);
        }
        for (int i = 0; i < 5; i++) {
            new Thread(philosophers[i]).start();
        }
    }
}

class Philosopher implements Runnable{
    private final int id;
    private final Fork leftFork;
    private final Fork rightFork;

    public Philosopher(int id, Fork leftFork, Fork rightFork){
        this.id = id;
        this.leftFork = leftFork;
        this.rightFork = rightFork;
    }

    @Override
    public void run() {
        Fork minFork = leftFork.id < rightFork.id ? leftFork : rightFork;
        Fork maxFork = leftFork.id < rightFork.id ? rightFork : leftFork;

        try{
            for (int i = 0; i < 10; i++) {
                System.out.println("Philosopher " + id + " thinking time " + i);
                minFork.access.acquire();
                System.out.println("Philosopher " + id + " took fork " + minFork.id);
                maxFork.access.acquire();
                System.out.println("Philosopher " + id + " took fork " + maxFork.id);
                System.out.println("Philosopher " + id + " eating time " + i);
                maxFork.access.release();
                System.out.println("Philosopher " + id + " put fork " + maxFork.id);
                minFork.access.release();
                System.out.println("Philosopher " + id + " put fork " + minFork.id);

            }
        }catch(InterruptedException e){
            e.printStackTrace();
        }
    }
}

class Fork{
    public int id;
    public Semaphore access;

    public Fork(int id){
        this.id = id;
        this.access = new Semaphore(1);
    }
}