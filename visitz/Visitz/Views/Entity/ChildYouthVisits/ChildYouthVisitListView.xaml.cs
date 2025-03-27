using CommunityToolkit.Mvvm.Messaging;
using Visitz.Services;
using Visitz.Services.Visits;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitListView : ViewModelContentView,
    ICaseloadItemHolder,
    IRequestedEntitySection,
    IRecipient<ServiceStateMessage>
{
    bool _disposed;

    new ChildYouthVisitListViewModel ViewModel => base.ViewModel as ChildYouthVisitListViewModel;

    public CaseloadItem CaseloadItem
    {
        get => ViewModel.CaseloadItem;
        set => ViewModel.CaseloadItem = value;
    }

    public EntitySection RequestedSection
    {
        get => ViewModel.RequestedSection;
        set => ViewModel.RequestedSection = value;
    }

    public ChildYouthVisitListView() : base(ServiceProvider.GetService<ChildYouthVisitListViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        WeakReferenceMessenger.Default.Register(this, PostAndRefreshVisitService.MakeId(CaseloadItem.RowId));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    public void Receive(ServiceStateMessage message)
    {
        if (message.FinishedSuccess)
            VisitsCollectionView.ScrollTo(0);
    }
}
