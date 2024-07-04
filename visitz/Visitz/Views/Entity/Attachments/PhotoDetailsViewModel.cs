using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;

namespace Visitz.Views.Entity.Attachments;

internal partial class PhotoDetailsViewModel : VisitzViewModel
{
	[ObservableProperty]
	public Attachment attachment;

	[ObservableProperty]
	public ImageSource detailImage;
}
