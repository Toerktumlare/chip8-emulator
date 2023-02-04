namespace Chip8;

public class Register
{
    private byte[] V = new byte[16];

    public Register() {
        for (int i = 0; i < 16; i++) {
            V[i] = 0x00;
        }
    }

    public byte Get(int index) {
        return V[index];
    }

    public void Set(int index, byte value) {
        V[index] = value;
    }

    public void Apply(int index, Func<byte,byte> func) {
        V[index] = func(V[index]);
    }
}
