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
        public string dateOfBirth { get; set; }
        public string sex { get; set; }
        public string relationship { get; set; }
        public string personIdICM { get; set; }
        public string aboriginalOrigin { get; set; }
        public string livingCommunityBand { get; set; }
        public string email { get; set; }
        public string homePhone { get; set; }
        public string cellPhone { get; set; }
        public string contactUnitNo { get; set; }
        public string contactAddressLine1 { get; set; }
        public string contactAddressLine2 { get; set; }
        public string contactCity { get; set; }
        public string contactPostalCode { get; set; }
        public string contactProvinceState { get; set; }
        public string contactCountry { get; set; }
    }

    public class ListCaseIncident
    {
        public PayLoad payLoad { get; set; }
    }

    public class ListCaseIncident2
    {
        public string DisplayName => familyMembers.Where(mem => mem.keyPlayer.Equals("Y")).FirstOrDefault().lastName;

        public string Address => formatAddress();

        public string caseIncidentNumber { get; set; }
        public string entityType { get; set; }
        public List<FamilyMember> familyMembers { get; set; }
        public string createdDate { get; set; }
        public string caseIncidentType { get; set; }
        public string dateReported { get; set; }
        public string unitNo { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string provinceState { get; set; }
        public string country { get; set; }
        public string serviceOffice { get; set; }

        private string formatAddress()
        {
            var address = unitNo + addressLine1 + addressLine2
                + city + postalCode + provinceState + country;
            if (address.Length == 0)
            {
                return "NA";
            }
            else
            {
                return (unitNo.Length > 0 ? unitNo : "N/A") + ", " + (addressLine1.Length > 0 ? addressLine1 : "N/A") +
                    ", " + (addressLine2.Length > 0 ? addressLine2 : "N/A") + ", " + (city.Length > 0 ? city : "N/A") +
                    ", " + (postalCode.Length > 0 ? postalCode : "N/A") + ", " + (provinceState.Length > 0 ? provinceState : "N/A") +
                    ", " + (country.Length > 0 ? country : "N/A"); // Refactor this
            }
        }
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

