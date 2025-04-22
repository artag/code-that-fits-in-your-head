namespace Restaurant.RestApi.Tests;

public class UrlBuilderTests
{
    [Theory]
    [InlineData("Home")]
    [InlineData("Calendar")]
    [InlineData("Reservations")]
    public void WithControllerHandlesSuffix(string name)
    {
        var sut = new UrlBuilder();

        var actual = sut.WithController(name + "Controller");

        var expected = sut.WithController(name);
        Assert.Equal(expected, actual);
    }
}
