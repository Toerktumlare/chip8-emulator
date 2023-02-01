using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Chip8;

public class Emulator : Game
{
    private const int WIDTH = 64;
    private const int HEIGHT = 32;

    private const int SCALE = 10;
    private GraphicsDeviceManager graphics;
 
    private Chip8.CPU cpu;
    private readonly Chip8.Keyboard keyboard;
    private readonly Chip8.Memory memory;
    public Emulator(byte[] gameData)
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        this.keyboard = new Chip8.Keyboard();
        
        memory = new Chip8.Memory();
        memory.LoadData(gameData);
    }

    protected override void Initialize()
    {
        var screen = new Screen(this, WIDTH, HEIGHT, SCALE);
        this.Components.Add(screen);
        
        var random = new System.Random();
        var register = new Chip8.Register();
        this.cpu = new Chip8.CPU(this.memory, register, random, this.keyboard, screen);

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        // if(Keyboard.GetState().IsKeyDown())

        cpu.EmulateCycle();

        base.Update(gameTime);
    }


    // private void KeyPressed(Chip8.KeyEventArgs e) {
    //     keyboard.OnKeyPressed(e.GetKeyCode());
    // }

    // private void KeyReleased(KeyEventArgs e) {
    //     keyboard.OnKeyReleased(e.GetKeyCode());
    // }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        GraphicsDevice.Textures[0] = null;
        
        base.Draw(gameTime);
    }
}
