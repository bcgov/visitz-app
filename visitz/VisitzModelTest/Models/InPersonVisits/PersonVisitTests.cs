using VisitzApi.Models.Visits;
using VisitzModel.Models.InPersonVisits;

namespace VisitzModelTest.Models.InPersonVisits;

public class PersonVisitTests
{
    private static readonly List<VisitJson> visitJsons =
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
            VisitDetails =
            [
                new ()
                {
                    VisitDetailValue = "Visit Details 123"
                }
            ]
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
            VisitDetails =
            [
                new ()
                {
                    VisitDetailValue = "Visit Details abc"
                }
            ]
        }
    ];

    [Fact]
    public async Task SynchronizeAsync()
    {
        //Adding new visits into the realm
        var realm = await TestingUtilities.MakeRealm<PersonVisitTests>();
        List<VisitJson> visits = visitJsons;

        await realm.Write(async () => await PersonVisit.SynchronizeAsync(
            realm,
            visits));
        
        var allVisits = PersonVisit.GetAllByType(realm).ToList();

        Assert.Equal(visits.Count, allVisits.Count);

        //Checking deletion of realm objects

        visits.Clear();
        visits.AddRange(
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
                VisitDetails =
                [
                    new ()
                    {
                        VisitDetailValue = "Visit Details 123"
                    }
                ]            
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
                VisitDetails =
                [
                    new ()
                    {
                        VisitDetailValue = "Visit Details 123"
                    }
                ]
            },
            new VisitJson
            {
                Created = "12/01/2026 13:50:02",
                CreatedBy = "TestUser5",
                DateOfVisit = "12/01/2026 13:50:02",
                Id = "85",
                LoginName = "Test",
                Name = "TestUser5",
                ParentId = "123",
                Type = "Case",
                Updated = "12/01/2026 13:50:02",
                UpdatedBy = "User B",
                VisitDescription = "Regular",
                VisitDetails =
                [
                    new ()
                    {
                        VisitDetailValue = "Visit Details 123"
                    }
                ]
            });

        await realm.Write(async () => await PersonVisit.SynchronizeAsync(
            realm,
            visits));

        allVisits = PersonVisit.GetAllByType(realm).ToList();

        Assert.Equal(3, allVisits.Count);
    }   
}
