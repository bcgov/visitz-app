using hestiapi.Models;
using hestiapi.Requests;
using System.Net;

namespace hestiapi
{
    public class HestiApi
    {
        private HttpClient HttpClient { get; }
        private string BaseHestiApiUrl { get; }

        public HestiApi(HttpClient httpClient, string baseHestiApiUrl)
        {
            HttpClient = httpClient;
            BaseHestiApiUrl = baseHestiApiUrl;
        }

        private async Task<T> CallApi<T>(HestiaBaseEndpoint<T> endpoint)
        {
            var response = await HttpClient.SendAsync(endpoint.MakeRequest());

            HestiaApiException.ThrowIfInvalid(response);

            return endpoint.HandleResponse(response);
        }

        public async Task<IEnumerable<CaseloadEntity>> GetCaseloadAsync(params string[] workerIds)
        {
            return await CallApi(new GetCaseloadEndpoint(BaseHestiApiUrl, workerIds));
        }

        public async Task<IEnumerable<NoteEntity>> GetNotesAsync(string entityNumber, string entityType)
        {
            return await CallApi(new NotesEndpoint(BaseHestiApiUrl, entityNumber, entityType));
        }

        public async Task<HttpStatusCode> SubmitNotesAsync(SubmitNoteEntity noteToSubmit)
        {
            return await CallApi(new SubmitNotesEndpoint(BaseHestiApiUrl, noteToSubmit));
        }
    }
}