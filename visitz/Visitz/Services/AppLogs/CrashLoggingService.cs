using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using VisitzModel.Models.Logging;

namespace Visitz.Services.AppLogs
{
    public class CrashLoggingService
    {
        private readonly CrashFileStore _crashFileStore;
        private readonly ILogger _logger;

        public CrashLoggingService(CrashFileStore crashFileStore)
        {
            _crashFileStore = crashFileStore;
            _logger = ServiceProvider.GetService<ILogger<CrashLoggingService>>();
        }

        public async Task ProcessPendingAsync()
        {
            foreach (string filePath in _crashFileStore.GetPendingCrashFiles())
            {
                try
                {
                    string json = await File.ReadAllTextAsync(filePath);

                    CrashRecord? crash = JsonSerializer.Deserialize<CrashRecord>(json);

                    if (crash == null)
                    {
                        _logger.LogWarning("Unable to deserialize crash file {FilePath}", filePath);

                        continue;
                    }

                    // Replay the previous crash through the normal logging pipeline
                    _logger.LogCritical(
                        "Recovered crash from previous app run. "
                            + "Timestamp: {TimestampToLog}, Message: {Message}, StackTrace: {ExceptionText}",
                        crash.TimestampToLog,
                        crash.Message,
                        crash.ExceptionText
                    );

                    // Successfully processed, so remove the temporary crash file
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process crash file {FilePath}", filePath);
                }
            }
        }
    }
}
