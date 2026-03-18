using VisitzModel.Utilities;

namespace VisitzModelTest.Utilities;

public class PhoneNumberFormatterTests
{
    const string ArbitraryNumber1 = "1";
    const string ArbitraryNumber2 = "12";
    const string ArbitraryNumber3 = "123";
    const string ArbitraryNumber4 = "1234";
    const string ArbitraryNumber5 = "12345";
    const string ArbitraryNumber6 = "123456";
    const string ArbitraryNumber7 = "1234567";
    const string ArbitraryNumber8 = "12345678";
    const string ArbitraryNumber9 = "123456789";
    const string ArbitraryNumber10 = "1234567890";
    const string ArbitraryNumber11 = "12345678901";
    const string ArbitraryNumber12 = "123456789012";

    const string FormattedArbitraryNumber7 = "123-4567";
    const string FormattedArbitraryNumber10 = "(123) 456-7890";

    [Fact]
    public void SevenDigitNumberFormatsCorrectly()
    {
        Assert.Equal(FormattedArbitraryNumber7, PhoneNumberFormatter.Format(ArbitraryNumber7));
    }

    [Fact]
    public void TenDigitNumberFormatsCorrectly()
    {
        Assert.Equal(FormattedArbitraryNumber10, PhoneNumberFormatter.Format(ArbitraryNumber10));
    }

    [Theory]
    [InlineData(ArbitraryNumber1)]
    [InlineData(ArbitraryNumber2)]
    [InlineData(ArbitraryNumber3)]
    [InlineData(ArbitraryNumber4)]
    [InlineData(ArbitraryNumber5)]
    [InlineData(ArbitraryNumber6)]
    [InlineData(ArbitraryNumber8)]
    [InlineData(ArbitraryNumber9)]
    [InlineData(ArbitraryNumber11)]
    [InlineData(ArbitraryNumber12)]
    public void NonSupportedNumbersDoNotFormat(string phoneNumber)
    {
        Assert.Equal(phoneNumber, PhoneNumberFormatter.Format(phoneNumber));
    }

    [Fact]
    public void ReturnNullWhenPhoneNumberIsNull()
    {
        Assert.Null(PhoneNumberFormatter.Format(null));
    }
}
