using Realms;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Storage.Migrations;

internal class PersonVisitMigrations
{
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
