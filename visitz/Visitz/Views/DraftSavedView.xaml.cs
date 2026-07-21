using CommunityToolkit.Maui;
using Visitz.Animations;
using Visitz.Resources.Localization;
using VisitzModel.Models.Drafts;

namespace Visitz.Views;

#nullable enable

public partial class DraftSavedView : ContentView
{
    [BindableProperty]
    public partial Thickness TagPadding { get; set; } = new Thickness(5.0d);

    [BindableProperty(PropertyChangedMethodName = nameof(SaveState_Changed))]
    public partial DraftSaveState SaveState { get; set; } = DraftSaveState.None;

    [BindableProperty(PropertyChangedMethodName = nameof(ShowText_Changed))]
    public partial bool ShowText { get; set; } = true;

    [BindableProperty]
    public partial string SavedText { get; set; } = LocalizedStrings.DraftSaved;

    [BindableProperty]
    public partial string SavingText { get; set; } = LocalizedStrings.Ellipsis;

    public DraftSavedView()
    {
        InitializeComponent();
    }

    static void SaveState_Changed(BindableObject obj, object _, object newValue)
    {
        if (obj is DraftSavedView view && newValue is DraftSaveState state)
            _ = view.SetState(state);
    }

    static void ShowText_Changed(BindableObject obj, object _, object newValue)
    {
        if (obj is DraftSavedView view && newValue is bool showText)
        {
            view.SavedText = showText ? LocalizedStrings.DraftSaved : string.Empty;
            view.SavingText = showText ? LocalizedStrings.Ellipsis : string.Empty;
        }
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
