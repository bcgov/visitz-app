using Realms;
using VisitzApi.Models;
using VisitzModel.Extensions;
using VisitzModel.Utilities;

namespace VisitzModel.Models.People
{
    public partial class FamilyMember : IEmbeddedObject
    {
        public static readonly int KeyPlayerSortPosition = 0;
        public static readonly int ParentCaregiverSortPosition = 1;
        public static readonly int SubjectChildSortPosition = 2;
        public static readonly int OtherSortPosition = int.MaxValue;

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
        public bool? SubjectFlag { get; set; }
        public bool? ParentCaregiver { get; set; }
        public bool? SubjectChild { get; set; }

        public string FullDisplayName => string.Join(" ",
            FirstName, MiddleName, LastName);

        public string HomePhoneFormatted => PhoneNumberFormatter.Format(HomePhone);

        public string CellPhoneFormatted => PhoneNumberFormatter.Format(CellPhone);

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

        public int SortPositionAsc
        {
            get
            {
                if (IsKeyPlayer)
                    return KeyPlayerSortPosition;
                else if (ParentCaregiver ?? false)
                    return ParentCaregiverSortPosition;
                else if (SubjectChild ?? false)
                    return SubjectChildSortPosition;
                else
                    return OtherSortPosition;
            }
        }

        public int? Age => DateTime.TryParse(DateOfBirth, out DateTime dateOfBirth)
                    ? (DateTimeExtensions.LocalNow - dateOfBirth).Days / 365
                    : null;
    }
}
