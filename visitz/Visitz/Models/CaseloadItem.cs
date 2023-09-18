using Realms;
using VisitzApi.Models;

namespace Visitz.Models
{
    public partial class CaseloadItem : IRealmObject
    {
        [PrimaryKey]
        public string CaseIncidentNumber { get; set; }

        public string EntityType { get; set; }
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
        public IList<FamilyMember> FamilyMembers { get; }
        public string KeyPlayerCellPhone { get; set; }
        public string KeyPlayerEmail { get; set; }
        public string KeyPlayerHomePhone { get; set; }
        public string CreatedDate { get; set; }
        public string DateReported { get; set; }
        public string MemoUrgent { get; set; }
        public string MemoCallDate { get; set; }
        public string MemoCallTime { get; set; }
        public string MemoRecordedBy { get; set; }

        public string DisplayDate
        {
            get
            {
                if (EntityType == IcmEntity.Incident)
                    return DateReported;
                else if (EntityType == IcmEntity.Memo)
                    return MemoCallDate;
                else // IcmEntity.Case, etc...
                    return CreatedDate;
            }
        }

        public string DisplayName
        {
            get
            {
                if (EntityType == IcmEntity.Memo)
                    return WorkerFullName;
                else if (TryGetKeyPlayer(out FamilyMember keyPlayer))
                    return $"{keyPlayer.LastName}, {keyPlayer.FirstName}";
                else
                    return ServiceOffice;
            }
        }

        public bool TryGetKeyPlayer(out FamilyMember keyPlayer)
        {
            keyPlayer = FamilyMembers?.Where(mem => mem.IsKeyPlayer).FirstOrDefault();
            return keyPlayer != null;
        }

        public static DateTime DisplayDateTransform(CaseloadItem caseloadItem)
        {
            return caseloadItem.DisplayDate?.Length > 0
                ? DateTime.Parse(caseloadItem.DisplayDate)
                : DateTime.MinValue;
        }

        // Copied from previous implementation. TODO: review if this is required, and clean up if so
        public string Address
        {
            get
            {
                var address = UnitNo + AddressLine1 + AddressLine2
                + City + PostalCode + ProvinceState + Country;

                return address.Length == 0
                    ? "NA"
                    : (UnitNo.Length > 0 ? UnitNo : "N/A") + ", " + (AddressLine1.Length > 0 ? AddressLine1 : "N/A") +
                        ", " + (AddressLine2.Length > 0 ? AddressLine2 : "N/A") + ", " + (City.Length > 0 ? City : "N/A") +
                        ", " + (PostalCode.Length > 0 ? PostalCode : "N/A") + ", " + (ProvinceState.Length > 0 ? ProvinceState : "N/A") +
                        ", " + (Country.Length > 0 ? Country : "N/A");
            }
        }

        public static CaseloadItem FromApiEntity(CaseloadEntity caseloadEntity)
        {
            var caseloadItem = new CaseloadItem()
            {
                EntityType = caseloadEntity.EntityType,
                CaseIncidentNumber = caseloadEntity.CaseIncidentNumber,
                CaseIncidentType = caseloadEntity.CaseIncidentType,
                WorkerId = caseloadEntity.WorkerId,
                WorkerFullName = caseloadEntity.WorkerFullName,
                ServiceOffice = caseloadEntity.ServiceOffice,
                OfficeCode = caseloadEntity.OfficeCode,
                SafetyAssessmentExist = caseloadEntity.SafetyAssessmentExist,
                UnitNo = caseloadEntity.UnitNo,
                AddressLine1 = caseloadEntity.AddressLine1,
                AddressLine2 = caseloadEntity.AddressLine2,
                City = caseloadEntity.City,
                PostalCode = caseloadEntity.PostalCode,
                ProvinceState = caseloadEntity.ProvinceState,
                Country = caseloadEntity.Country,
                KeyPlayerCellPhone = caseloadEntity.KeyPlayerCellPhone,
                KeyPlayerEmail = caseloadEntity.KeyPlayerEmail,
                KeyPlayerHomePhone = caseloadEntity.KeyPlayerHomePhone,
                CreatedDate = caseloadEntity.CreatedDate,
                DateReported = caseloadEntity.DateReported,
                MemoUrgent = caseloadEntity.MemoUrgent,
                MemoCallDate = caseloadEntity.MemoCallDate,
                MemoCallTime = caseloadEntity.MemoCallTime,
                MemoRecordedBy = caseloadEntity.MemoRecordedBy,
            };

            if (caseloadEntity.FamilyMembers != null)
            {
                var family = FamilyMember.FromApiEntities(caseloadEntity.FamilyMembers);

                foreach (var familyMember in family)
                    caseloadItem.FamilyMembers.Add(familyMember);
            }

            return caseloadItem;
        }

        public static IEnumerable<CaseloadItem> FromApiEntities(IEnumerable<CaseloadEntity> caseloadEntities)
        {
            return caseloadEntities.Select(FromApiEntity);
        }
    }
}
