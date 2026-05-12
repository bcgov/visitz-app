using Visitz.Resources.Localization;
using Visitz.Views.Snackbar;

namespace Visitz.Extensions;

internal static class PageExtensions
{
    static async Task<bool> MessagePrompt(Page page, string message, bool promptDetails, string title = null)
    {
        if (promptDetails)
            return await page.DisplayAlertAsync(
                !string.IsNullOrEmpty(title) ? title : LocalizedStrings.Error,
                message,
                LocalizedStrings.Details,
                LocalizedStrings.Ok
            );
        else
            await page.DisplayAlertAsync(LocalizedStrings.Error, message, LocalizedStrings.Ok);

        return false;
    }

    static async Task<bool> DetailedMessagePrompt(Page page, string detailedMessage, string title = null)
    {
        string prompt = LocalizedStrings.ErrorDialogCopyPrompt + Environment.NewLine + Environment.NewLine;
        return await page.DisplayAlertAsync(
            title,
            prompt + detailedMessage,
            LocalizedStrings.CopyToClipboard,
            LocalizedStrings.Ok
        );
    }

    public static async Task DisplayErrorAlert(
        this Page page,
        string message,
        string detailedMessage = null,
        string title = null
    )
    {
        string displayTitle = !string.IsNullOrEmpty(title) ? title : LocalizedStrings.Error;
        if (await MessagePrompt(page, message, detailedMessage != null, displayTitle))
            if (await DetailedMessagePrompt(page, detailedMessage, displayTitle))
            {
                await Clipboard.Default.SetTextAsync(detailedMessage);
                SnackbarHandler.ShowText(LocalizedStrings.CopiedToClipboard);
            }
    }

    public static async Task DisplayErrorAlert(
        this Page page,
        Exception exception,
        string? title = "",
        string? message = ""
    )
    {
        message += Environment.NewLine + Environment.NewLine + exception.Message;
        await DisplayErrorAlert(page, message.Trim(), exception.Message + " -> " + exception.ToString(), title);
    }
}
