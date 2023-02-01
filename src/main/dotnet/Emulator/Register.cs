namespace Chip8;

public class Register
{
    private int[] V = new int[16];

    public Register() {
        for (int i = 0; i < 16; i++) {
            V[i] = 0;
        }
    }

    public int Get(int index) {
        return V[index];
    }

    public void Set(int index, int value) {
        V[index] = value;
    }

    public void Apply(int index, Func<int,int> func) {
        V[index] = func(V[index]);
    }
}
