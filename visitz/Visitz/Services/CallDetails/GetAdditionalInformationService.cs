using System;
using System.Collections.Generic;
using System.Text;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Services.People;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.CallDetails;
using VisitzApi.Requests;
using VisitzModel.Models.CallDetails;
using VisitzModel.Models.EntityTypes;
//need to review
using VisitzModel.Models.People;
using VisitzModel.Storage;

namespace Visitz.Services.CallDetails;

internal class GetAdditionalInformationService(Vpi vpi, LastUpdatedPrefs prefs)
    : ApiPaginationService(vpi, prefs)
{
    RecordServiceInfo Info => (RecordServiceInfo)Payload;

    public static string MakeId(string id)
    {
        return $"{nameof(GetAdditionalInformationService)}|{id}";
    }

    public static StartServiceMessage MakeStartMessage(RecordServiceInfo info)
    {
        return new()
        {
            ServiceId = MakeId(info.Id),
            ServiceType = typeof(GetAdditionalInformationService),
            Payload = info,
        };
    }

    public override string GetId()
    {
        return MakeId(Info.Id);
    }

    override protected async Task<int> RunPaginatedService(Pagination pagination)
    {
        var (total, contacts) = await Vpi.GetAdditionalInformation(
(ApiRecordType)Info.Type,
Info.Id,
pagination);
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
        await AdditionalInformation.SynchronizeAsync(realm, contacts, Info.Id, Info.Type));
        return total;
    }

}

