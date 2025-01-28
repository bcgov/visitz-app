using Realms;
using System.Reflection;
using VisitzModel.Models;
using VisitzModel.Models.Notes;

namespace VisitzModel.Storage.Migrations;

public static class IcmDataMigrations
{
    public static void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        MigrateCaseloadItems(migration, oldSchemaVersion);
        MigrateNoteItems(migration, oldSchemaVersion);
    }

	private static void MigrateCaseloadItems(Migration migration, ulong oldSchemaVersion)
	{
		const string CaseloadItemName = "CaseloadItem";

		var oldItems = migration.OldRealm.DynamicApi.All(CaseloadItemName);
		var newItems = migration.NewRealm.DynamicApi.All(CaseloadItemName);

		for (int i = 0; i < newItems.Count(); i++)
			//TODO: Migrate CaseloadItems
			MigrateItemFamilyMembers(oldSchemaVersion, oldItems.ElementAt(i), newItems.ElementAt(i));
	}

	private static void MigrateItemFamilyMembers(ulong oldSchemaVersion, IRealmObject _, IRealmObject newItem)
	{
		if (oldSchemaVersion < VisitzRealmBase.Version2_3_3)
		{
			const string SubjectFlag = "SubjectFlag";
			const string ParentCaregiver = "ParentCaregiver";
			const string SubjectChild = "SubjectChild";
			
			var newFamily = newItem.DynamicApi.GetList<IRealmObjectBase>("FamilyMembers");

			foreach (var member in newFamily)
			{
				var type = member.GetType();

				if (type.GetProperty(SubjectFlag) is PropertyInfo subjectFlag)
					subjectFlag.SetValue(member, null, null);

				if (type.GetProperty(ParentCaregiver) is PropertyInfo parentCaregiver)
					parentCaregiver.SetValue(member, null, null);

				if (type.GetProperty(SubjectChild) is PropertyInfo subjectChild)
					subjectChild.SetValue(member, null, null);
			}
		}
	}

    private static void MigrateNoteItems(Migration migration, ulong oldSchemaVersion)
    {
        var oldNotes = migration.OldRealm.DynamicApi.All("NoteItem");
        var newNotes = migration.NewRealm.All<NoteItem>();

        for (int i = 0; i < newNotes.Count(); i++)
            MigrateNoteItem(oldSchemaVersion, oldNotes.ElementAt(i), newNotes.ElementAt(i));
    }

    private static void MigrateNoteItem(ulong oldSchemaVersion, IRealmObject oldNote, NoteItem newNote)
    {
        if (oldSchemaVersion < VisitzRealmBase.Version2_0)
        {
            // Make Primary Key
            var oldIcmId = oldNote.DynamicApi.Get<string>("IcmId");
            var oldNotePeriod = oldNote.DynamicApi.Get<string>("NotePeriod");
            var oldCreatedDate = oldNote.DynamicApi.Get<string>("CreatedDate");
            newNote.FullID = $"{oldIcmId}-{oldNotePeriod}-{oldCreatedDate}";

            // Fill DateTimeOffset fields
            newNote.NotePeriodDateTime = oldNotePeriod?.Length > 0
                    ? DateTimeOffset.Parse(oldNotePeriod)
                    : DateTimeOffset.MinValue;
            newNote.CreatedDateTime = oldCreatedDate?.Length > 0
                    ? DateTimeOffset.Parse(oldCreatedDate)
                    : DateTimeOffset.MinValue;
        }
    }
}
