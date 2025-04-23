using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.BaseClasses.Publishing;
using Visitz.Views.Snackbar;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Storage.Filesystem;

namespace Visitz.Views.Entity.Attachments;

#nullable enable

internal partial class PdfDetailsViewModel : VisitzViewModel, ICaseloadItemHolder
{
    static readonly string EmbedHtmlPath = Path.Join("PDF", "pdf-embed.html");

    [ObservableProperty]
    public WebViewSource? source;

    [ObservableProperty]
    public bool showActivityIndicator = true;

    [ObservableProperty]
    public string? errorText;

    [ObservableProperty]
    public bool hasError;

    [ObservableProperty]
    public bool showDraftButtons;

    [ObservableProperty]
    public bool isRemovable;

    [ObservableProperty]
    public Attachment? attachment;

    public CaseloadItem? CaseloadItem { get; set; }

    public bool IsDownloadedAttachment { get; set; }

    AttachmentFiler? Filer { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (Attachment == null || CaseloadItem == null)
        {
            ErrorText = LocalizedStrings.PdfContentMissing;
            return;
        }

        Filer = await VisitzFiles.GetAsync(
            Attachment,
            CaseloadItem.KeyPlayer.FirstName,
            CaseloadItem.KeyPlayer.LastName);

        if (Assembly.GetEntryAssembly() is Assembly entry)
            Source = GetEmbedPath(entry);
        else
        {
            ErrorText = LocalizedStrings.UnableToLoadPdf;

            ServiceProvider.GetService<ILogger<PdfDetailsViewModel>>()
                .LogError("{ErrorText} -> Couldn't load entry assembly", ErrorText);

            return;
        }

        ShowDraftButtons = Attachment.FileExistsLocally
            && !IsDownloadedAttachment
            && Attachment.HasDraft;

        IsRemovable = Attachment.FileExistsLocally && IsDownloadedAttachment;
    }

    static string? GetEmbedPath(Assembly entry)
    {
        return Path.Join(Path.GetDirectoryName(entry.Location), EmbedHtmlPath);
    }

    partial void OnErrorTextChanged(string? value)
    {
        HasError = value?.Length > 0;
    }

    public async Task<string> MakeBase64Pdf()
    {
        if (Attachment != null && Filer != null)
        {
            Stream stream = await Filer.GetAppDataFileAsync(Attachment.RelativePath);
            return Convert.ToBase64String(await stream.AsBytesAsync());
        }
        else
            return "";
    }

    [RelayCommand]
    async Task PromptRemoveFromDeviceAsync()
    {
        if (Attachment == null)
            return;

        bool shouldRemove = await Navigator.CurrentOpenPage.DisplayAlert(
            LocalizedStrings.RemoveAttachmentFromDevice,
            LocalizedStrings.RemoveAttachmentDescription,
            LocalizedStrings.Remove,
            LocalizedStrings.Cancel);

        if (shouldRemove)
        {
            Attachment.RemoveFileFromDevice();

            string removedText = string.Format(
                LocalizedStrings.RemovedAttachmentFromDevice,
                Attachment.Filename);

            await Navigator.Navigation.PopAsync();
            SnackbarHandler.ShowText(removedText);
        }
    }

    [RelayCommand]
    async Task PromptDiscardAttachmentAsync()
    {
        if (Attachment == null || !Attachment.HasDraft)
            return;

        bool shouldDiscard = await Navigator.CurrentOpenPage.DisplayAlert(
            LocalizedStrings.DiscardDraft,
            LocalizedStrings.DiscardAttachmentDraftDescription,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel);

        if (shouldDiscard)
        {
            string filename = Attachment.Filename;

            await Attachment.DeleteAsync();
            await Navigator.Navigation.PopAsync();

            SnackbarHandler.ShowText(LocalizedStrings.FileDiscarded.Format(filename));
        }
    }

    [RelayCommand]
    async Task PublishAttachmentDraftAsync()
    {
        var attachmentPublishVm = ServiceProvider.Current.GetService<AttachmentDraftPublishViewModel>();

        if (Attachment?.Draft == null || attachmentPublishVm == null || Filer == null)
            return;

        attachmentPublishVm.AttachmentDraft = Attachment.Draft;
        attachmentPublishVm.AttachmentFiler = Filer;

        await Navigator.Navigation.PushModalAsync(new PublishPage(attachmentPublishVm));
    }
}
