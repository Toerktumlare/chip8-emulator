﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Input = Microsoft.Xna.Framework.Input;

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
    private Input.KeyboardState oldState;
    private double updateCounter = 0, drawCounter = 0;
    public Emulator(byte[] gameData)
    {
        graphics = new GraphicsDeviceManager(this);
        graphics.IsFullScreen = true;
        graphics.PreferredBackBufferHeight = 320;
        graphics.PreferredBackBufferWidth = 640;
        graphics.SynchronizeWithVerticalRetrace = false;
        IsMouseVisible = true;

        this.keyboard = new Chip8.Keyboard();
        
        memory = new Chip8.Memory();
        memory.LoadData(gameData);

        this.IsFixedTimeStep = false;
        graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        var screen = new Screen(this, WIDTH, HEIGHT, SCALE);
        this.Components.Add(screen);
        
        var random = new System.Random();
        var register = new Chip8.Register();
        this.cpu = new Chip8.CPU(this.memory, register, random, this.keyboard, screen);

        GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap; 
        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        if (Input.GamePad.GetState(PlayerIndex.One).Buttons.Back == Input.ButtonState.Pressed || Input.Keyboard.GetState().IsKeyDown(Input.Keys.Escape)) {
            System.Console.WriteLine($"Updates: {updateCounter}");
            System.Console.WriteLine($"Draws: {drawCounter}");
            System.Console.WriteLine($"Instructions: {cpu.InstructionsCounter}");
            Exit();
        }

        HandleInput();
        
        cpu.EmulateCycle();
        base.Update(gameTime);
        updateCounter++;
    }

    private void HandleInput() {
        var state = Input.Keyboard.GetState();
        TestKey(Input.Keys.D1, oldState, state);
        TestKey(Input.Keys.D2, oldState, state);
        TestKey(Input.Keys.D3, oldState, state);
        TestKey(Input.Keys.D4, oldState, state);
        TestKey(Input.Keys.Q, oldState, state);
        TestKey(Input.Keys.W, oldState, state);
        TestKey(Input.Keys.E, oldState, state);
        TestKey(Input.Keys.R, oldState, state);
        TestKey(Input.Keys.A, oldState, state);
        TestKey(Input.Keys.S, oldState, state);
        TestKey(Input.Keys.D, oldState, state);
        TestKey(Input.Keys.F, oldState, state);
        TestKey(Input.Keys.Z, oldState, state);
        TestKey(Input.Keys.X, oldState, state);
        TestKey(Input.Keys.C, oldState, state);
        TestKey(Input.Keys.V, oldState, state);
        oldState = state;
    }

    private void TestKey(Input.Keys key, Input.KeyboardState oldState, Input.KeyboardState newState) {
        if(oldState.IsKeyUp(key) && newState.IsKeyDown(key)) {
            keyboard.OnKeyPressed((int)key);
        } else if(oldState.IsKeyDown(key) && newState.IsKeyUp(key)) {
            keyboard.OnKeyReleased((int)key);
        }        
    }

    protected override void Draw(GameTime gameTime)
    {
        if(this.cpu.DrawFlag) {    
            base.Draw(gameTime);
            this.cpu.DrawFlag = false;
        }
        drawCounter++;
    }
}
