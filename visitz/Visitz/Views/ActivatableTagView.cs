using Visitz.VisualStates;

namespace Visitz.Views;

public partial class ActivatableTagView : TagView, IActiveState
{
    public event EventHandler<IActiveState.ActiveChangedEventArgs> ActiveStateChanged;

    private bool isActive;
    
    public bool IsActive
    {
        get => isActive;
        set
        {
            isActive = value;
            VisualStateManager.GoToState(this, (this as IActiveState).GetActiveState());
            ActiveStateChanged?.Invoke(this, new IActiveState.ActiveChangedEventArgs(IsActive));
        }
    }

    public ActivatableTagView() : base()
	{
        var tap = new TapGestureRecognizer();
        tap.Tapped += Tapped;
        GestureRecognizers.Add(tap);
	}

    private void Tapped(object sender, TappedEventArgs e)
    {
        IsActive = !IsActive;
    }
}