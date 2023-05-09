using System;

namespace hestia.Models.DTOs
{
    /// <summary>
    /// The data transfer object that would be used by the networking module to deserialize response.
    /// </summary>
    public class CaseIncidentListDTO
    {
        public ListCaseIncident listCaseIncident { get; set; }
    }
}

