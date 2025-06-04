using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Entity.Details;

public partial class EntityDetailsView : ViewModelContentView, IBusinessObjectHolder
{
    public IBusinessObject BusinessObject
    {
        get => (ViewModel as IBusinessObjectHolder).BusinessObject;
        set => (ViewModel as IBusinessObjectHolder).BusinessObject = value;
    }

    public EntityDetailsView() : base(ServiceProvider.GetService<EntityDetailsViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
