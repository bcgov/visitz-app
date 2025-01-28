using Microsoft.Extensions.Logging;
using System.Net;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.Base;
using VisitzApi.Models.Caseload;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Services.Caseload
{
    public class GetCaseloadService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
    {
        public static string MakeId()
        {
            return nameof(GetCaseloadService);
        }

        public static StartServiceMessage MakeStartMessage(string idir)
        {
            return new StartServiceMessage
            {
                ServiceId = MakeId(),
                ServiceType = typeof(GetCaseloadService),
                Payload = idir
            };
        }

        public string Idir => (string)Payload;

        protected override async Task RunApiServiceAsync()
        {
            await GetCaseloadV1Async();
            await DownloadAndSaveCaseloadV2Async();

            ResultCode = Result.Successful;
        }

        private async Task GetCaseloadV1Async()
        {
            var caseloadFromApi = await Vpi.GetCaseloadV1Async(Idir);
            var caseloadContent = CaseloadItem.FromApiEntities(caseloadFromApi);

            caseloadContent = FilterNonCasesAndIncidents(caseloadContent);

            using var realm = await VisitzRealms.GetIcmDataRealmAsync();
            await CaseloadItem.ReplaceCaseloadWithAsync(realm, caseloadContent);
        }

        private async Task DownloadAndSaveCaseloadV2Async()
        {
            CaseloadJson caseloadFromApi = await Vpi.GetCaseloadV2Async(after: null);

            using var realm = await VisitzRealms.GetIcmDataRealmAsync();

            if (IsSuccess(caseloadFromApi.Cases))
                await CaseRecord.SynchronizeCasesAsync(realm, caseloadFromApi.Cases);
            else
                throw new InvalidOperationException(caseloadFromApi.Cases.GetFirstMessage() +
                    " -> " + caseloadFromApi.Cases.GetFirstError());

            if (IsSuccess(caseloadFromApi.Incidents))
                await IncidentRecord.SynchronizeAsync(realm, caseloadFromApi.Incidents);
            else
            {
                string msg = caseloadFromApi.Incidents.GetFirstMessage()
                    + " -> " + caseloadFromApi.Incidents.GetFirstError();
                ServiceProvider.GetService<ILogger<GetCaseloadService>>().LogError(msg);

                // TODO: proper partial error handling when incidents are available
            }
        }

        private static bool IsSuccess<T>(SectionJson<T> section) where T : AssignableRecordJson
        {
            HttpStatusCode status = (HttpStatusCode)section.Status;
            return status == HttpStatusCode.OK || status == HttpStatusCode.NoContent;
        }

        public override string GetId()
        {
            return MakeId();
        }

        /// <summary>
        /// As of v1.0, it is currently a business decision to only allow users to interact with Cases and Incidents 
        /// from their caseload.
        /// </summary>
        /// <param name="caseloadItems"></param>
        /// <returns></returns>
        private IEnumerable<CaseloadItem> FilterNonCasesAndIncidents(IEnumerable<CaseloadItem> caseloadItems)
        {
            return caseloadItems.Where(item =>
            {
                EntityType type = item.EntityType.ParseEntityType();
                return type == EntityType.Case || type == EntityType.Incident;
            });
        }
    }
}
