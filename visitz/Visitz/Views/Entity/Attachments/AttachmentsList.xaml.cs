using Visitz.Views.BaseClasses;
using VisitzModel.Models;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsList : ViewModelContentView, ICaseloadItemHolder
{
	new AttachmentsListViewModel ViewModel => base.ViewModel as AttachmentsListViewModel;

	public CaseloadItem CaseloadItem
	{
		get => ViewModel.CaseloadItem;
		set => ViewModel.CaseloadItem = value;
	}

	public AttachmentsList() : base(ServiceProvider.GetService<AttachmentsListViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}
