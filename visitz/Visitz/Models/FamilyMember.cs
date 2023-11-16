using Realms;
using Visitz.Extensions;
using VisitzApi.Models;

namespace Visitz.Models
{
    public partial class FamilyMember : IEmbeddedObject
    {
        public string ContactId { get; set; }
        public string KeyPlayer { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string DateOfBirth { get; set; }
        public string Sex { get; set; }
        public string Relationship { get; set; }
        public string PersonIdICM { get; set; }
        public string AboriginalOrigin { get; set; }
        public string LivingCommunityBand { get; set; }
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

        public string FullDisplayName => string.Join(" ",
            FirstName, MiddleName, LastName);

        public bool IsKeyPlayer => KeyPlayer == "Y";

        public string Address =>
            (ContactUnitNo.FormatAddressPart("-")
            + ContactAddressLine1.FormatAddressPart(" ")
            + ContactAddressLine2.FormatAddressPart(" ")
            + ContactCity.FormatAddressPart(", ")
            + ContactProvinceState.FormatAddressPart(", ")
            + ContactCountry.FormatAddressPart(", "))
            + ContactPostalCode.FormatAddressPart("")
            .TrimEnd([',', ' ', '-'])
            .TrimEnd([',', ' ', '-']);

        public int Age => (DateTime.Now - DateTime.Parse(DateOfBirth)).Days / 365;

        public static FamilyMember FromApiEntity(FamilyMemberEntity familyMember)
        {
            return new FamilyMember()
            {
                ContactId = familyMember.ContactId,
                KeyPlayer = familyMember.KeyPlayer,
                LastName = familyMember.LastName,
                FirstName = familyMember.FirstName,
                MiddleName = familyMember.MiddleName,

                // TODO: Properly handle DateTime in FamilyMember object
                DateOfBirth = familyMember.DateOfBirth,

                Sex = familyMember.Sex,
                Relationship = familyMember.Relationship,
                PersonIdICM = familyMember.PersonIdICM,
                AboriginalOrigin = familyMember.AboriginalOrigin,
                LivingCommunityBand = familyMember.LivingCommunityBand,
                Email = familyMember.Email,
                HomePhone = familyMember.HomePhone,
                CellPhone = familyMember.CellPhone,
                ContactUnitNo = familyMember.ContactUnitNo,
                ContactAddressLine1 = familyMember.ContactAddressLine1,
                ContactAddressLine2 = familyMember.ContactAddressLine2,
                ContactCity = familyMember.ContactCity,
                ContactPostalCode = familyMember.ContactPostalCode,
                ContactProvinceState = familyMember.ContactProvinceState,
                ContactCountry = familyMember.ContactCountry,
            };
        }

        public static IEnumerable<FamilyMember> FromApiEntities(IEnumerable<FamilyMemberEntity> familyMembers)
        {
            return familyMembers.Select(FromApiEntity);
        }
    }
}

