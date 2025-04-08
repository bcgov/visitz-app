using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentDraftsListView : ViewModelContentView, ICaseloadItemHolder, IFocusDraftItem
{
    public static readonly BindableProperty CaseloadItemProperty =
        BindableProperty.Create(nameof(CaseloadItem), typeof(CaseloadItem), typeof(AttachmentDraftsListView));

    public static readonly BindableProperty FocusedDraftItemProperty =
        BindableProperty.Create(nameof(FocusedDraftItem), typeof(IDraftItem), typeof(AttachmentDraftsListView));

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

		var draft = ViewModel.AttachmentDrafts.FirstOrDefault(draftItem =>
			draftItem.Preview == FocusedDraftItem.Preview
			&& draftItem.LastUpdated == FocusedDraftItem.LastUpdated);

		ScrollToDraft(draft);
		ViewModel.OpenAttachment(draft);

		FocusedDraftItem = null;
	}

	void ScrollToDraft(AttachmentDraft draft)
	{
		DraftsCollection.ScrollTo(draft, position: ScrollToPosition.Center);
	}
}
