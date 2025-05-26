using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.Navigation;

public partial class EntityNavView : ViewModelContentView, IBusinessObjectHolder
{
    new EntityNavViewModel ViewModel => base.ViewModel as EntityNavViewModel;

    public IBusinessObject BusinessObject
    {
        get => ViewModel.BusinessObject;
        set => ViewModel.BusinessObject = value;
    }

    public EntityNavView() : base(ServiceProvider.GetService<EntityNavViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    public void SetRequestedSection(EntitySection section, IDraftItem focusedDraftItem = null)
    {
        ViewModel.SetRequestedSection(section, focusedDraftItem);
    }
}
