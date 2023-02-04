namespace Chip8;

public interface IKeyboard {
    void OnKeyPressed(int keyCode);
    void OnKeyReleased(int keyCode);

    bool IsPressed(int index);
    bool[] GetKeys();
}