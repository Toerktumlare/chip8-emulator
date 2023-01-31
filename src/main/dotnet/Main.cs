public class Program {

    public static int Main(string[] args) {

        if(args.Length == 0) {
            System.Console.WriteLine("You must supply a game argument, example: /roms/INVADERS.ch8");
            return 1;
        } else {
            return Utils.Load(args[0])
                .Select(bytes => {
            //     final Emulator emulator = new Emulator(bytes);
            //     emulator.start();
                    return 0; })
                .DefaultIfEmpty( () => 1)
                .Single();
        }
    }
}
