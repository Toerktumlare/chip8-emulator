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

        int x = (opcode & 0x0F00) >> 8;
        int y = (opcode & 0x00F0) >> 4;

        switch(opcode) {
 
            case 0x00E0: // Clears the screen
                screen.Clear();
                drawFlag = true;
                pc += 2;
                return;
 
            case 0x00EE:
                pc = stack.Pop();
                pc += 2;
                return;
 
            default:
                break;
        }

        switch(opcode & 0xF000) {
 
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
                if(register.Get(x) != (opcode & 0x00FF)) {
                    pc += 4;
                } else {
                    pc += 2;
                }
                break;

            case 0x5000:
                if(register.Get(x) == register.Get(y)) {
                    pc += 4;
                } else {
                    pc += 2;
                }
                break;

            case 0x6000:
                register.Set(x, opcode & 0x00FF);
                pc += 2;
                break;

            case 0x7000:
                register.Apply(x, vx => vx + opcode & 0x00FF);
                pc += 2;
                break;

            case 0x8000:{
                switch(opcode & 0x000F) {
                    case (0x0000):
                        register.Set(x, register.Get(y));
                        pc += 2;
                        break;

                    case (0x0001):
                        register.Apply(x, vx => vx | register.Get(y));
                        pc += 2;
                        break;

                    default:
                        break;
                    }
                }
                break;

            default:
                break;
        }
    }

    public int DelayTimer { get { return delayTimer; } set { delayTimer = value; } }
    public bool DrawFlag { get { return drawFlag; } set { drawFlag = value; } }

    public int SoundTimer => soundTimer;

    public int PC { get { return pc; } }

    public int I { get { return i; } }
}
