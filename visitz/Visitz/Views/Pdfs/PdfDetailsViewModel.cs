using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;

namespace Visitz.Views.Pdfs;

#nullable enable

internal partial class PdfDetailsViewModel : VisitzViewModel
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

    public Stream? PdfStream { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (PdfStream == null)
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
        }
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            PdfStream?.Dispose();

            disposed = true;
        }
        base.Dispose(disposing);
    }

    static string? GetEmbedPath(Assembly entry)
    {
        return Path.Join(Path.GetDirectoryName(entry.Location), EmbedHtmlPath);
    }

    partial void OnErrorTextChanged(string? value)
    {
        HasError = value?.Length > 0;
    }

    public async Task<string?> MakeBase64Pdf()
    {
        return PdfStream != null
            ? Convert.ToBase64String(await PdfStream.AsBytesAsync())
            : null;
    }
}
