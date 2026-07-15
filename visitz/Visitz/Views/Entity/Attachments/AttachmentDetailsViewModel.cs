using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.BaseClasses.Publishing;
using Visitz.Views.Snackbar;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.People;
using VisitzModel.Storage;
using VisitzModel.Storage.Filesystem;

namespace Visitz.Views.Entity.Attachments;

#nullable enable

public abstract partial class AttachmentDetailsViewModel : IcmRecordViewModel
{
    [ObservableProperty]
    public partial Attachment? Attachment { get; set; }

    [ObservableProperty]
    public partial bool ShowActivityIndicator { get; set; } = true;

    [ObservableProperty]
    public partial bool ShowDraftButtons { get; set; }

    [ObservableProperty]
    public partial bool IsRemovable { get; set; }

    [ObservableProperty]
    public partial string? ErrorText { get; set; }

    [ObservableProperty]
    public partial bool HasError { get; set; }
    protected AttachmentFiler? Filer { get; set; }

    protected abstract string LoadErrorText { get; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (Attachment == null)
        {
            ErrorText = LoadErrorText;
            return;
        }

        IcmContact keyPlayer =
            BusinessObject.GetKeyPlayer() ?? throw new InvalidOperationException("Unable to load key player");

        Filer = await VisitzFiles.GetAsync(Attachment, keyPlayer.FirstName, keyPlayer.LastName);

        if (Attachment.HasDraft)
            ShowDraftButtons = Attachment.FileExistsLocally;
        else
            IsRemovable = Attachment.FileExistsLocally;
    }

    partial void OnErrorTextChanged(string? value)
    {
        HasError = value?.Length > 0;
    }

    [RelayCommand]
    async Task PromptRemoveFromDeviceAsync()
    {
        if (Attachment == null)
            return;

        bool shouldRemove = await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.RemoveAttachmentFromDevice,
            LocalizedStrings.RemoveAttachmentDescription,
            LocalizedStrings.Remove,
            LocalizedStrings.Cancel
        );

        if (shouldRemove)
        {
            var prefs = ServiceProvider.GetService<UserIgnoredContentPrefs>();
            prefs.SetUserIgnoredContent(Attachment.Id, true);

            Attachment.RemoveFileFromDevice();

            string removedText = string.Format(LocalizedStrings.RemovedAttachmentFromDevice, Attachment.Filename);

            await Navigator.Navigation.PopAsync();
            SnackbarHandler.ShowText(removedText);
        }
    }

    [RelayCommand]
    async Task PromptDiscardAttachmentDraftAsync()
    {
        if (Attachment == null || !Attachment.HasDraft)
            return;

        bool shouldDiscard = await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.DiscardDraft,
            LocalizedStrings.DiscardAttachmentDraftDescription,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel
        );

        if (shouldDiscard)
        {
            string filename = Attachment.Filename;

            await Attachment.DeleteAsync();
            await Navigator.Navigation.PopAsync();

            SnackbarHandler.ShowText(string.Format(LocalizedStrings.FileDiscarded, filename));
        }
    }

    [RelayCommand]
    async Task PublishAttachmentDraftAsync()
    {
        var attachmentPublishVm = ServiceProvider.Current.GetService<AttachmentDraftPublishViewModel>();
        var logger = ServiceProvider.GetService<ILogger<PublishPage>>();

        if (Attachment?.Draft == null || attachmentPublishVm == null || Filer == null)
            return;

        await attachmentPublishVm.SetPayload(BusinessObject, Attachment.Draft, Filer);
        await Navigator.Navigation.PushModalAsync(new PublishPage(attachmentPublishVm, logger));
    }
}
