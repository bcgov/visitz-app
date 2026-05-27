using System.Collections.Concurrent;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Requests;
using VisitzModel.Extensions;
using VisitzModel.Models.SafetyAssess;
using VisitzModel.Storage;

namespace Visitz.Services.SafetyAssessments;

#nullable enable

internal class GetSafetyAssessmentsService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    public RecordServiceInfo Info => (RecordServiceInfo)Payload;

    readonly ConcurrentBag<SafetyAssessment> _assessments = [];

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

    protected override async Task<int> RunPageInParallelAsync(Pagination pagination)
    {
        var (total, assessmentJson) = await Vpi.GetSafetyAssessments(Info.Id, pagination);

        var assessments = SafetyAssessment.FromApiJson(Info.FileNumber, assessmentJson);
        _assessments.AddAll(assessments);

        return total;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await SafetyAssessment.SynchronizeAsync(realm, Info.FileNumber, _assessments)
        );
    }
}
