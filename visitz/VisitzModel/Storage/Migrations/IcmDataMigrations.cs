using Realms;
using VisitzModel.Models.Caseload;

namespace VisitzModel.Storage.Migrations;

public static class IcmDataMigrations
{
    public static void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        MigrateCaseloadItems(migration, oldSchemaVersion);
        MigratePersonVisits(migration, oldSchemaVersion);
    }

    static void MigrateCaseloadItems(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion < VisitzRealmBase.Version2_7_0)
        {
            const string CaseloadItemName = "CaseloadItem";

            var newCaseloadItems = migration.NewRealm.DynamicApi.All(CaseloadItemName);

            migration.NewRealm.RemoveRange(newCaseloadItems);
        }

        if (oldSchemaVersion < VisitzRealmBase.Version2_8_0)
        {
            foreach (var @case in migration.NewRealm.All<CaseRecord>())
                if (@case.Realm != null)
                    ((IBusinessObject)@case).UpsertLocalState(@case.Realm, false);

            foreach (var incident in migration.NewRealm.All<IncidentRecord>())
                if (incident.Realm != null)
                    ((IBusinessObject)incident).UpsertLocalState(incident.Realm, false);
        }

        if (oldSchemaVersion < VisitzRealmBase.Version3_0_0)
        {
            VisitzRealmBase.MapAll<CaseRecord>(
                "CaseRecord",
                migration,
                (n, o) =>
                {
                    n.Id = o.DynamicApi.Get<string>("Id") ?? string.Empty;
                    n.CreatedBy = o.DynamicApi.Get<string>("CreatedBy") ?? string.Empty;
                    n.CreatedById = o.DynamicApi.Get<string>("CreatedById") ?? string.Empty;
                    n.UpdatedBy = o.DynamicApi.Get<string>("UpdatedBy") ?? string.Empty;
                    n.UpdatedById = o.DynamicApi.Get<string>("UpdatedById") ?? string.Empty;
                    n.CreatedDate = o.DynamicApi.Get<DateTimeOffset>("CreatedDate");
                    n.UpdatedDate = o.DynamicApi.Get<DateTimeOffset>("UpdatedDate");
                    n.FileNumber = o.DynamicApi.Get<string>("FileNumber") ?? string.Empty;
                    n.GivenNames = o.DynamicApi.Get<string>("GivenNames") ?? string.Empty;
                    n.LastName = o.DynamicApi.Get<string>("LastName") ?? string.Empty;
                    n.AssignedTo = o.DynamicApi.Get<string>("AssignedTo") ?? string.Empty;
                    n.AssignedToId = o.DynamicApi.Get<string>("AssignedToId") ?? string.Empty;

                    foreach (string assignee in o.DynamicApi.GetList<string>("Assignees"))
                        n.Assignees.Add(assignee);

                    n.Caseload = o.DynamicApi.Get<string>("Caseload") ?? string.Empty;
                    n.ClosedDate = o.DynamicApi.Get<DateTimeOffset?>("ClosedDate");
                    n.CloseReason = o.DynamicApi.Get<string>("CloseReason") ?? string.Empty;
                    n.EarlyOpenReason = o.DynamicApi.Get<string>("EarlyOpenReason") ?? string.Empty;
                    n.IntegrationState = o.DynamicApi.Get<string>("IntegrationState") ?? string.Empty;
                    n.LegacyFileNumber = o.DynamicApi.Get<string>("LegacyFileNumber") ?? string.Empty;
                    n.MiddleName = o.DynamicApi.Get<string>("MiddleName") ?? string.Empty;
                    n.MyFSFlag = o.DynamicApi.Get<bool>("MyFSFlag");
                    n.Name = o.DynamicApi.Get<string>("Name") ?? string.Empty;
                    n.ServiceOffice = o.DynamicApi.Get<string>("ServiceOffice") ?? string.Empty;
                    n.Organization = o.DynamicApi.Get<string>("Organization") ?? string.Empty;
                    n.RegionName = o.DynamicApi.Get<string>("RegionName") ?? string.Empty;
                    n.RenewReviewDate = o.DynamicApi.Get<DateTimeOffset?>("RenewReviewDate");
                    n.ReopenedDate = o.DynamicApi.Get<DateTimeOffset?>("ReopenedDate");
                    n.RestrictedFlag = o.DynamicApi.Get<bool>("RestrictedFlag");
                    n.Status = o.DynamicApi.Get<string>("Status") ?? string.Empty;
                    n.TypeInt = o.DynamicApi.Get<int>("TypeInt"); // EntitySubtype
                    n.WorkQueue = o.DynamicApi.Get<string>("WorkQueue") ?? string.Empty;
                    ((IBusinessObject)n).UpsertLocalState(migration.NewRealm);
                }
            );

            VisitzRealmBase.MapAll<IncidentRecord>(
                "IncidentRecord",
                migration,
                (n, o) =>
                {
                    n.Id = o.DynamicApi.Get<string>("Id") ?? string.Empty;
                    n.CreatedBy = o.DynamicApi.Get<string>("CreatedBy") ?? string.Empty;
                    n.CreatedById = o.DynamicApi.Get<string>("CreatedById") ?? string.Empty;
                    n.UpdatedBy = o.DynamicApi.Get<string>("UpdatedBy") ?? string.Empty;
                    n.UpdatedById = o.DynamicApi.Get<string>("UpdatedById") ?? string.Empty;
                    n.CreatedDate = o.DynamicApi.Get<DateTimeOffset>("CreatedDate");
                    n.UpdatedDate = o.DynamicApi.Get<DateTimeOffset>("UpdatedDate");
                    n.FileNumber = o.DynamicApi.Get<string>("FileNumber") ?? string.Empty;
                    n.GivenNames = o.DynamicApi.Get<string>("GivenNames") ?? string.Empty;
                    n.LastName = o.DynamicApi.Get<string>("LastName") ?? string.Empty;
                    n.AssignedTo = o.DynamicApi.Get<string>("AssignedTo") ?? string.Empty;
                    n.AssignedToId = o.DynamicApi.Get<string>("AssignedToId") ?? string.Empty;

                    foreach (string assignee in o.DynamicApi.GetList<string>("Assignees"))
                        n.Assignees.Add(assignee ?? string.Empty);

                    n.AddressComments = o.DynamicApi.Get<string>("AddressComments") ?? string.Empty;
                    n.Address = o.DynamicApi.Get<string>("Address") ?? string.Empty;
                    n.AreAnyOfTheFamilyMembersIndigenous =
                        o.DynamicApi.Get<string>("AreAnyOfTheFamilyMembersIndigenous") ?? string.Empty;
                    n.CallerAddress = o.DynamicApi.Get<string>("CallerAddress") ?? string.Empty;
                    n.CallerEmail = o.DynamicApi.Get<string>("CallerEmail") ?? string.Empty;
                    n.CallerName = o.DynamicApi.Get<string>("CallerName") ?? string.Empty;
                    n.CallerPhone = o.DynamicApi.Get<string>("CallerPhone") ?? string.Empty;
                    n.Caseload = o.DynamicApi.Get<string>("Caseload") ?? string.Empty;
                    n.CellPhone = o.DynamicApi.Get<string>("CellPhone") ?? string.Empty;
                    n.ClosedDate = o.DynamicApi.Get<DateTimeOffset?>("ClosedDate");
                    n.CreatedByOffice = o.DynamicApi.Get<string>("CreatedByOffice") ?? string.Empty;
                    n.DateReported = o.DynamicApi.Get<DateTimeOffset?>("DateReported");
                    n.HomePhone = o.DynamicApi.Get<string>("HomePhone") ?? string.Empty;
                    n.MedicalExamRequired = o.DynamicApi.Get<string>("MedicalExamRequired") ?? string.Empty;
                    n.Method = o.DynamicApi.Get<string>("Method") ?? string.Empty;
                    n.NatureOfCall = o.DynamicApi.Get<string>("NatureOfCall") ?? string.Empty;
                    n.PccSummary = o.DynamicApi.Get<string>("PccSummary") ?? string.Empty;
                    n.PoliceForce = o.DynamicApi.Get<string>("PoliceForce") ?? string.Empty;
                    n.PoliceInvestigation = o.DynamicApi.Get<string>("PoliceInvestigation") ?? string.Empty;
                    n.PoliceNotifiedDate = o.DynamicApi.Get<DateTimeOffset?>("PoliceNotifiedDate");
                    n.PoliceReportNumber = o.DynamicApi.Get<string>("PoliceReportNumber") ?? string.Empty;
                    n.PreferredContactMethod = o.DynamicApi.Get<string>("PreferredContactMethod") ?? string.Empty;
                    n.ProtectionResponse = o.DynamicApi.Get<string>("ProtectionResponse") ?? string.Empty;
                    n.Resolution = o.DynamicApi.Get<string>("Resolution") ?? string.Empty;
                    n.ResponsePriority = o.DynamicApi.Get<string>("ResponsePriority") ?? string.Empty;
                    n.RestrictedFlag = o.DynamicApi.Get<bool>("RestrictedFlag");
                    n.ServiceOffice = o.DynamicApi.Get<string>("ServiceOffice") ?? string.Empty;
                    n.Status = o.DynamicApi.Get<string>("Status") ?? string.Empty;
                    n.TypeInt = o.DynamicApi.Get<int>("TypeInt");
                    n.TypeOfCaller = o.DynamicApi.Get<string>("TypeOfCaller") ?? string.Empty;
                    ((IBusinessObject)n).UpsertLocalState(migration.NewRealm);
                }
            );
        }
    }

    private static void MigratePersonVisits(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion < VisitzRealmBase.Version2_7_1)
            PersonVisitMigrations.Migrate_2_7_1(migration);
    }
}
