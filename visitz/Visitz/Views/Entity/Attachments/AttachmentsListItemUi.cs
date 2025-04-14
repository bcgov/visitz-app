using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Resources.Styles;
using Visitz.Services;
using Visitz.Services.Attachments;
using Visitz.Services.Base;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsListItemUi : ObservableObject, IRecipient<ServiceStateMessage>
{
    [ObservableProperty]
    Attachment attachment;

    [ObservableProperty]
    public bool showDeleteButton;

    [ObservableProperty]
    public bool isDownloading;

    [ObservableProperty]
    public bool showDownloadButton;

    [ObservableProperty]
    public Color toDownloadTextColor;

    EntityType EntityType { get; set; }

    string RecordId { get; set; }

    string ServiceId { get; set; }

    bool FileExists => Attachment?.RelativePath is not null && Attachment.RelativePath.Trim() != "";

    ServiceHandler ServiceHandler { get; set; } = ServiceProvider.GetService<ServiceHandler>();

    public AttachmentsListItemUi(EntityType type, string recordId, Attachment item)
    {
        attachment = item;
        ShowDeleteButton = FileExists;
        ShowDownloadButton = !ShowDeleteButton;
        if (ShowDeleteButton)
            ToDownloadTextColor = VisitzColors.BC_TextColor;
        else
            ToDownloadTextColor = VisitzColors.BC_TextColor_Lighter;

        EntityType = type;
        RecordId = recordId;

        ServiceId = GetAttachmentContentService.MakeId(EntityType, RecordId, Attachment.Id);
        WeakReferenceMessenger.Default.Register(this, ServiceId);

        SetButtonsDownloadingState(ServiceHandler.GetServiceState(ServiceId));
    }

    public void Receive(ServiceStateMessage message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            SetButtonsDownloadingState(message.Status);
        });
    }

    void SetButtonsDownloadingState(VisitzService.State state)
    {
        if (state == VisitzService.State.Running)
        {
            ShowDeleteButton = false;
            IsDownloading = true;
            ShowDownloadButton = false;
        }
        else if (state == VisitzService.State.Stopped)
        {
            ShowDeleteButton = FileExists;
            IsDownloading = false;
            ShowDownloadButton = !ShowDeleteButton;
        }
    }
}
