namespace VisitzModel.Utilities;

public static class PhoneNumberFormatter
{
    public static string? Format(string phoneNumber)
    {
        if (phoneNumber == null)
            return null;

        return phoneNumber.Length switch
        {
            10 => $"({phoneNumber[..3]}) {phoneNumber[3..6]}-{phoneNumber[6..]}",
            7 => $"{phoneNumber[..3]}-{phoneNumber[3..]}",
            _ => phoneNumber,
        };
    }
}
