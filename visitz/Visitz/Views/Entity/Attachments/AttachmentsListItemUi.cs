using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Resources.Styles;
using Visitz.Services;
using Visitz.Services.Attachments;
using Visitz.Services.Base;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsListItemUi :
    ObservableObject,
    IRecipient<ServiceStateMessage>,
    IDisposable
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

    private bool disposedValue;

    EntityType EntityType { get; set; }

    string RecordId { get; set; }

    string ServiceId { get; set; }

    ServiceHandler ServiceHandler { get; set; } = ServiceProvider.GetService<ServiceHandler>();

    public AttachmentsListItemUi(EntityType type, string recordId, Attachment item)
    {
        attachment = item;
        ShowDeleteButton = Attachment.FileExistsLocally;
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
            ShowDeleteButton = Attachment.FileExistsLocally;
            IsDownloading = false;
            ShowDownloadButton = !ShowDeleteButton;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                WeakReferenceMessenger.Default.UnregisterAll(this);
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
