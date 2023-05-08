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

        public async List<CaseloadBaseItem> GetCaseload(params string[] workerIds)
        {
            var caseload = new GetCaseloadEndpoint(BaseHestiApiUrl, workerIds);
            var response = await HttpClient.SendAsync(caseload.MakeRequest());
        }
    }
}