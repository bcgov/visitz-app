using Visitz.Storage;
using Visitz.ViewModels;

namespace Visitz.Pages;

public partial class DebugOptionsPage : VisitzPage
{
    public static bool IsOpen => VisitzApp.CurrentOpenPage?.GetType() == typeof(DebugOptionsPage);

    public DebugOptionsPage(DebugOptionsViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	public static async Task TryOpen(Page fromPage = null)
	{
		if (DebugOptions.Enabled && !IsOpen)
			await NavigateTo<DebugOptionsPage>(fromPage);
    }
}