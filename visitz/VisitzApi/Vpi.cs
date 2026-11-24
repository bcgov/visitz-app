using Microsoft.Extensions.Logging;
using VisitzApi.Endpoints;
using VisitzApi.Endpoints.AppLogs;
using VisitzApi.Endpoints.Attachments;
using VisitzApi.Endpoints.CallDetails;
using VisitzApi.Endpoints.Caseload;
using VisitzApi.Endpoints.Notes;
using VisitzApi.Endpoints.People;
using VisitzApi.Endpoints.SafetyAssess;
using VisitzApi.Endpoints.Visits;
using VisitzApi.ErrorHandling;
using VisitzApi.Models.AppLogs;
using VisitzApi.Models.Attachments;
using VisitzApi.Models.CallDetails;
using VisitzApi.Models.Caseload;
using VisitzApi.Models.ListOfValues;
using VisitzApi.Models.Notes;
using VisitzApi.Models.People;
using VisitzApi.Models.SafetyAssess;
using VisitzApi.Models.Visits;
using VisitzApi.Requests;

namespace VisitzApi;

/// <summary>
/// VPI - Visitz (A)PI - convenience wrapper class for interaction with Visitz' API endpoints.
/// </summary>
public partial class Vpi(HttpClient httpClient, string baseVisitzApiUrl, ILogger<Vpi> logger)
{
    internal static readonly string EmptyJsonBody = "{}";
    internal static readonly string V1 = "v1";
    internal static readonly string V2 = "v2";

    private HttpClient HttpClient { get; } = httpClient;
    private string BaseVisitzApiUrl { get; } = baseVisitzApiUrl;

    private bool IsV1Endpoint<T>(VisitzBaseEndpoint<T> endpoint)
    {
        return endpoint.RequestUrl.StartsWith(BaseVisitzApiUrl.Trim('/') + "/" + V1);
    }

    private bool IsV2WorkflowEndpoint<T>(VisitzBaseEndpoint<T> endpoint)
    {
        return endpoint.RequestUrl.StartsWith(BaseVisitzApiUrl.Trim('/') + $"/{V2}/wf");
    }

    private async Task<T> CallApi<T>(VisitzBaseEndpoint<T> endpoint)
    {
        var request = endpoint.MakeRequest();

        if (logger.IsEnabled(LogLevel.Debug))
            await DebugLogRequest(endpoint, request);

        request.RequestUri ??= endpoint.RequestUri;
        request.VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;

        var response = await HttpClient.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();

        if (content.Trim().Length <= 0)
            content = EmptyJsonBody;

        using ResponseBodyParser bodyParser = new(content);

        endpoint.ThrowOnHttpErrors(response, bodyParser);

        if (IsV2WorkflowEndpoint(endpoint) || IsV1Endpoint(endpoint))
            endpoint.ThrowOnErrorsInBody(response, bodyParser);

        return endpoint.HandleResponse(response, content);
    }

