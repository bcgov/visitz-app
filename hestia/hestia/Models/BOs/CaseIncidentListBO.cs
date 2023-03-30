using System;
using System.Linq;
using hestia.Models.DTOs;

namespace hestia.Models.BOs
{
    public class FamilyMember
    {
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
        public string DisplayName => familyMembers.Where(mem => mem.keyPlayer.Equals("Y")).FirstOrDefault().lastName;

        public string caseIncidentNumber { get; set; }
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

        public static CaseIncidentListBO ToBO(CaseIncidentListDTO dto)
        {
            var listCaseIncidents = dto?.listCaseIncident?.payLoad?.listCaseIncidents;
            var newListCaseIncidents = new List<ListCaseIncident2>();
            listCaseIncidents?.ForEach(item =>
            {
                List<FamilyMember> familyMembers = item?.familyMembers?.Select(fmObj =>
                    new FamilyMember()
                    {
                        keyPlayer = fmObj?.keyPlayer,
                        lastName = fmObj?.lastName,
                        firstName = fmObj?.firstName,
                        middleName = fmObj?.middleName
                    }).ToList();

                ListCaseIncident2 newObj = new()
                {
                    caseIncidentNumber = item?.caseIncidentNumber,
                    familyMembers = familyMembers,
                    createdDate = item?.createdDate
                };
                newListCaseIncidents.Add(newObj);
            });

            return new CaseIncidentListBO()
            {
                listCaseIncident = new ListCaseIncident()
                {
                    payLoad = new PayLoad()
                    {
                        listCaseIncidents = newListCaseIncidents
                    }
                }
            };
        }
    }
}

