namespace Chip8;

public class Memory 
{
    private int[] memory = new int[4096];

    public Memory() {
        System.Array.Copy(Keyboard.FONTS, 0, memory, 0x50, 80);
    }

    public void LoadData(byte[] data) {
        for (int i = 0; i < data.Length; i++) {
            SetByte(i + 512, (data[i] & 0xFF));
        }

    }

    public int GetByte(int index) {
        return memory[index];
    }

    public void SetByte(int index, int value) {
        memory[index] = value;
    }

    public int Size() {
        return memory.Length;
    }

    public int GetOpcode(int pc) {
        return (GetByte(pc)) << 8 | GetByte(pc + 1);
    }
}