    private async Task DebugLogRequest<T>(VisitzBaseEndpoint<T> endpoint, HttpRequestMessage request)
    {
        string body = request.Content == null ? "<null>" : await request.Content.ReadAsStringAsync();
        LogRequest(request.Method, endpoint.GetType().Name, body);
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "{method} | {endpointName} | Body: {body}")]
    private partial void LogRequest(HttpMethod method, string endpointName, string body);

    public async Task<(int TotalRecords, CaseloadJson)> GetCaseloadAsync(Pagination pagination)
    {
        return await CallApi(new GetCaseloadEndpoint(BaseVisitzApiUrl, pagination));
    }

    public async Task<(int TotalRecords, OfficeCaseloadJson)> GetOfficeCaseloadAsync(Pagination? pagination = null)
    {
        return await CallApi(new GetOfficeCaseloadEndpoint(BaseVisitzApiUrl, pagination));
    }

    public async Task<(int TotalRecords, IEnumerable<CaseNoteJson>)> GetCaseNotesAsync(
        string entityNumber,
        Pagination? pagination = null
    )
    {
        return await CallApi(new GetCaseNotesEndpoint(BaseVisitzApiUrl, entityNumber, pagination));
    }

    public async Task<(int TotalRecords, IEnumerable<ResponseNarrativeJson>)> GetResponseNarrativesAsync(
        ApiRecordType type,
        string rowId,
        Pagination? pagination = null
    )
    {
        return await CallApi(new GetResponseNarrativeEndpoint(BaseVisitzApiUrl, type, rowId, pagination));
    }

    public async Task<(bool success, string noteId)> SubmitNotesAsync(SubmitNoteEntity noteToSubmit)
    {
        return await CallApi(new SubmitNotesEndpoint(BaseVisitzApiUrl, noteToSubmit));
    }

    public async Task<(bool success, string status)> SubmitSafetyAssessmentAsync(
        SubmitSafetyAssessmentJson safetyAssessment
    )
    {
        return await CallApi(new SubmitSafetyAssessmentEndpoint(BaseVisitzApiUrl, safetyAssessment));
    }

    public async Task<(int TotalRecords, IEnumerable<VisitJson>)> GetVisitsAsync(
        string caseId,
        Pagination? pagination = null
    )
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
        Pagination? pagination = null
    )
    {
        return await CallApi(new GetContactsEndpoint(BaseVisitzApiUrl, type, id, pagination));
    }

    public async Task<(int TotalRecords, IEnumerable<SupportNetworkJson>)> GetSupportNetworkAsync(
        ApiRecordType type,
        string id,
        Pagination? pagination = null
    )
    {
        return await CallApi(new GetSupportNetworkEndpoint(BaseVisitzApiUrl, type, id, pagination));
    }

    public async Task<bool> SubmitSupportNetworkItemAsync(
        ApiRecordType type,
        string id,
        SubmitSupportNetworkJson supportNetwork
    )
    {
        return await CallApi(new SubmitSupportNetworkEndpoint(BaseVisitzApiUrl, type, id, supportNetwork));
    }

    public async Task<(int TotalRecords, IEnumerable<AttachmentJson>)> GetAttachmentsAsync(
        ApiRecordType type,
        string id,
        Pagination? pagination = null
    )
    {
        return await CallApi(new GetAttachmentsEndpoint(BaseVisitzApiUrl, type, id, pagination));
    }

    public async Task<AttachmentJson?> GetAttachmentDetailsAsync(
        ApiRecordType type,
        string recordId,
        string attachmentId,
        DateTimeOffset? after = null
    )
    {
        return await CallApi(new GetAttachmentDetailsEndpoint(BaseVisitzApiUrl, type, recordId, attachmentId, after));
    }

    public async Task<(bool TotalCount, string AttachmentId)> SubmitAttachmentAsync(
        ApiRecordType type,
        string recordId,
        AttachmentFormData data
    )
    {
        return await CallApi(new PostAttachmentEndpoint(BaseVisitzApiUrl, type, recordId, data));
    }

    public async Task<(int TotalRecords, IEnumerable<GetSafetyAsessmentJson>)> GetSafetyAssessments(
        string incidentId,
        Pagination? pagination = null
    )
    {
        return await CallApi(new GetSafetyAssessmentsEndpoint(BaseVisitzApiUrl, incidentId, pagination));
    }

    public async Task<(int TotalRecords, IEnumerable<IncidentConcernsJson>)> GetIncidentConcerns(
        string incidentId,
        Pagination? pagination = null
    )
    {
        return await CallApi(new IncidentConcernsEndpoint(BaseVisitzApiUrl, incidentId, pagination));
    }

    public async Task<(int TotalRecords, IEnumerable<CallInformationJson>)> GetCallInformation(
        ApiRecordType type,
        string recordId,
        Pagination? pagination = null
    )
    {
        return await CallApi(new CallInformationEndpoint(BaseVisitzApiUrl, type, recordId, pagination));
    }

    public async Task<(int TotalRecords, IEnumerable<AdditionalInformationJson>)> GetAdditionalInformation(
        ApiRecordType type,
        string id,
        Pagination? pagination = null
    )
    {
        return await CallApi(new AdditionalInformationEndpoint(BaseVisitzApiUrl, type, id, pagination));
    }

    public async Task<(int TotalRecords, IEnumerable<GetContactMedicalBehavioralJson>)> GetContactMedicalBehavioral(
        ApiRecordType type,
        string recordId,
        string contactId,
        Pagination? pagination = null
    )
    {
        return await CallApi(
            new GetContactMedicalBehavioralEndpoint(BaseVisitzApiUrl, type, recordId, contactId, pagination)
        );
    }

    public async Task<bool> SubmitContactMedicalBehavioralAsync(
        ApiRecordType recordType,
        string parentRowId,
        string parentContactId,
        SubmitContactMedicalBehavioralJson submitJson
    )
    {
        return await CallApi(
            new SubmitContactMedicalBehavioralEndpoint(
                BaseVisitzApiUrl,
                recordType,
                parentRowId,
                parentContactId,
                submitJson
            )
        );
    }

    public async Task<(int TotalRecords, IEnumerable<GetContactLanguageJson>)> GetContactLanguageAsync(
        ApiRecordType type,
        string recordId,
        string contactId,
        Pagination? pagination = null
    )
    {
        return await CallApi(new GetContactLanguagesEndpoint(BaseVisitzApiUrl, type, recordId, contactId, pagination));
    }

    public async Task<bool> SubmitContactLanguageAsync(
        ApiRecordType recordType,
        string parentRowId,
        string parentContactId,
        SubmitContactLanguageJson submitJson
    )
    {
        return await CallApi(
            new SubmitContactLanguagesEndpoint(BaseVisitzApiUrl, recordType, parentRowId, parentContactId, submitJson)
        );
    }

    public async Task<(int TotalRecords, IEnumerable<GetContactLegalAuthorityJson>)> GetContactLegalAuthority(
        ApiRecordType type,
        string recordId,
        string contactId,
        Pagination? pagination = null
    )
    {
        return await CallApi(
            new GetContactLegalAuthorityEndpoint(BaseVisitzApiUrl, type, recordId, contactId, pagination)
        );
    }

    public async Task<(int TotalRecords, IEnumerable<GetContactEducationJson>)> GetContactEducation(
        ApiRecordType type,
        string recordId,
        string contactId,
        Pagination? pagination = null
    )
    {
        return await CallApi(new GetContactEducationEndpoint(BaseVisitzApiUrl, type, recordId, contactId, pagination));
    }

    public async Task<bool> SubmitContactEducationAsync(
        ApiRecordType recordType,
        string parentRowId,
        string parentContactId,
        SubmitContactEducationJson submitJson
    )
    {
        return await CallApi(
            new SubmitContactEducationEndpoint(BaseVisitzApiUrl, recordType, parentRowId, parentContactId, submitJson)
        );
    }

    public async Task<HttpResponseMessage> SendAppLogs(IList<AppLogJson> logs)
    {
        return await CallApi(new SendAppLogsEndpoint(BaseVisitzApiUrl, logs));
    }

    public async Task<(int TotalRecords, IEnumerable<ListedValueJson>)> GetListOfValuesAsync(
        string type,
        Pagination? pagination = null
    )
    {
        return await CallApi(new GetListOfValuesEndpoint(BaseVisitzApiUrl, type, pagination));
    }
}
