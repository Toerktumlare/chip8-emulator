namespace Chip8;

public class Emulator 
{
    private const int WIDTH = 64;
    private const int HEIGHT = 32;

    private const int SCALE = 10;
    private readonly CPU cpu;

    private Thread thread;
    private bool isRunning;

    private readonly Keyboard keyboard;

    //public final JFrame frame;
    private readonly Screen screen;


    public Emulator(byte[] gameData) {

        this.keyboard = new Keyboard();

        var memory = new Memory();
        var register = new Register();
        var random = new Random();

        memory.LoadData(gameData);


        screen = new Screen(WIDTH, HEIGHT, SCALE);
        //screen.SetPreferredSize(new Dimension(WIDTH * SCALE, HEIGHT * SCALE));

        // frame = new JFrame();
        // frame.setResizable(false);
        // frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        // frame.setTitle("Chip-8 Emulator");
        // frame.add(screen);
        // frame.pack();
        // frame.setLocationRelativeTo(null);
        // frame.setVisible(true);
        // frame.setAlwaysOnTop(true);
        // frame.addKeyListener(this);

        this.cpu = new CPU(memory, register, random, keyboard, screen);
    }

    public void Start() {
        isRunning = true;
        thread = new Thread(Run);
        thread.Start();
    }

    public void Stop() {
        isRunning = false;
        try {
            thread.Join();
        } catch (Exception e) {
            System.Console.WriteLine(e.Message);
            //e.printStackTrace();
        }
    }

    public void Run() {
        while (isRunning) {
            Update();
            Render();
        }
    }

    private void Render() {

        // if(cpu.getDrawFlag()) {
        //     screen.render();
        //     cpu.setDrawFlag(false);
        // }


        // if (cpu.getDelayTimer() > 0) {
        //     cpu.setDelayTimer(cpu.getDelayTimer()-1);
        // }

        // try {
        //     Thread.Sleep(10000);
        // } catch (Exception e) {
        //     Console.WriteLine(e.Message);
        // }
    }

    private void Update() {
        cpu.EmulateCycle();
    }

    public void KeyTyped(KeyEventArgs e) {
        // Not used
    }

    public void KeyPressed(KeyEventArgs e) {
        keyboard.OnKeyPressed(e.GetKeyCode());
    }

    public void KeyReleased(KeyEventArgs e) {
        keyboard.OnKeyReleased(e.GetKeyCode());
    }
}
