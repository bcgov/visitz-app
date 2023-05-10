using System;
using System.Linq;

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
                        ContactId = fmObj?.contactId,
                        KeyPlayer = fmObj?.keyPlayer,
                        LastName = fmObj?.lastName,
                        FirstName = fmObj?.firstName,
                        MiddleName = fmObj?.middleName,
                        DateOfBirth = fmObj?.dateOfBirth,
                        Sex = fmObj?.sex,
                        Relationship = fmObj?.relationship,
                        PersonIdICM = fmObj?.personIdICM,
                        AboriginalOrigin = fmObj?.aboriginalOrigin,
                        LivingCommunityBand = fmObj?.livingCommunityBand,
                        Email = fmObj?.email,
                        HomePhone = fmObj?.homePhone,
                        CellPhone = fmObj?.cellPhone,
                        ContactUnitNo = fmObj?.contactUnitNo,
                        ContactAddressLine1 = fmObj?.contactAddressLine1,
                        ContactAddressLine2 = fmObj?.contactAddressLine2,
                        ContactCity = fmObj?.contactCity,
                        ContactPostalCode = fmObj?.contactPostalCode,
                        ContactProvinceState = fmObj?.contactProvinceState,
                        ContactCountry = fmObj?.contactCountry,
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

