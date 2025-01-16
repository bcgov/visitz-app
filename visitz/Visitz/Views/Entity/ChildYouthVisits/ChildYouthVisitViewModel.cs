using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitViewModel : VisitzViewModel, ICaseloadItemHolder
{
    private static readonly int CharacterLimit = 4000;
    public static readonly string RemainingCharactersString = "{0}/" + CharacterLimit;
    private bool _disposed;
    Realm Realm { get; set; }
    public CaseloadItem CaseloadItem { get; set; }

    [ObservableProperty]
    public PersonVisit personVisit;

    protected override async Task InitAsync()
    {
        await base.InitAsync();
        await InitVisitDraft();

    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private async Task InitVisitDraft()
    {
        Realm = await VisitzRealms.GetPersonVisitDraftsRealmAsync();
        // PersonVisit = PersonVisit.FindByEntityId(Realm, CaseloadItem.CaseIncidentNumber) ?? CreateNoteDraft();
    }
}