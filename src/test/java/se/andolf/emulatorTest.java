package se.andolf;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import java.util.Random;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.mockito.ArgumentMatchers.anyInt;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;

class emulatorTest {

    private CPU cpu;
    private Memory memory;
    private Register register;
    private Random random;

    @BeforeEach
    void beforeEach() {
        memory = new Memory();
        register = new Register();
        random = mock(Random.class);
        cpu = new CPU(memory, register, random, null, null);
    }

    @DisplayName("code 1XXX jumps to address NNN")
    @Test
    void shouldTestOpcode0x1XXX() {
        byte[] data = {0x12, 0x02, 0x60, 0x01};

        memory.loadData(data);
        emulate(data);

        assertEquals("1", Integer.toHexString(register.get(0)));
    }

    @DisplayName("code 2XXX jump to subroutine")
    @Test
    void shouldTestOpcode0x2XXX() {

        byte[] data = {0x22, 0x02, 0x00, -0x12};
        memory.loadData(data);

        emulate(data);

        assertEquals(514, cpu.getPC());
    }

    @DisplayName("code 3XNN Should skip the next instruction if VX == NN")
    @Test
    void shouldSkipNextInstruction() {
        byte[] data = {0x60, 0x01, 0x30, 0x01};

        memory.loadData(data);
        emulate(data);

        assertEquals(518, cpu.getPC());
    }

    @DisplayName("code 3XNN Should NOT skip the next instruction if VX != NN")
    @Test
    void shouldNotSkipNextInstruction() {
        byte[] data = {0x60, 0x01, 0x30, 0x02};

        memory.loadData(data);
        emulate(data);

        assertEquals(516, cpu.getPC());
    }

    @DisplayName("code 4XNN Should skip the next instruction if VX != NN")
    @Test
    void shouldSkipNextInstructionIfNotEquals() {
        byte[] data = {0x60, 0x01, 0x40, 0x02};

        memory.loadData(data);
        emulate(data);

        assertEquals(518, cpu.getPC());
    }

    @DisplayName("code 4XNN Should NOT skip the next instruction if VX == NN")
    @Test
    void shouldNotSkipNextInstructionIfEquals() {
        byte[] data = {0x60, 0x01, 0x40, 0x01};

        memory.loadData(data);
        emulate(data);

        assertEquals(516, cpu.getPC());
    }

    @DisplayName("code 5XY0 Skips the next instruction if VX == VY")
    @Test
    void shouldSkipNextInstructionIfVxEqualsVy() {
        byte[] data = {0x60, 0x01, 0x61, 0x01, 0x50, 0x10};

        memory.loadData(data);
        emulate(data);

        assertEquals(520, cpu.getPC());
    }

    @DisplayName("code 5XY0 Does not skip the next instruction if VX != VY")
    @Test
    void shouldNotSkipNextInstructionIfVxNotEqualsVy() {
        byte[] data = {0x60, 0x01, 0x61, 0x02, 0x50, 0x10};

        memory.loadData(data);
        emulate(data);

        assertEquals(518, cpu.getPC());
    }

    @DisplayName("code 6XXX set register to value")
    @Test
    void shouldTestOpcode0x6000() {
        byte[] data = {0x60, 0x01};

        memory.loadData(data);
        emulate(data);

        assertEquals("1", Integer.toHexString(register.get(0)));
    }

    @DisplayName("code 7XNN Adds NN to VX")
    @Test
    void shouldAddValueToValueInRegistry() {
        byte[] data = {0x60, 0x01, 0x70, 0x01};

        memory.loadData(data);
        emulate(data);

        assertEquals(2, register.get(0));
    }

    @DisplayName("code 7XNN Adds NN to VX and resolves overflow (Carry flag is not changed)")
    @Test
    void shouldAddValueToValueInRegistryAndResolveOverflow() {
        byte[] data = {0x60, -0x01, 0x70, -0x01};

        memory.loadData(data);
        emulate(data);

        assertEquals(254, register.get(0));
    }

    @DisplayName("code 8XY0 Sets VX to the value of VY.")
    @Test
    void shouldTestOpcode0x8XY0() {
        byte[] data = {0x60, 0x01, -0x80, 0x20};

        memory.loadData(data);

        cpu.emulateCycle();
        assertEquals("1", Integer.toHexString(register.get(0)));

        cpu.emulateCycle();
        assertEquals("2", Integer.toHexString(register.get(0)));
    }

