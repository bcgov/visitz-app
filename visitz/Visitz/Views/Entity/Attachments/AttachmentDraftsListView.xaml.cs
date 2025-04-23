using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentDraftsListView : ViewModelContentView, ICaseloadItemHolder, IFocusDraftItem
{
    new AttachmentDraftsListViewModel ViewModel => base.ViewModel as AttachmentDraftsListViewModel;

    public CaseloadItem CaseloadItem
    {
        get => ViewModel.CaseloadItem;
        set => ViewModel.CaseloadItem = value;
    }

    public IDraftItem FocusedDraftItem { get; set; }

    readonly TaskCompletionSource loadingTcs = new();

    public AttachmentDraftsListView() : base(ServiceProvider.GetService<AttachmentDraftsListViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        Loaded += AttachmentDraftsListView_Loaded;
    }

    private void AttachmentDraftsListView_Loaded(object sender, EventArgs e)
    {
        loadingTcs.TrySetResult();
    }

    protected override async void Creating()
    {
        base.Creating();

        await Task.WhenAll(loadingTcs.Task, ViewModel.attachmentsLoadedTcs.Task);

        TryNavigateToFocusDraft();
    }

    void TryNavigateToFocusDraft()
    {
        if (FocusedDraftItem == null)
            return;

        var draftItem = ViewModel.AttachmentDrafts.FirstOrDefault(draftItem =>
            draftItem.Attachment.Draft.Preview == FocusedDraftItem.Preview
            && draftItem.Attachment.Draft.LastUpdated == FocusedDraftItem.LastUpdated);

        ScrollToDraft(draftItem);
        ViewModel.OpenAttachment(draftItem.Attachment.Draft);

        FocusedDraftItem = null;
    }

    void ScrollToDraft(AttachmentDraftListItemUi draft)
    {
        DraftsCollection.ScrollTo(draft, position: ScrollToPosition.Center);
    }
}
