using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.CallDetails;

public partial class CallDetailsView : IcmRecordContentView<CallDetailsViewModel>
{
    public CallDetailsView(CallDetailsViewModel viewModel)
        : base(viewModel, LocalizedStrings.CallDetails)
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
