namespace Chip8;

public class Program 
{
    public static int Main(string[] args) {

        if(args.Length == 0) {
            System.Console.WriteLine("You must supply a game argument, example: /roms/INVADERS.ch8");
            return 1;
        } else {
            return Utils.Load(args[0])
                .Map(bytes => {
                    var emulator = new Chip8.Emulator(bytes);
                    emulator.Run();
                    return 0; })
                .Reduce( () => 1);
        }
    }
}
