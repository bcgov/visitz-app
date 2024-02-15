using System.Text;
using Visitz.Services.Messages;
using VisitzApi;

namespace Visitz.Services
{
    public class GetNotesForRangeService(Vpi vpi, ServiceHandler serviceHandler) : VisitzApiService(vpi)
    {
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
            List<string> successIds = [];
            List<string> erroredIds = [];

            /*
             * TODO: Improve efficiency of this operation.
             * 
             * Previously, GetNotesService was run concurrently but after crashing issues it has been converted to run
             * sequentially instead.
             * 
             * Maybe run network requests concurrently, then Realm I/O sequentially as Tasks complete?
             */
            foreach (var (id, entityType) in IdEntityItems)
            {
                try
                {
                    await ServiceHandler.TryRunServiceAsync(GetNotesService.MakeStartMessage(id, entityType));
                    successIds.Add(id);
                }
                catch
                {
                    erroredIds.Add(id);
                }
            }

            ResultCode = erroredIds.Count <= 0 
                ? Result.Successful 
                : throw new PartialErrorException(successIds, erroredIds);
        }
    }

    public class PartialErrorException(List<string> successIds, List<string> errorIds) 
        : Exception(MakeMessage(errorIds))
    {
        public List<string> SuccessIds { get; set; } = successIds;

        public List<string> ErrorIds { get; set; } = errorIds;

        public static string MakeMessage(List<string> errorIds)
        {
            StringBuilder sb = new($"{nameof(GetNotesForRangeService)} error for IDs:\n\n");

            foreach (var id in errorIds.Order())
                sb.AppendLine($"\t{id}");

            return sb.ToString();
        }
    }
}
