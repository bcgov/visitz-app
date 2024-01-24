using Visitz.ViewModels;

namespace Visitz.Pages;

public partial class PublishPage : VisitzPage
{
	public PublishPage(PublishViewModel publishViewModel) : base(publishViewModel)
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}
}