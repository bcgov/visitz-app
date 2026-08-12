using Realms;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.People;

namespace VisitzModel.Storage.Migrations;

public static class IcmDataMigrations
{
    public static void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        MigrateCaseloadItems(migration, oldSchemaVersion);
        MigratePersonVisits(migration, oldSchemaVersion);
        MigrateContacts(migration, oldSchemaVersion);
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

    static void MigrateContacts(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion < VisitzRealmBase.Version3_0_0)
        {
            VisitzRealmBase.MapAll<IcmContact>(
                "IcmContact",
                migration,
                (n, o) =>
                {
                    n.LocalId = o.DynamicApi.Get<string>("LocalId") ?? string.Empty;
                    n.Id = o.DynamicApi.Get<string>("Id") ?? string.Empty;
                    n.CreatedBy = o.DynamicApi.Get<string>("CreatedBy") ?? string.Empty;
                    n.CreatedById = o.DynamicApi.Get<string>("CreatedById") ?? string.Empty;
                    n.UpdatedBy = o.DynamicApi.Get<string>("UpdatedBy") ?? string.Empty;
                    n.UpdatedById = o.DynamicApi.Get<string>("UpdatedById") ?? string.Empty;
                    n.CreatedDate = o.DynamicApi.Get<DateTimeOffset>("CreatedDate");
                    n.UpdatedDate = o.DynamicApi.Get<DateTimeOffset>("UpdatedDate");
                    n.ParentId = o.DynamicApi.Get<string>("ParentId") ?? string.Empty;
                    n.ParentTypeInt = o.DynamicApi.Get<int>("ParentTypeInt");
                    n._921Agt = o.DynamicApi.Get<string>("_921Agt") ?? string.Empty;
                    n.ActiveAddresses = o.DynamicApi.Get<int>("ActiveAddresses");
                    n.Age = o.DynamicApi.Get<int>("Age");
                    n.AkaFirstName = o.DynamicApi.Get<string>("AkaFirstName") ?? string.Empty;
                    n.AkaLastName = o.DynamicApi.Get<string>("AkaLastName") ?? string.Empty;
                    n.Alerts = o.DynamicApi.Get<string>("Alerts") ?? string.Empty;
                    n.AutismFundingPaused = o.DynamicApi.Get<string>("AutismFundingPaused") ?? string.Empty;
                    n.BceIdUserName = o.DynamicApi.Get<string>("BceIdUserName") ?? string.Empty;
                    n.CanadianCitizen = o.DynamicApi.Get<string>("CanadianCitizen") ?? string.Empty;
                    n.CellPhone = o.DynamicApi.Get<string>("CellPhone") ?? string.Empty;
                    n.Citizen = o.DynamicApi.Get<string>("Citizen") ?? string.Empty;
                    n.Citizenship = o.DynamicApi.Get<string>("Citizenship") ?? string.Empty;
                    n.City = o.DynamicApi.Get<string>("City") ?? string.Empty;
                    n.CollaborateId = o.DynamicApi.Get<string>("CollaborateId") ?? string.Empty;
                    n.Comments = o.DynamicApi.Get<string>("Comments") ?? string.Empty;
                    n.ConcernsOutcome = o.DynamicApi.Get<string>("ConcernsOutcome") ?? string.Empty;
                    n.CoordinationAgtCa = o.DynamicApi.Get<string>("CoordinationAgtCa") ?? string.Empty;
                    n.Country = o.DynamicApi.Get<string>("Country") ?? string.Empty;
                    n.CountryOfBirth = o.DynamicApi.Get<string>("CountryOfBirth") ?? string.Empty;
                    n.CurrentStartDate = o.DynamicApi.Get<DateTimeOffset?>("CurrentStartDate");
                    n.Cysn = o.DynamicApi.Get<string>("Cysn") ?? string.Empty;
                    n.DateOfBirth = o.DynamicApi.Get<DateTimeOffset?>("DateOfBirth");
                    n.CitizenUpdatedDate = o.DynamicApi.Get<DateTimeOffset?>("CitizenUpdatedDate");
                    n.CitizenshipUpdatedDate = o.DynamicApi.Get<DateTimeOffset?>("CitizenshipUpdatedDate");
                    n.Deceased = o.DynamicApi.Get<string>("Deceased") ?? string.Empty;
                    n.DeceasedDate = o.DynamicApi.Get<DateTimeOffset?>("DeceasedDate");
                    n.EndDate = o.DynamicApi.Get<string>("EndDate") ?? string.Empty;
                    n.FirstName = o.DynamicApi.Get<string>("FirstName") ?? string.Empty;
                    n.Gender = o.DynamicApi.Get<string>("Gender") ?? string.Empty;
                    n.GivenNames = o.DynamicApi.Get<string>("GivenNames") ?? string.Empty;
                    n.HomePhone = o.DynamicApi.Get<string>("HomePhone") ?? string.Empty;
                    n.ImmigrationStatus = o.DynamicApi.Get<string>("ImmigrationStatus") ?? string.Empty;
                    n.ImmigrationStatusUpdated = o.DynamicApi.Get<string>("ImmigrationStatusUpdated") ?? string.Empty;
                    n.Indigenous = o.DynamicApi.Get<string>("Indigenous") ?? string.Empty;
                    n.IntegrationState = o.DynamicApi.Get<string>("IntegrationState") ?? string.Empty;
                    n.InvestigationOutcomeSummary =
                        o.DynamicApi.Get<string>("InvestigationOutcomeSummary") ?? string.Empty;
                    n.LastName = o.DynamicApi.Get<string>("LastName") ?? string.Empty;
                    n.LegacyDependentSequence = o.DynamicApi.Get<string>("LegacyDependentSequence") ?? string.Empty;
                    n.LegalStatus = o.DynamicApi.Get<string>("LegalStatus") ?? string.Empty;
                    n.MessagePhone = o.DynamicApi.Get<string>("MessagePhone") ?? string.Empty;
                    n.MiddleNames = o.DynamicApi.Get<string>("MiddleNames") ?? string.Empty;
                    n.OriginalStartDate = o.DynamicApi.Get<DateTimeOffset?>("OriginalStartDate");
                    n.IsParentCaregiver = o.DynamicApi.Get<bool>("IsParentCaregiver");
                    n.PersonIdIcm = o.DynamicApi.Get<string>("PersonIdIcm") ?? string.Empty;
                    n.PersonIdMis = o.DynamicApi.Get<string>("PersonIdMis") ?? string.Empty;
                    n.ResponsibleForAllegedMaltreatment = o.DynamicApi.Get<bool?>("ResponsibleForAllegedMaltreatment");
                    n.PersonalHealthNumber = o.DynamicApi.Get<string>("PersonalHealthNumber") ?? string.Empty;
                    n.PersonalHealthNumberVerified =
                        o.DynamicApi.Get<string>("PersonalHealthNumberVerified") ?? string.Empty;
                    n.PostalCode = o.DynamicApi.Get<string>("PostalCode") ?? string.Empty;
                    n.PotentialDuplicate = o.DynamicApi.Get<string>("PotentialDuplicate") ?? string.Empty;
                    n.PotentialDuplicateComments =
                        o.DynamicApi.Get<string>("PotentialDuplicateComments") ?? string.Empty;
                    n.PreferredLanguage = o.DynamicApi.Get<string>("PreferredLanguage") ?? string.Empty;
                    n.Primary = o.DynamicApi.Get<string>("Primary") ?? string.Empty;
                    n.PrimaryAddress = o.DynamicApi.Get<string>("PrimaryAddress") ?? string.Empty;
                    n.PrimaryEmail = o.DynamicApi.Get<string>("PrimaryEmail") ?? string.Empty;
                    n.ProjectCode = o.DynamicApi.Get<string>("ProjectCode") ?? string.Empty;
                    n.Province = o.DynamicApi.Get<string>("Province") ?? string.Empty;
                    n.PstScore = o.DynamicApi.Get<string>("PstScore") ?? string.Empty;
                    n.Relationship = o.DynamicApi.Get<string>("Relationship") ?? string.Empty;
                    n.Role = o.DynamicApi.Get<string>("Role") ?? string.Empty;
                    n.RowId = o.DynamicApi.Get<string>("RowId") ?? string.Empty;
                    n.SaetPaused = o.DynamicApi.Get<string>("SaetPaused") ?? string.Empty;
                    n.SocialInsuranceNumber = o.DynamicApi.Get<string>("SocialInsuranceNumber") ?? string.Empty;
                    n.StartDate = o.DynamicApi.Get<DateTimeOffset?>("StartDate");
                    n.StreetAddress = o.DynamicApi.Get<string>("StreetAddress") ?? string.Empty;
                    n.StreetAddress2 = o.DynamicApi.Get<string>("StreetAddress2") ?? string.Empty;
                    n.Subject = o.DynamicApi.Get<string>("Subject") ?? string.Empty;
                    n.IsSubjectChild = o.DynamicApi.Get<bool>("IsSubjectChild");
                    n.Title = o.DynamicApi.Get<string>("Title") ?? string.Empty;
                    n.UnitNumber = o.DynamicApi.Get<string>("UnitNumber") ?? string.Empty;
                    n.WorkPhone = o.DynamicApi.Get<string>("WorkPhone") ?? string.Empty;
                }
            );
        }
    }
}
