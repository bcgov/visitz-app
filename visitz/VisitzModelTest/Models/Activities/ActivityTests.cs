using VisitzApi.Models.Activities;
using VisitzModel.Models.Activities;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace VisitzModelTest.Models.Activities;

public class ActivityTests
{
    private const string CaseId = "CASE123";
    private const string IncidentId = "INC123";
    private const string MemoId = "MEM123";
    private const string SRId = "SR123";
    private const string ActivityID1 = "ACT1";
    private const string ActivityID2 = "ACT2";
    private const string ActivityID3 = "ACT3";
    private const string ActivityID4 = "ACT4";
    private const string ActivityID5 = "ACT5";
    private const string ActivityID6 = "ACT6";
    private const string ActivityID7 = "ACT7";
    private string localIdForACT1 = Activity.MakeLocalId(ActivityID1, CaseId);
    private string localIdForACT2 = Activity.MakeLocalId(ActivityID2, IncidentId);
    private string localIdForACT3 = Activity.MakeLocalId(ActivityID3, CaseId, IncidentId);
    private string localIdForACT4 = Activity.MakeLocalId(ActivityID4, CaseId, SRId);
    private string localIdForACT5 = Activity.MakeLocalId(ActivityID5, CaseId, MemoId);
    private string localIdForACT6 = Activity.MakeLocalId(ActivityID6, MemoId);
    private string localIdForACT7 = Activity.MakeLocalId(ActivityID7, SRId);

    CaseRecord @case = new CaseRecord { Id = CaseId };
    IncidentRecord incident = new IncidentRecord { Id = IncidentId };
    ServiceRequestRecord sr = new ServiceRequestRecord { Id = SRId };
    MemoRecord memo = new MemoRecord { Id = MemoId };

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
    private static List<ActivityJson> ActivitiesForCaseAndSR =>
        [
            new() { Id = ActivityID1, CaseId = CaseId },
            new()
            {
                Id = ActivityID4,
                CaseId = CaseId,
                ActivitySRId = SRId,
            },
        ];
    private static List<ActivityJson> ActivitiesForCaseAndMemo =>
        [
            new() { Id = ActivityID1, CaseId = CaseId },
            new()
            {
                Id = ActivityID5,
                CaseId = CaseId,
                ICMMemoId = MemoId,
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
    private static List<ActivityJson> ActivitiesForMemo =>
        [
            new() { Id = ActivityID6, ICMMemoId = MemoId },
            new()
            {
                Id = ActivityID5,
                CaseId = CaseId,
                ICMMemoId = MemoId,
            },
        ];
    private static List<ActivityJson> ActivitiesForSR =>
        [
            new() { Id = ActivityID7, ActivitySRId = SRId },
            new()
            {
                Id = ActivityID4,
                CaseId = CaseId,
                ActivitySRId = SRId,
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

    [Fact]
    public async Task RemoveByParentWhenCaseIsRemovedAndSRStays()
    {
        var realm = await TestingUtilities.MakeRealm<ActivityTests>();

        // Create Case and SR records.
        realm.Write(() =>
        {
            realm.Add(@case);
            realm.Add(sr);
        });

        // Populate activities using synchronization.
        await Activity.SynchronizeAsync(realm, ActivitiesForCaseAndSR, CaseId, EntityType.Case);

        //Confirm all 3 activities are in Realm
        Assert.Equal(2, realm.All<Activity>().Count());

        // Remove the Case , by removing related activities first.
        realm.Write(() =>
        {
            Activity.RemoveByParent(realm, EntityType.Case, CaseId);
            realm.Remove(@case);
        });

        //Only 2 activities should be left.The one only related to CaseRecord should be removed
        Assert.Equal(1, realm.All<Activity>().Count());
        Assert.Null(realm.Find<Activity>(localIdForACT1));
        Assert.NotNull(realm.Find<Activity>(localIdForACT4));
    }

    [Fact]
    public async Task RemoveByParentWhenCaseIsRemovedAndMemoStays()
    {
        var realm = await TestingUtilities.MakeRealm<ActivityTests>();

        // Create Case and SR records.
        realm.Write(() =>
        {
            realm.Add(@case);
            realm.Add(memo);
        });

        // Populate activities using synchronization.
        await Activity.SynchronizeAsync(realm, ActivitiesForCaseAndMemo, CaseId, EntityType.Case);

        //Confirm all 2 activities are in Realm
        Assert.Equal(2, realm.All<Activity>().Count());
        Assert.NotNull(realm.Find<Activity>(localIdForACT1));
        Assert.NotNull(realm.Find<Activity>(localIdForACT5));

        // Remove the Case , by removing related activities first.
        realm.Write(() =>
        {
            Activity.RemoveByParent(realm, EntityType.Case, CaseId);
            realm.Remove(@case);
        });

        //Only 2 activities should be left.The one only related to CaseRecord should be removed
        Assert.Equal(1, realm.All<Activity>().Count());
        Assert.Null(realm.Find<Activity>(localIdForACT1));
        Assert.NotNull(realm.Find<Activity>(localIdForACT5));
    }

    [Fact]
    public async Task RemoveByParentWhenMemoIsRemoved()
    {
        var realm = await TestingUtilities.MakeRealm<ActivityTests>();

        // Create Case and SR records.
        realm.Write(() =>
        {
            realm.Add(@case);
            realm.Add(memo);
        });

        // Populate activities using synchronization.
        await Activity.SynchronizeAsync(realm, ActivitiesForMemo, MemoId, EntityType.Memo);

        //Confirm all 2 activities are in Realm
        Assert.Equal(2, realm.All<Activity>().Count());
        Assert.NotNull(realm.Find<Activity>(localIdForACT5));
        Assert.NotNull(realm.Find<Activity>(localIdForACT6));

        // Remove the Case , by removing related activities first.
        realm.Write(() =>
        {
            Activity.RemoveByParent(realm, EntityType.Memo, MemoId);
            realm.Remove(memo);
        });

        //Only 2 activities should be left.The one only related to CaseRecord should be removed
        Assert.Equal(1, realm.All<Activity>().Count());
        Assert.NotNull(realm.Find<Activity>(localIdForACT5));
        Assert.Null(realm.Find<Activity>(localIdForACT6));
    }

    [Fact]
    public async Task RemoveByParentWhenSRIsRemoved()
    {
        var realm = await TestingUtilities.MakeRealm<ActivityTests>();

        // Create Case and SR records.
        realm.Write(() =>
        {
            realm.Add(@case);
            realm.Add(sr);
        });

        // Populate activities using synchronization.
        await Activity.SynchronizeAsync(realm, ActivitiesForSR, MemoId, EntityType.Memo);

        //Confirm all 2 activities are in Realm
        Assert.Equal(2, realm.All<Activity>().Count());
        Assert.NotNull(realm.Find<Activity>(localIdForACT7));
        Assert.NotNull(realm.Find<Activity>(localIdForACT4));

        // Remove the Case , by removing related activities first.
        realm.Write(() =>
        {
            Activity.RemoveByParent(realm, EntityType.ServiceRequest, SRId);
            realm.Remove(sr);
        });

        //Only 2 activities should be left.The one only related to CaseRecord should be removed
        Assert.Equal(1, realm.All<Activity>().Count());
        Assert.NotNull(realm.Find<Activity>(localIdForACT4));
        Assert.Null(realm.Find<Activity>(localIdForACT7));
    }
}
