namespace Visitz.VisualStates;

internal interface ISelectedState : IVisualStateBase
{
    public static readonly string SelectedStateName = "Selected";

    bool IsSelected { get; set; }

    string GetSelectedState() => IsSelected ? SelectedStateName : NormalStateName;
}
