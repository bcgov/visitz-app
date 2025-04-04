using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Events;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity.SafetyAssess;

public partial class EntitySafetyAssessView : ViewModelContentView, ICaseloadItemHolder
{
    protected new EntitySafetyAssessViewModel ViewModel => (EntitySafetyAssessViewModel)base.ViewModel;

    // It's preferable to use lifecycle methods to determine when auto-scrolling is allowed, but MAUI's lifecycles can
    // be unreliable--so we'll use a time-delayed bool.
    private bool canAutoScroll;

    private bool disposed;

    public CaseloadItem CaseloadItem 
    {
        get => ViewModel.CaseloadItem;
        set => ViewModel.CaseloadItem = value;
    }

    public SafetyAssessment Assessment
    {
        get => ViewModel.Assessment;
        set => ViewModel.Assessment = value;
    }

    public EntitySafetyAssessView() : base(ServiceProvider.GetService<EntitySafetyAssessViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        ViewModel.SaveStateHandler.SaveStateChanged += SaveStateHandler_SaveStateChanged;
        
        _ = DelayCanAutoScroll();
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            ViewModel.SaveStateHandler.SaveStateChanged -= SaveStateHandler_SaveStateChanged;
            ViewModel.SaveStateHandler?.Dispose();
            disposed = true;
        }
        base.Dispose(disposing);
    }

    private async Task DelayCanAutoScroll()
    {
        await Task.Delay(1500);
        canAutoScroll = true;
    }

    private async void SaveStateHandler_SaveStateChanged(object sender, DraftSaveStatusEventArgs e)
    {
        await DraftAppBar.SetDraftState(e.State);
    }

    private async void DiscardButton_Clicked(object sender, EventArgs e)
    {
        if (await PromptDiscard())
        {
            await ViewModel.Reset();
            SnackbarHandler.ShowText(LocalizedStrings.DiscardedSafetyAssessmentDraft);
        }
    }

    private async static Task<bool> PromptDiscard()
    {
        return await Navigator.CurrentOpenPage.DisplayAlert(
            LocalizedStrings.DiscardDraftQuestion,
            LocalizedStrings.DiscardSafetyAssessmentDraftDescription,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel);
    }

    private async void SomeChildrenPlaced_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (canAutoScroll && e.Value)
        {
            await Task.Delay(100);
            await MainScrollView.ScrollToAsync(ChildrenInCareSection.X, ChildrenInCareSection.Y, true);
        }
    }
}
