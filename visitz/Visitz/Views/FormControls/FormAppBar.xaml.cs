using System.Windows.Input;
using CommunityToolkit.Maui;
using VisitzModel.Models.Drafts;

namespace Visitz.Views.FormControls;

public partial class FormAppBar : ContentView
{
    [BindableProperty]
    public partial bool AllowDiscard { get; set; } = true;

    [BindableProperty]
    public partial bool AllowPublish { get; set; } = true;

    [BindableProperty]
    public partial ICommand DiscardCommand { get; set; }

    [BindableProperty]
    public partial ICommand PublishCommand { get; set; }

    [BindableProperty]
    public partial bool IsReadOnly { get; set; }

    [BindableProperty]
    public partial DraftSaveState DraftSaveState { get; set; } = DraftSaveState.None;

    public FormAppBar()
    {
        InitializeComponent();
    }
}
