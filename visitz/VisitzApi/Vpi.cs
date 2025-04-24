using VisitzApi.Endpoints;
using VisitzApi.Endpoints.SafetyAssess;
using VisitzApi.Endpoints.Visits;
using VisitzApi.Models;
using VisitzApi.Models.Attachments;
using VisitzApi.Models.Caseload;
using VisitzApi.Models.People;
using VisitzApi.Models.SafetyAssess;
using VisitzApi.Models.Visits;
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

        private bool IsV1Endpoint<T>(VisitzBaseEndpoint<T> endpoint)
        {
            return endpoint.RequestUrl.StartsWith(BaseVisitzApiUrl.Trim('/') + "/" + V1);
        }

        private async Task<T> CallApi<T>(VisitzBaseEndpoint<T> endpoint)
        {
            var response = await HttpClient.SendAsync(endpoint.MakeRequest());
            string content = await response.Content.ReadAsStringAsync();

            endpoint.ThrowOnHttpErrors(response, content);

            if (IsV1Endpoint(endpoint))
                endpoint.ThrowOnWebMethodsErrors(response, content);

            return endpoint.HandleResponse(response, content);
        }

        public async Task<IEnumerable<CaseloadEntity>> GetCaseloadV1Async(params string[] workerIds)
        {
            return await CallApi(new Requests.GetCaseloadEndpoint(BaseVisitzApiUrl, workerIds));
        }

        public async Task<CaseloadJson> GetCaseloadV2Async(DateTimeOffset? after = null)
        {
            return await CallApi(new Endpoints.GetCaseloadEndpoint(BaseVisitzApiUrl, after));
        }

        public async Task<IEnumerable<NoteEntity>> GetNotesAsync(string entityNumber, string entityType)
        {
            return await CallApi(new NotesEndpoint(BaseVisitzApiUrl, entityNumber, entityType));
        }

        public async Task<(bool success, string noteId)> SubmitNotesAsync(SubmitNoteEntity noteToSubmit)
        {
            return await CallApi(new SubmitNotesEndpoint(BaseVisitzApiUrl, noteToSubmit));
        }

        public async Task<(bool success, string status)> SubmitSafetyAssessmentAsync(SubmitSafetyAssessmentJson safetyAssessment)
        {
            return await CallApi(new SubmitSafetyAssessmentEndpoint(BaseVisitzApiUrl, safetyAssessment));
        }

        public async Task<(bool success, string attachmentId)> SubmitAttachmentAsync(SubmitAttachmentEntity attachment)
        {
            return await CallApi(new SubmitAttachmentEndpoint(BaseVisitzApiUrl, attachment));
        }

        public async Task<(int TotalRecords, IEnumerable<VisitJson>)> GetVisitsAsync(
            string caseId,
            Pagination pagination = null)
        {
            return await CallApi(new GetVisitsEndpoint(BaseVisitzApiUrl, caseId, pagination));
        }

        public async Task<bool> PostVisitAsync(string caseId, PostVisitJson visitJsonToSend)
        {
            return await CallApi(new PostVisitEndpoint(BaseVisitzApiUrl, caseId, visitJsonToSend));
        }

        public async Task<(int TotalRecords, IEnumerable<ContactJson>)> GetContactsAsync(
            ApiRecordType type,
            string id,
            Pagination pagination = null)
        {
            return await CallApi(new GetContactsEndpoint(BaseVisitzApiUrl, type, id, pagination));
        }

        public async Task<(int TotalRecords, IEnumerable<SupportNetworkJson>)>
            GetSupportNetworkAsync(
            ApiRecordType type,
            string id,
            Pagination pagination = null)
        {
            return await CallApi(new GetSupportNetworkEndpoint(BaseVisitzApiUrl, type, id, pagination));
        }

        public async Task<(int TotalRecords, IEnumerable<AttachmentJson>)> GetAttachmentsAsync(
            ApiRecordType type,
            string id,
            Pagination pagination = null)
        {
            return await CallApi(new GetAttachmentsEndpoint(BaseVisitzApiUrl, type, id, pagination));
        }

        public async Task<AttachmentJson> GetAttachmentDetailsAsync(
            ApiRecordType type,
            string recordId,
            string attachmentId,
            DateTimeOffset? after = null)
        {
            return await CallApi(new GetAttachmentDetailsEndpoint(BaseVisitzApiUrl, type, recordId, attachmentId, after));
        }

        public async Task<(int TotalRecords, IEnumerable<SafetyAsessmentJson>)> GetSafetyAssessments(
            string incidentId,
            Pagination pagination = null)
        {
            return await CallApi(new GetSafetyAssessmentsEndpoint(BaseVisitzApiUrl, incidentId, pagination));
        }
    }
}
