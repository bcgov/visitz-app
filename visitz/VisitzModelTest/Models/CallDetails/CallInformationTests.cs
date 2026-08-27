using VisitzApi.Models.CallDetails;
using VisitzModel.Models.CallDetails;
using VisitzModel.Models.EntityTypes;

namespace VisitzModelTest.Models.CallDetails;

public class CallInformationTests
{
    private static readonly List<CallInformationJson> incidentCallInformation =
    [
        new()
        {
            Created = "12/10/2025 13:50:02",
            CreatedBy = "jdijdi",
            CreatedByName = "abc",
            Id = "1",
            Updated = "12/10/2025 13:50:02",
            UpdatedBy = "abc",
            UpdatedByName = "test",
            IncidentId = "123",
            CallInformation = "dghweg",
        },
        new()
        {
            Created = "12/10/2025 13:50:02",
            CreatedBy = "jdijdi",
            CreatedByName = "abc",
            Id = "2",
            Updated = "12/10/2025 13:50:02",
            UpdatedBy = "abc",
            UpdatedByName = "test",
            IncidentId = "123",
            CallInformation = "dghweg",
        },
    ];

    private static readonly List<CallInformationJson> memoCallInformation =
    [
        new()
        {
            Created = "12/10/2025 13:50:02",
            CreatedBy = "jdijdi",
            CreatedByName = "abc",
            Id = "1",
            Updated = "12/10/2025 13:50:02",
            UpdatedBy = "abc",
            UpdatedByName = "test",
            MemoId = "456",
            Note = "dghweg",
        },
    ];

    private static readonly List<CallInformationJson> serviceRequestCallInformation =
    [
        new()
        {
            Created = "12/10/2025 13:50:02",
            CreatedBy = "jdijdi",
            CreatedByName = "abc",
            Id = "11",
            Updated = "12/10/2025 13:50:02",
            UpdatedBy = "abc",
            UpdatedByName = "test",
            SrId = "789",
            CallInformation = "dghweg",
        },
        new()
        {
            Created = "12/10/2025 13:50:02",
            CreatedBy = "jdijdi",
            CreatedByName = "abc",
            Id = "12",
            Updated = "12/10/2025 13:50:02",
            UpdatedBy = "abc",
            UpdatedByName = "test",
            SrId = "789",
            CallInformation = "dghweg",
        },
        new()
        {
            Created = "12/10/2025 13:50:02",
            CreatedBy = "jdijdi",
            CreatedByName = "abc",
            Id = "13",
            Updated = "12/10/2025 13:50:02",
            UpdatedBy = "abc",
            UpdatedByName = "test",
            SrId = "789",
            CallInformation = "dghweg",
        },
    ];

    [Fact]
    public async Task SynchronizeAsyncAddIncidentCallInformationToRealm()
    {
        var realm = await TestingUtilities.MakeRealm<CallInformationTests>();
        List<CallInformationJson> incidentCallInfo = incidentCallInformation;

        var numberOfIncidentCallInformationBeforeInsertion = realm.All<CallInformation>().Count();
        await CallInformation.SynchronizeAsync(realm, incidentCallInfo, "123", EntityType.Incident);

        var numberOfIncidentCallInformationAfterInsertion = realm.All<CallInformation>().Count();

        Assert.Equal(0, numberOfIncidentCallInformationBeforeInsertion);
        Assert.Equal(2, numberOfIncidentCallInformationAfterInsertion);
    }

    [Fact]
    public async Task SynchronizeAsyncAddMemoCallInformationToRealm()
    {
        var realm = await TestingUtilities.MakeRealm<CallInformationTests>();
        List<CallInformationJson> memoCallInfo = memoCallInformation;

        var numberOfMemoCallInformationBeforeInsertion = realm.All<CallInformation>().Count();
        await CallInformation.SynchronizeAsync(realm, memoCallInfo, "456", EntityType.Memo);

        var numberOfMemoCallInformationAfterInsertion = realm.All<CallInformation>().Count();

        Assert.Equal(0, numberOfMemoCallInformationBeforeInsertion);
        Assert.Equal(1, numberOfMemoCallInformationAfterInsertion);
    }

