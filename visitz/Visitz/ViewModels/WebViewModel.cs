using CommunityToolkit.Mvvm.ComponentModel;

namespace Visitz.ViewModels;

public partial class WebViewModel : VisitzViewModel
{
	[ObservableProperty]
	public Uri authUri;
}
