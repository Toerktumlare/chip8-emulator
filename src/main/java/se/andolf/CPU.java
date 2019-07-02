package se.andolf;

import se.andolf.utils.Utils;

import java.util.Random;
import java.util.Stack;

public class CPU {

    private final Memory memory;
    private final Register register;
    private final Random random;
    private final Keyboard keyboard;
    private final Screen screen;
    private final Stack<Integer> stack;

    private boolean drawFlag;

    private int I;
    private int pc;
    private int delayTimer, soundTimer;

    public CPU(Memory memory, Register register, Random random, Keyboard keyboard, Screen screen) {
        this.memory = memory;
        this.register = register;
        this.random = random;
        this.keyboard = keyboard;
        this.screen = screen;

        stack = new Stack<>();

        pc = 0x200;
        I = 0;
    }

    public void emulateCycle() {

        final int opcode = memory.getOpcode(pc);
        System.out.println(Utils.getHex(opcode));


        int x = (opcode & 0x0F00) >>> 8;
        int y = (opcode & 0x00F0) >>> 4;

        switch(opcode) {
            case 0x00E0:
                screen.clear();
                drawFlag = true;
                pc += 2;
                return;

            case 0x00EE:
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
                if(register.get(x) == (opcode & 0x00FF)) {
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
                register.apply(x, xv -> {
                    int result = xv + (opcode & 0x00FF);
                    if(result >= 256) {
                        return (result - 256);
                    } else {
                        return result;
                    }
                });
                pc += 2;
                break;
        }


        switch (opcode & 0xF00F) {

            case 0x8000:
                register.set(x, y);
                pc += 2;
                break;

            case 0x8001:
                register.apply(x, vx -> vx | register.get(y));
                pc += 2;
                break;

            case 0x8002:
                register.apply(x, vx -> vx & register.get(y));
                pc += 2;
                break;

            case 0x8003:
                register.apply(x, vx -> vx ^ register.get(y));
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
                pc += 2;
                break;

            case 0xD000:

                final int height = opcode & 0x000F;

                register.set(0xF, 0);

                for (int yLine = 0; yLine < height; yLine++) {

                    int pixelValue = memory.getByte(I + yLine);

                    for (int xLine = 0; xLine < 8; xLine++) {

                        if (Utils.getBitValue(pixelValue, xLine) != 0) {

                            int xCoord = (register.get(x) + xLine);
                            int yCoord = (register.get(y) + yLine) ;

                            if(xCoord >= screen.getWidth() )
                                xCoord %= screen.getWidth();

                            if(yCoord >= screen.getHeight())
                                yCoord %= screen.getHeight();

                            if (screen.getPixel(xCoord, yCoord) == 1)
                                register.set(0xF, 1);

                            screen.setPixel(xCoord, yCoord);
                        }
                    }
                }
                drawFlag = true;
                pc += 2;
                break;
        }

        switch (opcode & 0xF0FF) {

            case 0xE09E:
                if(keyboard.isPressed(register.get(x))) {
                    pc += 4;
                } else {
                    pc += 2;
                }
                break;

            case 0xE0A1:
                if(!keyboard.isPressed(register.get(x))) {
                    pc += 4;
                } else {
                    pc += 2;
                }
                break;

            case 0xF00A:
                for (int i = 0; i < keyboard.getKeys().length; i++) {
                    if(keyboard.isPressed(i)){
                        register.set(x, i);
                        pc += 2;
                    }
                }
                break;

            case 0xF007:
                register.set(x, this.delayTimer);
                pc += 2;
                break;

            case 0xF015:
                this.delayTimer = register.get(x);
                pc += 2;
                break;

            case 0xF018:
                this.soundTimer = register.get(x);
                pc += 2;
                break;

            case 0xF01E:
                this.I += register.get(x);
                pc += 2;
                break;

            case 0xF029:
                this.I = register.get(x) * 5;
                pc += 2;
                break;

            case 0xF033:

                memory.setByte(I, register.get(x) / 100);
                memory.setByte(I + 1, (register.get(x) % 100) / 10);
                memory.setByte(I + 2, (register.get(x) % 100) % 10);

                pc += 2;
                return;

            case 0xF055:
                for (int j = 0; j <= x; j++) {
                    memory.setByte(I + j, register.get(j));
                }

                pc += 2;
                return;

            case 0xF065:
                for (int j = 0; j <= x; j++) {
                    register.set(j, memory.getByte(I + j) & 0xFF);
                }
                pc += 2;
                break;
        }
    }

    public int getDelayTimer() {
        return delayTimer;
    }

    public int getSoundTimer() {
        return soundTimer;
    }

    public void setDelayTimer(int value) {
        delayTimer = value;
    }

    public boolean getDrawFlag() {
        return drawFlag;
    }

    public void setDrawFlag(boolean value) {
        drawFlag = value;
    }

    public int getPC() {
        return pc;
    }

    public int getI() {
        return I;
    }
}
