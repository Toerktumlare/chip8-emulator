namespace Chip8.Tests;

public class UtilsTest
{
    [Fact]
    public void RegisterApplyTest()
    {
        var register = new Chip8.Register();
        register.Set(2, 4);
        int x = 0;
        int y = 2;
        register.Apply(x, vx => vx | register.Get(y));

        Assert.Equal(4, register.Get(0));
    }
}
