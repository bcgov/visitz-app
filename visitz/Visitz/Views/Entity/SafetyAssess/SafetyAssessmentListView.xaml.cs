using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;

namespace Visitz.Views.Entity.SafetyAssess;

public partial class SafetyAssessmentListView : ViewModelContentView, ICaseloadItemHolder
{
    new SafetyAssessmentListViewModel ViewModel => base.ViewModel as SafetyAssessmentListViewModel;

    public CaseloadItem CaseloadItem
    {
        get => ViewModel.CaseloadItem;
        set => ViewModel.CaseloadItem = value;
    }

    public SafetyAssessmentListView()
        : base(ServiceProvider.GetService<SafetyAssessmentListViewModel>())
	{
		InitializeComponent();
        BindingContext = ViewModel;
	}
}
