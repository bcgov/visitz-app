using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Caseload;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Todo;

public partial class TodoVisitsViewModel : VisitzViewModel
{
    private bool _disposed;

    [ObservableProperty]
    public ObservableCollection<TodoVisitsDisplayItem> todoItems = [];

    Realm icmDataRealm;

    readonly ObservableRealmQueryMap realmQuery = new();

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        icmDataRealm = await VisitzRealms.GetIcmDataRealmAsync();
        realmQuery.ItemsChanged += RealmQuery_ItemsChanged;
        realmQuery.Subscribe(icmDataRealm, PersonVisit.GetAllByType(icmDataRealm));
    }

    private void RealmQuery_ItemsChanged(
        object sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e
    )
    {
        UpdateTodoItemsList();
    }

    private void UpdateTodoItemsList()
    {
        var upcomingVisits = PersonVisit.GetUpcomingVisits(icmDataRealm).ToList();
        TodoItems.Clear();

        foreach (var visit in upcomingVisits)
        {
            var businessObject = GetRelatedBusinessObjectFrom(icmDataRealm, visit);
            TodoItems.Add(new TodoVisitsDisplayItem(visit, businessObject));
        }
    }

    [RelayCommand]
    private static void TodoItemSelected(TodoVisitsDisplayItem item)
    {
        if (item.BusinessObject != null)
            NavigateTo(item.BusinessObject, item.SectionToOpen);
    }

    private static IBusinessObject GetRelatedBusinessObjectFrom(Realm realm, PersonVisit todoItem)
    {
        return CaseRecord.GetByPersonVisitItem(realm, todoItem);
    }

    static void NavigateTo(IBusinessObject businessObject, EntitySection section)
    {
        var appNav = new AppNavMessage(new() { ContentViewType = typeof(CaseloadContainerView) });
        StrongReferenceMessenger.Default.Send(appNav);

        var caseloadNav = new BusinessObjectSelectedMessage(businessObject, section);
        StrongReferenceMessenger.Default.Send(caseloadNav);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            realmQuery.ItemsChanged -= RealmQuery_ItemsChanged;
            realmQuery.Dispose();
            icmDataRealm.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }
}
