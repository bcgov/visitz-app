using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Entity.Attachments;

#nullable enable

public partial class PdfDetailsView : ViewModelContentView, IBusinessObjectHolder
{
    static readonly string LoadPdfFromBase64Js = "loadPdfFromBase64('{0}')";

    new PdfDetailsViewModel ViewModel =>
        base.ViewModel as PdfDetailsViewModel ?? throw new InvalidOperationException("ViewModel is null");

    public IBusinessObject? BusinessObject
    {
        get => ViewModel.BusinessObject;
        set => ViewModel.BusinessObject = value;
    }

    public Attachment? Attachment
    {
        get => ViewModel.Attachment;
        set => ViewModel.Attachment = value;
    }

    public bool IsDownloadedAttachment
    {
        get => ViewModel.IsDownloadedAttachment;
        set => ViewModel.IsDownloadedAttachment = value;
    }

    public PdfDetailsView()
        : base(ServiceProvider.GetService<PdfDetailsViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    async void WebView_Navigated(object? sender, WebNavigatedEventArgs e)
    {
        ViewModel.ShowActivityIndicator = false;

        if (!e.Url.StartsWith("file:"))
            return;
        else if (sender is WebView wv)
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
