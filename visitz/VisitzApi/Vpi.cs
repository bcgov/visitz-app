using VisitzApi.Models;
using VisitzApi.Models.Attachments;
using VisitzApi.Models.SafetyAssess;
using VisitzApi.Requests;

namespace VisitzApi
{
    /// <summary>
    /// VPI - Visitz (A)PI - convenience wrapper class for interaction with Visitz' API endpoints.
    /// </summary>
    public class Vpi(HttpClient httpClient, string baseVisitzApiUrl)
    {
        internal static readonly string V1 = "v1";
        internal static readonly string V2 = "v2";

        private HttpClient HttpClient { get; } = httpClient;
        private string BaseVisitzApiUrl { get; } = baseVisitzApiUrl;

        private async Task<T> CallApi<T>(VisitzBaseEndpoint<T> endpoint)
        {
            var response = await HttpClient.SendAsync(endpoint.MakeRequest());
            string content = await response.Content.ReadAsStringAsync();

            endpoint.ThrowOnHttpErrors(response, content);
            endpoint.ThrowOnWebMethodsErrors(response, content);

            return endpoint.HandleResponse(content);
        }

        public async Task<IEnumerable<CaseloadEntity>> GetCaseloadV1Async(params string[] workerIds)
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

        public async Task<(bool success, string status)> SubmitSafetyAssessmentAsync(SafetyAssessmentEntity safetyAssessment)
        {
            return await CallApi(new SubmitSafetyAssessmentEndpoint(BaseVisitzApiUrl, safetyAssessment));
        }

		public async Task<(bool success, string attachmentId)> SubmitAttachmentAsync(SubmitAttachmentEntity attachment)
		{
			return await CallApi(new SubmitAttachmentEndpoint(BaseVisitzApiUrl, attachment));
		}
    }
}
