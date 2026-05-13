using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Snackbar;

public partial class VisitzSnackbarViewModel : VisitzViewModel
{
    [ObservableProperty]
    public partial string Message { get; set; }

    [ObservableProperty]
    public partial string ActionText { get; set; }

    [ObservableProperty]
    public partial Action Action { get; set; }

    [ObservableProperty]
    public partial bool ActionVisible { get; set; } = false;

    [RelayCommand]
    public void ActionButtonSelected()
    {
        Action?.Invoke();
    }

    partial void OnActionChanged(Action value)
    {
        UpdateActionVisible();
    }

    partial void OnActionTextChanged(string value)
    {
        UpdateActionVisible();
    }

    void UpdateActionVisible()
    {
        ActionVisible = Action != null && !string.IsNullOrWhiteSpace(ActionText);
    }
}
