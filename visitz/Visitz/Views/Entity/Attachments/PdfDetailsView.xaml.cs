using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;

namespace Visitz.Views.Entity.Attachments;

#nullable enable

public partial class PdfDetailsView : ViewModelContentView, ICaseloadItemHolder
{
    static readonly string LoadPdfFromBase64Js = "loadPdfFromBase64('{0}')";

    new PdfDetailsViewModel ViewModel => base.ViewModel as PdfDetailsViewModel
        ?? throw new InvalidOperationException("ViewModel is null");

    public CaseloadItem? CaseloadItem
    {
        get => ViewModel.CaseloadItem;
        set => ViewModel.CaseloadItem = value;
    }

    public Attachment? Attachment
    {
        get => ViewModel.Attachment;
        set => ViewModel.Attachment = value;
    }

    public PdfDetailsView() : base(ServiceProvider.GetService<PdfDetailsViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        ViewModel.ShowActivityIndicator = false;

        if (sender is WebView wv)
            await TryLoadPdf(wv, await ViewModel.MakeBase64Pdf());
    }

    async Task TryLoadPdf(WebView webView, string? base64)
    {
        if (base64 != null)
        {
            string script = string.Format(LoadPdfFromBase64Js, base64);
            await webView.EvaluateJavaScriptAsync(script);
        }
        else
        {
            ViewModel.ErrorText = LocalizedStrings.PdfContentMissing;
        }
    }
}
