package se.andolf;

import se.andolf.utils.Utils;

public class Main {

    public static void main(String[] args) {

        if(args.length == 0) {
            System.out.println("You must supply a game argument, example: /roms/INVADERS.ch8");
            System.exit(1);
        } else {
            Utils.load(args[0]).ifPresentOrElse(bytes -> {
                final Emulator emulator = new Emulator(bytes);
                emulator.start();
            }, () -> System.exit(1));
        }
    }
}
