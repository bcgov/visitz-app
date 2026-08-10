using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzApi.Models.SafetyAssess;
using VisitzModel.Models.SafetyAssess;
using VisitzModel.Storage;

namespace Visitz.Services.SafetyAssessments;

public class SubmitSafetyAssessmentService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
    public static string MakeId(string entityId)
    {
        return $"{nameof(SubmitSafetyAssessmentService)}-{entityId}";
    }

    public static StartServiceMessage MakeStartMessage(SafetyAssessment safetyAssessment)
    {
        return new StartServiceMessage()
        {
            Payload = safetyAssessment.ToApiJson(),
            ServiceId = MakeId(safetyAssessment.IncidentNumber),
            ServiceType = typeof(SubmitSafetyAssessmentService),
        };
    }

    private new SubmitSafetyAssessmentJson Payload => (SubmitSafetyAssessmentJson)base.Payload;

    public override string GetId()
    {
        return MakeId(Payload.Payload.First().IncidentNumber);
    }

    protected override async Task RunApiServiceAsync()
    {
        await SubmitSafetyAssessment();
    }

    private async Task SubmitSafetyAssessment()
    {
        var (success, _) = await Vpi.SubmitSafetyAssessmentAsync(Payload);

        ResultCode = success ? Result.Successful : Result.Error;

        if (ResultCode.Equals(Result.Successful))
            new SurveyFeedbackTracker(Preferences.Default).SetHasPublishedAnything();
    }
}
