using Oidc;
using Realms;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Services.Notes;
using Visitz.Services.People;
using Visitz.Services.Visits;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Models;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Services.Caseload
{
    public class GetAllDataForOfflineService(Vpi vpi, ServiceHandler serviceHandler, LastUpdatedPrefs prefs)
        : VisitzApiService(vpi, prefs)
    {
        public static string MakeId()
        {
            return nameof(GetAllDataForOfflineService);
        }

        public static StartServiceMessage MakeStartMessage()
        {
            return new StartServiceMessage()
            {
                ServiceId = MakeId(),
                ServiceType = typeof(GetAllDataForOfflineService),
                Payload = null,
            };
        }

        private ServiceHandler ServiceHandler { get; set; } = serviceHandler;

        public override string GetId()
        {
            return MakeId();
        }

        protected override async Task RunApiServiceAsync()
        {
            await Task.Run(async () =>
            {
                await GetCaseload();

                using var realm = await VisitzRealms.GetIcmDataRealmAsync();
                await Task.WhenAll(
                    GetAllNotes(realm),
                    GetAllVisits(realm),
                    GetAllContacts(realm)
                );
            });

            ResultCode = Result.Successful;
        }

        private async Task GetCaseload()
        {
            var info = await OidcSessionInfo.GetAsync();
            await ServiceHandler.TryRunServiceAsync(GetCaseloadService.MakeStartMessage(info.Idir));
        }

        private async Task GetAllNotes(Realm realm)
        {
            var allIdEntities = realm
                .All<CaseloadItem>()
                .Freeze()
                .AsEnumerable()
                .Select(item => (item.CaseIncidentNumber, item.EntityType));

            var startMessage = GetNotesForRangeService.MakeStartMessage(allIdEntities);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }

        private async Task GetAllVisits(Realm realm)
        {
            var allCaseIds = realm
                .All<CaseRecord>()
                .Freeze()
                .AsEnumerable()
                .Select(@case => @case.Id);

            var startMessage = GetVisitsByRangeService.MakeStartMessage(allCaseIds);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }

        private async Task GetAllContacts(Realm realm)
        {
            var cases = realm
                .All<CaseRecord>()
                .Freeze()
                .AsEnumerable()
                .Select(@case => new ContactServiceInfo()
                {
                    Type = EntityType.Case,
                    Id = @case.Id,
                    Label = @case.Name,
                });

            var incidents = realm
                .All<IncidentRecord>()
                .Freeze()
                .AsEnumerable()
                .Select(incident => new ContactServiceInfo()
                {
                    Type = EntityType.Case,
                    Id = incident.Id,
                    Label = incident.Name,
                });

            // TODO: Memos, SRs

            var all = cases.Concat(incidents);

            var startMessage = GetContactsByRangeService.MakeStartMessage(all);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }
    }
}
