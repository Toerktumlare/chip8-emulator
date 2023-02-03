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
        random = new Random();
        cpu = new Chip8.CPU(memory, register, random, null, null);
    }

    [Fact(DisplayName = "code 1XXX jumps to address NNN")]
    void ShouldTestOpcode0x1XXX() {
        byte[] data = {0x12, 0x02, 0x60, 0x01};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal("1", register.Get(0).ToString("X"));
    }

    [Fact(DisplayName = "code 2XXX jump to subroutine")]
    void ShouldTestOpcode0x2XXX() {

        byte[] data = {0x22, 0x02, 0x00, 0xEE};
        memory.LoadData(data);

        Emulate(data);

        Assert.Equal(514, cpu.PC);
    }

    [Fact(DisplayName = "code 3XNN Should skip the next instruction if VX == NN")]
    void ShouldSkipNextInstruction() {
        byte[] data = {0x60, 0x01, 0x30, 0x01};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(518, cpu.PC);
    }

    [Fact(DisplayName = "code 3XNN Should NOT skip the next instruction if VX != NN")]
    void ShouldNotSkipNextInstruction() {
        byte[] data = {0x60, 0x01, 0x30, 0x02};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(516, cpu.PC);
    }

    [Fact(DisplayName = "code 4XNN Should skip the next instruction if VX != NN")]
    void ShouldSkipNextInstructionIfNotEquals() {
        byte[] data = {0x60, 0x01, 0x40, 0x02};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(518, cpu.PC);
    }

    [Fact(DisplayName = "code 4XNN Should NOT skip the next instruction if VX == NN")]
    void ShouldNotSkipNextInstructionIfEquals() {
        byte[] data = {0x60, 0x01, 0x40, 0x01};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(516, cpu.PC);
    }

    [Fact(DisplayName = "code 5XY0 Skips the next instruction if VX == VY")]
    void ShouldSkipNextInstructionIfVxEqualsVy() {
        byte[] data = {0x60, 0x01, 0x61, 0x01, 0x50, 0x10};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(520, cpu.PC);
    }

    [Fact(DisplayName = "code 5XY0 Does not skip the next instruction if VX != VY")]
    void ShouldNotSkipNextInstructionIfVxNotEqualsVy() {
        byte[] data = {0x60, 0x01, 0x61, 0x02, 0x50, 0x10};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(518, cpu.PC);
    }

    [Fact(DisplayName = "code 6XXX set register to value")]
    void ShouldTestOpcode0x6000() {
        byte[] data = {0x60, 0x01};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal("1", register.Get(0).ToString("X"));
    }

    [Fact(DisplayName = "code 7XNN Adds NN to VX")]
    void ShouldAddValueToValueInRegistry() {
        byte[] data = {0x60, 0x01, 0x70, 0x01};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(2, register.Get(0));
    }

    [Fact(DisplayName = "code 7XNN Adds NN to VX and resolves overflow (Carry flag is not changed)")]
    void ShouldAddValueToValueInRegistryAndResolveOverflow() {
        byte[] data = {0x60, 0xFF, 0x70, 0xFF};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(254, register.Get(0));
    }

    [Fact(DisplayName = "code 8XY0 Sets VX to the value of VY.")]
    void ShouldTestOpcode0x8XY0() {
        byte[] data = {0x60, 0x01, 0x62, 0x02, 0x80, 0x20};

        memory.LoadData(data);

        cpu.EmulateCycle();
        cpu.EmulateCycle();
        Assert.Equal("1", register.Get(0).ToString("X"));

        cpu.EmulateCycle();
        Assert.Equal("2", register.Get(0).ToString("X"));
    }

    [Fact(DisplayName = "code 8XY1 Sets VX to VX or VY. (Bitwise OR operation)")]
    void ShouldTestOpcode0x8XY1() {
        byte[] data = {0x60, 0x01, 0x61, 0x06, 0x80, 0x11};

        memory.LoadData(data);

        Emulate(data);

        Assert.Equal(7, register.Get(0));
    }

    [Fact(DisplayName = "code 8XY2 Sets VX to VX and VY. (Bitwise AND operation)")]
    void ShouldTestOpcode0x8XY2() {
        byte[] data = {0x60, 0x0C, 0x61, 0x06, 0x80, 0x12};

        memory.LoadData(data);

        Emulate(data);

        Assert.Equal(4, register.Get(0));
    }

    [Fact(DisplayName = "code 8XY3 Sets VX to VX xor VY.")]
    void ShouldTestOpcode0x8XY3() {
        byte[] data = {0x60, 0x09, 0x61, 0x05, 0x80, 0x13};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(12, register.Get(0));
    }

    [Fact(DisplayName = "code 8XY4 Adds VY to VX. VF is set to 0 when there's no carry.")]
    void ShouldTestOpcode0x8XY4NoCarrySetRegisterFtoZero() {
        byte[] data = {0x60, 0x01, 0x61, 0x01, 0x80, 0x14};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(2, register.Get(0));
        Assert.Equal(0, register.Get(0xF));
    }

    [Fact(DisplayName = "code 8XY4 Adds VY to VX. VF is set to 1 when there's a carry")]
    void ShouldTestOpcode0x8XY4WithCarrySetRegisterFtoOne() {
        byte[] data = {0x60, 0xF1, 0x61, 0xF1, 0x80, 0x14};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(226, register.Get(0));
        Assert.Equal(1, register.Get(0xF));
    }

    [Fact(DisplayName = "code 8XY5 VY is subtracted from VX. VF is set to 1 when there's no borrow")]
    void ShouldTestOpcode0x8XY5IfSumIsNonNegativeValue() {
        byte[] data = {0x60, 0x03, 0x61, 0x02, 0x80, 0x15};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(1, register.Get(0));
        Assert.Equal(1, register.Get(0xF));
    }

    [Fact(DisplayName = "code 8XY5 VY is subtracted from VX. VF is set to 1 when there's no borrow")]
    void ShouldTestOpcode0x8XY5IfSumIsNegativeValue() {
        byte[] data = {0x60, 0x02, 0x61, 0x03, 0x80, 0x15};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(255, register.Get(0));
        Assert.Equal(0, register.Get(0xF));
    }

    [Fact(DisplayName = "code 8XY6 Stores the least significant bit of VX in VF and then shifts VX to the right by 1.")]
    void ShouldTestOpcode0x8XY6IfSumHasLeastSignificantBitOfOne() {

        byte[] data = {0x60, 0x03, 0x80, 0x16};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(1, register.Get(0));
        Assert.Equal(1, register.Get(0xF));

    }

    [Fact(DisplayName = "code 8XY6 Stores the least significant bit of VX in VF and then shifts VX to the right by 1.")]
    void ShouldTestOpcode0x8XY6IfSumHasLeastSignificantBitOfZero() {

        byte[] data = {0x60, 0x02, 0x80, 0x16};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(1, register.Get(0));
        Assert.Equal(0, register.Get(0xF));

    }

    [Fact(DisplayName = "code 8XY7 Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.")]
    void ShouldTestOpcode0x8XY7IfThereIsNoBorrowSetVFToOne() {

        byte[] data = {0x60, 0x02, 0x61, 0x03, 0x80, 0x17};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(1, register.Get(0));
        Assert.Equal(1, register.Get(0xF));
    }

    [Fact(DisplayName = "code 8XY7 Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.")]
    void ShouldTestOpcode0x8XY7IfThereIsBorrowSetVFToZero() {

        byte[] data = {0x60, 0x03, 0x61, 0x02, 0x80, 0x17};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(255, register.Get(0));
        Assert.Equal(0, register.Get(0xF));
    }

    [Fact(DisplayName = "code 8XYE Stores the most significant bit of VX in VF and then shifts VX to the left by 1")]
    void ShouldTestOpcode0x8XYEMSBShouldBeOne() {

        byte[] data = {0x60, 0xFF, 0x80, 0x0E};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(1, register.Get(0xF));
        Assert.Equal(254, register.Get(0));
    }

    [Fact(DisplayName = "code 8XYE Stores the most significant bit of VX in VF and then shifts VX to the left by 1")]
    void ShouldTestOpcode0x8XYEMSBShouldBeZero() {

        byte[] data = {0x60, 0x01, 0x80, 0x0E};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(0, register.Get(0xF));
        Assert.Equal(2, register.Get(0));
    }

    [Fact(DisplayName = "code 9XY0 Skips the next instruction when VX doesn't equal VY")]
    void ShouldTestOpcode0x9XY0SkipsNextInstruction() {

        byte[] data = {0x60, 0x01, 0x61, 0x02, 0x90, 0x10};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(520, cpu.PC);
    }

    [Fact(DisplayName = "code 9XY0 Does not skip the next instruction when VX equal VY")]
    void ShouldTestOpcode0x9XY0ShouldNotSkipsNextInstruction() {

        byte[] data = {0x60, 0x01, 0x61, 0x01, 0x90, 0x10};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(518, cpu.PC);

    }

    [Fact(DisplayName = "code ANNN Sets I to the address NNN")]
    void ShouldTestOpcode0xANNNShouldSetItoNNN() {

        byte[] data = {0xAF, 0xFF};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(4095, cpu.I);

    }

    [Fact(DisplayName = "code BNNN Jumps to the address NNN plus V0.")]
    void ShouldTestOpcode0xBNNNShouldJumpToNNNPlusVZero() {

        byte[] data = {0x60, 0x01, 0xB2, 0x05};

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(518, cpu.PC);

    }

    // [Fact(DisplayName = "code CXNN Sets VX to the result of a bitwise and operation on a random number (Typically: 0 to 255) and NN.")]
    // void ShouldTestOpcode0xCXNNShouldSetVxToRandomNumberANDNN() {

    //     byte[] data = { 0xC0, 0x07 };

    //     when(random.nextInt(anyInt())).thenReturn(85);

    //     memory.LoadData(data);
    //     Emulate(data);

    //     Assert.Equal(5, register.Get(0));

    // }

    [Fact(DisplayName = "code FX15 Sets the delay timer to VX")]
    void ShouldTestOpcodeFX15SetDelayTimerToVX() {

        byte[] data = { 0x60, 0x01, 0xF0, 0x15 };

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(1, cpu.DelayTimer);

    }

    [Fact(DisplayName = "code FX18 Sets the sound timer to VX")]
    void ShouldTestOpcodeFX18SetSoundTimerToVX() {

        byte[] data = { 0x60, 0x01, 0xF0, 0x18 };

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(1, cpu.SoundTimer);

    }

    [Fact(DisplayName = "code FX1E adds Vx to I")]
    void ShouldTestOpcodeFX1EAddVxToI() {

        byte[] data = { 0x60, 0x01, 0xF0, 0x1E };

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(1, cpu.I);

    }

    [Fact(DisplayName = "code FX07 set Vx to the delayTimer")]
    void ShouldTestOpcodeFX07SetVxToTheDelayTimer() {

        cpu.DelayTimer = 1;
        byte[] data = { 0xF0, 0x07 };

        memory.LoadData(data);
        Emulate(data);

        Assert.Equal(1, register.Get(0));

    }

    private void Emulate(byte[] data) {
        for (int i = 0; i < (data.Length / 2); i++) {
            cpu.EmulateCycle();
        }
    }
}
