package se.andolf;

import java.awt.*;
import java.awt.image.BufferStrategy;

public class Screen extends Canvas {

    private final int width, height, scale;
    private final int[][] pixels;

    public Screen(int width, int height, int scale) {
        this.width = width;
        this.height = height;
        this.scale = scale;
        pixels = new int[this.width][this.height];
        clear();

        setPreferredSize(new Dimension(this.width * this.scale, this.height * this.scale));
    }

    public void clear() {
        for (int y = 0; y < this.height; y++) {
            for (int x = 0; x < this.width; x++) {
                pixels[x][y] = 0;
            }
        }
    }

    public void render() {

        final BufferStrategy bs = getBufferStrategy();
        if(bs == null) {
            createBufferStrategy(3);
            return;
        }

        final Graphics gc = bs.getDrawGraphics();

        for(int y = 0; y < this.height; y++) {
            for(int x = 0; x < this.width; x++) {
                if (pixels[x][y] == 1) {
                    gc.setColor(Color.WHITE);
                } else {
                    gc.setColor(Color.BLACK);
                }

                gc.fillRect(x * scale, y * scale, scale, scale);
            }
        }

        gc.dispose();
        bs.show();
    }

    public void setPixel(int xCoord, int yCoord) {
        pixels[xCoord][yCoord] ^= 1;
    }

    public int getPixel(int xCoord, int yCoord) {
        return pixels[xCoord][yCoord];
    }
}
