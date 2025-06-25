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

    public static void Migrate_2_7_1_Visits(Migration migration)
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

    public static void Migrate_2_7_1_Drafts(Migration migration)
    {
        var oldItems = migration.OldRealm.DynamicApi.All("PersonVisitDraft");
        var newItems = migration.NewRealm.All<PersonVisitDraft>();

        for (int i = 0; i < oldItems.Count(); i++)
        {
            var old = oldItems.ElementAt(i);
            var @new = newItems.ElementAt(i);

            IRealmObject oldVisit = old.DynamicApi.Get<IRealmObject>("Visit");
            string group = oldVisit.DynamicApi.Get<string>("VisitDetailsGroup");
            string value = oldVisit.DynamicApi.Get<string>("VisitDetailsValue");

            @new.Visit.VisitDetails.Add(MakeDetailsValue(group, value));
        }
    }
}
