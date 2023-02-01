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
    
    public Screen(Game game, int width, int height, int scale) : base(game) {
        this.width = width;
        this.height = height;
        this.scale = scale;
        pixels = new uint[this.width * this.height];
        Clear();

        tracedSize = new Rectangle(0,0, width, height);
        canvas = new Texture2D(GraphicsDevice, tracedSize.Width, tracedSize.Height, false, SurfaceFormat.Color);
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
        spriteBatch.Begin();
        spriteBatch.GraphicsDevice.BlendState = BlendState.Opaque;
        spriteBatch.Draw(canvas, Vector2.Zero, new Rectangle(0, 0, tracedSize.Width, tracedSize.Height), Color.White, 0.0f, Vector2.Zero, this.scale, SpriteEffects.None, 0);
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