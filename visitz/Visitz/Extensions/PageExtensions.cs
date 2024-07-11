using Visitz.Resources.Localization;
using Visitz.Views.Snackbar;

namespace Visitz.Extensions;

internal static class PageExtensions
{
	static async Task<bool> MessagePrompt(Page page, string message, bool promptDetails)
	{
		if (promptDetails)
			return await page.DisplayAlert(
				LocalizedStrings.Error,
				message,
				LocalizedStrings.Details,
				LocalizedStrings.Ok);
		else
			await page.DisplayAlert(LocalizedStrings.Error, message, LocalizedStrings.Ok);

		return false;
	}

	static async Task<bool> DetailedMessagePrompt(Page page, string detailedMessage)
	{
		return await page.DisplayAlert(
			LocalizedStrings.Error,
			detailedMessage,
			LocalizedStrings.CopyToClipboard,
			LocalizedStrings.Ok);
	}

	public static async Task DisplayErrorAlert(this Page page, string message, string detailedMessage = null)
	{
		if (await MessagePrompt(page, message, detailedMessage != null))
			if (await DetailedMessagePrompt(page, detailedMessage))
			{
				await Clipboard.Default.SetTextAsync(detailedMessage);
				SnackbarHandler.ShowText(LocalizedStrings.CopiedToClipboard);
			}
	}

	public static async Task DisplayErrorAlert(this Page page, Exception exception)
	{
		await DisplayErrorAlert(page, exception.Message, exception.Message + " -> " + exception.StackTrace);
	}
}
