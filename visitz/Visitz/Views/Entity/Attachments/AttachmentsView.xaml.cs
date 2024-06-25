using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsView : ViewModelContentView
{
	public AttachmentsView() : base(ServiceProvider.GetService<AttachmentsViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}
