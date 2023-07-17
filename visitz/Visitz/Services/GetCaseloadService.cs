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
            var caseloadContent = CaseloadItem.FromApiEntities(caseloadFromApi).Where(IsApprovedItem);

            using var realm = await VisitzRealm.GetIcmDataAsync();
            await realm.WriteAsync(() =>
            {
                // Remove everything from the local caseload so we can remove
                // items if they've been unassigned from the user.
                var allCaseloadQuery = realm.All<CaseloadItem>();
                realm.RemoveRange(allCaseloadQuery);

                realm.Add(caseloadContent);
            });

            ResultCode = Result.Successful;
        }

        private bool IsApprovedItem(CaseloadItem caseloadItem)
        {
            // This restriction is a business requirement. It may change in the future.
            return caseloadItem.EntityType == IcmEntity.Case
                || caseloadItem.EntityType == IcmEntity.Incident;
        }

        public override string GetId()
        {
            return MakeId();
        }
    }
}
