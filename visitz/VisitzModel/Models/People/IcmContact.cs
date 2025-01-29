using Realms;
using VisitzApi.Models.People;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;

namespace VisitzModel.Models.People;

public partial class IcmContact : IRealmObject, IRowMetadata, IApiJson<ContactJson>, IParentRecord
{
    [PrimaryKey]
    public string LocalId { get; set; }

    public string Id { get; set; }

    public string CreatedBy { get; set; }

    public string CreatedById { get; set; }

    public string UpdatedBy { get; set; }

    public string UpdatedById { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset UpdatedDate { get; set; }

    public string ParentId { get; set; }

    private int ParentTypeInt { get; set; }

    public EntityType ParentType
    {
        get => (EntityType)ParentTypeInt;
        set => ParentTypeInt = (int)value;
    }

    public string AboriginalCalc { get; set; }

    public int Age { get; set; }

    public string CaseConEndDt { get; set; }

    public string CaseConOriginalStartDt { get; set; }

    public string CaseConParentCaregiver { get; set; }

    public string CaseConReportedOn { get; set; }

    public string CaseConStartDt { get; set; }

    public string CaseConSubjectChild { get; set; }

    public string CaseRelTypeCode { get; set; }

    public string CaseSubject { get; set; }

    public string CoordinationAgtCa { get; set; }

    public string CysnCalc { get; set; }

    public string CysnPstScore { get; set; }

    public string DateOfBirth { get; set; }

    public string DependentSequenceNumber { get; set; }

    public string GivenName { get; set; }

    public string InvolvedFamilyAlerts { get; set; }

    public string Is921BandFoundCalc { get; set; }

    public string LastName { get; set; }

    public string LegalStatus { get; set; }

    public string Sex { get; set; }

    public string SsaPrimaryField { get; set; }

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
        AboriginalCalc = json.AboriginalCalc;
        Age = int.Parse(json.Age);
        CaseConEndDt = json.CaseConEndDt;
        CaseConOriginalStartDt = json.CaseConOriginalStartDt;
        CaseConParentCaregiver = json.CaseConParentCaregiver;
        CaseConReportedOn = json.CaseConReportedOn;
        CaseConStartDt = json.CaseConStartDt;
        CaseConSubjectChild = json.CaseConSubjectChild;
        CaseRelTypeCode = json.CaseRelTypeCode;
        CaseSubject = json.CaseSubject;
        CoordinationAgtCa = json.CoordinationAGTCA;
        CysnCalc = json.CYSNCalc;
        CysnPstScore = json.CYSNPSTScore;
        DateOfBirth = json.DateofBirth;
        DependentSequenceNumber = json.DependentSequenceNumber;
        GivenName = json.GivenName;
        InvolvedFamilyAlerts = json.InvolvedFamilyAlerts;
        Is921BandFoundCalc = json.Is921BandFoundCalc;
        LastName = json.LastName;
        LegalStatus = json.LegalStatus;
        Sex = json.Sex;
        SsaPrimaryField = json.SSAPrimaryField;
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
            AboriginalCalc = AboriginalCalc,
            Age = Age.ToString(),
            CaseConEndDt = CaseConEndDt,
            CaseConOriginalStartDt = CaseConOriginalStartDt,
            CaseConParentCaregiver = CaseConParentCaregiver,
            CaseConReportedOn = CaseConReportedOn,
            CaseConStartDt = CaseConStartDt,
            CaseConSubjectChild = CaseConSubjectChild,
            CaseRelTypeCode = CaseRelTypeCode,
            CaseSubject = CaseSubject,
            CoordinationAGTCA = CoordinationAgtCa,
            CYSNCalc = CysnCalc,
            CYSNPSTScore = CysnPstScore,
            DateofBirth = DateOfBirth,
            DependentSequenceNumber = DependentSequenceNumber,
            GivenName = GivenName,
            InvolvedFamilyAlerts = InvolvedFamilyAlerts,
            Is921BandFoundCalc = Is921BandFoundCalc,
            LastName = LastName,
            LegalStatus = LegalStatus,
            Sex = Sex,
            SSAPrimaryField = SsaPrimaryField,
        };
    }

    public static IEnumerable<IcmContact> FromApiArray(
        IEnumerable<ContactJson> contacts,
        string parentId,
        EntityType type)
    {
        List<IcmContact> outList = [];

        foreach (var contactJson in contacts)
            outList.Add(new IcmContact(contactJson, parentId, type));

        return outList;
    }

    public static async Task SaveContactsAsync(
        Realm realm,
        IEnumerable<ContactJson> contacts,
        string parentId,
        EntityType type)
    {
        await RealmExtensions.CommitAsync(realm, () => realm.Upsert(FromApiArray(contacts, parentId, type)));
    }
}
