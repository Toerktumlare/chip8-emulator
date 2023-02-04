namespace Chip8.Tests;

public class KeyboardMock : IKeyboard
{
    public bool[] GetKeys()
    {
        throw new NotImplementedException();
    }

    public bool IsPressed(int index)
    {
        throw new NotImplementedException();
    }

    public void OnKeyPressed(int keyCode)
    {
        throw new NotImplementedException();
    }

    public void OnKeyReleased(int keyCode)
    {
        throw new NotImplementedException();
    }
}