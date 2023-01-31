namespace Chip8.Tests;

public class UtilsTest
{
    [Fact]
    public void RegisterApplyTest()
    {
        var register = new Chip8.Register();
        int x = 0;
        int y = 0;
        register.Apply(x, vx => vx | register.Get(y));
    }
}
