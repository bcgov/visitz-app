using System.Net;
using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.ListOfValues;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints;

internal class GetListOfValuesEndpoint(string baseUrl, string type, Pagination? pagination = null)
    : VisitzBaseEndpoint<(int Count, IEnumerable<ListedValueJson> Items)>(baseUrl, Vpi.V2, ListOfValuesPath)
{
    static readonly string ListOfValuesPath = "/list-of-values";

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = WithQueryParams(pagination: pagination, @params: ("type", type)),
        };
    }

    public override (int Count, IEnumerable<ListedValueJson> Items) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return (-1, []);

        JsonElement items = JsonDocument.Parse(responseContent).RootElement.GetProperty("items");

        return (
            response.GetRecordCount(),
            items.Deserialize<IEnumerable<ListedValueJson>>(PayloadOptions.SiebelGet) ?? []
        );
    }
}
