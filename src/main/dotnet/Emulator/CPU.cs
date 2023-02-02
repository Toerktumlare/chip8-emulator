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
        // Console.WriteLine(Utils.GetHex(opcode));

        // TODO: Implement chip 8 instruction set
    }

    public int DelayTimer { get { return delayTimer; } set { delayTimer = value; } }
    public bool DrawFlag { get { return drawFlag; } set { drawFlag = value; } }

    public int SoundTimer => soundTimer;

    public int PC { get { return pc; } }

    public int I { get { return i; } }
}
