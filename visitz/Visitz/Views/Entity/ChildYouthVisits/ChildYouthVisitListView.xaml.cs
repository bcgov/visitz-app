using CommunityToolkit.Mvvm.Messaging;
using Visitz.Services;
using Visitz.Services.Visits;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitListView
    : ViewModelContentView,
        IBusinessObjectHolder,
        IRequestedEntitySection,
        IRecipient<ServiceStateMessage>
{
    bool _disposed;

    new ChildYouthVisitListViewModel ViewModel => base.ViewModel as ChildYouthVisitListViewModel;

    public IBusinessObject BusinessObject
    {
        get => ViewModel.BusinessObject;
        set => ViewModel.BusinessObject = value;
    }

    public EntitySection RequestedSection
    {
        get => ViewModel.RequestedSection;
        set => ViewModel.RequestedSection = value;
    }

    public ChildYouthVisitListView()
        : base(ServiceProvider.GetService<ChildYouthVisitListViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        WeakReferenceMessenger.Default.Register(this, PostAndRefreshVisitService.MakeId(BusinessObject.Id));
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
