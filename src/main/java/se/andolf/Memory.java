package se.andolf;

public class Memory {

    private int[] memory = new int[4096];

    public Memory() {
        System.arraycopy(Keyboard.FONTS, 0, memory, 0, 80);
    }

    public void loadData(byte[] data) {
        for (int i = 0; i < data.length; i++) {
            setByte(i + 512, (data[i] & 0xFF));
        }

    }

    public int getByte(int index) {
        return memory[index];
    }

    public void setByte(int index, int value) {
        memory[index] = value;
    }

    public int size() {
        return memory.length;
    }

    public int getOpcode(int pc) {
        return (getByte(pc)) << 8 | getByte(pc + 1);
    }
}
