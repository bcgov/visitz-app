using Visitz.Views.BaseClasses;

namespace Visitz.Views.AppLogs;

public partial class AppLogsList : ViewModelContentView<AppLogsListViewModel>
{
    public AppLogsList(AppLogsListViewModel viewModel)
        : base(viewModel, "App logs")
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
