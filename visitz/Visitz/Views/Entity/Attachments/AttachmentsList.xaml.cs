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

		if (FocusedDraftItem != null)
			await ScrollToFocusDraft();
	}

	async Task ScrollToFocusDraft()
	{
		if (!LoadingTcs.Task.IsCompleted)
			await LoadingTcs.Task;

		var d = ViewModel.AttachmentDrafts.FirstOrDefault(draft =>
			draft.Preview == FocusedDraftItem.Preview && draft.LastUpdated == FocusedDraftItem.LastUpdated);

		DraftsCollection.ScrollTo(d, position: ScrollToPosition.Center);
	}
}
