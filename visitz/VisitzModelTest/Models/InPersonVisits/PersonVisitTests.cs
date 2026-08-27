using VisitzApi.Models.Visits;
using VisitzModel.Models.InPersonVisits;

namespace VisitzModelTest.Models.InPersonVisits;

public class PersonVisitTests
{
    private static readonly List<VisitJson> initialVisitJsonList =
    [
        new()
        {
            Created = "12/10/2025 13:50:02",
            CreatedBy = "TestUser",
            DateOfVisit = "12/10/2025 13:50:02",
            Id = "123",
            LoginName = "Test",
            Name = "TestUser",
            ParentId = "80",
            Type = "Case",
            Updated = "12/10/2025 13:50:02",
            UpdatedBy = "User A",
            VisitDescription = "Regular",
            VisitDetails = [new() { VisitDetailValue = "Visit Details 123" }],
        },
        new()
        {
            Created = "12/10/2018 13:50:02",
            CreatedBy = "TestUser",
            DateOfVisit = "12/10/2018 13:50:02",
            Id = "81",
            LoginName = "Test2",
            Name = "TestUser2",
            ParentId = "123",
            Type = "Case",
            Updated = "12/10/2018 13:50:02",
            UpdatedBy = "User A",
            VisitDescription = "Regular",
            VisitDetails = [new() { VisitDetailValue = "Visit Details abc" }],
        },
        new VisitJson
        {
            Created = "12/11/2025 13:50:02",
            CreatedBy = "TestUser3",
            DateOfVisit = "12/11/2025 13:50:02",
            Id = "87",
            LoginName = "Test",
            Name = "TestUser3",
            ParentId = "123",
            Type = "Case",
            Updated = "12/11/2025 13:50:02",
            UpdatedBy = "User B",
            VisitDescription = "Regular",
            VisitDetails = [new() { VisitDetailValue = "Visit Details 123" }],
        },
        new VisitJson
        {
            Created = "12/12/2025 13:50:02",
            CreatedBy = "TestUser4",
            DateOfVisit = "12/12/2025 13:50:02",
            Id = "86",
            LoginName = "Test",
            Name = "TestUser4",
            ParentId = "123",
            Type = "Case",
            Updated = "12/12/2025 13:50:02",
            UpdatedBy = "User B",
            VisitDescription = "Regular",
            VisitDetails = [new() { VisitDetailValue = "Visit Details 123" }],
        },
    ];

    [Fact]
    public async Task SynchronizeAsyncDeletesDifferenceFromRealm()
    {
        //Adding new visits into the realm
        var realm = await TestingUtilities.MakeRealm<PersonVisitTests>();
        List<VisitJson> visits = initialVisitJsonList;
        string parentId = "123";

        await PersonVisit.SynchronizeAsync(realm, visits, parentId);
        var numberOfPersonVisitsBeforeDeletion = realm.All<PersonVisit>().Count();

        //Checking deletion of realm objects
        visits.RemoveAll(item => item.Id == "86");
        await PersonVisit.SynchronizeAsync(realm, visits, parentId);

        var numberOfPersonVisitsAfterDeletion = realm.All<PersonVisit>().Count();

        Assert.Equal(4, numberOfPersonVisitsBeforeDeletion);
        Assert.Equal(numberOfPersonVisitsBeforeDeletion - 1, numberOfPersonVisitsAfterDeletion);
    }
}
