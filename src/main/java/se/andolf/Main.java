package se.andolf;

import se.andolf.utils.Utils;

import javax.swing.*;
import java.awt.*;

public class Main {

    public static void main(String[] args) {

        final Memory memory = new Memory();
        final byte[] gameData = Utils.load("/roms/tetris.c8");
        memory.loadData(gameData);
        final Register register = new Register();
        final Emulator emulator = new Emulator(memory, register);

        for(;;)
        {
            // Emulate one cycle
            emulator.emulateCycle();

            // If the draw flag is set, update the screen
            if(emulator.getDrawFlag())


            // Store key press state (Press and Release)
            emulator.setKeys();
        }
    }

    private static void startWindow() {
        JFrame window = new JFrame();
        window.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        window.setBounds(30, 30, 300, 300);
        window.getContentPane().add(new MyCanvas());
        window.setVisible(true);
    }
}

class MyCanvas extends JComponent {

    public void paint(Graphics g) {
        g.drawRect (10, 10, 200, 200);
    }
}
