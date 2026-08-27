namespace Visitz.Views.FormControls;

public partial class InfoItemView : ContentView
{
    public InfoItemView()
    {
        InitializeComponent();
    }

    void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
    {
        if (BindingContext is InfoItem item)
            item.TapAction?.Invoke();
    }
}
