using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Models.SafetyAssess;
using VisitzModel.Storage;

namespace Visitz.Services.SafetyAssessments;

internal class GetSafetyAssessmentsService(Vpi vpi, LastUpdatedPrefs prefs)
    : VisitzApiService(vpi, prefs)
{
    public RecordServiceInfo Info => (RecordServiceInfo)Payload;

    public static StartServiceMessage MakeStartMessage(RecordServiceInfo info)
    {
        return new StartServiceMessage()
        {
            ServiceId = MakeId(info),
            ServiceType = typeof(GetSafetyAssessmentsService),
            Payload = info,
        };
    }

    public static string MakeId(RecordServiceInfo info)
    {
        return $"{nameof(GetSafetyAssessmentsService)}|{info.Id}";
    }

    public override string GetId()
    {
        return MakeId(Info);
    }

    protected override async Task RunApiServiceAsync()
    {
        await DownloadAndSynchronizeSafetyAssessments();

        ResultCode = Result.Successful;
    }

    async Task DownloadAndSynchronizeSafetyAssessments()
    {
        var assessmentJson = await Vpi.GetSafetyAssessments(Info.Id, pagination: null);
        var assessments = SafetyAssessment.FromApiJson(Info.FileNumber, assessmentJson);

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await SafetyAssessment.SynchronizeAsync(realm, Info.FileNumber, assessments));
    }
}
