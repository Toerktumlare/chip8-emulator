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
    public int InstructionsCounter;

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
        InstructionsCounter++;
        int opcode = memory.GetOpcode(pc);
        //Console.WriteLine(Utils.GetHex(opcode));

        int x = (opcode & 0x0F00) >> 8;
        int y = (opcode & 0x00F0) >> 4;

        pc += 2;

        switch(opcode) {
 
            case 0x00E0: // Clears the screen
                screen.Clear();
                drawFlag = true;
                return;
 
            case 0x00EE: // Returns from a subroutine.
                pc = stack.Pop();
                return;
 
            default:
                break;
        }

        switch(opcode & 0xF000) {
 
            case 0x1000: // Jumps to address NNN.
                pc = opcode & 0x0FFF;
                break;
 
            case 0x2000: // Calls subroutine at NNN.
                stack.Push(pc);
                pc = opcode & 0x0FFF;
                break; 

            case 0x3000: // Skips the next instruction if VX equals NN. (Usually the next instruction is a jump to skip a code block)
                if(register.Get(x) == (opcode & 0x00FF)) {
                    pc += 2;
                }
                break;

            case 0x4000: // Skips the next instruction if VX doesn't equal NN. (Usually the next instruction is a jump to skip a code block)
                if(register.Get(x) != (opcode & 0x00FF)) {
                    pc += 2;
                }
                break;

            case 0x5000: // Skips the next instruction if VX equals VY. (Usually the next instruction is a jump to skip a code block)
                if(register.Get(x) == register.Get(y)) {
                    pc += 2;
                }
                break;

            case 0x6000: // Sets VX to NN.
                register.Set(x, opcode & 0x00FF);
                break;

            case 0x7000: // Adds NN to VX. (Carry flag is not changed)
                register.Apply(x, vx => vx + opcode & 0x00FF);
                break;

            case 0x8000:{
                switch(opcode & 0x000F) {
                    case 0x0000: // Sets VX to the value of VY.
                        register.Set(x, register.Get(y));
                        break;

                    case 0x0001: //	Sets VX to VX or VY. (Bitwise OR operation)
                        register.Apply(x, vx => vx | register.Get(y));
                        break;

                    case 0x0002: // Sets VX to VX and VY. (Bitwise AND operation)
                        register.Apply(x, vx => vx & register.Get(y));
                        break;

                    case 0x0003: // Sets VX to VX xor VY.
                        register.Apply(x, vx => vx ^ register.Get(y));
                        break;

                    case 0x0004: // Adds VY to VX. VF is set to 1 when there's a carry, and to 0 when there isn't.
                        register.Apply(x, vx => {
                            var sum = vx + register.Get(y);
                            register.Set(0xF, sum > 0xFF ? 1 : 0);
                            return sum & 0xFF;
                        });
                        break;

                    case 0x0005: // VY is subtracted from VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
                        register.Apply(x, vx => {
                            register.Set(0xF, register.Get(y) > vx ? 0 : 1);
                            return (vx - register.Get(y)) & 0xFF;
                        });
                        break;

                    case 0x0006: // Stores the least significant bit of VX in VF and then shifts VX to the right by 1.[2]
                        register.Apply(x, vx => {
                            register.Set(0xF, vx & 0x1);
                            return vx >> 1;
                        });
                        break;

                    case 0x0007: // Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
                        register.Apply(x, vx => {
                            register.Set(0xF, vx > register.Get(y) ? 0 : 1);
                            return (register.Get(y) - vx) & 0xFF;
                        });
                        break;

                    case 0x000E: // Stores the most significant bit of VX in VF and then shifts VX to the left by 1.[3]
                        register.Apply(x, vx => {
                            register.Set(0xF, (vx & 0x80) > 0 ? 1 : 0);
                            return (vx << 1) & 0xFF;
                        });
                        break;
                    default:
                        break;
                    }
                }
                break;

            case 0x9000: // Skips the next instruction if VX doesn't equal VY. (Usually the next instruction is a jump to skip a code block)
                pc += register.Get(x) != register.Get(y) ? 4 : 2;
                break;

            case 0xA000: // Sets I to the address NNN.
                i = opcode & 0x0FFF;
                break;

            case 0xB000: // Jumps to the address NNN plus V0..
                pc = (register.Get(0) + opcode & 0x0FFF) & 0x0FFF;
                break;

            case 0xC000: // Sets VX to the result of a bitwise and operation on a random number (Typically: 0 to 255) and NN.
                register.Set(x, (random.Next() % 256) & opcode & 0xFF);
                break;
            
            case 0xD000: // Draws a sprite at coordinate (VX, VY) that has a width of 8 pixels and a height of N pixels. Each row of 8 pixels is read as bit-coded starting from memory location I; I value doesn’t change after the execution of this instruction. As described above, VF is set to 1 if any screen pixels are flipped from set to unset when the sprite is drawn, and to 0 if that doesn’t happen
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
                break;
            
            case 0xE000: {
                switch (opcode & 0x00FF) {
                    
                    case 0x009E: // Skips the next instruction if the key stored in VX is pressed. (Usually the next instruction is a jump to skip a code block)
                        pc += keyboard.IsPressed(register.Get(x)) ? 4 : 2;
                        break;

                    case 0x00A1: // Skips the next instruction if the key stored in VX isn't pressed. (Usually the next instruction is a jump to skip a code block)
                        pc += keyboard.IsPressed(register.Get(x)) ? 2 : 4;
                        break;

                    default:
                        break;
                }
                break;
            }

            case 0xF000: {
                switch (opcode & 0x00FF) {

                    case 0x0007: // Sets VX to the value of the delay timer.
                        register.Set(x, delayTimer & 0xFF);
                        break;

                    case 0x000A: // A key press is awaited, and then stored in VX. (Blocking Operation. All instruction halted until next key event)
                        for (int i = 0; i < keyboard.GetKeys().Length; i++) {
                            if(keyboard.IsPressed(i)){
                                register.Set(x, i);
                            }
                        }
                        break;

                    case 0x0015: // Sets the delay timer to VX.
                        delayTimer = register.Get(x) & 0xFF;
                        break;

                    case 0x0018: // Sets the sound timer to VX.
                        soundTimer = register.Get(x) & 0xFF;
                        break;

                    case 0x001E: // Adds VX to I.
                        i += (register.Get(x) & 0xFF);
                        break;

                    case 0x0029: // Sets I to the location of the sprite for the character in VX. Characters 0-F (in hexadecimal) are represented by a 4x5 font.
                        i = 0x50 + register.Get(x) * 5;
                        break;

                    case 0x0033: // VX, with the most significant of three digits at the address in I, the middle digit at I plus 1, and the least significant digit at I plus 2. (In other words, take the decimal representation of VX, place the hundreds digit in memory at location in I, the tens digit at location I+1, and the ones digit at location I+2.)
                        memory.SetByte(i, register.Get(x) / 100);
                        memory.SetByte(i + 1, (register.Get(x) % 100) / 10);
                        memory.SetByte(i + 2, register.Get(x) % 10);
                        break;

                    case 0x0055: // Stores V0 to VX (including VX) in memory starting at address I. The offset from I is increased by 1 for each value written, but I itself is left unmodified.
                        for(int p = 0; p <= x; p++)
                            memory.SetByte(i + p, register.Get(p));
                        break;

                    case 0x0065: // Fills V0 to VX (including VX) with values from memory starting at address I. The offset from I is increased by 1 for each value written, but I itself is left unmodified.
                        for(int p = 0; p <= x; p++)
                            register.Set(p, memory.GetByte(i + p) & 0xFF);
                        break;

                    default:
                        break;
                    }
                }
                break;

            default:
                break;
        }

        if(this.delayTimer > 0) {
            this.delayTimer -= 1;
        }

        if(this.soundTimer > 0) {
            this.soundTimer -= 1;
        }
    }

    public int DelayTimer { get { return delayTimer; } set { delayTimer = value; } }
    public bool DrawFlag { get { return drawFlag; } set { drawFlag = value; } }

    public int SoundTimer => soundTimer;

    public int PC { get { return pc; } }

    public int I { get { return i; } }
}
