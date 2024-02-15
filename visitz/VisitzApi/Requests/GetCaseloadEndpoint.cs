using System.Text.Json;
using System.Text.Json.Nodes;
using VisitzApi.ErrorHandling;
using VisitzApi.Models;

namespace VisitzApi.Requests
{
    internal class GetCaseloadEndpoint : VisitzBaseEndpoint<IEnumerable<CaseloadEntity>>
    {
        private static readonly string CaseloadPath = "/v1/620b";

        private static readonly string WorkerIdsListKey = "workerIds";
        private static readonly string WorkerIdKey = "workerId";

        private static readonly string GetListCaseIncidentKey = "getListCaseIncident";

        private static readonly string ListCaseIncidentKey = "listCaseIncident";
        private static readonly string ListCaseIncidentsListKey = "listCaseIncidents";

        private static readonly string NoRecordsFoundError = "No records found";

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
                        [JsonKey.PayLoad] = new JsonObject
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

        public override void ThrowOnWebMethodsErrors(HttpResponseMessage response, string content)
        {
            if (WebMethodsJsonError.TryFindFirstError(content, out string errorMessage))
            {
                // Because of questionable API design, we're treating this "No records found" message as the equivalent
                // of an HTTP 200 empty reponse. Any other error message should trigger an exception.
                if (errorMessage != NoRecordsFoundError)
                    throw new VisitzApiException(response.StatusCode, errorMessage);
            }
        }

        public override IEnumerable<CaseloadEntity> HandleResponse(string responseContent)
        {
            if (WebMethodsJsonError.TryFindFirstError(responseContent, out string errorMessage))
                if (errorMessage == NoRecordsFoundError)
                    return new List<CaseloadEntity>();

            var caseloadJson = JsonDocument.Parse(responseContent)
                .RootElement
                .GetProperty(ListCaseIncidentKey)
                .GetProperty(JsonKey.PayLoad)
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
