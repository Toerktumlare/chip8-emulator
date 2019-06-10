package se.andolf.utils;

import java.io.BufferedInputStream;
import java.io.DataInputStream;
import java.io.IOException;
import java.io.InputStream;

public class Utils {

    public static byte[] load(String filename) {
        final InputStream is = Utils.class.getResourceAsStream(filename);
        byte[] b = null;
        try (DataInputStream in = new DataInputStream(new BufferedInputStream(is))) {
            b = new byte[is.available()];
            in.read(b);

        } catch (IOException e) {
            e.printStackTrace();
        }
        return b;
    }

    public static int getBitValue(int value, int bitIndex) {
        return value & (0x80 >> bitIndex);
    }

    public static String getHex(int value) {
        return Integer.toHexString(value);
    }
}
