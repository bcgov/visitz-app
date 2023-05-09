using hestiapi.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace hestiapi.Requests
{
    internal class GetCaseloadEndpoint : HestiaBaseEndpoint<IEnumerable<CaseloadBaseEntity>>
    {
        private static readonly string CaseloadPath = "/v1/620b";

        private static readonly string WorkerIdsListKey = "workerIds";
        private static readonly string WorkerIdKey = "workerId";

        private static readonly string GetListCaseIncidentKey = "getListCaseIncident";

        private static readonly string ListCaseIncidentKey = "listCaseIncident";
        private static readonly string ListCaseIncidentsListKey = "listCaseIncidents";

        private readonly string[] _workerIds;

        private string RequestPayload
        {
            get
            {
                return new JsonObject
                {
                    [GetListCaseIncidentKey] = new JsonObject
                    {
                        [PayloadKey] = new JsonObject
                        {
                            [WorkerIdsListKey] = new JsonArray
                            {
                                _workerIds.Select(id => new JsonObject
                                {
                                    [WorkerIdKey] = id
                                })
                            }
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
                Content = new FormUrlEncodedContent(FormDataCollection(DocRequestKey, RequestPayload)),
                Method = HttpMethod.Post,
                RequestUri = RequestUri
            };
        }

        public override IEnumerable<CaseloadBaseEntity> HandleResponse(HttpResponseMessage response)
        {
            var content = response.Content.ReadAsStringAsync().Result;

            var caseloadJson = JsonDocument.Parse(content)
                .RootElement
                .GetProperty(ListCaseIncidentKey)
                .GetProperty(PayloadKey)
                .GetProperty(ListCaseIncidentsListKey);

            var caseloadList = caseloadJson.Deserialize(typeof(List<CaseloadBaseEntity>));

            return (IEnumerable<CaseloadBaseEntity>)caseloadList;
        }
    }
}
