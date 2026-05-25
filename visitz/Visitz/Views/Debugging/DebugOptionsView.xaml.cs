using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Debugging;

public partial class DebugOptionsView : ViewModelContentView<DebugOptionsViewModel>
{
    public DebugOptionsView()
        : base(ServiceProvider.GetService<DebugOptionsViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    private async void ShowDocumentsButton_Clicked(object? sender, EventArgs e)
    {
        var popup = new Popup()
        {
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            Content = new ScrollView()
            {
                Orientation = ScrollOrientation.Both,
                Content = new Label()
                {
                    HorizontalOptions = LayoutOptions.Start,
                    Text = DebugOptions.Default.ListDocumentsFiles(),
                },
            },
        };

        await Navigator.CurrentOpenPage.ShowPopupAsync(popup);
    }

    private void Entry_HighlightWhenFocused(object? sender, FocusEventArgs e)
    {
        if (sender is Entry entry)
        {
            entry.CursorPosition = 0;
            entry.SelectionLength = entry.Text.Length;
        }
    }

    private void WindowWidthEntry_Unfocused(object sender, FocusEventArgs e)
    {
        if (sender is Entry entry && double.TryParse(entry.Text, out double width))
            DebugOptions.Default.WindowWidth = width;
    }

    private void WindowHeightEntry_Unfocused(object sender, FocusEventArgs e)
    {
        if (sender is Entry entry && double.TryParse(entry.Text, out double height))
            DebugOptions.Default.WindowHeight = height;
    }
}
