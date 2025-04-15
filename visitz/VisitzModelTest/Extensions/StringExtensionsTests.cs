using VisitzModel.Extensions;

namespace VisitzModelTest.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("", null)]
    [InlineData(" ", null)]
    [InlineData("\t", null)]
    [InlineData("Y", true)]
    [InlineData("Yes", true)]
    [InlineData("Yeah", true)]
    [InlineData("Yep", true)]
    [InlineData("N", false)]
    [InlineData("No", false)]
    [InlineData("Nope", false)]
    [InlineData("Negative", false)]
    public void ParseEmptyWordTruthiness_Valid(string input, bool? expected)
    {
        bool? emptyWordTruthiness = input.ParseEmptyWordTruthiness();

        Assert.Equal(expected, emptyWordTruthiness);
    }
}
