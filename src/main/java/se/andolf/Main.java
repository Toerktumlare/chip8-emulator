package se.andolf;

import se.andolf.utils.Utils;

import java.util.Random;

public class Main {

    public static void main(String[] args) {

        final Memory memory = new Memory();
        final byte[] gameData = Utils.load("/roms/TICTAC.ch8");

        memory.loadData(gameData);

        final Register register = new Register();
        final Emulator emulator = new Emulator(memory, register, new Random(), new Keyboard());

        emulator.start();
    }
}
