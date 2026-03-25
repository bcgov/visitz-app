using VisitzModel.Extensions;

namespace VisitzTest.Extensions;

public class BooleanExtenionsTests
{
    const string Y = "Y";
    const string N = "N";

    const string Yes = "Yes";
    const string No = "No";

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AsTruthyChar_Valid(bool value)
    {
        var truthyChar = BooleanExtensions.AsTruthyChar(value);

        var expected = value ? Y : N;
        Assert.Equal(expected, truthyChar);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AsTruthyWord_Valid(bool value)
    {
        var truthyWord = BooleanExtensions.AsTruthyWord(value);

        var expected = value ? Yes : No;
        Assert.Equal(expected, truthyWord);
    }
}
