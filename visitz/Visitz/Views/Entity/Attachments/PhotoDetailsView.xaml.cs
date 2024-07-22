using CommunityToolkit.Mvvm.Messaging;
using Visitz.Services;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;

namespace Visitz.Views.Entity.Attachments;

public partial class PhotoDetailsView : ViewModelContentView, ICaseloadItemHolder, IRecipient<ServiceStateMessage>
{
	new PhotoDetailsViewModel ViewModel => base.ViewModel as PhotoDetailsViewModel;

	public Attachment Attachment
	{
		get => ViewModel.Attachment;
		set => ViewModel.Attachment = value;
	}
	public CaseloadItem CaseloadItem
	{
		get => ViewModel.CaseloadItem;
		set => ViewModel.CaseloadItem = value;
	}

	public PhotoDetailsView() : base(ServiceProvider.GetService<PhotoDetailsViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;

		Unloaded += PhotoDetailsView_Unloaded;
	}

	protected override void Creating()
	{
		base.Creating();

		var attachment = ViewModel.Attachment;

		if (attachment.Draft is AttachmentDraft draft)
		{
			string id = SubmitAttachmentService.MakeId(draft.RelatedEntityId, attachment.Filename);
			WeakReferenceMessenger.Default.Register(this, id);
		}
	}

	private void PhotoDetailsView_Unloaded(object sender, EventArgs e)
	{
		WeakReferenceMessenger.Default.UnregisterAll(this);
	}

	public void Receive(ServiceStateMessage message)
	{
		if (message.FinishedSuccess)
		{
			Navigator.Navigation.RemovePage(Navigator.CurrentOpenPage);
			WeakReferenceMessenger.Default.UnregisterAll(this);
		}
	}
}
