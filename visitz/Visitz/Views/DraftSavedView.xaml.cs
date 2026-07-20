using CommunityToolkit.Maui;
using Visitz.Animations;
using VisitzModel.Models.Drafts;

namespace Visitz.Views;

#nullable enable

public partial class DraftSavedView : ContentView
{
    [BindableProperty]
    public partial Thickness TagPadding { get; set; } = new Thickness(5.0d);

    [BindableProperty(PropertyChangedMethodName = nameof(SaveState_Changed))]
    public partial DraftSaveState SaveState { get; set; } = DraftSaveState.None;

    public DraftSavedView()
    {
        InitializeComponent();
    }

    static void SaveState_Changed(BindableObject obj, object oldValue, object newValue)
    {
        if (obj is DraftSavedView view && newValue is DraftSaveState state)
            _ = view.SetState(state);
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
        await Task.WhenAll(SetSavingDraftPromptVisible(showSaving), SetDraftSavedPromptVisible(showSaved));
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
