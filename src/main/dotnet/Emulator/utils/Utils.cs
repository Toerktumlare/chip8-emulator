namespace Chip8;

public static class Utils {

    public static Option<byte[]> Load(string filename) {
        try {
            return new Some<byte[]>(File.ReadAllBytes(filename));
        } catch(Exception) {
            return None.Value;
        }
    }

    public static int GetBitValue(int value, int bitIndex) {
        return value & (0x80 >> bitIndex);
    }
}
