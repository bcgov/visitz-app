using Visitz.Views.BaseClasses;
using VisitzModel.Models;

namespace Visitz.Views.Entity.Attachments;

public partial class PhotoDetailsView : ViewModelContentView
{
	new PhotoDetailsViewModel ViewModel => base.ViewModel as PhotoDetailsViewModel;

	public Attachment Attachment
	{
		get => ViewModel.Attachment;
		set => ViewModel.Attachment = value;
	}

	public PhotoDetailsView() : base(ServiceProvider.GetService<PhotoDetailsViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}
