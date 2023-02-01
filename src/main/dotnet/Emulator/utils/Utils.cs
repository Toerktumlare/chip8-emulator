namespace Chip8;

public static class Utils {

    public static Option<byte[]> Load(string filename) {
        byte[] result = null;
        try {
            return new Some<byte[]>(File.ReadAllBytes(filename));
        } catch(Exception ex) {
            return None.Value;
        }
    }

    public static int GetBitValue(int value, int bitIndex) {
        return value & (0x80 >> bitIndex);
    }

    public static string GetHex(int value) {
        return value.ToString("X");
    }
}
