using System.Globalization;
using CommunityToolkit.Maui.Converters;

namespace Visitz.Converters;

internal partial class DateTimeOffsetNullableConverter : BaseConverter<DateTimeOffset?, DateTime?>
{
    public override DateTime? DefaultConvertReturnValue
    {
        get;
        set => field = null;
    }
    public override DateTimeOffset? DefaultConvertBackReturnValue
    {
        get;
        set => field = null;
    }

    public override DateTimeOffset? ConvertBackTo(DateTime? value, CultureInfo? culture)
    {
        return value is DateTime dt ? new DateTimeOffsetConverter().ConvertBackTo(dt) : null;
    }

    public override DateTime? ConvertFrom(DateTimeOffset? value, CultureInfo? culture)
    {
        return value is DateTimeOffset dto ? new DateTimeOffsetConverter().ConvertFrom(dto) : null;
    }
}
