using CommunityToolkit.Mvvm.Messaging;
using Visitz.Services.Caseload;
using Visitz.Services.Messages;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Navigation;

public partial class HorizontalNavRailView : ViewModelContentView<NavRailViewModel>, IRecipient<ServiceStateMessage>
{
    public HorizontalNavRailView()
        : base(ServiceProvider.GetService<NavRailViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        WeakReferenceMessenger.Default.Register(this, GetAllDataForOfflineService.MakeId());
    }

    bool _disposed;

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
        ProgressIndicator.IsRunning = message.IsRunning;
        ProgressIndicator.IsVisible = message.IsRunning;
    }
}
