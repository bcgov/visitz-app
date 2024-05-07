using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Visitz.ViewModels;

internal partial class VisitzSnackbarViewModel : VisitzViewModel
{
	[ObservableProperty]
	public string message;

	[ObservableProperty]
	public string actionText;

	[ObservableProperty]
	public Action action;

	[RelayCommand]
	public void ActionButtonSelected()
	{
		Action?.Invoke();
	}
}
