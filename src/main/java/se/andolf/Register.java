package se.andolf;

import java.util.function.IntUnaryOperator;

public class Register {
    private Integer[] V = new Integer[16];

    public int get(int index) {
        return V[index];
    }

    public void set(int index, int to) {
        V[index] = to;
    }

    public void apply(int index, IntUnaryOperator func) {
        V[index] = func.applyAsInt(V[index]);
    }
}
