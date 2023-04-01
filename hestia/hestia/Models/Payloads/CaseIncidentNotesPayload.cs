using System;
namespace hestia.Models.Payloads
{
    public class RequestGetNotes
    {
        public PayLoad payLoad { get; set; } = new();

        public class PayLoad
        {
            public string entityNumber { get; set; } = "";
            public string entityType { get; set; } = "";
        }
    }

    /// <summary>
    /// The payload object that would be used by the networking module during the CaseIncidentNotes API invocation.
    /// </summary>
    public class CaseIncidentNotesPayload
    {
        public RequestGetNotes requestGetNotes { get; set; } = new();
    }
}

