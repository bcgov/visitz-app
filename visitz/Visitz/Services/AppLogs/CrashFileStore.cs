using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using VisitzModel.Models.Logging;

namespace Visitz.Services.AppLogs
{
    public class CrashFileStore
    {
        private readonly string _crashDirectory;

        public CrashFileStore()
        {
            _crashDirectory = Path.Combine(FileSystem.AppDataDirectory, "pending-crashes");
        }

        public void Initialize()
        {
            Directory.CreateDirectory(_crashDirectory);
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

            string json = JsonSerializer.Serialize(crash, new JsonSerializerOptions { WriteIndented = true });

            string fileName = $"crash-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{crash.Id}.json";

            string finalPath = Path.Combine(_crashDirectory, fileName);

            string tempPath = finalPath + ".tmp";

            using (var stream = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(json);

                writer.Flush();

                stream.Flush(flushToDisk: true);
            }

            File.Move(tempPath, finalPath);
        }

        public IEnumerable<string> GetPendingCrashFiles()
        {
            return Directory.EnumerateFiles(_crashDirectory, "crash-*.json");
        }
    }
}
