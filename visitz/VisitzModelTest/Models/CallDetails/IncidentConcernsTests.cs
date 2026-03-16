using VisitzApi.Models.CallDetails;
using VisitzModel.Models.CallDetails;

namespace VisitzModelTest.Models.CallDetails;

public class IncidentConcernsTests
{
    private static readonly List<IncidentConcernsJson> incidentConcernsList =
        [
            new()
            {
                Concern = "Physical HArm",
                Created = "abc",
                CreatedBy = "jdijdi",
                CreatedByName = "abc",
                EndDate = "12/10/2025 13:50:02",
                Id = "1",
                IncidentId = "1",
                OriginalConcern = "PH",
                StartDate = "12/10/2025 13:50:02",
                Updated = "djie",
                UpdatedBy = "abc",
                UpdatedByName = "test"
            },
            new()
            {
                Concern = "Physical HArm",
                Created = "abc",
                CreatedBy = "jdijdi",
                CreatedByName = "abc",
                EndDate = "12/10/2025 13:50:02",
                Id = "2",
                IncidentId = "1",
                OriginalConcern = "PH",
                StartDate = "12/10/2025 13:50:02",
                Updated = "djie",
                UpdatedBy = "abc",
                UpdatedByName = "test"
            },
            new()
            {
                Concern = "Physical HArm",
                Created = "abc",
                CreatedBy = "jdijdi",
                CreatedByName = "abc",
                EndDate = "12/10/2025 13:50:02",
                Id = "3",
                IncidentId = "1",
                OriginalConcern = "PH",
                StartDate = "12/10/2025 13:50:02",
                Updated = "djie",
                UpdatedBy = "abc",
                UpdatedByName = "test"
            },
            new()
            {
                Concern = "Physical HArm",
                Created = "abc",
                CreatedBy = "jdijdi",
                CreatedByName = "abc",
                EndDate = "12/10/2025 13:50:02",
                Id = "4",
                IncidentId = "1",
                OriginalConcern = "PH",
                StartDate = "12/10/2025 13:50:02",
                Updated = "djie",
                UpdatedBy = "abc",
                UpdatedByName = "test"
            }
        ];

    [Fact]
    public async Task SynchronizeAsyncAddIncidentConcernsToRealm()
    {
        var realm = await TestingUtilities.MakeRealm<IncidentConcernsTests>();
        List<IncidentConcernsJson> incidentConcerns = incidentConcernsList;

        var numberOfIncidentConcernsBeforeInsertion = realm.All<IncidentConcerns>().Count();
        await IncidentConcerns.SynchronizeAsync(realm, incidentConcerns);

        var numberOfIncidentConcernsAfterInsertion = realm.All<IncidentConcerns>().Count();

        Assert.Equal(0, numberOfIncidentConcernsBeforeInsertion);
        Assert.Equal(4, numberOfIncidentConcernsAfterInsertion);
    }

    [Fact]
    public async Task SynchronizeAsyncDeletesDifferenceFromRealm()
    {
        var realm = await TestingUtilities.MakeRealm<IncidentConcernsTests>();
        List<IncidentConcernsJson> incidentConcerns = incidentConcernsList;

        await IncidentConcerns.SynchronizeAsync(realm, incidentConcerns);
        var numberOfIncidentConcernsBeforeDeletion = realm.All<IncidentConcerns>().Count();

        incidentConcerns.RemoveAll(item => item.Id == "1");
        incidentConcerns.RemoveAll(item => item.Id == "4");
        await IncidentConcerns.SynchronizeAsync(realm, incidentConcerns);

        var numberOfIncidentConcernsAfterDeletion = realm.All<IncidentConcerns>().Count();

        Assert.Equal(4, numberOfIncidentConcernsBeforeDeletion);
        Assert.Equal(numberOfIncidentConcernsBeforeDeletion - 2, numberOfIncidentConcernsAfterDeletion);
    }
}
