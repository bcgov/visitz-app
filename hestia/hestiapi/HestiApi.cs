using hestiapi.Models;
using hestiapi.Requests;

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

        public async Task<IEnumerable<CaseloadBaseEntity>> GetCaseloadAsync(params string[] workerIds)
        {
            var caseload = new GetCaseloadEndpoint(BaseHestiApiUrl, workerIds);

            var response = await HttpClient.SendAsync(caseload.MakeRequest());
            
            return caseload.HandleResponse(response);
        }

        public async Task<IEnumerable<NoteEntity>> GetNotesAsync(string entityNumber, string entityType)
        {
            var notes = new NotesEndpoint(BaseHestiApiUrl, entityNumber, entityType);

            var response = await HttpClient.SendAsync(notes.MakeRequest());

            return notes.HandleResponse(response);
        }
    }
}