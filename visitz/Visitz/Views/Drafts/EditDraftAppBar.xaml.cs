using CommunityToolkit.Mvvm.Input;
using VisitzModel.Models.Drafts;

namespace Visitz.Views.Drafts;

public partial class EditDraftAppBar : ContentView
{
    public static readonly BindableProperty AllowDiscardProperty =
        BindableProperty.Create(nameof(AllowDiscard), typeof(bool), typeof(EditDraftAppBar));

    public static readonly BindableProperty AllowPublishProperty =
        BindableProperty.Create(nameof(AllowPublish), typeof(bool), typeof(EditDraftAppBar));

    public static readonly BindableProperty DiscardCommandProperty =
        BindableProperty.Create(nameof(DiscardCommand), typeof(IAsyncRelayCommand), typeof(EditDraftAppBar));

    public static readonly BindableProperty PublishCommandProperty =
        BindableProperty.Create(nameof(PublishCommand), typeof(IAsyncRelayCommand), typeof(EditDraftAppBar));

    public bool AllowDiscard
    {
        get => (bool)GetValue(AllowDiscardProperty);
        set => SetValue(AllowDiscardProperty, value);
    }

    public bool AllowPublish
    {
        get => (bool)GetValue(AllowPublishProperty);
        set => SetValue(AllowPublishProperty, value);
    }

    public IAsyncRelayCommand DiscardCommand
    {
        get => (IAsyncRelayCommand)GetValue(DiscardCommandProperty);
        set => SetValue(DiscardCommandProperty, value);
    }

    public IAsyncRelayCommand PublishCommand
    {
        get => (IAsyncRelayCommand)GetValue(PublishCommandProperty);
        set => SetValue(PublishCommandProperty, value);
    }

    public event EventHandler DiscardClicked
    {
        add { DiscardButton.Clicked += value; }
        remove { DiscardButton.Clicked -= value; }
    }

    public EditDraftAppBar()
    {
        InitializeComponent();
    }

    public async Task SetDraftState(DraftSaveState state)
    {
        await DraftSavedIndicator.SetState(state);
    }
}
