using Visitz.Models.SafetyAssess;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzApi.Models.SafetyAssess;
using VisitzModel.Models;

namespace Visitz.Services;

public class SubmitSafetyAssessmentService(Vpi vpi) : VisitzApiService(vpi)
{
    public static string MakeId(string entityId)
    {
        return $"{nameof(SubmitSafetyAssessmentService)}-{entityId}";
    }

    public static string MakeId(CaseloadItem caseloadItem)
    {
        return MakeId(caseloadItem.CaseIncidentNumber);
    }

    public static StartServiceMessage MakeStartMessage(SafetyAssessment safetyAssessment)
    {
        return new StartServiceMessage()
        {
            Payload = safetyAssessment.ToApiEntity(),
            ServiceId = MakeId(safetyAssessment.IncidentNumber),
            ServiceType = typeof(SubmitSafetyAssessmentService),
        };
    }

    private new SafetyAssessmentEntity Payload => (SafetyAssessmentEntity)base.Payload;

    public override string GetId()
    {
        return MakeId(Payload.IncidentNumber);
    }

    protected override async Task RunApiServiceAsync()
    {
        await SubmitSafetyAssessment();
    }

    private async Task SubmitSafetyAssessment()
    {
        var (success, _) = await Vpi.SubmitSafetyAssessmentAsync(Payload);

        ResultCode = success ? Result.Successful : Result.Error;
    }
}
