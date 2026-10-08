using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace VisitzModel.Models.Logging
{
    public record CrashRecord(
        Guid Id,
        DateTimeOffset TimestampUtc,
        string Platform,
        string ExceptionType,
        string Message,
        string ExceptionText
    )
    {
        public string TimestampToLog =>
            TimestampUtc.ToString("yyyy-MM-dd h:mm:ss tt zzz", CultureInfo.InvariantCulture);
    }
}
