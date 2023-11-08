using Visitz.Storage;

namespace Visitz.Pages;

public partial class DebugOptionsPage : ContentPage
{
    public static bool IsOpen => Navigator.CurrentOpenPage?.GetType() == typeof(DebugOptionsPage);

    public DebugOptionsPage()
	{
		InitializeComponent();
	}

	public static async Task TryOpen(Page fromPage = null)
	{
		if (DebugOptions.Enabled && !IsOpen)
			await Navigator.GoToPage<DebugOptionsPage>(fromPage);
    }
}