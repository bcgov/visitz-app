using VisitzApi.Models.Activities;
using VisitzModel.Models.Activities;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace VisitzModelTest.Models.Activities;

public class ActivityTests
{
    private const string CaseId = "CASE123";
    private const string IncidentId = "INC123";
    private const string ActivityID1 = "ACT1";
    private const string ActivityID2 = "ACT2";
    private const string ActivityID3 = "ACT3";
    private string localIdForACT1 = Activity.MakeLocalId(ActivityID1, CaseId);
    private string localIdForACT2 = Activity.MakeLocalId(ActivityID2, IncidentId);
    private string localIdForACT3 = Activity.MakeLocalId(ActivityID3, CaseId, IncidentId);

    CaseRecord @case = new CaseRecord { Id = CaseId };
    IncidentRecord incident = new IncidentRecord { Id = IncidentId };

    private static List<ActivityJson> ActivitiesForCase =>
        [
            new() { Id = ActivityID1, CaseId = CaseId },
            new()
            {
                Id = ActivityID3,
                CaseId = CaseId,
                IncidentId = IncidentId,
            },
        ];
    private static List<ActivityJson> ActivitiesForIncident =>
        [
            new() { Id = ActivityID2, IncidentId = IncidentId },
            new()
            {
                Id = ActivityID3,
                CaseId = CaseId,
                IncidentId = IncidentId,
            },
        ];

    [Fact]
    public async Task RemoveByParentWhenCaseIsRemoved()
    {
        var realm = await TestingUtilities.MakeRealm<ActivityTests>();

        // Create Case and Incident records.
        realm.Write(() =>
        {
            realm.Add(@case);
            realm.Add(incident);
        });

        // Populate activities using synchronization.
        await Activity.SynchronizeAsync(realm, ActivitiesForCase, CaseId, EntityType.Case);
        await Activity.SynchronizeAsync(realm, ActivitiesForIncident, IncidentId, EntityType.Incident);

        //Confirm all 3 activities are in Realm
        Assert.Equal(3, realm.All<Activity>().Count());

        // Remove the Case , by removing related activities first.
        realm.Write(() =>
        {
            Activity.RemoveByParent(realm, EntityType.Case, CaseId);
            realm.Remove(@case);
        });

        //Only 2 activities should be left.The one only related to CaseRecord should be removed
        Assert.Equal(2, realm.All<Activity>().Count());
        Assert.Null(realm.Find<Activity>(localIdForACT1));        
        Assert.NotNull(realm.Find<Activity>(localIdForACT3));
        Assert.NotNull(realm.Find<Activity>(localIdForACT2));
    }

    [Fact]
    public async Task RemoveByParentWhenIncidentIsRemoved()
    {
        var realm = await TestingUtilities.MakeRealm<ActivityTests>();

        // Create Case and Incident records.
        realm.Write(() =>
        {
            realm.Add(@case);
            realm.Add(incident);
        });

        // Populate activities using synchronization.
        await Activity.SynchronizeAsync(realm, ActivitiesForCase, CaseId, EntityType.Case);
        await Activity.SynchronizeAsync(realm, ActivitiesForIncident, IncidentId, EntityType.Incident);

        Assert.Equal(3, realm.All<Activity>().Count());

        // Remove the Case.Also remove activities related
        realm.Write(() =>
        {
            Activity.RemoveByParent(realm, EntityType.Incident, IncidentId);
            realm.Remove(incident);
        });

        //Only 2 activities should be left.The one only related to IncidentRecord should be removed
        Assert.Equal(2, realm.All<Activity>().Count());        
        Assert.NotNull(realm.Find<Activity>(localIdForACT1));        
        Assert.NotNull(realm.Find<Activity>(localIdForACT3));
        Assert.Null(realm.Find<Activity>(localIdForACT2));
    }
}
