using hestiapi.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace hestiapi.Requests
{
    internal class GetCaseloadEndpoint : HestiaBaseEndpoint<IEnumerable<CaseloadEntity>>
    {
        private static readonly string CaseloadPath = "/v1/620b";

        private static readonly string WorkerIdsListKey = "workerIds";
        private static readonly string WorkerIdKey = "workerId";

        private static readonly string GetListCaseIncidentKey = "getListCaseIncident";

        private static readonly string ListCaseIncidentKey = "listCaseIncident";
        private static readonly string ListCaseIncidentsListKey = "listCaseIncidents";

        private readonly string[] _workerIds;

        private JsonArray WorkerIds
        {
            get
            {
                var ids = _workerIds.Select(id => new JsonObject { [WorkerIdKey] = id });
                return new JsonArray(ids.ToArray());
            }
        }

        private string RequestPayload
        {
            get
            {
                return new JsonObject
                {
                    [GetListCaseIncidentKey] = new JsonObject
                    {
                        [JsonKey.Payload] = new JsonObject
                        {
                            [WorkerIdsListKey] = WorkerIds
                        }
                    }
                }.ToString();
            }
        }

        public GetCaseloadEndpoint(string baseUrl, params string[] workerIds) : base(baseUrl, CaseloadPath)
        {
            _workerIds = workerIds;
        }

        public override HttpRequestMessage MakeRequest()
        {
            return new HttpRequestMessage()
            {
                Content = new FormUrlEncodedContent(FormDataCollection(JsonKey.DocRequest, RequestPayload)),
                Method = HttpMethod.Post,
                RequestUri = RequestUri
            };
        }

        public override IEnumerable<CaseloadEntity> HandleResponse(HttpResponseMessage response)
        {
            var content = response.Content.ReadAsStringAsync().Result;

            var caseloadJson = JsonDocument.Parse(content)
                .RootElement
                .GetProperty(ListCaseIncidentKey)
                .GetProperty(JsonKey.Payload)
                .GetProperty(ListCaseIncidentsListKey);

            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

            var caseloadList = caseloadJson
                .EnumerateArray()
                .Select(jsonItem => jsonItem.Deserialize(typeof(CaseloadEntity), options))
                .Cast<CaseloadEntity>();

            return caseloadList;
        }
    }
}
