using Microsoft.Extensions.Logging;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Services.Base;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Notes;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Drafts;

public partial class DraftsListItem : BaseContentView
{
    readonly ServiceActivityListener _activityListener = new();

    public DraftsListItem()
    {
        InitializeComponent();
        _activityListener.Started += ActivityListener_Started;
        _activityListener.Stopped += ActivityListener_Stopped;
    }

    protected override async void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is IDraftItem item)
        {
            Icon.Text = GetIconGlyph(item);

            using var realm = await VisitzRealms.GetIcmDataRealmAsync();
            if (item.GetRelatedBusinessObjectFrom(realm) is IBusinessObject record)
                _activityListener.RegisterForMessages(record);
        }
        else
        {
            Icon.Text = string.Empty;
            _activityListener.UnregisterFromMessages();
        }

        UpdateDownloadActivityIndicator();
    }

    static string GetIconGlyph(IDraftItem item)
    {
        if (item is PersonVisitDraft)
            return MaterialIcons.Person_pin_circle;
        else if (item is NoteDraft)
            return MaterialIcons.Edit_document;
        else if (item is AssessmentDraft)
            return MaterialIcons.Assignment;
        else if (item is AttachmentDraft)
            return MaterialIcons.Attachment;
        else
            return MaterialIcons.Unknown_document;
    }

    void ActivityListener_Started(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(UpdateDownloadActivityIndicator);
    }

    void ActivityListener_Stopped(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(UpdateDownloadActivityIndicator);
    }

    private void UpdateDownloadActivityIndicator()
    {
        try
        {
            DownloadActivity.IsRunning = _activityListener.HasActivity;
            DownloadActivity.IsVisible = _activityListener.HasActivity;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"Couldn't update network activity UI for {nameof(DraftsListItem)}");
        }
    }
}
