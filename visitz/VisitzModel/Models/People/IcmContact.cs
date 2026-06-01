using System.Globalization;
using Realms;
using VisitzApi.Models.People;
using VisitzModel.Extensions;
using VisitzModel.Formats;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;
using VisitzModel.Utilities;

namespace VisitzModel.Models.People;

public partial class IcmContact
    : IRealmObject,
        IRowMetadata,
        IApiJson<ContactJson>,
        IParentRecord,
        IEquatable<IcmContact>
{
    public static readonly int KeyPlayerSortPosition = 0;
    public static readonly int ParentCaregiverSortPosition = 1;
    public static readonly int SubjectChildSortPosition = 2;
    public static readonly int OtherSortPosition = int.MaxValue;

    static readonly string KeyPlayer = "Key player";

    [PrimaryKey]
    public string LocalId { get; set; } = string.Empty;

    public string Id { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;

    public string CreatedById { get; set; } = string.Empty;

    public string UpdatedBy { get; set; } = string.Empty;

    public string UpdatedById { get; set; } = string.Empty;

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.UtcNow;

    public string ParentId { get; set; } = string.Empty;

    private int ParentTypeInt { get; set; } = (int)EntityType.Unknown;

    public EntityType ParentType
    {
        get => (EntityType)ParentTypeInt;
        set => ParentTypeInt = (int)value;
    }

    public string _921Agt { get; set; } = string.Empty;

    public int ActiveAddresses { get; set; }

    public int Age { get; set; }

    public string AkaFirstName { get; set; } = string.Empty;

    public string AkaLastName { get; set; } = string.Empty;

    public string Alerts { get; set; } = string.Empty;

    public string AutismFundingPaused { get; set; } = string.Empty;

    public string BceIdUserName { get; set; } = string.Empty;

    public string CanadianCitizen { get; set; } = string.Empty;

    public string CellPhone { get; set; } = string.Empty;

    public string Citizen { get; set; } = string.Empty;

    public string Citizenship { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string CollaborateId { get; set; } = string.Empty;

    public string Comments { get; set; } = string.Empty;

    public string ConcernsOutcome { get; set; } = string.Empty;

    public string CoordinationAgtCa { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string CountryOfBirth { get; set; } = string.Empty;

    public DateTimeOffset? CurrentStartDate { get; set; }

    public string Cysn { get; set; } = string.Empty;

    public DateTimeOffset? DateOfBirth { get; set; }

    public DateTimeOffset? CitizenUpdatedDate { get; set; }

    public DateTimeOffset? CitizenshipUpdatedDate { get; set; }

    public string Deceased { get; set; } = string.Empty;

    public DateTimeOffset? DeceasedDate { get; set; }

    public string EndDate { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public string GivenNames { get; set; } = string.Empty;

    public string HomePhone { get; set; } = string.Empty;

    public string ImmigrationStatus { get; set; } = string.Empty;

    public string ImmigrationStatusUpdated { get; set; } = string.Empty;

    public string Indigenous { get; set; } = string.Empty;

    public string IntegrationState { get; set; } = string.Empty;

    public string InvestigationOutcomeSummary { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string LegacyDependentSequence { get; set; } = string.Empty;

    public string LegalStatus { get; set; } = string.Empty;

    public string MessagePhone { get; set; } = string.Empty;

    public string MiddleNames { get; set; } = string.Empty;

    public DateTimeOffset? OriginalStartDate { get; set; }

    public bool IsParentCaregiver { get; set; }

    public string PersonIdIcm { get; set; } = string.Empty;

    public string PersonIdMis { get; set; } = string.Empty;

    public bool? ResponsibleForAllegedMaltreatment { get; set; }

    public string PersonalHealthNumber { get; set; } = string.Empty;

    public string PersonalHealthNumberVerified { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string PotentialDuplicate { get; set; } = string.Empty;

    public string PotentialDuplicateComments { get; set; } = string.Empty;

    public string PreferredLanguage { get; set; } = string.Empty;

    public string Primary { get; set; } = string.Empty;

    public string PrimaryAddress { get; set; } = string.Empty;

    public string PrimaryEmail { get; set; } = string.Empty;

    public string ProjectCode { get; set; } = string.Empty;

    public string Province { get; set; } = string.Empty;

    public string PstScore { get; set; } = string.Empty;

    public string Relationship { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string RowId { get; set; } = string.Empty;

    public string SaetPaused { get; set; } = string.Empty;

    public string SocialInsuranceNumber { get; set; } = string.Empty;

    public DateTimeOffset? StartDate { get; set; }

    public string StreetAddress { get; set; } = string.Empty;

    public string StreetAddress2 { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public bool IsSubjectChild { get; set; }

    public string Title { get; set; } = string.Empty;

    public string UnitNumber { get; set; } = string.Empty;

    public string WorkPhone { get; set; } = string.Empty;

    public string FullDisplayName => string.Join(" ", FirstName, MiddleNames, LastName);

    public string DateOfBirthFormatted =>
        DateOfBirth?.ToString(IcmDateFormats.BasicTimestampShort, CultureInfo.InvariantCulture) ?? string.Empty;

    public string HomePhoneFormatted => PhoneNumberFormatter.Format(HomePhoneBinding) ?? string.Empty;

    public string CellPhoneFormatted => PhoneNumberFormatter.Format(CellPhoneBinding) ?? string.Empty;

    public bool IsKeyPlayer => Relationship == KeyPlayer;

    public string DisplayCoordinationAgtCa => CoordinationAgtCa?.ExtendYOrN() ?? string.Empty;

    public string Display_921Agt => _921Agt?.ExtendYOrN() ?? string.Empty;

    public int SortPositionAsc
    {
        get
        {
            if (IsKeyPlayer)
                return KeyPlayerSortPosition;
            else if (IsParentCaregiver)
                return ParentCaregiverSortPosition;
            else if (IsSubjectChild)
                return SubjectChildSortPosition;
            else
                return OtherSortPosition;
        }
    }

    public IcmContact() { }

    public IcmContact(ContactJson json, string parentId, EntityType type)
    {
        LocalId = MakeLocalId(json.Id, parentId);
        Id = json.Id;
        CreatedBy = json.CreatedBy;
        CreatedById = json.CreatedById;
        UpdatedBy = json.UpdatedBy;
        UpdatedById = json.UpdatedById;
        CreatedDate = DateTimeOffset.Parse(json.CreatedDate);
        UpdatedDate = DateTimeOffset.Parse(json.UpdatedDate);
        ParentId = parentId;
        ParentType = type;
        _921Agt = json._92_1AGT;
        ActiveAddresses = int.TryParse(json.ActiveAddresses, out int addresses) ? addresses : -1;
        AkaFirstName = json.AKAFirstName;
        AkaLastName = json.AKALastName;
        Alerts = json.Alerts;
        AutismFundingPaused = json.AutismFundingPaused;
        BceIdUserName = json.BCeIDUserName;
        CanadianCitizen = json.CanadianCitizen;
        CellPhone = json.CellPhone;
        Citizen = json.Citizen;
        Citizenship = json.Citizenship;
        City = json.City;
        CollaborateId = json.CollaborateID;
        Comments = json.Comments;
        ConcernsOutcome = json.ConcernsOutcome;
        CoordinationAgtCa = json.CoordinationAGTCA;
        Country = json.Country;
        CountryOfBirth = json.CountryofBirth;
        CurrentStartDate = Timestamp.ParseDateTimeOffsetNullable(json.CurrentStartDate);
        Cysn = json.CYSN;
        DateOfBirth = Timestamp.ParseDateTimeOffsetNullable(json.DateofBirth);
        CitizenUpdatedDate = Timestamp.ParseDateTimeOffsetNullable(json.DateUpdated_CitizenUpdatedDate);
        CitizenshipUpdatedDate = Timestamp.ParseDateTimeOffsetNullable(json.DateUpdated_CitizenshipUpdatedDate);
        Deceased = json.Deceased;
        DeceasedDate = Timestamp.ParseDateTimeOffsetNullable(json.DeceasedDate);
        EndDate = json.EndDate;
        FirstName = json.FirstName;
        Gender = json.Gender;
        GivenNames = json.GivenNames;
        HomePhone = json.HomePhone;
        ImmigrationStatus = json.ImmigrationStatus;
        ImmigrationStatusUpdated = json.ImmigrationStatusUpdated;
        Indigenous = json.Indigenous;
        IntegrationState = json.IntegrationState;
        InvestigationOutcomeSummary = json.InvestigationOutcomeSummary;
        LastName = json.LastName;
        LegacyDependentSequence = json.LegacyDependentSequence;
        LegalStatus = json.LegalStatus;
        MessagePhone = json.MessagePhone;
        MiddleNames = json.MiddleNames;
        OriginalStartDate = Timestamp.ParseDateTimeOffsetNullable(json.OriginalStartDate);
        IsParentCaregiver = json.Parent_Caregiver?.ParseWordTruthiness() ?? false;
        PersonIdIcm = json.PersonIDICM;
        PersonIdMis = json.PersonIDMIS;
        ResponsibleForAllegedMaltreatment =
            json.PersonResponsibleforAllegedMaltreatment?.ParseWordTruthiness() ?? false;
        PersonalHealthNumber = json.PHN;
        PersonalHealthNumberVerified = json.PHNVerified;
        PostalCode = json.PostalCode;
        PotentialDuplicate = json.PotentialDuplicate;
        PotentialDuplicateComments = json.PotentialDuplicateComments;
        PreferredLanguage = json.PreferredLanguage;
        Primary = json.Primary;
        PrimaryAddress = json.PrimaryAddress;
        PrimaryEmail = json.PrimaryEmail;
        ProjectCode = json.ProjectCode;
        Province = json.Prov;
        PstScore = json.PSTScore;
        Relationship = json.Relationship;
        Role = json.Role;
        RowId = json.RowId;
        SaetPaused = json.SAETPaused;
        SocialInsuranceNumber = json.SIN;
        StartDate = Timestamp.ParseDateTimeOffsetNullable(json.StartDate);
        StreetAddress = json.StreetAddress;
        StreetAddress2 = json.StreetAddress2;
        Subject = json.Subject;
        IsSubjectChild = json.SubjectChild.ParseWordTruthiness();
        Title = json.Title;
        UnitNumber = json.UnitNumber;
        WorkPhone = json.WorkPhone;

        Age = TryParseAge(json.Age, json.DateofBirth);
    }

    static int TryParseAge(string age, string dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(age) && !string.IsNullOrWhiteSpace(dateOfBirth))
            return (DateTimeOffset.UtcNow - DateTimeOffset.Parse(dateOfBirth)).Days / 365;
        else
            return int.TryParse(age, out int parsed) ? parsed : -1;
    }

    static string MakeLocalId(string contactId, string parentId)
    {
        return $"{contactId}|{parentId}";
    }

    public ContactJson ToApiJson(string dateFormat = "s")
    {
        return new()
        {
            Id = Id,
            CreatedBy = CreatedBy,
            CreatedById = CreatedById,
            UpdatedBy = UpdatedBy,
            UpdatedById = UpdatedById,
            CreatedDate = CreatedDate.ToString(dateFormat),
            UpdatedDate = UpdatedDate.ToString(dateFormat),
            _92_1AGT = _921Agt,
            ActiveAddresses = ActiveAddresses.ToString(),
            Age = Age.ToString(),
            AKAFirstName = AkaFirstName,
            AKALastName = AkaLastName,
            Alerts = Alerts,
            AutismFundingPaused = AutismFundingPaused,
            BCeIDUserName = BceIdUserName,
            CanadianCitizen = CanadianCitizen,
            CellPhone = CellPhone,
            Citizen = Citizen,
            Citizenship = Citizenship,
            City = City,
            CollaborateID = CollaborateId,
            Comments = Comments,
            ConcernsOutcome = ConcernsOutcome,
            CoordinationAGTCA = CoordinationAgtCa,
            Country = Country,
            CountryofBirth = CountryOfBirth,
            CurrentStartDate = CurrentStartDate?.ToString(dateFormat) ?? string.Empty,
            CYSN = Cysn,
            DateofBirth = DateOfBirth?.ToString(dateFormat) ?? string.Empty,
            DateUpdated_CitizenUpdatedDate = CitizenUpdatedDate?.ToString(dateFormat) ?? string.Empty,
            DateUpdated_CitizenshipUpdatedDate = CitizenshipUpdatedDate?.ToString(dateFormat) ?? string.Empty,
            Deceased = Deceased,
            DeceasedDate = DeceasedDate?.ToString(dateFormat) ?? string.Empty,
            EndDate = EndDate,
            FirstName = FirstName,
            Gender = Gender,
            GivenNames = GivenNames,
            HomePhone = HomePhone,
            ImmigrationStatus = ImmigrationStatus,
            ImmigrationStatusUpdated = ImmigrationStatusUpdated,
            Indigenous = Indigenous,
            IntegrationState = IntegrationState,
            InvestigationOutcomeSummary = InvestigationOutcomeSummary,
            LastName = LastName,
            LegacyDependentSequence = LegacyDependentSequence,
            LegalStatus = LegalStatus,
            MessagePhone = MessagePhone,
            MiddleNames = MiddleNames,
            OriginalStartDate = OriginalStartDate?.ToString(dateFormat) ?? string.Empty,
            Parent_Caregiver = IsParentCaregiver.AsTruthyChar(),
            PersonIDICM = PersonIdIcm,
            PersonIDMIS = PersonIdMis,
            PersonResponsibleforAllegedMaltreatment = ResponsibleForAllegedMaltreatment?.AsTruthyChar() ?? string.Empty,
            PHN = PersonalHealthNumber,
            PHNVerified = PersonalHealthNumberVerified,
            PostalCode = PostalCode,
            PotentialDuplicate = PotentialDuplicate,
            PotentialDuplicateComments = PotentialDuplicateComments,
            PreferredLanguage = PreferredLanguage,
            Primary = Primary,
            PrimaryAddress = PrimaryAddress,
            PrimaryEmail = PrimaryEmail,
            ProjectCode = ProjectCode,
            Prov = Province,
            PSTScore = PstScore,
            Relationship = Relationship,
            Role = Role,
            RowId = RowId,
            SAETPaused = SaetPaused,
            SIN = SocialInsuranceNumber,
            StartDate = StartDate?.ToString(dateFormat) ?? string.Empty,
            StreetAddress = StreetAddress,
            StreetAddress2 = StreetAddress2,
            Subject = Subject,
            SubjectChild = IsSubjectChild.AsTruthyChar(),
            Title = Title,
            UnitNumber = UnitNumber,
            WorkPhone = WorkPhone,
        };
    }

    public static IEnumerable<IcmContact> FromApiArray(
        IEnumerable<ContactJson> contacts,
        string parentId,
        EntityType type
    )
    {
        List<IcmContact> outList = [];

        foreach (var contactJson in contacts)
            outList.Add(new IcmContact(contactJson, parentId, type));

        return outList;
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        IEnumerable<ContactJson> contacts,
        string parentId,
        EntityType type
    )
    {
        await realm.CommitAsync(() =>
        {
            var incomingContacts = FromApiArray(contacts, parentId, type);
            var existingContacts = GetByParentIdType(realm, parentId, type).ToList();
            var contactsToDelete = existingContacts.Except(incomingContacts).ToList();

            foreach (var item in contactsToDelete)
            {
                if (item != null && item.IsValid)
                    realm.Remove(item);
            }

            realm.Upsert(incomingContacts);
        });
    }

    public static void RemoveByParent(Realm realm, EntityType type, string parentId)
    {
        var contacts = realm
            .All<IcmContact>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)type);

        foreach (var contact in contacts)
        {
            ContactMedicalBehavioral.RemoveByParent(realm, contact.Id);
            ContactLegalAuthority.RemoveByParent(realm, contact.Id);
            ContactLanguage.RemoveByParent(realm, contact.Id);
            ContactEducation.RemoveByParent(realm, contact.Id);
        }

        realm.RemoveRange(contacts);
    }

    public static IQueryable<IcmContact> GetByParentIdType(Realm realm, string id, EntityType type)
    {
        return realm.All<IcmContact>().Where(contact => contact.ParentId == id && contact.ParentTypeInt == (int)type);
    }

    public static IQueryable<IcmContact> GetByParentObject(Realm realm, IBusinessObject businessObject)
    {
        return realm
            .All<IcmContact>()
            .Where(contact =>
                contact.ParentId == businessObject.Id && contact.ParentTypeInt == (int)businessObject.EntityType
            );
    }

    public static IcmContact? GetKeyPlayerFor(Realm realm, IBusinessObject businessObject)
    {
        return GetByParentObject(realm, businessObject)
            .Where(contact => contact.Relationship == KeyPlayer)
            .FirstOrDefault();
    }

    public bool Equals(IcmContact? other)
    {
        return ReferenceEquals(this, other) || this?.Id == other?.Id;
    }

    public override bool Equals(object? other)
    {
        return other is IcmContact contact ? Equals(contact) : base.Equals(other);
    }

    public override int GetHashCode()
    {
#pragma warning disable SS008 // GetHashCode() refers to mutable or static member
        // Id is not meant to change
        return Id.GetHashCode();
#pragma warning restore SS008 // GetHashCode() refers to mutable or static member
    }

    public override string ToString()
    {
        return $"{nameof(IcmContact)}  {Id}";
    }
}
