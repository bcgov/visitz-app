using System.Text;
using Visitz.Services.Messages;
using VisitzApi;

namespace Visitz.Services
{
    public class GetNotesForRangeService(Vpi vpi, ServiceHandler serviceHandler) : VisitzApiService(vpi)
    {
		readonly List<string> successIds = [];
		readonly List<string> erroredIds = [];

		public static string MakeId()
        {
            return nameof(GetNotesForRangeService);
        }

        public static StartServiceMessage MakeStartMessage(IEnumerable<ValueTuple<string, string>> idEntityItems)
        {
            return new StartServiceMessage()
            {
                ServiceId = MakeId(),
                ServiceType = typeof(GetNotesForRangeService),
                Payload = idEntityItems,
            };
        }

        private ServiceHandler ServiceHandler { get; set; } = serviceHandler;

        private IEnumerable<ValueTuple<string,string>> IdEntityItems => 
            (IEnumerable<ValueTuple<string, string>>)Payload;

        public override string GetId()
        {
            return MakeId();
        }

        protected override async Task RunApiServiceAsync()
        {
            await GetAllNotesAsync();
        }

        private async Task GetAllNotesAsync()
        {
			await Parallel.ForEachAsync(IdEntityItems, GetNotesForRecord);

            ResultCode = erroredIds.Count <= 0 
                ? Result.Successful 
                : throw new PartialRangeErrorException(nameof(GetNotesForRangeService), successIds, erroredIds);
        }

		private async ValueTask GetNotesForRecord((string id, string entityType) tuple, CancellationToken token)
		{
			var (id, entityType) = tuple;

            try
            {
                await ServiceHandler.TryRunServiceAsync(GetNotesService.MakeStartMessage(id, entityType));
                successIds.Add(id);
            }
            catch (Exception ex)
            {
                erroredIds.Add(id + " -> " + ex.Message);
            }
		}
	}
}
