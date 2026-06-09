using VisitzApi.Models.People;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;

namespace VisitzModelTest.Models.People;

public class SupportNetworkItemTests
{
    private static List<SupportNetworkJson> InitialSupportNetworkJsonList =>
        [
            new()
            {
                Active = "true",
                Address = "2312313",
                Agency = "cdscs",
                Cell = "3423423",
                Comments = "dishfs",
                CreatedBy = "1",
                CreatedById = "2",
                CreatedDate = "12/10/2018 13:50:02",
                EntityId = "3",
                EntityName = "Case",
                Id = "1",
                Name = "ABC",
                Phone = "23242",
                Relationship = "Mother",
                UpdatedBy = "2",
                UpdatedById = "3",
                UpdatedDate = "12/10/2018 13:50:02",
            },
            new()
            {
                Active = "true",
                Address = "2312313",
                Agency = "cdscs",
                Cell = "3423423",
                Comments = "dishfs",
                CreatedBy = "1",
                CreatedById = "2",
                CreatedDate = "12/10/2018 13:50:02",
                EntityId = "3",
                EntityName = "Case",
                Id = "2",
                Name = "SDE",
                Phone = "23242",
                Relationship = "Father",
                UpdatedBy = "2",
                UpdatedById = "3",
                UpdatedDate = "12/10/2018 13:50:02",
            },
            new()
            {
                Active = "true",
                Address = "2312313",
                Agency = "cdscs",
                Cell = "3423423",
                Comments = "dishfs",
                CreatedBy = "1",
                CreatedById = "2",
                CreatedDate = "12/10/2018 13:50:02",
                EntityId = "3",
                EntityName = "Case",
                Id = "3",
                Name = "QWE",
                Phone = "23242",
                Relationship = "SON",
                UpdatedBy = "2",
                UpdatedById = "3",
                UpdatedDate = "12/10/2018 13:50:02",
            },
            new()
            {
                Active = "true",
                Address = "2312313",
                Agency = "cdscs",
                Cell = "3423423",
                Comments = "dishfs",
                CreatedBy = "1",
                CreatedById = "2",
                CreatedDate = "12/10/2018 13:50:02",
                EntityId = "3",
                EntityName = "Case",
                Id = "4",
                Name = "ABC",
                Phone = "23242",
                Relationship = "Mother",
                UpdatedBy = "2",
                UpdatedById = "3",
                UpdatedDate = "12/10/2018 13:50:02",
            },
        ];

    [Fact]
    public async Task SynchronizeAsyncDeletesDifferenceFromRealm()
    {
        var realm = await TestingUtilities.MakeRealm<SupportNetworkItemTests>();
        List<SupportNetworkJson> supportNetworks = InitialSupportNetworkJsonList;
        string parentId = "12";

        await SupportNetworkItem.SynchronizeAsync(realm, supportNetworks, parentId, EntityType.Case);
        var numberOfNetworkItemsBeforeDeletion = realm.All<SupportNetworkItem>().Count();

        //Checking deletion of realm objects
        supportNetworks = InitialSupportNetworkJsonList;
        supportNetworks.RemoveAll(item => item.Id == "3");

        await SupportNetworkItem.SynchronizeAsync(realm, supportNetworks, parentId, EntityType.Case);
        var numberOfNetworkItemsAfterDeletion = realm.All<SupportNetworkItem>().Count();

        Assert.Equal(4, numberOfNetworkItemsBeforeDeletion);
        Assert.Equal(numberOfNetworkItemsBeforeDeletion - 1, numberOfNetworkItemsAfterDeletion);
    }
}
