namespace Visitz.Behaviors;

public partial class DisableWhenInternetUnavailableBehavior : Behavior<View>
{
    private View View { get; set; }

    protected override void OnAttachedTo(View bindable)
    {
        base.OnAttachedTo(bindable);
        View = bindable;

        bindable.BindingContextChanged += Bindable_BindingContextChanged;
        Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
    }

    protected override void OnDetachingFrom(View bindable)
    {
        Connectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
        bindable.BindingContextChanged -= Bindable_BindingContextChanged;

        base.OnDetachingFrom(bindable);
    }

    private void Bindable_BindingContextChanged(object sender, EventArgs e)
    {
        ApplyNetworkStyles(Connectivity.Current.NetworkAccess);
    }

    private void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        ApplyNetworkStyles(e.NetworkAccess);
    }

    private void ApplyNetworkStyles(NetworkAccess networkAccess)
    {
        View.IsEnabled = networkAccess == NetworkAccess.Internet;
    }
}
