using System;
namespace hestia.Models.Payloads
{
    public class GetListCaseIncident
    {
        public PayLoad payLoad { get; set; } = new();
    }

    public class PayLoad
    {
        public List<WorkerId> workerIds { get; set; } = new();
    }

    /// <summary>
    /// The payload object that would be used by the networking module during the API invocation.
    /// </summary>
    public class CaseIncidentListPayload
    {
        public GetListCaseIncident getListCaseIncident { get; set; } = new();
    }

    public class WorkerId
    {
        public string workerId { get; set; }

        public WorkerId(string workerId)
        {
            this.workerId = workerId;
        }
    }
}

