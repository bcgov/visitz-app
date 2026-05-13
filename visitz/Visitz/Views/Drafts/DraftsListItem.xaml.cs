using Microsoft.Extensions.Logging;
using Realms;
using Visitz.FontIcons;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Interfaces;
using VisitzModel.Models.Notes;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Drafts;

#nullable enable

public partial class DraftsListItem : BaseContentView
{
    readonly ServiceHandler serviceHandler = ServiceProvider.GetService<ServiceHandler>();

    public DraftsListItem()
    {
        InitializeComponent();

        serviceHandler.ServiceStarted += ServiceHandler_ServiceStarted;
        serviceHandler.ServiceFinished += ServiceHandler_ServiceFinished;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        UpdateDownloadActivityIndicator();

        Icon.Text = BindingContext is IDraftItem item ? GetIconGlyph(item) : string.Empty;
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

    private void ServiceHandler_ServiceFinished(object? sender, VisitzService e)
    {
        MainThread.BeginInvokeOnMainThread(UpdateDownloadActivityIndicator);
    }

    private void ServiceHandler_ServiceStarted(object? sender, string e)
    {
        MainThread.BeginInvokeOnMainThread(UpdateDownloadActivityIndicator);
    }

    private void UpdateDownloadActivityIndicator()
    {
        try
        {
            bool isRunning =
                BindingContext is IRealmObject realmObj
                && realmObj.IsValid
                && BindingContext is IRecordInfo info
                && serviceHandler.IsAnyServiceRunning(info.RelatedEntityId);

            DownloadActivity.IsRunning = isRunning;
            DownloadActivity.IsVisible = isRunning;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Couldn't update network activity UI for {nameof(DraftsListItem)}");
        }
    }
}
