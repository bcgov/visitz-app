using VisitzApi.Models.People;
using VisitzModel.Models.People;

namespace VisitzModelTest.Models.People;

public class ContactMedicalBehavioralTests
{
    private static readonly List<ContactMedicalBehavioralJson> medicalBehavioralJsons =
    [
        new()
        {
            Category = "uyg",
            Comments = "njhhu",
            Condition = "uhuhuh",
            ContactFirstName = "abc",
            ContactId = "123",
            ContactLastName = "abc",
            ContactMiddleName = "abc",
            ContactRowNum = "123",
            Created = "12/11/2025 13:50:02",
            CreatedBy = "abc",
            CreatedByName = "abc",
            DiagnosedBy = "abc",
            DiagnosisDate = "12/11/2025 13:50:02",
            EndDate = "12/11/2025 13:50:02",
            Id = "1",
            Name = "abc",
            ParentCaseNum = "123",
            StartDate = "12/11/2025 13:50:02",
            TreatmentPlan = "hdedhd",
            Type = "Case",
            Updated = "12/11/2025 13:50:02",
            UpdatedBy = "jdjd",
            UpdatedByName = "jsshjd",
        },
        new()
        {
            Category = "uyg",
            Comments = "njhhu",
            Condition = "uhuhuh",
            ContactFirstName = "abc",
            ContactId = "456",
            ContactLastName = "abc",
            ContactMiddleName = "abc",
            ContactRowNum = "456",
            Created = "12/11/2025 13:50:02",
            CreatedBy = "abc",
            CreatedByName = "abc",
            DiagnosedBy = "abc",
            DiagnosisDate = "12/11/2025 13:50:02",
            EndDate = "12/11/2025 13:50:02",
            Id = "2",
            Name = "abc",
            ParentCaseNum = "123",
            StartDate = "12/11/2025 13:50:02",
            TreatmentPlan = "hdedhd",
            Type = "Case",
            Updated = "12/11/2025 13:50:02",
            UpdatedBy = "jdjd",
            UpdatedByName = "jsshjd",
        },
    ];

    private static readonly List<ContactMedicalBehavioralJson> serviceRequestMedicalBehavioralJsons =
    [
        new()
        {
            Category = "uyg",
            Comments = "njhhu",
            Condition = "uhuhuh",
            ContactFirstName = "abc",
            ContactId = "333",
            ContactLastName = "abc",
            ContactMiddleName = "abc",
            ContactRowNum = "1",
            Created = "12/11/2025 13:50:02",
            CreatedBy = "abc",
            CreatedByName = "abc",
            DiagnosedBy = "abc",
            DiagnosisDate = "12/11/2025 13:50:02",
            EndDate = "12/11/2025 13:50:02",
            Id = "20",
            Name = "abc",
            ParentCaseNum = "123",
            StartDate = "12/11/2025 13:50:02",
            TreatmentPlan = "hdedhd",
            Type = "ServiceRequest",
            Updated = "12/11/2025 13:50:02",
            UpdatedBy = "jdjd",
            UpdatedByName = "jsshjd",
        },
        new()
        {
            Category = "uyg",
            Comments = "njhhu",
            Condition = "uhuhuh",
            ContactFirstName = "abc",
            ContactId = "222",
            ContactLastName = "abc",
            ContactMiddleName = "abc",
            ContactRowNum = "2",
            Created = "12/11/2025 13:50:02",
            CreatedBy = "abc",
            CreatedByName = "abc",
            DiagnosedBy = "abc",
            DiagnosisDate = "12/11/2025 13:50:02",
            EndDate = "12/11/2025 13:50:02",
            Id = "21",
            Name = "abc",
            ParentCaseNum = "123",
            StartDate = "12/11/2025 13:50:02",
            TreatmentPlan = "hdedhd",
            Type = "ServiceRequest",
            Updated = "12/11/2025 13:50:02",
            UpdatedBy = "jdjd",
            UpdatedByName = "jsshjd",
        },
        new()
        {
            Category = "uyg",
            Comments = "njhhu",
            Condition = "uhuhuh",
            ContactFirstName = "abc",
            ContactId = "111",
            ContactLastName = "abc",
            ContactMiddleName = "abc",
            ContactRowNum = "3",
            Created = "12/11/2025 13:50:02",
            CreatedBy = "abc",
            CreatedByName = "abc",
            DiagnosedBy = "abc",
            DiagnosisDate = "12/11/2025 13:50:02",
            EndDate = "12/11/2025 13:50:02",
            Id = "22",
            Name = "abc",
            ParentCaseNum = "123",
            StartDate = "12/11/2025 13:50:02",
            TreatmentPlan = "hdedhd",
            Type = "ServiceRequest",
            Updated = "12/11/2025 13:50:02",
            UpdatedBy = "jdjd",
            UpdatedByName = "jsshjd",
        },
    ];

