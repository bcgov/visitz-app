using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Controls;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Drafts;

namespace Visitz.Views.Drafts;

#nullable enable

public partial class DraftsContainerViewModel : VisitzViewModel
{
    [ObservableProperty]
    public partial ObservableCollection<FilterOption<IDraftItem>> FilterOptions { get; set; } = [];

    [ObservableProperty]
    public partial FilterOption<IDraftItem>? SelectedFilter { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);

            disposed = true;
        }

        base.Dispose(disposing);
    }
}
