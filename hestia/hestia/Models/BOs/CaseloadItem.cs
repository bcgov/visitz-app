using hestiapi.Models;

namespace hestia.Models.BOs
{
    public class CaseloadItem
    {
        public string EntityType { get; set; }
        public string CaseIncidentNumber { get; set; }
        public string CaseIncidentType { get; set; }
        public string WorkerId { get; set; }
        public string WorkerFullName { get; set; }
        public string ServiceOffice { get; set; }
        public string OfficeCode { get; set; }
        public string SafetyAssessmentExist { get; set; }
        public string UnitNo { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string ProvinceState { get; set; }
        public string Country { get; set; }
        public List<FamilyMember> FamilyMembers { get; set; }
        public string KeyPlayerCellPhone { get; set; }
        public string KeyPlayerEmail { get; set; }
        public string KeyPlayerHomePhone { get; set; }
        public string CreatedDate { get; set; }
        public string DateReported { get; set; }

        // Copied from previous implementation. TODO: review if this is required, and clean up if so
        public string DisplayName => FamilyMembers.Where(mem => mem.KeyPlayer.Equals("Y")).FirstOrDefault().LastName;

        // Copied from previous implementation. TODO: review if this is required, and clean up if so
        public string Address
        {
            get
            {
                var address = UnitNo + AddressLine1 + AddressLine2
                + City + PostalCode + ProvinceState + Country;
                if (address.Length == 0)
                {
                    return "NA";
                }
                else
                {
                    return (UnitNo.Length > 0 ? UnitNo : "N/A") + ", " + (AddressLine1.Length > 0 ? AddressLine1 : "N/A") +
                        ", " + (AddressLine2.Length > 0 ? AddressLine2 : "N/A") + ", " + (City.Length > 0 ? City : "N/A") +
                        ", " + (PostalCode.Length > 0 ? PostalCode : "N/A") + ", " + (ProvinceState.Length > 0 ? ProvinceState : "N/A") +
                        ", " + (Country.Length > 0 ? Country : "N/A");
                }
            }
        }

        public CaseloadItem(CaseloadEntity caseloadEntity)
        {
            EntityType = caseloadEntity.EntityType;
            CaseIncidentNumber = caseloadEntity.CaseIncidentNumber;
            CaseIncidentType = caseloadEntity.CaseIncidentType;
            WorkerId = caseloadEntity.WorkerId;
            WorkerFullName = caseloadEntity.WorkerFullName;
            ServiceOffice = caseloadEntity.ServiceOffice;
            OfficeCode = caseloadEntity.OfficeCode;
            SafetyAssessmentExist = caseloadEntity.SafetyAssessmentExist;
            UnitNo = caseloadEntity.UnitNo;
            AddressLine1 = caseloadEntity.AddressLine1;
            AddressLine2 = caseloadEntity.AddressLine2;
            City = caseloadEntity.City;
            PostalCode = caseloadEntity.PostalCode;
            ProvinceState = caseloadEntity.ProvinceState;
            Country = caseloadEntity.Country;
            FamilyMembers = caseloadEntity.FamilyMembers
                .ConvertAll(entity => new FamilyMember(entity));
            KeyPlayerCellPhone = caseloadEntity.KeyPlayerCellPhone;
            KeyPlayerEmail = caseloadEntity.KeyPlayerEmail;
            KeyPlayerHomePhone = caseloadEntity.KeyPlayerHomePhone;
            CreatedDate = caseloadEntity.CreatedDate;
            DateReported = caseloadEntity.DateReported;
        }
    }
}
