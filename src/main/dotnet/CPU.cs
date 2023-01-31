namespace Chip8;

public class CPU 
{
    private readonly Memory memory;
    private readonly Register register;
    private readonly Random random;
    private readonly Keyboard keyboard;
    private readonly Screen screen;
    private Stack<int> stack;

    private bool drawFlag;

    private int I;
    private int pc;
    private int delayTimer, soundTimer;

    public CPU(Memory memory, Register register, Random random, Keyboard keyboard, Screen screen) {
        this.memory = memory;
        this.register = register;
        this.random = random;
        this.keyboard = keyboard;
        this.screen = screen;

        stack = new Stack<int>();

        pc = 0x200;
        I = 0;
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
    }
}
