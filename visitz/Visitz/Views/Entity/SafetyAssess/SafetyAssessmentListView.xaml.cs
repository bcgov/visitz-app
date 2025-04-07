using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.SafetyAssess;

public partial class SafetyAssessmentListView :
    ViewModelContentView,
    ICaseloadItemHolder,
    IRequestedEntitySection
{
    new SafetyAssessmentListViewModel ViewModel => base.ViewModel as SafetyAssessmentListViewModel;

    public CaseloadItem CaseloadItem
    {
        get => ViewModel.CaseloadItem;
        set => ViewModel.CaseloadItem = value;
    }

    public EntitySection RequestedSection { get; set; }

    public SafetyAssessmentListView()
        : base(ServiceProvider.GetService<SafetyAssessmentListViewModel>())
	{
		InitializeComponent();
        BindingContext = ViewModel;
	}

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (RequestedSection == EntitySection.SafetyAssessmentEntry)
            await ViewModel.OpenSafetyAssessmentView();
    }
}
