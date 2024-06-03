using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Services
{
    public class GetCaseloadService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi)
    {
		LastUpdatedPrefs LastUpdated { get; set; } = prefs;

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
            await GetCaseloadAsync();
        }

        private async Task GetCaseloadAsync()
        {
            var caseloadFromApi = await Vpi.GetCaseloadAsync(Idir);
            var caseloadContent = CaseloadItem.FromApiEntities(caseloadFromApi);

            caseloadContent = FilterNonCasesAndIncidents(caseloadContent);

            using var realm = await VisitzRealms.GetIcmDataRealmAsync();
            await CaseloadItem.ReplaceCaseloadWithAsync(realm, caseloadContent);

            ResultCode = Result.Successful;
			await MainThread.InvokeOnMainThreadAsync(() => LastUpdated.Set(GetId(), DateTimeExtensions.LocalNow));
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
