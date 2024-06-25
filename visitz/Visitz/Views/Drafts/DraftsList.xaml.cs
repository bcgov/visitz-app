using Visitz.Resources.Localization;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models.Drafts;

namespace Visitz.Views.Drafts;

public partial class DraftsList : ViewModelContentView
{
	new DraftsListViewModel ViewModel => base.ViewModel as DraftsListViewModel;

	public DraftsList() : base(ServiceProvider.GetService<DraftsListViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}

	protected override void Creating()
	{
		base.Creating();

		ViewModel.SelectedItemRelatedMissing += ViewModel_SelectedItemRelatedMissing;
	}

	protected override void Destroying()
	{
		base.Destroying();

		ViewModel.SelectedItemRelatedMissing -= ViewModel_SelectedItemRelatedMissing;
	}

	void ViewModel_SelectedItemRelatedMissing(object sender, IDraftItem draft)
	{
		_ = DoPromptDiscardAsync(draft);
	}

	static async Task DoPromptDiscardAsync(IDraftItem draft)
	{
		if (await PromptDiscardDraftAsync(draft))
			await DraftsListViewModel.DeleteDraft(draft);
	}

	static async Task<bool> PromptDiscardDraftAsync(IDraftItem draft)
	{
		string message = string.Format(
			LocalizedStrings.DiscardUnlinkedDraftDesc,
			draft.RelatedEntityType.GetDisplayString(),
			!string.IsNullOrWhiteSpace(draft.DraftLocation) ? draft.DraftLocation : draft.RelatedEntityId
		);

		return await Navigator.CurrentOpenPage.DisplayAlert(
			LocalizedStrings.DiscardUnlinkedDraft,
			message,
			LocalizedStrings.DiscardDraft,
			LocalizedStrings.CancelAndKeepDraft);
	}
}
