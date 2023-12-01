using Visitz.Models;
using Visitz.VisualStates;

namespace Visitz.Views.Caseload;

public partial class FilterPopupView : ViewModelContentView
{
	public event EventHandler<SubtypeSelectedEventArgs> SubtypeSelected;

	public FilterPopupView() : base(ServiceProvider.GetService<FilterPopupViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}

    private void ActivatableTagView_ActiveStateChanged(object sender, IActiveState.ActiveChangedEventArgs e)
    {
		var tag = sender as ActivatableTagView;
		var caseloadItem = tag.BindingContext as CaseloadItem;

		SubtypeSelected?.Invoke(this, new SubtypeSelectedEventArgs(caseloadItem.CaseIncidentType));
    }

	public class SubtypeSelectedEventArgs(string subtype) : EventArgs
    {
		public string Subtype { get; set; } = subtype;
	}
}