using Realms;

namespace VisitzModel.Storage.Migrations.SafetyAssessments;

internal static class SafetyAssessment2_3_3
{
	const string SafetyAssessmentName = "SafetyAssessment";
	const string AssessmentDraftName = "AssessmentDraft";

	const string IncidentNumberName = "IncidentNumber";
	const string DraftCreatedName = "DraftCreated";
	const string LastCreatedName = "LastUpdated";

	public static void Migrate(Migration migration)
	{
		var oldItems = migration.OldRealm.DynamicApi.All(SafetyAssessmentName);

		for (int i = 0; i < oldItems.Count(); i++)
			Create(migration, oldItems.ElementAt(i));
	}

	static void Create(Migration migration, IRealmObject oldItem)
	{
		string pk = oldItem.DynamicApi.Get<string>(IncidentNumberName);
		var newDraft = migration.NewRealm.DynamicApi.CreateObject(AssessmentDraftName, pk);

		newDraft.DynamicApi.Set(DraftCreatedName, DateTimeOffset.MinValue);
		newDraft.DynamicApi.Set(LastCreatedName, DateTimeOffset.MinValue);
	}
}
