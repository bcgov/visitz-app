using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzApi.Requests;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;
using VisitzModel.Storage;

namespace Visitz.Services.People;

internal class SubmitChildYouthInfoService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
    new ValueTuple<RecordServiceInfo, string, IDraftItem> Payload =>
        ((RecordServiceInfo, string, IDraftItem))base.Payload;

    RecordServiceInfo RecordServiceInfo => Payload.Item1;

    string ParentContactId => Payload.Item2;

    IDraftItem Draft => Payload.Item3;

    public static string MakeId(
        string parentRowId,
        EntityType parentType,
        string parentContactId,
        IDraftItem childYouthInfoDraft
    )
    {
        return $"{nameof(SubmitChildYouthInfoService)}|{parentType}|{parentRowId}|{parentContactId}|{childYouthInfoDraft.Preview}";
    }

    public static StartServiceMessage MakeStartMessage(
        RecordServiceInfo recordServiceInfo,
        string parentContactId,
        IDraftItem childYouthInfoDraft
    )
    {
        return new()
        {
            ServiceId = MakeId(recordServiceInfo.Id, recordServiceInfo.Type, parentContactId, childYouthInfoDraft),
            ServiceType = typeof(SubmitChildYouthInfoService),
            Payload = (recordServiceInfo, parentContactId, childYouthInfoDraft),
        };
    }

    public override string GetId()
    {
        return MakeId(RecordServiceInfo.Id, RecordServiceInfo.Type, ParentContactId, Draft);
    }

    protected override async Task RunApiServiceAsync()
    {
        ApiRecordType type = (ApiRecordType)RecordServiceInfo.Type;
        string parentRowId = RecordServiceInfo.Id;
        bool result;

        if (Draft is ContactEducationDraft education && education.ContactEducation != null)
        {
            result = await Vpi.SubmitContactEducationAsync(
                type,
                parentRowId,
                ParentContactId,
                education.ContactEducation.ToApiJson()
            );
        }
        else if (Draft is ContactMedicalBehavioralDraft medical && medical.ContactMedicalBehavioral != null)
        {
            result = await Vpi.SubmitContactMedicalBehavioralAsync(
                type,
                parentRowId,
                ParentContactId,
                medical.ContactMedicalBehavioral.ToApiJson()
            );
        }
        else if (Draft is ContactLanguageDraft language && language.ContactLanguage != null)
        {
            result = await Vpi.SubmitContactLanguageAsync(
                type,
                parentRowId,
                ParentContactId,
                language.ContactLanguage.ToApiJson()
            );
        }
        else
            throw new InvalidOperationException();

        if (result)
            ResultCode = Result.Successful;
    }
}
