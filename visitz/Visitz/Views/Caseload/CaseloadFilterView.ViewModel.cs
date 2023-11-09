using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using System.Collections.ObjectModel;
using Visitz.Models;
using Visitz.Storage;
using Visitz.ViewModels;

namespace Visitz.Views.Caseload;

public partial class CaseloadFilterViewModel : VisitzViewModel
{
    [ObservableProperty]
    public ObservableCollection<FilterItem> filterItems = new();

    private Realm Realm { get; set; }

    private IQueryable<CaseloadItem> TypeFilterQuery { get; set; }

    private IDisposable TypeFilterQueryToken { get; set; }

    private IQueryable<CaseloadItem> SubTypeFilterQuery { get; set; }

    private IDisposable SubTypeFilterQueryToken { get; set; }

    public override async void PageCreated()
    {
        base.PageCreated();

        await Setup();

        RunTypeQueries();
    }

    public override void PageDestroyed()
    {
        Teardown();

        base.PageDestroyed();
    }

    private async Task Setup()
    {
        Realm = await VisitzRealm.GetIcmDataAsync();

        string type = nameof(CaseloadItem.EntityType);
        TypeFilterQuery = Realm.All<CaseloadItem>()
            .Filter($"TRUEPREDICATE DISTINCT({type}) SORT({type} ASC)");
        TypeFilterQueryToken = TypeFilterQuery.SubscribeForNotifications(Caseload_Changed);

        string subtype = nameof(CaseloadItem.CaseIncidentType);
        SubTypeFilterQuery = Realm.All<CaseloadItem>()
            .Filter($"TRUEPREDICATE DISTINCT({subtype}) SORT({subtype} ASC)");
        SubTypeFilterQueryToken = SubTypeFilterQuery.SubscribeForNotifications(Caseload_Changed);
    }

    private void Teardown()
    {
        SubTypeFilterQueryToken?.Dispose();
        SubTypeFilterQueryToken = null;
        SubTypeFilterQuery = null;

        TypeFilterQueryToken?.Dispose();
        TypeFilterQueryToken = null;
        TypeFilterQuery = null;

        Realm?.Dispose();
        Realm = null;
    }

    private void RunTypeQueries()
    {
        FilterItems.Clear();

        var types = TypeFilterQuery
            .ToList()
            .Select(item => new FilterItem() { Text = item.EntityType });
        foreach (var item in types)
            FilterItems.Add(item);

        var subtypes = SubTypeFilterQuery
            .ToList()
            .Select(item => new FilterItem() { Text = item.CaseIncidentType });
        foreach (var item in subtypes)
            FilterItems.Add(item);
    }

    private void Caseload_Changed(IRealmCollection<CaseloadItem> sender, ChangeSet changes)
    {
        if (changes != null)
            RunTypeQueries();
    }
}
