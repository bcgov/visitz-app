using Realms;
using VisitzModel.Models.SafetyAssess;

namespace VisitzModel.Storage.Migrations;

internal static class SafetyAssessmentMigrations
{
    internal static void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion < VisitzRealmBase.Version2_3_3)
            Migrate2_3_3(migration);
        if (oldSchemaVersion < VisitzRealmBase.Version3_0_0)
            Migrate3_0_0(migration);
    }

    static void Migrate2_3_3(Migration migration)
    {
        const string SafetyAssessmentName = "SafetyAssessment";
        const string AssessmentDraftName = "AssessmentDraft";

        const string IncidentNumberName = "IncidentNumber";
        const string DraftCreatedName = "DraftCreated";

        const string LastCreatedName = "LastUpdated";

        static void Create(Migration migration, IRealmObject oldItem)
        {
            string pk = oldItem.DynamicApi.Get<string>(IncidentNumberName) ?? string.Empty;
            var newDraft = migration.NewRealm.DynamicApi.CreateObject(AssessmentDraftName, pk);

            newDraft.DynamicApi.Set(DraftCreatedName, DateTimeOffset.MinValue);
            newDraft.DynamicApi.Set(LastCreatedName, DateTimeOffset.MinValue);
        }

        var oldItems = migration.OldRealm.DynamicApi.All(SafetyAssessmentName);

        for (int i = 0; i < oldItems.Count(); i++)
            Create(migration, oldItems.ElementAt(i));
    }

    static void Migrate3_0_0(Migration migration)
    {
        VisitzRealmBase.MapAll<SafetyAssessment>(
            "SafetyAssessment",
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
                n.IncidentNumber = o.DynamicApi.Get<string>("IncidentNumber") ?? string.Empty;
                n.WorkerId = o.DynamicApi.Get<string>("WorkerId") ?? string.Empty;
                n.FamilyName = o.DynamicApi.Get<string>("FamilyName") ?? string.Empty;
                n.DateOfAssessment = o.DynamicApi.Get<DateTimeOffset?>("DateOfAssessment");
                n.Operation = o.DynamicApi.Get<string>("Operation") ?? string.Empty;
                // No need to migrate to link:
                // - n.FactorInfluence
                // - n.SafetyFactors
                // - n.ProtectiveCapacity
                // - n.SafetyInterventions
                // - n.SafetyDecisions

                foreach (string child in o.DynamicApi.GetList<string>("ChildsInOutCare"))
                    n.ChildsInOutCare.Add(child);

                n.ApprovedBy = o.DynamicApi.Get<string>("ApprovedBy") ?? string.Empty;
                n.ApprovedDate = o.DynamicApi.Get<string>("ApprovedDate") ?? string.Empty;
                n.ApprovedToFinalize = o.DynamicApi.Get<string>("ApprovedToFinalize") ?? string.Empty;
                n.ApprovedToFinalizeDate = o.DynamicApi.Get<DateTimeOffset?>("ApprovedToFinalizeDate");
                n.FinalizedDate = o.DynamicApi.Get<DateTimeOffset?>("FinalizedDate");
                n.ApprovedToFinalizeDS = o.DynamicApi.Get<string>("ApprovedToFinalizeDS") ?? string.Empty;
                n.DataStewardRole = o.DynamicApi.Get<string>("DataStewardRole") ?? string.Empty;
                n.SocialWorkerFirstName = o.DynamicApi.Get<string>("SocialWorkerFirstName") ?? string.Empty;
                n.SocialWorkerId = o.DynamicApi.Get<string>("SocialWorkerId") ?? string.Empty;
                n.SocialWorkerLastName = o.DynamicApi.Get<string>("SocialWorkerLastName") ?? string.Empty;
                n.TeamLeaderFirstName = o.DynamicApi.Get<string>("TeamLeaderFirstName") ?? string.Empty;
                n.TeamLeaderId = o.DynamicApi.Get<string>("TeamLeaderId") ?? string.Empty;
                n.TeamLeaderLastName = o.DynamicApi.Get<string>("TeamLeaderLastName") ?? string.Empty;
                n.TeamLeaderLoginName = o.DynamicApi.Get<string>("TeamLeaderLoginName") ?? string.Empty;
                n.Type = o.DynamicApi.Get<string>("Type") ?? string.Empty;
            }
        );

        VisitzRealmBase.MapAll<FactorInfluence>(
            "FactorInfluence",
            migration,
            (n, o) =>
            {
                n.AgeUptoFive = o.DynamicApi.Get<bool>("AgeUptoFive");
                n.MedicalMentalDisorder = o.DynamicApi.Get<bool>("MedicalMentalDisorder");
                n.NotReadilyAccessible = o.DynamicApi.Get<bool>("NotReadilyAccessible");
                n.DiminishedMental = o.DynamicApi.Get<bool>("DiminishedMental");
                n.DiminishedPhysical = o.DynamicApi.Get<bool>("DiminishedPhysical");
            }
        );

        VisitzRealmBase.MapAll<SafetyFactors>(
            "SafetyFactors",
            migration,
            (n, o) =>
            {
                n.PhysicalHarm = o.DynamicApi.Get<bool?>("PhysicalHarm");
                n.SeriousInjuryAbuse = o.DynamicApi.Get<bool>("SeriousInjuryAbuse");
                n.FearsMaltreatChild = o.DynamicApi.Get<bool>("FearsMaltreatChild");
                n.ThreatAgainstChild = o.DynamicApi.Get<bool>("ThreatAgainstChild");
                n.ExcessiveForce = o.DynamicApi.Get<bool>("ExcessiveForce");
                n.SubsExposedInfant = o.DynamicApi.Get<bool>("SubsExposedInfant");
                n.CmtClarification = o.DynamicApi.Get<string>("CmtClarification") ?? string.Empty;
                n.CurrentCircumstances = o.DynamicApi.Get<bool?>("CurrentCircumstances");
                n.CmtCircumstances = o.DynamicApi.Get<string>("CmtCircumstances") ?? string.Empty;
                n.SexAbuse = o.DynamicApi.Get<bool?>("SexAbuse");
                n.CmtAbuse = o.DynamicApi.Get<string>("CmtAbuse") ?? string.Empty;
                n.UnableToProtect = o.DynamicApi.Get<bool?>("UnableToProtect");
                n.CmtProtect = o.DynamicApi.Get<string>("CmtProtect") ?? string.Empty;
                n.InjuryExplanation = o.DynamicApi.Get<bool?>("InjuryExplanation");
                n.CmtExplanation = o.DynamicApi.Get<string>("CmtExplanation") ?? string.Empty;
                n.RefuseAccess = o.DynamicApi.Get<bool?>("RefuseAccess");
                n.CmtAccess = o.DynamicApi.Get<string>("CmtAccess") ?? string.Empty;
                n.ImmediateNeeds = o.DynamicApi.Get<bool?>("ImmediateNeeds");
                n.CmtNeeds = o.DynamicApi.Get<string>("CmtNeeds") ?? string.Empty;
                n.PhysicalCondition = o.DynamicApi.Get<bool?>("PhysicalCondition");
                n.CmtCondition = o.DynamicApi.Get<string>("CmtCondition") ?? string.Empty;
                n.CurrentAbuse = o.DynamicApi.Get<bool?>("CurrentAbuse");
                n.CmtCurrent = o.DynamicApi.Get<string>("CmtCurrent") ?? string.Empty;
                n.PartnerViolence = o.DynamicApi.Get<bool?>("PartnerViolence");
                n.CmtViolence = o.DynamicApi.Get<string>("CmtViolence") ?? string.Empty;
                n.PredominantlyNegative = o.DynamicApi.Get<bool?>("PredominantlyNegative");
                n.CmtNegative = o.DynamicApi.Get<string>("CmtNegative") ?? string.Empty;
                n.EmotionalStability = o.DynamicApi.Get<bool?>("EmotionalStability");
                n.CmtEmotional = o.DynamicApi.Get<string>("CmtEmotional") ?? string.Empty;
                n.ChildFearful = o.DynamicApi.Get<bool?>("ChildFearful");
                n.CmtFearful = o.DynamicApi.Get<string>("CmtFearful") ?? string.Empty;
                n.OtherFactors = o.DynamicApi.Get<bool?>("OtherFactors");
                n.CmtOtherFactors = o.DynamicApi.Get<string>("CmtOtherFactors") ?? string.Empty;
                n.CurretAbuse = o.DynamicApi.Get<bool?>("CurretAbuse");
            }
        );

        VisitzRealmBase.MapAll<ProtectiveCapacity>(
            "ProtectiveCapacity",
            migration,
            (n, o) =>
            {
                n.ChildCognitive = o.DynamicApi.Get<bool>("ChildCognitive");
                n.ParentCognitive = o.DynamicApi.Get<bool>("ParentCognitive");
                n.ParentWillingness = o.DynamicApi.Get<bool>("ParentWillingness");
                n.ParentResources = o.DynamicApi.Get<bool>("ParentResources");
                n.ParentSupportive = o.DynamicApi.Get<bool>("ParentSupportive");
                n.ParentProtect = o.DynamicApi.Get<bool>("ParentProtect");
                n.ParentAccept = o.DynamicApi.Get<bool>("ParentAccept");
                n.ParentRelationship = o.DynamicApi.Get<bool>("ParentRelationship");
                n.ParentAware = o.DynamicApi.Get<bool>("ParentAware");
                n.ParentProbSolving = o.DynamicApi.Get<bool>("ParentProbSolving");
                n.NoProCapPresent = o.DynamicApi.Get<bool>("NoProCapPresent");
                n.CapacitiesOther = o.DynamicApi.Get<bool>("CapacitiesOther");
                n.CmtProtectiveCapacity01 = o.DynamicApi.Get<string>("CmtProtectiveCapacity01") ?? string.Empty;
                n.CmtProtectiveCapacity02 = o.DynamicApi.Get<string>("CmtProtectiveCapacity02") ?? string.Empty;
            }
        );

        VisitzRealmBase.MapAll<SafetyInterventions>(
            "SafetyInterventions",
            migration,
            (n, o) =>
            {
                n.DirectIntervention = o.DynamicApi.Get<bool>("DirectIntervention");
                n.UseOfIndividuals = o.DynamicApi.Get<bool>("UseOfIndividuals");
                n.UseCommAgencies = o.DynamicApi.Get<bool>("UseCommAgencies");
                n.ProtectVictim = o.DynamicApi.Get<bool>("ProtectVictim");
                n.LeaveHome = o.DynamicApi.Get<bool>("LeaveHome");
                n.NonOffendingParent = o.DynamicApi.Get<bool>("NonOffendingParent");
                n.LegalIntPlanned = o.DynamicApi.Get<bool>("LegalIntPlanned");
                n.OtherSafetyInterventions = o.DynamicApi.Get<bool>("OtherSafetyInterventions");
                n.CmtSafetyInterventions = o.DynamicApi.Get<string>("CmtSafetyInterventions") ?? string.Empty;
                n.ChildOutsideHome = o.DynamicApi.Get<bool>("ChildOutsideHome");
                n.ChildRemoved = o.DynamicApi.Get<bool>("ChildRemoved");
            }
        );

        VisitzRealmBase.MapAll<SafetyDecisions>(
            "SafetyDecisions",
            migration,
            (n, o) =>
            {
                n.NoSafetyFactors = o.DynamicApi.Get<bool>("NoSafetyFactors");
                n.SafeInterventions = o.DynamicApi.Get<bool>("SafeInterventions");
                n.UnsafeSafetyFactors = o.DynamicApi.Get<bool>("UnsafeSafetyFactors");
                n.DecisionUnsafe = o.DynamicApi.Get<string?>("DecisionUnsafe");
                n.DecisionUnsafeDescription = o.DynamicApi.Get<string?>("DecisionUnsafeDescription");
                n.Comments = o.DynamicApi.Get<string?>("Comments");
                n.Narrative = o.DynamicApi.Get<string?>("Narrative");
                n.ReadyFinalize = o.DynamicApi.Get<bool>("ReadyFinalize");
                n.ReadyFinalizeDate = o.DynamicApi.Get<DateTimeOffset?>("ReadyFinalizeDate");
            }
        );
    }
}
