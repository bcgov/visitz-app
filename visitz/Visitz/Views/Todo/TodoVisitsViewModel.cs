using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Todo;

public partial class TodoVisitsViewModel : VisitzViewModel
{
    private bool _disposed;

    [ObservableProperty]
    public ObservableCollection<PersonVisit> todoItems = [];

    Realm icmDataRealm;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        icmDataRealm = await VisitzRealms.GetIcmDataRealmAsync();
        GetTodoItems(PersonVisit.GetUpcomingVisits(icmDataRealm));
    }

    public void LoadTodoItemsForNavItem(NavItem navItem)
    {
        if (navItem.ContentViewType == typeof(TodoVisitsView))
        {
            // TodoItems.Clear();

            var upcomingVisits = PersonVisit.GetUpcomingVisits(icmDataRealm);
            GetTodoItems(upcomingVisits);
        }
    }

     public  void GetTodoItems(IOrderedEnumerable<PersonVisit> items)
    {
        foreach (var item in items)
            TodoItems.Add(item);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            icmDataRealm.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }
}
