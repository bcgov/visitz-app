using Visitz.Device;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitView : IcmRecordContentView<ChildYouthVisitViewModel>
{
    public ChildYouthVisitView()
        : base(ServiceProvider.GetService<ChildYouthVisitViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
