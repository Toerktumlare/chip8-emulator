namespace Chip8;

public interface IScreen {

    int Width { get; }
    int Height { get; }
    void Clear();
    void SetPixel(int xCoord, int yCoord);
    uint GetPixel(int xCoord, int yCoord);
}
