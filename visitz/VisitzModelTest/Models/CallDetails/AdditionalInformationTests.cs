using System;
using System.Collections.Generic;
using System.Text;
using VisitzApi.Models.CallDetails;
using VisitzModel.Models.CallDetails;
using VisitzModel.Models.EntityTypes;

namespace VisitzModelTest.Models.CallDetails;

public class AdditionalInformationTests
{
    private static readonly List<AdditionalInformationJson> AddInfoList =
    [
        new()
        {
            AdditionalInformation = "Test",
            Created = "abcd",
            CreatedBy = "jdijdid",
            CreatedByName = "abcd",
            Id = "1",
            IncidentId = "1",
            Updated = "djie",
            UpdatedBy = "abc",
            UpdatedByName = "test",
        },
        new()
        {
            AdditionalInformation = "Test11",
            Created = "abc",
            CreatedBy = "jdijdi",
            CreatedByName = "abc",
            Id = "2",
            IncidentId = "2",
            Updated = "djies",
            UpdatedBy = "abcd",
            UpdatedByName = "test1",
        },
        new()
        {
            AdditionalInformation = "Test1",
            Created = "abcs",
            CreatedBy = "jdijdi",
            CreatedByName = "abc",
            Id = "3",
            IncidentId = "3",
            Updated = "djied",
            UpdatedBy = "abcg",
            UpdatedByName = "test4",
        },
        new()
        {
            AdditionalInformation = "Test",
            Created = "abc",
            CreatedBy = "jdijdi",
            CreatedByName = "abc",
            Id = "4",
            IncidentId = "1",
            Updated = "djie",
            UpdatedBy = "abc",
            UpdatedByName = "test",
        },
    ];

    [Fact]
    public async Task SynchronizeAsyncAddAdditionalInfoToRealm()
    {
        var realm = await TestingUtilities.MakeRealm<AdditionalInformationTests>();
        List<AdditionalInformationJson> AdditionalInfo = AddInfoList;

        var numberOfAdditionalInfoBeforeInsertion = realm.All<AdditionalInformation>().Count();
        await AdditionalInformation.SynchronizeAsync(realm, AdditionalInfo, "1", EntityType.Incident);

        var numberOfAdditionalInfoAfterInsertion = realm.All<AdditionalInformation>().Count();

        Assert.Equal(0, numberOfAdditionalInfoBeforeInsertion);
        Assert.Equal(4, numberOfAdditionalInfoAfterInsertion);
    }

    [Fact]
    public async Task SynchronizeAsyncDeletesDifferenceFromRealm()
    {
        var realm = await TestingUtilities.MakeRealm<AdditionalInformationTests>();
        List<AdditionalInformationJson> AdditionalInfo = [.. AddInfoList];

        await AdditionalInformation.SynchronizeAsync(realm, AdditionalInfo, "1", EntityType.Incident);
        var numberOfAdditionalInfoBeforeDeletion = realm.All<AdditionalInformation>().Count();

        AdditionalInfo.RemoveAll(item => item.Id == "1");
        AdditionalInfo.RemoveAll(item => item.Id == "4");
        await AdditionalInformation.SynchronizeAsync(realm, AdditionalInfo, "1", EntityType.Incident);

        var numberOfAdditionalInfoAfterDeletion = realm.All<AdditionalInformation>().Count();

        Assert.Equal(4, numberOfAdditionalInfoBeforeDeletion);
        Assert.Equal(numberOfAdditionalInfoBeforeDeletion - 2, numberOfAdditionalInfoAfterDeletion);
    }
}
