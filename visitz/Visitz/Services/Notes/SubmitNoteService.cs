using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Views.Debugging;
using VisitzApi;
using VisitzApi.Models.Notes;
using VisitzModel.Storage;

namespace Visitz.Services.Notes;

internal class SubmitNoteService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
    public static string MakeId(RecordServiceInfo info, string notePeriod)
    {
        return $"{nameof(SubmitNoteService)}-{info.Type}-{info.Id}-{notePeriod}";
    }

    public static StartServiceMessage MakeStartMessage(RecordServiceInfo info, SubmitNoteEntity submitEntity)
    {
        return new StartServiceMessage()
        {
            Payload = (info, submitEntity),
            ServiceId = MakeId(info, submitEntity.NotePeriod),
            ServiceType = typeof(SubmitNoteService),
        };
    }

    private new (RecordServiceInfo, SubmitNoteEntity) Payload => ((RecordServiceInfo, SubmitNoteEntity))base.Payload;

    private RecordServiceInfo ParentInfo => Payload.Item1;

    private SubmitNoteEntity SubmitNoteEntity => Payload.Item2;

    public override string GetId()
    {
        return MakeId(ParentInfo, SubmitNoteEntity.NotePeriod);
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
        var (status, _) = await Vpi.SubmitNotesAsync(SubmitNoteEntity);

        ResultCode = status ? Result.Successful : Result.Error;

        if (ResultCode.Equals(Result.Successful))
            new SurveyFeedbackTracker(Preferences.Default).SetHasPublishedAnything();
    }
}
