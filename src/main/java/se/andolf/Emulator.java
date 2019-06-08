package se.andolf;

import java.util.Random;
import java.util.Stack;

public class Emulator {

    private final Memory memory;
    private final Register register;
    private final Random random;

    //Index register
    private int I;

    //program counter
    private int pc;

    private byte[] gfx = new byte[64 * 32];

    private byte delay_timer;
    private byte sound_timer;

    private Stack<Integer> stack = new Stack<>();
    private int sp;

    private byte[] key = new byte[16];

    private boolean drawFlag;

    public Emulator(Memory memory, Register register, Random random) {

        this.memory = memory;
        this.register = register;
        this.random = random;

        pc = 0x200;
        I = 0;
        sp = 0;
    }

    public void emulateCycle() {

        final int opcode = memory.getOpcode(pc);
        printOpcode(opcode);

        switch(opcode) {
            case 0x00EE: // Returns from a subroutine
                pc = stack.pop();
                drawFlag = true;
                pc += 2;
                break;
        }

        switch (opcode & 0xF000) {

            case 0x1000:
                pc = opcode & 0x0FFF;
                break;

            case 0x2000:
                stack.push(pc);
                pc = opcode & 0x0FFF;
                break;

            case 0x3000:
                if(register.get((opcode & 0x0F00) >>> 8) == (opcode & 0x00FF)) {
                    pc += 4;
                } else {
                    pc += 2;
                }
                break;

            case 0x4000:
                if(register.get((opcode & 0x0F00) >>> 8) != (opcode & 0x00FF)) {
                    pc += 4;
                } else {
                    pc += 2;
                }
                break;

            case 0x5000:
                if(register.get((opcode & 0x0F00) >>> 8) == register.get((opcode & 0x00F0) >>> 4)) {
                    pc += 4;
                } else {
                    pc += 2;
                }
                break;

            case 0x6000:
                register.set((opcode & 0x0F00) >>> 8, (opcode & 0x00FF));
                pc += 2;
                break;

            case 0x7000:
                register.apply((opcode & 0x0F00) >>> 8, x -> {
                    int result = x + (opcode & 0x00FF);
                    if(result >= 256) {
                        return (result - 256);
                    } else {
                        return result;
                    }
                });
                pc += 2;
                break;

            case 0xA000:
                I = opcode & 0x00FF;
                pc += 2;
                break;

            case 0xD000:
                System.out.println("Draw");
                pc += 2;
                break;
        }

        int x = (opcode & 0x0F00) >>> 8;
        int y = (opcode & 0x00F0) >>> 4;

        switch (opcode & 0xF00F) {

            case 0x8000:
                register.set(x, y);
                pc += 2;
                break;

            case 0x8001:
                register.apply(x, vx -> vx | y);
                pc += 2;
                break;

            case 0x8002:
                register.apply(x, vx -> vx & y);
                pc += 2;
                break;

            case 0x8003:
                register.apply(x, vx -> vx ^ y);
                pc += 2;
                break;

            case 0x8004:
                register.apply(x, operand -> {
                    int sum = operand + register.get(y);
                    register.set(0xF, sum > 0xFF ? 1 : 0);
                    return (sum & 0xFF);
                });
                pc += 2;
                break;

            case 0x8005:
                register.apply(x, vx -> {
                    register.set(0xF, register.get(y) > vx ? 0 : 1);
                    return (vx - register.get(y)) & 0xFF;
                });
                pc += 2;
                break;

            case 0x8006:
                register.set(0xF, (register.get(x) & 0x1));
                register.apply(x, vx -> vx >>> 1);
                pc += 2;
                break;

            case 0x8007:
                register.set(0xF, register.get(y) < register.get(x) ? 0 : 1);
                register.apply(x, vx -> (register.get(y) - vx) & 0xFF);
                pc += 2;
                break;

            case 0x800E:
                register.set(0xF, (register.get(x) & 0x80) > 0 ? 1 : 0);
                register.apply(x, vx -> (vx << 1) & 0xFF);
                pc += 2;
                break;

            case 0x9000:
                if(register.get(x) != register.get(y))
                    pc += 4;
                else
                    pc += 2;
                break;

        }

        switch (opcode & 0xF000) {
            case 0xA000:
                I = opcode & 0x0FFF;
                pc += 2;
                break;

            case 0xB000:
                pc = (opcode & 0x0FFF) + register.get(0);
                break;

            case 0xC000:
                register.set(x, random.nextInt(256) & (opcode & 0x00FF));
                break;
        }

        // Update timers
        if(delay_timer > 0)
            --delay_timer;

        if(sound_timer > 0)  {
            if(sound_timer == 1)
                System.out.println("BEEP!\n");
        }

    }

    public boolean getDrawFlag() {
        return drawFlag;
    }

    public void setKeys() {

    }

    public static void printOpcode(int opcode) {
        System.out.println("Executing opcode " + Integer.toHexString((opcode)));
    }

    public int getPC() {
        return pc;
    }

    public int getI() {
        return I;
    }
}
