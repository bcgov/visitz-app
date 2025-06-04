using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.Details;

public partial class EntityDetailsViewModel : VisitzViewModel, IBusinessObjectHolder
{
    [ObservableProperty]
    public IBusinessObject businessObject;

    [ObservableProperty]
    public IcmContact keyPlayer;

    protected override Task InitAsync()
    {
        var init = base.InitAsync();

        KeyPlayer = BusinessObject.GetKeyPlayer();

        return init;
    }
}
