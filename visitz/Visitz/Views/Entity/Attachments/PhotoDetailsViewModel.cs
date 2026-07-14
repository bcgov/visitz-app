using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using VisitzModel.Interfaces;
using VisitzModel.Models.Attachments;

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
            Logger.LogError(ex);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
            await Navigator.Navigation.PopAsync();
        }
    }

    async Task<Stream> GetPhoto(CancellationToken token)
    {
        return await Filer.GetAppDataFileAsync(Attachment.RelativePath, token);
    }
}
