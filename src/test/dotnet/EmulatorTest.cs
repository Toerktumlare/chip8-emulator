namespace Chip8.Tests;

public class EmulatorTest
{
    private Chip8.CPU cpu;
    private Chip8.Memory memory;
    private Chip8.Register register;
    private Random random;

    public EmulatorTest() {
        memory = new Memory();
        register = new Register();
//        random = mock(Random.class);
        cpu = new Chip8.CPU(memory, register, random, null, null);
    }

    [Fact(DisplayName = "code 1XXX jumps to address NNN")]
    void ShouldTestOpcode0x1XXX() {
        byte[] data = {0x12, 0x02, 0x60, 0x01};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal("1", register.Get(0).ToString("X"));
    }

    private void Emulate(byte[] data) {
        for (int i = 0; i < (data.Length / 2); i++) {
            cpu.EmulateCycle();
        }
    }
}
