using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;

namespace Visitz.ViewModels;

public partial class WebViewModel : VisitzViewModel
{
	[ObservableProperty]
	public Uri authUri;
}
