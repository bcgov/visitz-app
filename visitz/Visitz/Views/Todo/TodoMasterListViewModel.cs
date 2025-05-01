using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Drafts;

namespace Visitz.Views.Todo;

public partial class TodoMasterListViewModel : VisitzViewModel
{
    private bool _disposed;

    [ObservableProperty]
    public ObservableCollection<object> masterDraftItems = [];

    [ObservableProperty]
    public MasterDraftItem selectedItem;

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
            _disposed = true;
        }
        base.Dispose(disposing);
    }
}
