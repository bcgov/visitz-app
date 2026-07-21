using CommunityToolkit.Maui;

namespace Visitz.Views.Entity.CallDetails;

#nullable enable

public partial class FullDetailsView : ContentView
{
    [BindableProperty]
    public partial string Text { get; set; } = string.Empty;

    public FullDetailsView()
    {
        InitializeComponent();
    }
}
