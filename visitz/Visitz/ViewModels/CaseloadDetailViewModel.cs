using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Services;
using VisitzModel.Events;
using VisitzModel.Storage;

namespace Visitz.ViewModels;

internal partial class CaseloadDetailViewModel : VisitzViewModel
{
	LastUpdatedPrefs LastUpdatedPrefs { get; set; }

	[ObservableProperty]
	public DateTime? lastUpdated;

	public override void Create()
	{
		base.Create();

		LastUpdatedPrefs = ServiceProvider.GetService<LastUpdatedPrefs>();
		LastUpdatedPrefs.LastUpdatedChanged += LastUpdatedPrefs_LastUpdatedChanged;

		LastUpdated = LastUpdatedPrefs.Get(GetCaseloadService.MakeId());
	}

	public override void Destroy()
	{
		base.Destroy();

		LastUpdatedPrefs.LastUpdatedChanged -= LastUpdatedPrefs_LastUpdatedChanged;
	}

	private void LastUpdatedPrefs_LastUpdatedChanged(object sender, LastUpdatedChangedEventArgs e)
	{
		if (e.Id.Equals(GetCaseloadService.MakeId()))
			LastUpdated = (sender as LastUpdatedPrefs).Get(e.Id);
	}
}
