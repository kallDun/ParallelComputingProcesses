import java.util.concurrent.Semaphore;

public class Main {
    public static void main(String[] args) {
        Fork[] forks = new Fork[5];
        for (int i = 0; i < 5; i++) {
            forks[i] = new Fork(i);
        }
        Waiter waiter = new Waiter();
        Philosopher[] philosophers = new Philosopher[5];
        for (int i = 0; i < 5; i++) {
            philosophers[i] = new Philosopher(i, forks[i], forks[(i + 1) % 5], waiter);
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
    private final Waiter waiter;

    public Philosopher(int id, Fork leftFork, Fork rightFork, Waiter waiter){
        this.id = id;
        this.leftFork = leftFork;
        this.rightFork = rightFork;
        this.waiter = waiter;
    }

    @Override
    public void run() {
        try{
            for (int i = 0; i < 10; i++) {
                System.out.println("Philosopher " + id + " thinking time " + i);
                waiter.access.acquire();
                leftFork.access.acquire();
                System.out.println("Philosopher " + id + " took fork " + leftFork.id);
                rightFork.access.acquire();
                System.out.println("Philosopher " + id + " took fork " + rightFork.id);
                System.out.println("Philosopher " + id + " eating time " + i);
                rightFork.access.release();
                System.out.println("Philosopher " + id + " put fork " + rightFork.id);
                leftFork.access.release();
                System.out.println("Philosopher " + id + " put fork " + leftFork.id);
                waiter.access.release();
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
class Waiter{
    public Semaphore access;
    public Waiter(){
        this.access = new Semaphore(4);
    }
}