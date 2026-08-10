using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.FontIcons;
using Visitz.Resources.Styles;
using Visitz.Services;
using Visitz.Services.Attachments;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzModel.Formats;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsListItemUi : ObservableObject, IRecipient<ServiceStateMessage>, IDisposable
{
    [ObservableProperty]
    public partial Attachment Attachment { get; set; }

    [ObservableProperty]
    public partial bool ShowDeleteButton { get; set; }

    [ObservableProperty]
    public partial bool IsDownloading { get; set; }

    [ObservableProperty]
    public partial bool ShowDownloadButton { get; set; }

    [ObservableProperty]
    public partial Color ToDownloadTextColor { get; set; } = VisitzColors.BC_TextColor_Lighter;

    [ObservableProperty]
    public partial bool ShowTemplate { get; set; }

    [ObservableProperty]
    public partial string FileIconGlyph { get; set; } = FluentIcons.Document_20_regular;

    [ObservableProperty]
    public partial string FileSizeDisplay { get; set; } = string.Empty;

    private bool disposedValue;

    EntityType EntityType { get; set; }

    string RecordId { get; set; }

    string ServiceId { get; set; }

    ServiceHandler ServiceHandler { get; set; } = ServiceProvider.GetService<ServiceHandler>();

    public AttachmentsListItemUi(EntityType type, string recordId, Attachment item)
    {
        Attachment = item;
        EntityType = type;
        RecordId = recordId;

        ServiceId = GetAttachmentContentService.MakeId(EntityType, RecordId, Attachment.Id);
        WeakReferenceMessenger.Default.Register(this, ServiceId);

        SetButtonsDownloadingState(ServiceHandler.GetServiceState(ServiceId));
        Attachment.PropertyChanged += Attachment_PropertyChanged;
        UpdateItemUi();
    }

    private void Attachment_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Attachment.FileExistsLocally))
            UpdateItemUi();
    }

    void UpdateItemUi()
    {
        IsDownloading = false;
        ShowDeleteButton = Attachment.FileExistsLocally;
        ShowDownloadButton = !ShowDeleteButton;
        ShowTemplate = Attachment.Template?.Trim().Length > 0;

        if (Attachment.FileExistsLocally)
            ToDownloadTextColor = VisitzColors.BC_TextColor;
        else
            ToDownloadTextColor = VisitzColors.BC_TextColor_Lighter;

        FileIconGlyph = Attachment.Extension.Trim().ToLowerInvariant() switch
        {
            ".pdf" => FluentIcons.Document_pdf_20_regular,
            ".jpg" or ".jpeg" => FluentIcons.Image_20_regular,
            _ => FluentIcons.Document_20_regular,
        };

        FileSizeDisplay = Attachment.SizeBytes is int size ? Sizes.BytesToString(size) : "-";
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
            UpdateItemUi();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Attachment.PropertyChanged -= Attachment_PropertyChanged;
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
