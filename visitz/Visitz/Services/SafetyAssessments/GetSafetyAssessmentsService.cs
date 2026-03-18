using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Requests;
using VisitzModel.Models.SafetyAssess;
using VisitzModel.Storage;

namespace Visitz.Services.SafetyAssessments;

#nullable enable

internal class GetSafetyAssessmentsService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
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

    protected override async Task<int> RunPaginatedService(Pagination pagination)
    {
        var (total, assessmentJson) = await Vpi.GetSafetyAssessments(Info.Id, pagination);
        var assessments = SafetyAssessment.FromApiJson(Info.FileNumber, assessmentJson);

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await SafetyAssessment.SynchronizeAsync(realm, Info.FileNumber, assessments)
        );

        return total;
    }
}
