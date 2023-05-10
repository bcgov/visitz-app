using hestiapi.Models;

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

        public FamilyMember(FamilyMemberEntity familyMember)
        {
            contactId = familyMember.ContactId;
            keyPlayer = familyMember.KeyPlayer;
            lastName = familyMember.LastName;
            firstName = familyMember.FirstName;
            middleName = familyMember.MiddleName;

            // TODO: Properly handle DateTime in FamilyMember object
            dateOfBirth = familyMember.DateOfBirth.ToShortDateString();

            sex = familyMember.Sex;
            relationship = familyMember.Relationship;
            personIdICM = familyMember.PersonIdICM;
            aboriginalOrigin = familyMember.AboriginalOrigin;
            livingCommunityBand = familyMember.LivingCommunityBand;
            email = familyMember.Email;
            homePhone = familyMember.HomePhone;
            cellPhone = familyMember.CellPhone;
            contactUnitNo = familyMember.ContactUnitNo;
            contactAddressLine1 = familyMember.ContactAddressLine1;
            contactAddressLine2 = familyMember.ContactAddressLine2;
            contactCity = familyMember.ContactCity;
            contactPostalCode = familyMember.ContactPostalCode;
            contactProvinceState = familyMember.ContactProvinceState;
            contactCountry = familyMember.ContactCountry;
        }
    }
}

