using visitzApi.Models;
using visitzApi.Requests;
using System.Net;

namespace visitzApi
{
    public class VisitzApi
    {
        private HttpClient HttpClient { get; }
        private string BaseVisitzApiUrl { get; }

        public VisitzApi(HttpClient httpClient, string baseVisitzApiUrl)
        {
            HttpClient = httpClient;
            BaseVisitzApiUrl = baseVisitzApiUrl;
        }

        private async Task<T> CallApi<T>(VisitzBaseEndpoint<T> endpoint)
        {
            var response = await HttpClient.SendAsync(endpoint.MakeRequest());

            VisitzApiException.ThrowIfInvalid(response);

            return endpoint.HandleResponse(response);
        }

        public async Task<IEnumerable<CaseloadEntity>> GetCaseloadAsync(params string[] workerIds)
        {
            return await CallApi(new GetCaseloadEndpoint(BaseVisitzApiUrl, workerIds));
        }

        public async Task<IEnumerable<NoteEntity>> GetNotesAsync(string entityNumber, string entityType)
        {
            return await CallApi(new NotesEndpoint(BaseVisitzApiUrl, entityNumber, entityType));
        }

        public async Task<HttpStatusCode> SubmitNotesAsync(SubmitNoteEntity noteToSubmit)
        {
            return await CallApi(new SubmitNotesEndpoint(BaseVisitzApiUrl, noteToSubmit));
        }
    }
}