    [Fact]
    public async Task SynchronizeAsyncAddContactMedicalBehavioralInfoForCasesToRealm()
    {
        var realm = await TestingUtilities.MakeRealm<ContactMedicalBehavioralTests>();
        List<ContactMedicalBehavioralJson> contactMedicalBehavioral = medicalBehavioralJsons;

        var numberOfCaseContactMedicalBehavioralBeforeInsertion = realm.All<ContactMedicalBehavioral>().Count();
        await ContactMedicalBehavioral.SynchronizeAsync(realm, contactMedicalBehavioral, "10");

        var numberOfCaseContactMedicalBehavioralAfterInsertion = realm.All<ContactMedicalBehavioral>().Count();

        Assert.Equal(0, numberOfCaseContactMedicalBehavioralBeforeInsertion);
        Assert.Equal(2, numberOfCaseContactMedicalBehavioralAfterInsertion);
    }

    [Fact]
    public async Task SynchronizeAsyncDeletesCaseContactMedicalBehavioralDifferenceFromRealm()
    {
        var realm = await TestingUtilities.MakeRealm<ContactMedicalBehavioralTests>();
        List<ContactMedicalBehavioralJson> contactMedicalBehavioral = [.. medicalBehavioralJsons];
        string parentId = "10";

        await ContactMedicalBehavioral.SynchronizeAsync(realm, contactMedicalBehavioral, parentId);
        var numberOfCaseContactMedicalBehavioralBeforeDeletion = realm.All<ContactMedicalBehavioral>().Count();

        contactMedicalBehavioral.RemoveAll(item => item.Id == "1");

        await ContactMedicalBehavioral.SynchronizeAsync(realm, contactMedicalBehavioral, parentId);
        var numberOfCaseContactMedicalBehavioralAfterDeletion = realm.All<ContactMedicalBehavioral>().Count();

        Assert.Equal(2, numberOfCaseContactMedicalBehavioralBeforeDeletion);
        Assert.Equal(
            numberOfCaseContactMedicalBehavioralBeforeDeletion - 1,
            numberOfCaseContactMedicalBehavioralAfterDeletion
        );
    }

    [Fact]
    public async Task SynchronizeAsyncAddContactMedicalBehavioralInfoToRealm()
    {
        var realm = await TestingUtilities.MakeRealm<ContactMedicalBehavioralTests>();
        List<ContactMedicalBehavioralJson> contactMedicalBehavioral = serviceRequestMedicalBehavioralJsons;

        var numberOfCaseContactMedicalBehavioralBeforeInsertion = realm.All<ContactMedicalBehavioral>().Count();
        await ContactMedicalBehavioral.SynchronizeAsync(realm, contactMedicalBehavioral, "12");

        var numberOfCaseContactMedicalBehavioralAfterInsertion = realm.All<ContactMedicalBehavioral>().Count();

        Assert.Equal(0, numberOfCaseContactMedicalBehavioralBeforeInsertion);
        Assert.Equal(3, numberOfCaseContactMedicalBehavioralAfterInsertion);
    }

    [Fact]
    public async Task SynchronizeAsyncDeletesSrContactMedicalBehavioralDifferenceFromRealm()
    {
        var realm = await TestingUtilities.MakeRealm<ContactMedicalBehavioralTests>();
        List<ContactMedicalBehavioralJson> contactMedicalBehavioral = [.. serviceRequestMedicalBehavioralJsons];
        string parentId = "12";

        await ContactMedicalBehavioral.SynchronizeAsync(realm, contactMedicalBehavioral, parentId);
        var numberOfCaseContactMedicalBehavioralBeforeDeletion = realm.All<ContactMedicalBehavioral>().Count();

        contactMedicalBehavioral.RemoveAll(item => item.Id == "22");

        await ContactMedicalBehavioral.SynchronizeAsync(realm, contactMedicalBehavioral, parentId);
        var numberOfCaseContactMedicalBehavioralAfterDeletion = realm.All<ContactMedicalBehavioral>().Count();

        Assert.Equal(3, numberOfCaseContactMedicalBehavioralBeforeDeletion);
        Assert.Equal(
            numberOfCaseContactMedicalBehavioralBeforeDeletion - 1,
            numberOfCaseContactMedicalBehavioralAfterDeletion
        );
    }
}
