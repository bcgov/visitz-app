using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Services;
using Visitz.Views.BaseClasses;
using VisitzModel.Events;
using VisitzModel.Storage;

namespace Visitz.Views.Caseload;

internal partial class CaseloadDetailViewModel : VisitzViewModel
{
	LastUpdatedPrefs LastUpdatedPrefs { get; set; }

	[ObservableProperty]
	public DateTime? lastUpdated;

	public CaseloadDetailViewModel(LastUpdatedPrefs lastUpdatedPrefs)
	{
		LastUpdatedPrefs = lastUpdatedPrefs;

		LastUpdated = LastUpdatedPrefs.Get(GetCaseloadService.MakeId());
		LastUpdatedPrefs.LastUpdatedChanged += LastUpdatedPrefs_LastUpdatedChanged;
	}

	private void LastUpdatedPrefs_LastUpdatedChanged(object sender, LastUpdatedChangedEventArgs e)
	{
		if (e.Id.Equals(GetCaseloadService.MakeId()))
			LastUpdated = (sender as LastUpdatedPrefs).Get(e.Id);
	}
}