    [Fact]
    public async Task SynchronizeAsyncAddServiceRequestCallInformationToRealm()
    {
        var realm = await TestingUtilities.MakeRealm<CallInformationTests>();
        List<CallInformationJson> serviceRequestCallInfo = serviceRequestCallInformation;

        var numberOfServiceRequestCallInformationBeforeInsertion = realm.All<CallInformation>().Count();
        await CallInformation.SynchronizeAsync(realm, serviceRequestCallInfo, "789", EntityType.ServiceRequest);

        var numberOfServiceRequestCallInformationAfterInsertion = realm.All<CallInformation>().Count();

        Assert.Equal(0, numberOfServiceRequestCallInformationBeforeInsertion);
        Assert.Equal(3, numberOfServiceRequestCallInformationAfterInsertion);
    }

    [Fact]
    public async Task SynchronizeAsyncDeletesIncidentCallInfoDifferenceFromRealm()
    {
        var realm = await TestingUtilities.MakeRealm<CallInformationTests>();
        List<CallInformationJson> incidentCallInfo = [.. incidentCallInformation];

        await CallInformation.SynchronizeAsync(realm, incidentCallInfo, "123", EntityType.Incident);
        var numberOfIncidentCallInfoBeforeDeletion = realm.All<CallInformation>().Count();

        incidentCallInfo.RemoveAll(item => item.Id == "2");
        await CallInformation.SynchronizeAsync(realm, incidentCallInfo, "123", EntityType.Incident);

        var numberOfIncidentCallInfoAfterDeletion = realm.All<CallInformation>().Count();
        var inc = realm.All<CallInformation>().ToList();

        Assert.Equal(2, numberOfIncidentCallInfoBeforeDeletion);
        Assert.Equal(numberOfIncidentCallInfoBeforeDeletion - 1, numberOfIncidentCallInfoAfterDeletion);
    }

    [Fact]
    public async Task SynchronizeAsyncDeletesMemoCallInfoDifferenceFromRealm()
    {
        var realm = await TestingUtilities.MakeRealm<CallInformationTests>();
        List<CallInformationJson> memoCallInfo = [.. memoCallInformation];

        await CallInformation.SynchronizeAsync(realm, memoCallInfo, "456", EntityType.Memo);
        var numberOfMemoCallInfoBeforeDeletion = realm.All<CallInformation>().Count();

        memoCallInfo.RemoveAll(item => item.Id == "1");
        await CallInformation.SynchronizeAsync(realm, memoCallInfo, "456", EntityType.Memo);

        var numberOfMemoCallInfoAfterDeletion = realm.All<CallInformation>().Count();
        var inc = realm.All<CallInformation>().ToList();

        Assert.Equal(1, numberOfMemoCallInfoBeforeDeletion);
        Assert.Equal(numberOfMemoCallInfoBeforeDeletion - 1, numberOfMemoCallInfoAfterDeletion);
    }

    [Fact]
    public async Task SynchronizeAsyncDeletesServiceRequestCallInfoDifferenceFromRealm()
    {
        var realm = await TestingUtilities.MakeRealm<CallInformationTests>();
        List<CallInformationJson> serviceRequestCallInfo = [.. serviceRequestCallInformation];

        await CallInformation.SynchronizeAsync(realm, serviceRequestCallInfo, "789", EntityType.ServiceRequest);
        var numberOfServiceRequestCallInfoBeforeDeletion = realm.All<CallInformation>().Count();

        serviceRequestCallInfo.RemoveAll(item => item.Id == "12");
        await CallInformation.SynchronizeAsync(realm, serviceRequestCallInfo, "789", EntityType.ServiceRequest);

        var numberOfServiceRequestCallInfoAfterDeletion = realm.All<CallInformation>().Count();
        var inc = realm.All<CallInformation>().ToList();

        Assert.Equal(3, numberOfServiceRequestCallInfoBeforeDeletion);
        Assert.Equal(numberOfServiceRequestCallInfoBeforeDeletion - 1, numberOfServiceRequestCallInfoAfterDeletion);
    }
}
