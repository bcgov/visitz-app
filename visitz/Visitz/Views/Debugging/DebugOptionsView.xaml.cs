using Visitz.Views.BaseClasses;

namespace Visitz.Views.Debugging;

public partial class DebugOptionsView : ViewModelContentView
{
	public DebugOptionsView() : base(ServiceProvider.GetService<DebugOptionsViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}
