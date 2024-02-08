using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Models;

namespace Visitz.Services
{
    public class GetCaseloadService : VisitzApiService
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

        public GetCaseloadService(Vpi vpi) : base(vpi) { }

        protected override async Task RunApiServiceAsync()
        {
            await GetCaseloadAsync();
        }

        private async Task GetCaseloadAsync()
        {
            var caseloadFromApi = await Vpi.GetCaseloadAsync(Idir);
            var caseloadContent = CaseloadItem.FromApiEntities(caseloadFromApi);

            caseloadContent = FilterNonCasesAndIncidents(caseloadContent);

            using var realm = await VisitzRealm.GetIcmDataAsync();
            var currentCaseload = realm.All<CaseloadItem>();
            var deletedCaseload = currentCaseload.ExceptBy(caseloadContent.Select(CaseloadSelector), CaseloadSelector);

            await realm.WriteAsync(() =>
            {
                foreach (var deletedCaseloadItem in deletedCaseload)
                    realm.Remove(deletedCaseloadItem);

                realm.Add(caseloadContent, update: true);
            });

            ResultCode = Result.Successful;
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
                item.EntityType == IcmEntity.Case 
                || item.EntityType == IcmEntity.Incident
            );
        }

        static string CaseloadSelector(CaseloadItem caseloadItem) => caseloadItem.CaseIncidentNumber;
    }
}
