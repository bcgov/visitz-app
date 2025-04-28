using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;
using Tab = Visitz.Views.Navigation.Tab;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsView : ViewModelContentView, ICaseloadItemHolder, IFocusDraftItem
{
    static readonly IEnumerable<string> AllowedTypes = Attachment.AllowedImageTypes
        .Concat(Attachment.AllowedDocumentTypes);

    new AttachmentsViewModel ViewModel => base.ViewModel as AttachmentsViewModel;

    Tab DownloadedTab;

    Tab DraftsTab;

    public CaseloadItem CaseloadItem
    {
        get => ViewModel.CaseloadItem;
        set => ViewModel.CaseloadItem = value;
    }

    public IDraftItem FocusedDraftItem
    {
        get => ViewModel.FocusedDraftItem;
        set => ViewModel.FocusedDraftItem = value;
    }

    public AttachmentsView() : base(ServiceProvider.GetService<AttachmentsViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        DownloadedTab = new Tab(LocalizedStrings.InIcm, () =>
        {
            var listView = ServiceProvider.GetService<AttachmentsListView>();
            listView.CaseloadItem = CaseloadItem;
            return listView;
        });

        DraftsTab = new(LocalizedStrings.OnMyDevice, () =>
        {
            var draftsView = ServiceProvider.GetService<AttachmentDraftsListView>();
            draftsView.CaseloadItem = CaseloadItem;
            draftsView.FocusedDraftItem = FocusedDraftItem;
            return draftsView;
        });

        AttachmentsTabs.PairedDisplayView = TabDisplayView;
        AttachmentsTabs.Tabs = [DownloadedTab, DraftsTab];

        if (FocusedDraftItem != null)
            AttachmentsTabs.SelectedTab = DraftsTab;
    }

    private async void AddPhotos_Clicked(object sender, EventArgs e)
    {
        await OpenTakePhotoView();
    }

    private async void Browse_Clicked(object sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync(new()
        {
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>()
            {
                { DevicePlatform.WinUI, AllowedTypes },
            }),
        });

        await SaveFile(result);
    }

    private async Task SaveFile(FileResult result)
    {
        if (result == null)
            return;

        try
        {
            await ViewModel.SaveFile(result);
            // TODO: Switch to drafts tab on successful save.
            // Had some weird issues where Realm was getting disposed seemingly randomly.
            // Don't have time to debug it right now, so leaving this TODO here.
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    private async Task OpenTakePhotoView()
    {
        await TakePhotoView.TryOpenWithPermissionsAsync(CaseloadItem);
    }
}
