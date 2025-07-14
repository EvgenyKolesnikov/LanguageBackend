global using Xunit;
namespace Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var test = true;
        Assert.True(test);
    }
}