using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Storage.Filesystem;

namespace Visitz.Views.Pdfs;

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

    public Attachment? Attachment {  get; set; }

    public CaseloadItem? CaseloadItem { get; set; }

    AttachmentFiler? Filer { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (Attachment == null || CaseloadItem == null)
        {
            ErrorText = LocalizedStrings.PdfContentMissing;
            return;
        }

        if (Assembly.GetEntryAssembly() is Assembly entry)
            Source = GetEmbedPath(entry);
        else
        {
            ErrorText = LocalizedStrings.UnableToLoadPdf;

            ServiceProvider.GetService<ILogger<PdfDetailsViewModel>>()
                .LogError("{ErrorText} -> Couldn't load entry assembly", ErrorText);

            return;
        }

        Filer = await VisitzFiles.GetAsync(
            Attachment,
            CaseloadItem.KeyPlayer.FirstName,
            CaseloadItem.KeyPlayer.LastName);
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
    async Task PromptRemoveFromDevice()
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
}
