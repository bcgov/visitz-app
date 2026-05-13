using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Navigation;

#nullable enable

public partial class TabViewModel : VisitzViewModel
{
    [ObservableProperty]
    public partial IEnumerable<Tab>? Tabs { get; set; }

    [ObservableProperty]
    public partial Tab? SelectedTab { get; set; }
    public ContentView? PairedDisplayView { get; set; }

    partial void OnTabsChanged(IEnumerable<Tab>? value)
    {
        if (value is not null)
            SelectedTab = value.ElementAt(0);
    }

    partial void OnSelectedTabChanged(Tab? value)
    {
        if (PairedDisplayView is not null)
            PairedDisplayView.Content = SelectedTab?.ContentView;
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            foreach (var tab in Tabs ?? [])
                tab.Dispose();

            disposed = true;
        }

        base.Dispose(disposing);
    }
}
