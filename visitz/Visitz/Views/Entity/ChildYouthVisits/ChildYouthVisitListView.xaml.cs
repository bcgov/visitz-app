using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitListView : ViewModelContentView, ICaseloadItemHolder, IRequestedEntitySection
{
    new ChildYouthVisitListViewModel ViewModel => base.ViewModel as ChildYouthVisitListViewModel;

    public CaseloadItem CaseloadItem
    {
        get => ViewModel.CaseloadItem;
        set => ViewModel.CaseloadItem = value;
    }

    public EntitySection RequestedSection
    {
        get => ViewModel.RequestedSection;
        set => ViewModel.RequestedSection = value;
    }

    public ChildYouthVisitListView() : base(ServiceProvider.GetService<ChildYouthVisitListViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
