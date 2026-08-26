using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.Attachments;
using VisitzModel.Storage.Filesystem;

namespace Visitz.Views.Entity.Attachments;

public partial class PhotoDetailsViewModel : AttachmentDetailsViewModel, IBusinessObjectHolder
{
    [ObservableProperty]
    public partial ImageSource DetailImage { get; set; }

    protected override string LoadErrorText => LocalizedStrings.ImageContentMissing;

    protected override async Task InitAsync()
    {
        try
        {
            await base.InitAsync();

            DetailImage = ImageSource.FromStream(GetPhoto);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
            await Navigator.Navigation.PopAsync();
        }
    }

    async Task<Stream> GetPhoto(CancellationToken token)
    {
        if (Filer == null)
            throw new InvalidOperationException($"{nameof(AttachmentFiler)} should not be null");

        if (Attachment == null)
            throw new InvalidOperationException($"{nameof(Attachment)} should not be null");

        return await Filer.GetAppDataFileAsync(Attachment.RelativePath, token);
    }
}
