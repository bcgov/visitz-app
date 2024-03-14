using CommunityToolkit.Mvvm.ComponentModel;

namespace Visitz.ViewModels;

internal partial class CaseloadDetailViewModel : VisitzViewModel
{
	[ObservableProperty]
	public DateTime? lastUpdated;

	public override void Create()
	{
		base.Create();

		// TODO: load last time the caseload was updated
		//LastUpdated = 
	}
}
