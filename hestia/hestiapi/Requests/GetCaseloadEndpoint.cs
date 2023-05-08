using System.Text.Json.Nodes;

namespace hestiapi.Requests
{
    internal class GetCaseloadEndpoint : HestiaBaseEndpoint
    {
        private static readonly string CaseloadPath = "/v1/620b";

        private static readonly string WorkerIdsListKey = "workerIds";
        private static readonly string WorkerIdKey = "workerId";

        private static readonly string GetListCaseIncidentKey = "getListCaseIncident";

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

        public override HestiaBaseEndpoint HandleResponse()
        {
            throw new NotImplementedException();
        }
    }
}
