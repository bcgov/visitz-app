using System.Text.Json;
using MetroLog;

namespace Visitz.Storage;

internal class VisitzLogLayout : MetroLog.Layouts.Layout
{
    public override string GetFormattedString(LogWriteContext context, LogEventInfo info)
    {
        return JsonSerializer.Serialize(info);
    }
}
