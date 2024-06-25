using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsList : ViewModelContentView
{
	public AttachmentsList() : base(ServiceProvider.GetService<AttachmentsListViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}
