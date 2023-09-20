using Visitz.Models;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;

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

        protected override async Task RunAsync()
        {
            await GetCaseloadAsync();
        }

        private async Task GetCaseloadAsync()
        {
            var caseloadFromApi = await Vpi.GetCaseloadAsync(Idir);
            var caseloadContent = CaseloadItem.FromApiEntities(caseloadFromApi);

            caseloadContent = FilterNonCasesAndIncidents(caseloadContent);

            using var realm = await VisitzRealm.GetIcmDataAsync();
            await realm.WriteAsync(() =>
            {
                // Remove everything from the local caseload so we can remove
                // items if they've been unassigned from the user.
                realm.RemoveAll<CaseloadItem>();

                realm.Add(caseloadContent);
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
    }
}
