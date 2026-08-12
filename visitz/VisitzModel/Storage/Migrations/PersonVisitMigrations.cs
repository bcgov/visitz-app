using Realms;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Storage.Migrations;

internal class PersonVisitMigrations
{
    public static void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion < VisitzRealmBase.Version2_7_1)
            Migrate_2_7_1(migration);
        if (oldSchemaVersion < VisitzRealmBase.Version3_0_0)
        {
            VisitzRealmBase.MapAll<PersonVisit>(
                "PersonVisit",
                migration,
                (n, o) =>
                {
                    n.Id = o.DynamicApi.Get<string>("Id") ?? string.Empty;
                    n.ParentId = o.DynamicApi.Get<string>("ParentId") ?? string.Empty;
                    n.ParentTypeInt = o.DynamicApi.Get<int>("ParentTypeInt");
                    n.Name = o.DynamicApi.Get<string>("Name") ?? string.Empty;
                    n.VisitDescription = o.DynamicApi.Get<string>("VisitDescription") ?? string.Empty;
                    n.Type = o.DynamicApi.Get<string>("Type") ?? string.Empty;
                    n.DateOfVisit = o.DynamicApi.Get<DateTimeOffset>("DateOfVisit");

                    foreach (string detail in o.DynamicApi.GetList<string>("VisitDetails"))
                        n.VisitDetails.Add(detail ?? string.Empty);

                    n.LoginName = o.DynamicApi.Get<string>("LoginName") ?? string.Empty;
                    n.Created = o.DynamicApi.Get<DateTimeOffset>("Created");
                    n.Updated = o.DynamicApi.Get<DateTimeOffset>("Updated");
                    n.CreatedBy = o.DynamicApi.Get<string>("CreatedBy") ?? string.Empty;
                    n.UpdatedBy = o.DynamicApi.Get<string>("UpdatedBy") ?? string.Empty;
                }
            );
        }
    }

    static string MakeDetailsValue(string group, string value)
    {
        if (group.StartsWith(PersonVisitDetails.Type_PrivateVisit))
            return $"{group} {value}";
        else
            return $"{group} - {value}";
    }

    public static void Migrate_2_7_1(Migration migration)
    {
        var oldItems = migration.OldRealm.DynamicApi.All("PersonVisit");
        var newItems = migration.NewRealm.All<PersonVisit>();

        for (int i = 0; i < oldItems.Count(); i++)
        {
            var old = oldItems.ElementAt(i);
            var @new = newItems.ElementAt(i);

            string group = old.DynamicApi.Get<string>("VisitDetailsGroup");
            string value = old.DynamicApi.Get<string>("VisitDetailsValue");

            @new.VisitDetails.Add(MakeDetailsValue(group, value));
        }
    }
}
