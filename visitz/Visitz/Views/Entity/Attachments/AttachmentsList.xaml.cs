using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsList : ViewModelContentView, ICaseloadItemHolder, IFocusDraftItem
{
	new AttachmentsListViewModel ViewModel => base.ViewModel as AttachmentsListViewModel;

	public CaseloadItem CaseloadItem
	{
		get => ViewModel.CaseloadItem;
		set => ViewModel.CaseloadItem = value;
	}

	public IDraftItem FocusedDraftItem { get; set; }

	readonly TaskCompletionSource LoadingTcs = new();

	public AttachmentsList() : base(ServiceProvider.GetService<AttachmentsListViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;

		Loaded += AttachmentsList_Loaded;
	}

	private void AttachmentsList_Loaded(object sender, EventArgs e)
	{
		LoadingTcs.TrySetResult();
	}

	protected override async void Creating()
	{
		base.Creating();

		if (!LoadingTcs.Task.IsCompleted)
			await LoadingTcs.Task;

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
	}

	void ScrollToDraft(AttachmentDraft draft)
	{
		DraftsCollection.ScrollTo(draft, position: ScrollToPosition.Center);
	}
}
