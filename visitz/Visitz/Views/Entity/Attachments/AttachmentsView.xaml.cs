using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.Attachments;

#nullable enable

public partial class AttachmentsView : IcmRecordContentView<AttachmentsViewModel>, IFocusDraftItem
{
    static readonly IEnumerable<string> AllowedTypes = Attachment.AllowedImageTypes.Concat(
        Attachment.AllowedDocumentTypes
    );

    AttachmentsListView? _attachmentsListView;

    public IDraftItem? FocusedDraftItem
    {
        get => ViewModel.FocusedDraftItem;
        set => ViewModel.FocusedDraftItem = value;
    }

    public AttachmentsView()
        : base(ServiceProvider.GetService<AttachmentsViewModel>(), LocalizedStrings.Attachments)
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override async Task InitAsync()
    {
        try
        {
            await base.InitAsync();

            _attachmentsListView = ServiceProvider.GetService<AttachmentsListView>();
            _attachmentsListView.RowId = RowId;
            _attachmentsListView.EntityType = EntityType;
            _attachmentsListView.FocusedDraftItem = FocusedDraftItem;

            MainGrid.Add(_attachmentsListView, 0, 0);
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            _attachmentsListView?.Dispose();
            _attachmentsListView = null;
            disposed = true;
        }
        base.Dispose(disposing);
    }

    private async void AddPhotos_Clicked(object? sender, EventArgs e)
    {
        await OpenTakePhotoView();
    }

    private async void Browse_Clicked(object? sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync(
            new()
            {
                FileTypes = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>() { { DevicePlatform.WinUI, AllowedTypes } }
                ),
            }
        );

        if (result != null)
            await SaveFile(result);
    }

    private async Task SaveFile(FileResult result)
    {
        if (result == null)
            return;

        try
        {
            await ViewModel.SaveFile(result);
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    private async Task OpenTakePhotoView()
    {
        await TakePhotoView.TryOpenWithPermissionsAsync(BusinessObject);
    }
}
