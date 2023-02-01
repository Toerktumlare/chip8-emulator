using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace test;

public class Emulator : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private Texture2D canvas;
    Rectangle tracedSize;
    System.Random rnd;
    uint[] pixels;

    public Emulator()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        tracedSize = GraphicsDevice.PresentationParameters.Bounds;
        canvas = new Texture2D(GraphicsDevice, tracedSize.Width, tracedSize.Height, false, SurfaceFormat.Color);
        pixels = new uint[tracedSize.Width * tracedSize.Height];
        rnd = new System.Random();
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();


        pixels[rnd.Next(pixels.Length)] = 0xFF00FF00;
        canvas.SetData<uint>(pixels, 0, tracedSize.Width * tracedSize.Height);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);


        GraphicsDevice.Textures[0] = null;

        spriteBatch.Begin();
        spriteBatch.Draw(canvas, new Rectangle(0, 0, tracedSize.Width, tracedSize.Height), Color.White);
        spriteBatch.End();
        
        base.Draw(gameTime);
    }
}
