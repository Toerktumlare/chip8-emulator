package se.andolf;

import java.awt.event.KeyEvent;

public class Keyboard {

    private boolean keys[] = new boolean[16];

    public static int[] FONTS = {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0      0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1      5
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2     10
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3     15
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4     20
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5     25
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6     30
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7     35
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8     40
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
    };

    public void onKeyPressed(int keyCode) {
        switch (keyCode) {
            case KeyEvent.VK_1:
                keys[0] = true;
                break;
            case KeyEvent.VK_2:
                keys[1] = true;
                break;
            case KeyEvent.VK_3:
                keys[2] = true;
                break;
            case KeyEvent.VK_4:
                keys[3] = true;
                break;
            case KeyEvent.VK_Q:
                keys[4] = true;
                break;
            case KeyEvent.VK_W:
                keys[5] = true;
                break;
            case KeyEvent.VK_E:
                keys[6] = true;
                break;
            case KeyEvent.VK_R:
                keys[7] = true;
                break;
            case KeyEvent.VK_A:
                keys[8] = true;
                break;
            case KeyEvent.VK_S:
                keys[9] = true;
                break;
            case KeyEvent.VK_D:
                keys[10] = true;
                break;
            case KeyEvent.VK_F:
                keys[11] = true;
                break;
            case KeyEvent.VK_Z:
                keys[12] = true;
                break;
            case KeyEvent.VK_X:
                keys[13] = true;
                break;
            case KeyEvent.VK_C:
                keys[14] = true;
                break;
            case KeyEvent.VK_V:
                keys[15] = true;
                break;
            default:
                break;
        }
    }

    public void onKeyReleased(int keyCode) {
        switch (keyCode) {
            case KeyEvent.VK_1:
                keys[0] = false;
                break;
            case KeyEvent.VK_2:
                keys[1] = false;
                break;
            case KeyEvent.VK_3:
                keys[2] = false;
                break;
            case KeyEvent.VK_4:
                keys[3] = false;
                break;
            case KeyEvent.VK_Q:
                keys[4] = false;
                break;
            case KeyEvent.VK_W:
                keys[5] = false;
                break;
            case KeyEvent.VK_E:
                keys[6] = false;
                break;
            case KeyEvent.VK_R:
                keys[7] = false;
                break;
            case KeyEvent.VK_A:
                keys[8] = false;
                break;
            case KeyEvent.VK_S:
                keys[9] = false;
                break;
            case KeyEvent.VK_D:
                keys[10] = false;
                break;
            case KeyEvent.VK_F:
                keys[11] = false;
                break;
            case KeyEvent.VK_Z:
                keys[12] = false;
                break;
            case KeyEvent.VK_X:
                keys[13] = false;
                break;
            case KeyEvent.VK_C:
                keys[14] = false;
                break;
            case KeyEvent.VK_V:
                keys[15] = false;
                break;
            default:
                break;
        }
    }

    public boolean isPressed(int index) {
        return keys[index];
    }
}
