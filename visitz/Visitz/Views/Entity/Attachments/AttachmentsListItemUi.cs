using CommunityToolkit.Mvvm.ComponentModel;
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

    public AttachmentsListItemUi(Attachment item)
    {
        attachment = item;
        ShowDeleteButton = item?.RelativePath is not null && item.RelativePath.Trim() != "";
        ShowDownloadButton = !ShowDeleteButton;
    }
}
