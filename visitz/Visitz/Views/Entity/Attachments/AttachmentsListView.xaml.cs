using Visitz.Views.BaseClasses;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsListView : IcmRecordContentView<AttachmentsListViewModel>
{
    public IDraftItem? FocusedDraftItem { get; set; }

    readonly TaskCompletionSource loadingTcs = new();

    public AttachmentsListView()
        : base(ServiceProvider.GetService<AttachmentsListViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        Loaded += AttachmentsListView_Loaded;
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        await Task.WhenAll(loadingTcs.Task, ViewModel.AttachmentsLoadedTcs.Task);

        await TryNavigateToFocusDraft();
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            Loaded -= AttachmentsListView_Loaded;
            FocusedDraftItem = null;

            disposed = true;
        }
        base.Dispose(disposing);
    }

    private void AttachmentsListView_Loaded(object? sender, EventArgs e)
    {
        loadingTcs.TrySetResult();
    }

    async Task TryNavigateToFocusDraft()
    {
        if (FocusedDraftItem is not AttachmentDraft draft)
            return;

        var found = ViewModel.AttachmentsList.FirstOrDefault(attachmentVm =>
            attachmentVm.Attachment.Id == draft.Attachment?.Id
        );

        if (found != null)
        {
            ScrollToAttachment(found);

            if (found.Attachment.Draft != null)
                await ViewModel.OpenAttachment(found);
        }

        FocusedDraftItem = null;
    }

    void ScrollToAttachment(AttachmentsListItemUi draft)
    {
        AttachmentsCollection.ScrollTo(draft, position: ScrollToPosition.Center, animate: false);
    }
}
