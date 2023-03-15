using System;
using System.Linq;
using hestia.Models.DTOs;

namespace hestia.Models.BOs
{
    public class FamilyMember
    {
        public string contactId { get; set; }
        public string keyPlayer { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
    }

    public class ListCaseIncident
    {
        public PayLoad payLoad { get; set; }
    }

    public class ListCaseIncident2
    {
        public string caseIncidentNumber { get; set; }
        public string entityType { get; set; }
        public string caseIncidentType { get; set; }
        public string workerId { get; set; }
        public string workerFullName { get; set; }
        public string unitNo { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string provinceState { get; set; }
        public string country { get; set; }
        public List<FamilyMember> familyMembers { get; set; }
        public string createdDate { get; set; }
    }

    public class PayLoad
    {
        public List<ListCaseIncident2> listCaseIncidents { get; set; }
    }

    /// <summary>
    /// The business object that would be used by the app source.
    /// </summary>
    public class CaseIncidentListBO
    {
        public ListCaseIncident listCaseIncident { get; set; }
        /*
        public static CaseIncidentListBO ToBO(CaseIncidentListDTO dto)
        {
            var listCaseIncidents = dto?.listCaseIncident?.payLoad?.listCaseIncidents;
            if (listCaseIncidents is null) return null;
            List<ListCaseIncident2> boListCaseIncidents = listCaseIncidents.Select(obj =>
            {
                ListCaseIncident2 newObj = new();
                if (!(obj.familyMembers is null))
                {
                    
                }

                return newObj;
            }).ToList();
        }
        */
    }
}

