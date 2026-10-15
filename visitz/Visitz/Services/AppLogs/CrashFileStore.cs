using System.Text.Json;
using VisitzModel.Models.Logging;

namespace Visitz.Services.AppLogs;

public class CrashFileStore
{
    private const string CrashDirectoryName = "pending-crashes";
    private static readonly string s_crashDirectory = Path.Combine(FileSystem.AppDataDirectory, CrashDirectoryName);

    private static readonly JsonSerializerOptions s_jsonOptions = new() { WriteIndented = true };

    public void Initialize()
    {
        Directory.CreateDirectory(s_crashDirectory);
    }

    public void PersistSynchronously(Exception exception)
    {
        var crash = new CrashRecord(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            DeviceInfo.Current.Platform.ToString(),
            exception.GetType().FullName ?? "Unknown",
            exception.Message,
            exception.ToString()
        );

        string json = SerializeCrash(crash);

        string fileName = $"crash-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{crash.Id}.json";

        string filePath = Path.Combine(s_crashDirectory, fileName);

        File.WriteAllText(filePath, json);
    }

    public IEnumerable<string> GetPendingCrashFiles()
    {
        return Directory.EnumerateFiles(s_crashDirectory, "crash-*.json");
    }

    private static string SerializeCrash(CrashRecord crash)
    {
        return JsonSerializer.Serialize(crash, s_jsonOptions);
    }
}
