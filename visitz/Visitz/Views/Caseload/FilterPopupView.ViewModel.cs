using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Models;
using Visitz.Storage;
using Visitz.ViewModels;

namespace Visitz.Views.Caseload;

public partial class FilterPopupViewModel : VisitzViewModel
{
    private Realm Realm { get; set; }

    [ObservableProperty]
    private IEnumerable<CaseloadItem> itemsBySubtype;

    public override async void PageCreated()
    {
        base.PageCreated();

        Realm = await VisitzRealm.GetIcmDataAsync();

        ItemsBySubtype = CaseloadItem.GetAllByDistinctSubtypes(Realm, true);
    }

    public override void PageDestroyed()
    {
        ItemsBySubtype = null;

        Realm?.Dispose();
        Realm = null;

        base.PageDestroyed();
    }
}
