using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;

namespace Visitz.Views.Todo;

public partial class TodoMasterListViewModel : VisitzViewModel
{
    private bool _disposed;

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
