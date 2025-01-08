using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;

namespace Visitz.Views.Entity.ChildYouthVisits;

internal partial class ChildYouthVisitListViewModel : VisitzViewModel, ICaseloadItemHolder
{
    private bool _disposed;

    [ObservableProperty]
    public CaseloadItem caseloadItem;

    protected override async Task InitAsync()
    {
        await base.InitAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            // TODO
            _disposed = true;
        }

        base.Dispose(disposing);
    }
}
