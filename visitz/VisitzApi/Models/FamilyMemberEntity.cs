namespace VisitzApi.Models
{
    public class FamilyMemberEntity
    {
        public string ContactId { get; set; }
        public string PersonIdICM { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        public string ContactUnitNo { get; set; }
        public string ContactAddressLine1 { get; set; }
        public string ContactAddressLine2 { get; set; }
        public string ContactCity { get; set; }
        public string ContactPostalCode { get; set; }
        public string ContactProvinceState { get; set; }
        public string ContactCountry { get; set; }
        public string KeyPlayer { get; set; }
        public string Relationship { get; set; }
        public string LegalStatus { get; set; }
        public string LegalStatusEffectiveDate { get; set; }
        public string LegalStatusExpiryDate { get; set; }
        public string AboriginalOrigin { get; set; }
        public string LivingCommunityBand { get; set; }
        public string SubjectFlag { get; set; }
        public string ParentCaregiver { get; set; }
        public string SubjectChild { get; set; }
        public string PersonResponsibleMaltreatment { get; set; }
    }
}
