using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;

namespace Visitz.Services.SafetyAssessments;

#nullable enable

internal class GetSafetyAssessmentsByRangeService(Vpi vpi, LastUpdatedPrefs prefs, ServiceHandler serviceHandler)
    : VisitzApiRangeService<RecordServiceInfo>(vpi, prefs, serviceHandler)
{
    public static StartServiceMessage MakeStartMessage(IEnumerable<RecordServiceInfo> records)
    {
        return new StartServiceMessage()
        {
            ServiceId = MakeId(),
            ServiceType = typeof(GetSafetyAssessmentsByRangeService),
            Payload = records,
        };
    }

    public static string MakeId()
    {
        return nameof(GetSafetyAssessmentsByRangeService);
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunInParallelAsync(ServiceHandler serviceHandler, RecordServiceInfo item)
    {
        var msg = GetSafetyAssessmentsService.MakeStartMessage(item);
        await ServiceHandler.TryRunServiceAsync(msg);
    }

    protected override Exception MakeOverallException(List<ApiRangeItemException<RecordServiceInfo>> exceptions)
    {
        return exceptions.CombineIntoException();
    }
}
