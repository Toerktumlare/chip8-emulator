namespace Chip8;

public interface IScreen {

    int Width { get; }
    int Height { get; }
    void Clear();
    void SetPixel(int xCoord, int yCoord);
    uint GetPixel(int xCoord, int yCoord);
}

public class CPU 
{
    private readonly Memory memory;
    private readonly Register register;
    private readonly Random random;
    private readonly Keyboard keyboard;
    private readonly IScreen screen;
    private Stack<int> stack;

    private bool drawFlag;

    private int i;
    private int pc;
    private int delayTimer, soundTimer;

    public CPU(Memory memory, Register register, Random random, Keyboard keyboard, IScreen screen) {
        this.memory = memory;
        this.register = register;
        this.random = random;
        this.keyboard = keyboard;
        this.screen = screen;

        stack = new Stack<int>();

        pc = 0x200;
        i = 0;
    }


    public void EmulateCycle() 
    {
        int opcode = memory.GetOpcode(pc);
        Console.WriteLine(Utils.GetHex(opcode));


        int x = (opcode & 0x0F00) >>> 8;
        int y = (opcode & 0x00F0) >>> 4;

        switch(opcode) {
            case 0x00E0:
                screen.Clear();
                drawFlag = true;
                pc += 2;
                return;

            case 0x00EE:
                pc = stack.Pop();
                drawFlag = true;
                pc += 2;
                break;
        }

        switch (opcode & 0xF000) {

            case 0x1000:
                pc = opcode & 0x0FFF;
                break;

            case 0x2000:
                stack.Push(pc);
                pc = opcode & 0x0FFF;
                break;

            case 0x3000:
                if(register.Get(x) == (opcode & 0x00FF)) {
                    pc += 4;
                } else {
                    pc += 2;
                }
                break;

            case 0x4000:
                if(register.Get((opcode & 0x0F00) >>> 8) != (opcode & 0x00FF)) {
                    pc += 4;
                } else {
                    pc += 2;
                }
                break;

            case 0x5000:
                if(register.Get((opcode & 0x0F00) >>> 8) == register.Get((opcode & 0x00F0) >>> 4)) {
                    pc += 4;
                } else {
                    pc += 2;
                }
                break;

            case 0x6000:
                register.Set((opcode & 0x0F00) >>> 8, (opcode & 0x00FF));
                pc += 2;
                break;

            case 0x7000:
                register.Apply(x, xv => {
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
                register.Set(x, y);
                pc += 2;
                break;

            case 0x8001:
                register.Apply(x, vx => vx | register.Get(y));
                pc += 2;
                break;

            case 0x8002:
                register.Apply(x, vx => vx & register.Get(y));
                pc += 2;
                break;

            case 0x8003:
                register.Apply(x, vx => vx ^ register.Get(y));
                pc += 2;
                break;

            case 0x8004:
                register.Apply(x, operand => {
                    int sum = operand + register.Get(y);
                    register.Set(0xF, sum > 0xFF ? 1 : 0);
                    return (sum & 0xFF);
                });
                pc += 2;
                break;

            case 0x8005:
                register.Apply(x, vx => {
                    register.Set(0xF, register.Get(y) > vx ? 0 : 1);
                    return (vx - register.Get(y)) & 0xFF;
                });
                pc += 2;
                break;

            case 0x8006:
                register.Set(0xF, (register.Get(x) & 0x1));
                register.Apply(x, vx => vx >>> 1);
                pc += 2;
                break;

            case 0x8007:
                register.Set(0xF, register.Get(y) < register.Get(x) ? 0 : 1);
                register.Apply(x, vx => (register.Get(y) - vx) & 0xFF);
                pc += 2;
                break;

            case 0x800E:
                register.Set(0xF, (register.Get(x) & 0x80) > 0 ? 1 : 0);
                register.Apply(x, vx => (vx << 1) & 0xFF);
                pc += 2;
                break;

            case 0x9000:
                if(register.Get(x) != register.Get(y))
                    pc += 4;
                else
                    pc += 2;
                break;

        }

        switch (opcode & 0xF000) {
            case 0xA000:
                i = opcode & 0x0FFF;
                pc += 2;
                break;

            case 0xB000:
                pc = (opcode & 0x0FFF) + register.Get(0);
                break;

            case 0xC000:
                register.Set(x, random.Next(256) & (opcode & 0x00FF));
                pc += 2;
                break;

            case 0xD000:

                int height = opcode & 0x000F;

                register.Set(0xF, 0);

                for (int yLine = 0; yLine < height; yLine++) {

                    int pixelValue = memory.GetByte(I + yLine);

                    for (int xLine = 0; xLine < 8; xLine++) {

                        if (Utils.GetBitValue(pixelValue, xLine) != 0) {

                            int xCoord = (register.Get(x) + xLine);
                            int yCoord = (register.Get(y) + yLine) ;

                            if(xCoord >= screen.Width )
                                xCoord %= screen.Width;

                            if(yCoord >= screen.Height)
                                yCoord %= screen.Height;

                            if (screen.GetPixel(xCoord, yCoord) == 1)
                                register.Set(0xF, 1);

                            screen.SetPixel(xCoord, yCoord);
                        }
                    }
                }
                drawFlag = true;
                pc += 2;
                break;
        }

        switch (opcode & 0xF0FF) {

            case 0xE09E:
                if(keyboard.IsPressed(register.Get(x))) {
                    pc += 4;
                } else {
                    pc += 2;
                }
                break;

            case 0xE0A1:
                if(!keyboard.IsPressed(register.Get(x))) {
                    pc += 4;
                } else {
                    pc += 2;
                }
                break;

            case 0xF00A:
                for (int i = 0; i < keyboard.GetKeys().Length; i++) {
                    if(keyboard.IsPressed(i)){
                        register.Set(x, i);
                        pc += 2;
                    }
                }
                break;

            case 0xF007:
                register.Set(x, this.delayTimer);
                pc += 2;
                break;

            case 0xF015:
                this.delayTimer = register.Get(x);
                pc += 2;
                break;

            case 0xF018:
                this.soundTimer = register.Get(x);
                pc += 2;
                break;

            case 0xF01E:
                this.i += register.Get(x);
                pc += 2;
                break;

            case 0xF029:
                this.i = register.Get(x) * 5;
                pc += 2;
                break;

            case 0xF033:

                memory.SetByte(I, register.Get(x) / 100);
                memory.SetByte(I + 1, (register.Get(x) % 100) / 10);
                memory.SetByte(I + 2, (register.Get(x) % 100) % 10);

                pc += 2;
                return;

            case 0xF055:
                for (int j = 0; j <= x; j++) {
                    memory.SetByte(I + j, register.Get(j));
                }

                pc += 2;
                return;

            case 0xF065:
                for (int j = 0; j <= x; j++) {
                    register.Set(j, memory.GetByte(I + j) & 0xFF);
                }
                pc += 2;
                break;
        }
    }

    public int DelayTimer { get { return delayTimer;} set { delayTimer = value; } }
    public bool DrawFlag => drawFlag;

    public int SoundTimer => soundTimer;

    public int PC { get { return pc; } }

    public int I { get { return i; } }
}
