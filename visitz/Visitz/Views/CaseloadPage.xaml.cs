using Visitz.ViewModels;

namespace Visitz.Views;

public partial class CaseloadPage : VisitzPage
{
    private static CaseloadPage Instance { get; set; }

    private new CaseloadViewModel ViewModel => base.ViewModel as CaseloadViewModel;

    public static CaseloadPage GetInstance()
    {
        Instance ??= new CaseloadPage(new CaseloadViewModel());
        return Instance;
    }

    public CaseloadPage(CaseloadViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await ViewModel.OpenDebugOptionsPage();
    }
}
