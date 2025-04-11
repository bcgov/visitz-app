using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Resources.Styles;
using VisitzModel.Models.Attachments;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsListItemUi : ObservableObject
{
    [ObservableProperty]
    Attachment attachment;

    [ObservableProperty]
    public bool showDeleteButton;

    [ObservableProperty]
    public bool showDownloadButton;

    [ObservableProperty]
    public Color toDownloadTextColor;

    public AttachmentsListItemUi(Attachment item)
    {
        attachment = item;
        ShowDeleteButton = item?.RelativePath is not null && item.RelativePath.Trim() != "";
        ShowDownloadButton = !ShowDeleteButton;
        if (ShowDeleteButton)
            ToDownloadTextColor = VisitzColors.BC_TextColor;
        else
            ToDownloadTextColor = VisitzColors.BC_TextColor_Lighter;
    }
}
