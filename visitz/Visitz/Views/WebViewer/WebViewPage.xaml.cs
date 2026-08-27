using Microsoft.Extensions.Logging;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.WebViewer;

public partial class WebViewPage : VisitzPage<WebViewPage, WebViewModel>
{
    public Uri AuthUri
    {
        get => ViewModel.AuthUri;
        set => ViewModel.AuthUri = value;
    }

    public CancellationTokenSource? CancelTokenSource { get; set; }

    public WebViewPage(WebViewModel viewModel, ILogger<WebViewPage> logger)
        : base(viewModel, logger)
    {
        InitializeComponent();
        Setup();
    }

    partial void Setup();
}
