using System;
using System.Linq;
using hestia.Models.DTOs;

namespace hestia.Models.BOs
{

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
                        contactId = fmObj?.contactId,
                        keyPlayer = fmObj?.keyPlayer,
                        lastName = fmObj?.lastName,
                        firstName = fmObj?.firstName,
                        middleName = fmObj?.middleName,
                        dateOfBirth = fmObj?.dateOfBirth,
                        sex = fmObj?.sex,
                        relationship = fmObj?.relationship,
                        personIdICM = fmObj?.personIdICM,
                        aboriginalOrigin = fmObj?.aboriginalOrigin,
                        livingCommunityBand = fmObj?.livingCommunityBand,
                        email = fmObj?.email,
                        homePhone = fmObj?.homePhone,
                        cellPhone = fmObj?.cellPhone,
                        contactUnitNo = fmObj?.contactUnitNo,
                        contactAddressLine1 = fmObj?.contactAddressLine1,
                        contactAddressLine2 = fmObj?.contactAddressLine2,
                        contactCity = fmObj?.contactCity,
                        contactPostalCode = fmObj?.contactPostalCode,
                        contactProvinceState = fmObj?.contactProvinceState,
                        contactCountry = fmObj?.contactCountry,
                    }).ToList();

                ListCaseIncident2 newObj = new()
                {
                    caseIncidentNumber = item?.caseIncidentNumber,
                    entityType = item?.entityType,
                    familyMembers = familyMembers,
                    createdDate = item?.createdDate,
                    caseIncidentType = item?.caseIncidentType,
                    dateReported = item?.dateReported,
                    unitNo = item?.unitNo,
                    addressLine1 = item?.addressLine1,
                    addressLine2 = item?.addressLine2,
                    city = item?.city,
                    postalCode = item?.postalCode,
                    provinceState = item?.provinceState,
                    country = item?.country,
                    serviceOffice = item?.serviceOffice
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

