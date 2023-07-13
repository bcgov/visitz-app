using VisitzApi.Models;
using VisitzApi.Requests;
using System.Net;
using VisitzApi.ErrorHandling;

namespace VisitzApi
{
    /// <summary>
    /// VPI - Visitz (A)PI - convenience wrapper class for interaction with Visitz' API endpoints.
    /// </summary>
    public class Vpi
    {
        private HttpClient HttpClient { get; }
        private string BaseVisitzApiUrl { get; }

        public Vpi(HttpClient httpClient, string baseVisitzApiUrl)
        {
            HttpClient = httpClient;
            BaseVisitzApiUrl = baseVisitzApiUrl;
        }

        private async Task<T> CallApi<T>(VisitzBaseEndpoint<T> endpoint)
        {
            var response = await HttpClient.SendAsync(endpoint.MakeRequest());
            string content = await response.Content.ReadAsStringAsync();

            VisitzApiException.ThrowIfInvalid(response, content);

            return endpoint.HandleResponse(content);
        }

        public async Task<IEnumerable<CaseloadEntity>> GetCaseloadAsync(params string[] workerIds)
        {
            return await CallApi(new GetCaseloadEndpoint(BaseVisitzApiUrl, workerIds));
        }

        public async Task<IEnumerable<NoteEntity>> GetNotesAsync(string entityNumber, string entityType)
        {
            return await CallApi(new NotesEndpoint(BaseVisitzApiUrl, entityNumber, entityType));
        }

        public async Task<(bool success, string noteId)> SubmitNotesAsync(SubmitNoteEntity noteToSubmit)
        {
            return await CallApi(new SubmitNotesEndpoint(BaseVisitzApiUrl, noteToSubmit));
        }
    }
}