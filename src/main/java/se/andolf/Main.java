package se.andolf;

public class Main {

    public static void main(String[] args) {
        final Emulator emulator = new Emulator("/roms/INVADERS.ch8");
        emulator.start();
    }
}
