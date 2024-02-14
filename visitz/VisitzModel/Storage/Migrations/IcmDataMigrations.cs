using Realms;
using VisitzModel.Models;
using VisitzModel.Storage;

namespace VisitzModel.Storage.Migrations;

public static class IcmDataMigrations
{
    public static void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        MigrateCaseloadItems(migration, oldSchemaVersion);
        MigrateFamilyMembers(migration, oldSchemaVersion);
        MigrateNoteItems(migration, oldSchemaVersion);
    }

    private static void MigrateCaseloadItems(Migration migration, ulong oldSchemaVersion)
    {
        /* Nothing... yet. */
    }

    private static void MigrateFamilyMembers(Migration migration, ulong oldSchemaVersion)
    {
        /* Nothing... yet. */
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