    @DisplayName("code 8XY1 Sets VX to VX or VY. (Bitwise OR operation)")
    @Test
    void shouldTestOpcode0x8XY1() {
        byte[] data = {0x60, 0x01, 0x61, 0x06, -0x80, 0x11};

        memory.loadData(data);

        emulate(data);

        assertEquals(7, register.get(0));
    }

    @DisplayName("code 8XY2 Sets VX to VX and VY. (Bitwise AND operation)")
    @Test
    void shouldTestOpcode0x8XY2() {
        byte[] data = {0x60, 0x0C, 0x61, 0x06, -0x80, 0x12};

        memory.loadData(data);

        emulate(data);

        assertEquals(4, register.get(0));
    }

    @DisplayName("code 8XY3 Sets VX to VX xor VY.")
    @Test
    void shouldTestOpcode0x8XY3() {
        byte[] data = {0x60, 0x09, 0x61, 0x05, -0x80, 0x13};

        memory.loadData(data);
        emulate(data);

        assertEquals(12, register.get(0));
    }

    @DisplayName("code 8XY4 Adds VY to VX. VF is set to 0 when there's no carry.")
    @Test
    void shouldTestOpcode0x8XY4NoCarrySetRegisterFtoZero() {
        byte[] data = {0x60, 0x01, 0x61, 0x01, -0x80, 0x14};

        memory.loadData(data);
        emulate(data);

        assertEquals(2, register.get(0));
        assertEquals(0, register.get(0xF));
    }

    @DisplayName("code 8XY4 Adds VY to VX. VF is set to 1 when there's a carry")
    @Test
    void shouldTestOpcode0x8XY4WithCarrySetRegisterFtoOne() {
        byte[] data = {0x60, -0x0F, 0x61, -0x0F, -0x80, 0x14};

        memory.loadData(data);
        emulate(data);

        assertEquals(226, register.get(0));
        assertEquals(1, register.get(0xF));
    }

    @DisplayName("code 8XY5 VY is subtracted from VX. VF is set to 1 when there's no borrow")
    @Test
    void shouldTestOpcode0x8XY5IfSumIsNonNegativeValue() {
        byte[] data = {0x60, 0x03, 0x61, 0x02, -0x80, 0x15};

        memory.loadData(data);
        emulate(data);

        assertEquals(1, register.get(0));
        assertEquals(1, register.get(0xF));
    }

    @DisplayName("code 8XY5 VY is subtracted from VX. VF is set to 1 when there's no borrow")
    @Test
    void shouldTestOpcode0x8XY5IfSumIsNegativeValue() {
        byte[] data = {0x60, 0x02, 0x61, 0x03, -0x80, 0x15};

        memory.loadData(data);
        emulate(data);

        assertEquals(255, register.get(0));
        assertEquals(0, register.get(0xF));
    }

    @DisplayName("code 8XY6 Stores the least significant bit of VX in VF and then shifts VX to the right by 1.")
    @Test
    void shouldTestOpcode0x8XY6IfSumHasLeastSignificantBitOfOne() {

        byte[] data = {0x60, 0x03, -0x80, 0x16};

        memory.loadData(data);
        emulate(data);

        assertEquals(1, register.get(0));
        assertEquals(1, register.get(0xF));

    }

    @DisplayName("code 8XY6 Stores the least significant bit of VX in VF and then shifts VX to the right by 1.")
    @Test
    void shouldTestOpcode0x8XY6IfSumHasLeastSignificantBitOfZero() {

        byte[] data = {0x60, 0x02, -0x80, 0x16};

        memory.loadData(data);
        emulate(data);

        assertEquals(1, register.get(0));
        assertEquals(0, register.get(0xF));

    }

    @DisplayName("code 8XY7 Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.")
    @Test
    void shouldTestOpcode0x8XY7IfThereIsNoBorrowSetVFToOne() {

        byte[] data = {0x60, 0x02, 0x61, 0x03, -0x80, 0x17};

        memory.loadData(data);
        emulate(data);

        assertEquals(1, register.get(0));
        assertEquals(1, register.get(0xF));
    }

    @DisplayName("code 8XY7 Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.")
    @Test
    void shouldTestOpcode0x8XY7IfThereIsBorrowSetVFToZero() {

        byte[] data = {0x60, 0x03, 0x61, 0x02, -0x80, 0x17};

        memory.loadData(data);
        emulate(data);

        assertEquals(255, register.get(0));
        assertEquals(0, register.get(0xF));
    }

