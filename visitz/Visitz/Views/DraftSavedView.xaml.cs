using Visitz.Animations;
using VisitzModel.Models.Drafts;

namespace Visitz.Views;

public partial class DraftSavedView : ContentView
{
    public static readonly BindableProperty TagPaddingProperty =
        BindableProperty.Create(nameof(TagPadding), typeof(Thickness), typeof(DraftSavedView),
            defaultValue: new Thickness(5.0d));

    public Thickness TagPadding
    {
        get => (Thickness)GetValue(TagPaddingProperty);
        set => SetValue(TagPaddingProperty, value);
    }

    public DraftSavedView()
    {
        InitializeComponent();
    }

    public async Task SetState(DraftSaveState state)
    {
        switch (state)
        {
            case DraftSaveState.None:
                await AnimateCrossfade(showSaving: false, showSaved: false);
                break;
            case DraftSaveState.Saving:
                await AnimateCrossfade(showSaving: true, showSaved: false);
                break;
            case DraftSaveState.Saved:
                await AnimateCrossfade(showSaving: false, showSaved: true);
                break;
            default:
                throw new NotImplementedException($"Not implemented: {state}");
        }
    }

    private async Task AnimateCrossfade(bool showSaving, bool showSaved)
    {
        await Task.WhenAll
        (
            SetSavingDraftPromptVisible(showSaving),
            SetDraftSavedPromptVisible(showSaved)
        );
    }

    private async Task SetDraftSavedPromptVisible(bool visible)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (DraftSavedTagView.IsVisible == visible)
                return;

            var visibilityAnimation = new VisibilityAnimation(visible, 100, Easing.CubicInOut);
            await visibilityAnimation.Animate(DraftSavedTagView);
        });
    }

    private async Task SetSavingDraftPromptVisible(bool visible)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (SavingDraftTagView.IsVisible == visible)
                return;

            var visibilityAnimation = new VisibilityAnimation(visible, 100, Easing.CubicInOut);
            await visibilityAnimation.Animate(SavingDraftTagView);
        });
    }
}
