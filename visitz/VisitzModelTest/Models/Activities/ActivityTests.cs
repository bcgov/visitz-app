using Realms;
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
    private string localIdForACT1 = Activity.MakeLocalId(ActivityID1, CaseId, null, null, null);
    private string localIdForACT2 = Activity.MakeLocalId(ActivityID2, null, IncidentId, null, null);
    private string localIdForACT3 = Activity.MakeLocalId(ActivityID3, CaseId, IncidentId, null, null);
    private string localIdForACT4 = Activity.MakeLocalId(ActivityID4, CaseId, null, SRId, null);
    private string localIdForACT5 = Activity.MakeLocalId(ActivityID5, CaseId, null, null, MemoId);
    private string localIdForACT6 = Activity.MakeLocalId(ActivityID6, null, null, null, MemoId);
    private string localIdForACT7 = Activity.MakeLocalId(ActivityID7, null, null, SRId, null);

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

    [Theory]
    // Case removal - Incident remains
    [InlineData(EntityType.Case, EntityType.Incident, ActivityID1, false)]
    [InlineData(EntityType.Case, EntityType.Incident, ActivityID2, true)]
    [InlineData(EntityType.Case, EntityType.Incident, ActivityID3, true)]
    // Incident removal - Case remains
    [InlineData(EntityType.Incident, EntityType.Case, ActivityID1, true)]
    [InlineData(EntityType.Incident, EntityType.Case, ActivityID2, false)]
    [InlineData(EntityType.Incident, EntityType.Case, ActivityID3, true)]
    // Case removal - Service Request remains
    [InlineData(EntityType.Case, EntityType.ServiceRequest, ActivityID1, false)]
    [InlineData(EntityType.Case, EntityType.ServiceRequest, ActivityID4, true)]
    // Case removal - Memo remains
    [InlineData(EntityType.Case, EntityType.Memo, ActivityID1, false)]
    [InlineData(EntityType.Case, EntityType.Memo, ActivityID5, true)]
    // Memo removal - Case remains
    [InlineData(EntityType.Memo, EntityType.Case, ActivityID5, true)]
    [InlineData(EntityType.Memo, EntityType.Case, ActivityID6, false)]
    // Service Request removal - Case remains
    [InlineData(EntityType.ServiceRequest, EntityType.Case, ActivityID4, true)]
    [InlineData(EntityType.ServiceRequest, EntityType.Case, ActivityID7, false)]
    public async Task RemoveByParent_ShouldHandleActivities(
        EntityType parent1_type,
        EntityType parent2_type,
        string activityId,
        bool shouldExist
    )
    {
        var realm = await TestingUtilities.MakeRealm<ActivityTests>(Guid.NewGuid().ToString());

        // Create parent records.
        realm.Write(() =>
        {
            AddParent(realm, parent1_type);
            AddParent(realm, parent2_type);
        });

        // Populate activities through synchronization.
        await SynchronizeActivities(realm, parent1_type, parent2_type);

        // Remove associated activities and then the parent.
        realm.Write(() =>
        {
            Activity.RemoveByParent(realm, parent1_type, GetParentId(parent1_type));

            RemoveParent(realm, parent1_type);
        });

        // Verify the expected activity state.
        var localId = GetLocalId(activityId);

        Assert.Equal(shouldExist, realm.Find<Activity>(localId) != null);
    }

    private static string GetParentId(EntityType type) =>
        type switch
        {
            EntityType.Case => CaseId,
            EntityType.Incident => IncidentId,
            EntityType.ServiceRequest => SRId,
            EntityType.Memo => MemoId,
            _ => throw new InvalidOperationException(nameof(type)),
        };

    private static void AddParent(Realm realm, EntityType type)
    {
        switch (type)
        {
            case EntityType.Case:
                realm.Add(new CaseRecord { Id = CaseId });
                break;

            case EntityType.Incident:
                realm.Add(new IncidentRecord { Id = IncidentId });
                break;

            case EntityType.ServiceRequest:
                realm.Add(new ServiceRequestRecord { Id = SRId });
                break;

            case EntityType.Memo:
                realm.Add(new MemoRecord { Id = MemoId });
                break;

            default:
                throw new InvalidOperationException(nameof(type));
        }
    }

    private static void RemoveParent(Realm realm, EntityType type)
    {
        switch (type)
        {
            case EntityType.Case:
                realm.Remove(realm.Find<CaseRecord>(CaseId)!);
                break;

            case EntityType.Incident:
                realm.Remove(realm.Find<IncidentRecord>(IncidentId)!);
                break;

            case EntityType.ServiceRequest:
                realm.Remove(realm.Find<ServiceRequestRecord>(SRId)!);
                break;

            case EntityType.Memo:
                realm.Remove(realm.Find<MemoRecord>(MemoId)!);
                break;

            default:
                throw new InvalidOperationException(nameof(type));
        }
    }

    private static async Task SynchronizeActivities(Realm realm, EntityType removeType, EntityType otherType)
    {
        // Case + Incident scenario.
        // Synchronize from both parents to create all 3 activities.
        if (
            (removeType == EntityType.Case && otherType == EntityType.Incident)
            || (removeType == EntityType.Incident && otherType == EntityType.Case)
        )
        {
            await Activity.SynchronizeAsync(realm, ActivitiesForCase, CaseId, EntityType.Case);

            await Activity.SynchronizeAsync(realm, ActivitiesForIncident, IncidentId, EntityType.Incident);

            return;
        }

        // Other parent combinations.
        List<ActivityJson> activities = (removeType, otherType) switch
        {
            (EntityType.Case, EntityType.ServiceRequest) => ActivitiesForCaseAndSR,

            (EntityType.Case, EntityType.Memo) => ActivitiesForCaseAndMemo,

            (EntityType.Memo, EntityType.Case) => ActivitiesForMemo,

            (EntityType.ServiceRequest, EntityType.Case) => ActivitiesForSR,

            _ => throw new InvalidOperationException(nameof(removeType)),
        };

        await Activity.SynchronizeAsync(realm, activities, GetParentId(removeType), removeType);
    }

    private static string GetLocalId(string activityId) =>
        activityId switch
        {
            ActivityID1 => Activity.MakeLocalId(ActivityID1, CaseId, null, null, null),

            ActivityID2 => Activity.MakeLocalId(ActivityID2, null, IncidentId, null, null),

            ActivityID3 => Activity.MakeLocalId(ActivityID3, CaseId, IncidentId, null, null),

            ActivityID4 => Activity.MakeLocalId(ActivityID4, CaseId, null, SRId, null),

            ActivityID5 => Activity.MakeLocalId(ActivityID5, CaseId, null, null, MemoId),

            ActivityID6 => Activity.MakeLocalId(ActivityID6, null, null, null, MemoId),

            ActivityID7 => Activity.MakeLocalId(ActivityID7, null, null, SRId, null),

            _ => throw new InvalidOperationException(nameof(activityId)),
        };
}