    @DisplayName("code 8XYE Stores the most significant bit of VX in VF and then shifts VX to the left by 1")
    @Test
    void shouldTestOpcode0x8XYEMSBShouldBeOne() {

        byte[] data = {0x60, -0x01, -0x80, 0x0E};

        memory.loadData(data);
        emulate(data);

        assertEquals(1, register.get(0xF));
        assertEquals(254, register.get(0));
    }

    @DisplayName("code 8XYE Stores the most significant bit of VX in VF and then shifts VX to the left by 1")
    @Test
    void shouldTestOpcode0x8XYEMSBShouldBeZero() {

        byte[] data = {0x60, 0x01, -0x80, 0x0E};

        memory.loadData(data);
        emulate(data);

        assertEquals(0, register.get(0xF));
        assertEquals(2, register.get(0));
    }

    @DisplayName("code 9XY0 Skips the next instruction when VX doesn't equal VY")
    @Test
    void shouldTestOpcode0x9XY0SkipsNextInstruction() {

        byte[] data = {0x60, 0x01, 0x61, 0x02, -0x70, 0x10};

        memory.loadData(data);
        emulate(data);

        assertEquals(520, cpu.getPC());
    }

    @DisplayName("code 9XY0 Does not skip the next instruction when VX equal VY")
    @Test
    void shouldTestOpcode0x9XY0ShouldNotSkipsNextInstruction() {

        byte[] data = {0x60, 0x01, 0x61, 0x01, -0x70, 0x10};

        memory.loadData(data);
        emulate(data);

        assertEquals(518, cpu.getPC());

    }

    @DisplayName("code ANNN Sets I to the address NNN")
    @Test
    void shouldTestOpcode0xANNNShouldSetItoNNN() {

        byte[] data = {-0x51, -0x01};

        memory.loadData(data);
        emulate(data);

        assertEquals(4095, cpu.getI());

    }

    @DisplayName("code BNNN Jumps to the address NNN plus V0.")
    @Test
    void shouldTestOpcode0xBNNNShouldJumpToNNNPlusVZero() {

        byte[] data = {0x60, 0x01, -0x4E, 0x05};

        memory.loadData(data);
        emulate(data);

        assertEquals(518, cpu.getPC());

    }

    @DisplayName("code CXNN Sets VX to the result of a bitwise and operation on a random number (Typically: 0 to 255) and NN.")
    @Test
    void shouldTestOpcode0xCXNNShouldSetVxToRandomNumberANDNN() {

        byte[] data = { -0x40, 0x07 };

        when(random.nextInt(anyInt())).thenReturn(85);

        memory.loadData(data);
        emulate(data);

        assertEquals(5, register.get(0));

    }

    @DisplayName("code FX15 Sets the delay timer to VX")
    @Test
    void shouldTestOpcodeFX15SetDelayTimerToVX() {

        byte[] data = { 0x60, 0x01, -0x10, 0x15 };

        memory.loadData(data);
        emulate(data);

        assertEquals(1, cpu.getDelayTimer());

    }

    @DisplayName("code FX18 Sets the sound timer to VX")
    @Test
    void shouldTestOpcodeFX18SetSoundTimerToVX() {

        byte[] data = { 0x60, 0x01, -0x10, 0x18 };

        memory.loadData(data);
        emulate(data);

        assertEquals(1, cpu.getSoundTimer());

    }

    @DisplayName("code FX1E adds Vx to I")
    @Test
    void shouldTestOpcodeFX1EAddVxToI() {

        byte[] data = { 0x60, 0x01, -0x10, 0x1E };

        memory.loadData(data);
        emulate(data);

        assertEquals(1, cpu.getI());

    }

    @DisplayName("code FX07 set Vx to the delayTimer")
    @Test
    void shouldTestOpcodeFX07SetVxToTheDelayTimer() {

        cpu.setDelayTimer(1);
        byte[] data = { -0x10, 0x07 };

        memory.loadData(data);
        emulate(data);

        assertEquals(1, register.get(0));

    }

    private void emulate(byte[] data) {
        for (int i = 0; i < (data.length / 2); i++) {
            cpu.emulateCycle();
        }
    }
}
