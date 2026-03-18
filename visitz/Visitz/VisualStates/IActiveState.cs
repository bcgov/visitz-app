namespace Visitz.VisualStates;

public interface IActiveState : IVisualStateBase
{
    public static readonly string ActiveStateName = "Activated";

    event EventHandler<ActiveChangedEventArgs> ActiveStateChanged;

    bool IsActive { get; set; }

    string GetActiveState() => IsActive ? ActiveStateName : NormalStateName;

    public class ActiveChangedEventArgs(bool isActive) : EventArgs
    {
        public bool IsActive { get; set; } = isActive;
    }
}
