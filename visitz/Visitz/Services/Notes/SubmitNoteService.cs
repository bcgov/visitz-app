using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Views.Debugging;
using VisitzApi;
using VisitzApi.Models.Notes;
using VisitzModel.Storage;

namespace Visitz.Services.Notes;

#nullable enable

public class SubmitNoteService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
    public static string MakeId(string entityNumber, string notePeriod)
    {
        return $"{nameof(SubmitNoteService)}-{entityNumber}-{notePeriod}";
    }

    public static StartServiceMessage MakeStartMessage(SubmitNoteEntity submitEntity)
    {
        return new StartServiceMessage()
        {
            Payload = submitEntity,
            ServiceId = MakeId(submitEntity.EntityNumber, submitEntity.NotePeriod),
            ServiceType = typeof(SubmitNoteService),
        };
    }

    private new SubmitNoteEntity Payload => (SubmitNoteEntity)base.Payload;

    public override string GetId()
    {
        return MakeId(Payload.EntityNumber, Payload.NotePeriod);
    }

    protected override async Task RunApiServiceAsync()
    {
        if (DebugOptions.Default.DryFireSubmitNotes)
        {
            await Task.Delay(2500); // Simulate network activity
            ResultCode = DebugOptions.Default.DryFireSubmitNotesSimulateSuccess ? Result.Successful : Result.Error;
        }
        else
            await SubmitNoteAsync();
    }

    private async Task SubmitNoteAsync()
    {
        var (status, _) = await Vpi.SubmitNotesAsync(Payload);

        ResultCode = status ? Result.Successful : Result.Error;

        if (ResultCode.Equals(Result.Successful))
            new SurveyFeedbackTracker(Preferences.Default).SetHasPublishedAnything();
    }
}
