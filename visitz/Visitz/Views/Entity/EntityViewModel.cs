using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Extensions;
using Visitz.Views.BaseClasses;
using VisitzModel.Messaging;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity;

#nullable enable

public partial class EntityViewModel : IcmRecordViewModel
{
    [ObservableProperty]
    public EntityNavItem? selectedEntityNavItem;

    public EntitySection RequestedSection { get; set; }

    public IDraftItem? FocusedDraftItem { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        try
        {
            BuildNavList();
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    public void SetRequestedSection(EntitySection section, IDraftItem focusedDraftItem)
    {
        RequestedSection = section;
        FocusedDraftItem = focusedDraftItem;

        //SelectedEntityNavItem = GetMappedNavItem(section);
    }

    [RelayCommand]
    public static void GoBack()
    {
        StrongReferenceMessenger.Default.Send(new EntityNavBackMessage());
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            DisposeTabViews();
            disposed = true;
        }
        base.Dispose(disposing);
    }
}
