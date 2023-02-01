namespace Chip8;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Screen : DrawableGameComponent, Chip8.IScreen
{
    private readonly int width, height, scale;
    private readonly uint[] pixels;
    private Texture2D canvas;
    private Rectangle tracedSize;
    private SpriteBatch? spriteBatch;
    RenderTarget2D target;
    
    public Screen(Game game, int width, int height, int scale) : base(game) {
        this.width = width;
        this.height = height;
        this.scale = scale;
        pixels = new uint[this.width * this.height];
        Clear();

        tracedSize = new Rectangle(0,0, width, height);
        canvas = new Texture2D(GraphicsDevice, tracedSize.Width, tracedSize.Height, false, SurfaceFormat.Color);

        target = new RenderTarget2D(GraphicsDevice, width, height);
    }

    public int Width => width;
    public int Height => height;

    public void Clear() {
        var length = this.height * this.width;
        for (int i = 0; i < length; i++) {
            pixels[i] = 0;
        }
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        spriteBatch = new SpriteBatch(GraphicsDevice);

    }

    public override void Update(GameTime gameTime)
    {
        canvas.SetData<uint>(pixels, 0, tracedSize.Width * tracedSize.Height);

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(target);
        GraphicsDevice.Clear(Color.Black);

        spriteBatch.Begin();
        spriteBatch.Draw(canvas, new Rectangle(0, 0, tracedSize.Width, tracedSize.Height), Color.White);
        spriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(target, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
        spriteBatch.End();

        base.Draw(gameTime);
    }

    public void SetPixel(int xCoord, int yCoord) {
        pixels[yCoord * this.width + xCoord] ^= 0xFFFFFFFF;
    }

    public uint GetPixel(int xCoord, int yCoord) {
        return pixels[yCoord * this.width + xCoord];
    }
}