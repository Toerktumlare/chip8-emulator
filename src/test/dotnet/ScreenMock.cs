namespace Chip8.Tests;

public class ScreenMock : IScreen
{
    public int Width => throw new NotImplementedException();

    public int Height => throw new NotImplementedException();

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public uint GetPixel(int xCoord, int yCoord)
    {
        throw new NotImplementedException();
    }

    public void SetPixel(int xCoord, int yCoord)
    {
        throw new NotImplementedException();
    }
}