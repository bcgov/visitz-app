using Visitz.Resources.Localization;
using Visitz.Views;

namespace Visitz;

internal class SnackbarHandler
{
	public static readonly int DefaultDurationSeconds = 5;

	public static void ShowTextWithDetails(
		string snackPrompt,
		string messageTitle,
		string fullMessage,
		TimeSpan? duration = null)
	{
		if (Navigator.CurrentOpenPage is ISnackbarPresenter presenter)
			presenter.SetSnackbar(new VisitzSnackbar()
			{
				Message = snackPrompt,
				ActionText = LocalizedStrings.MoreInfo,
				Action = async () => await ShowDialogMessage(messageTitle, fullMessage),
				Duration = duration ?? TimeSpan.FromSeconds(DefaultDurationSeconds)
			});
	}

	static async Task ShowDialogMessage(string title, string message)
	{
		await Navigator.CurrentOpenPage.DisplayAlert(title, message, LocalizedStrings.Ok);
	}
}
