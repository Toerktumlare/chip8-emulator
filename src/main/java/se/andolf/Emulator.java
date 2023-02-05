package se.andolf;

import javax.swing.*;
import java.awt.*;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.util.Random;

public class Emulator implements Runnable, KeyListener {

    private static final int WIDTH = 64;
    private static final int HEIGHT = 32;

    public static final int SCALE = 10;
    private final CPU cpu;

    private Thread thread;
    private boolean isRunning;

    private final Keyboard keyboard;

    public final JFrame frame;
    private final Screen screen;


    public Emulator(byte[] gameData) {

        this.keyboard = new Keyboard();

        final Memory memory = new Memory();
        final Register register = new Register();
        final Random random = new Random();

        memory.loadData(gameData);


        screen = new Screen(WIDTH, HEIGHT, SCALE);
        screen.setPreferredSize(new Dimension(WIDTH * SCALE, HEIGHT * SCALE));

        frame = new JFrame();
        frame.setResizable(false);
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        frame.setTitle("Chip-8 Emulator");
        frame.add(screen);
        frame.pack();
        frame.setLocationRelativeTo(null);
        frame.setVisible(true);
        frame.setAlwaysOnTop(true);
        frame.addKeyListener(this);

        this.cpu = new CPU(memory, register, random, keyboard, screen);
    }

    public synchronized void start() {
        isRunning = true;
        thread = new Thread(this, "Display");
        thread.start();
    }

    public synchronized void stop() {
        isRunning = false;
        try {
            thread.join();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
    }

    @Override
    public void run() {
        while (isRunning) {
            update();
            render();
        }
    }

    private void render() {

        if(cpu.getDrawFlag()) {
            screen.render();
            cpu.setDrawFlag(false);
        }


        if (cpu.getDelayTimer() > 0) {
            cpu.setDelayTimer(cpu.getDelayTimer()-1);
        }

        try {
            Thread.sleep(1L, 10000);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
    }

    private void update() {
        cpu.emulateCycle();
    }

    @Override
    public void keyTyped(KeyEvent e) {
        // Not used
    }

    @Override
    public void keyPressed(KeyEvent e) {
        keyboard.onKeyPressed(e.getKeyCode());
    }

    @Override
    public void keyReleased(KeyEvent e) {
        keyboard.onKeyReleased(e.getKeyCode());
    }
}
