using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Resources.Styles;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Entity;

public partial class EntityContainerViewModel : VisitzViewModel, IBusinessObjectHolder
{
    [ObservableProperty]
    public IBusinessObject businessObject;

    [ObservableProperty]
    public Color entityTypeTextColor;

    partial void OnBusinessObjectChanged(IBusinessObject oldValue, IBusinessObject newValue)
    {
        if (newValue != null)
            EntityTypeTextColor = newValue.EntityType.GetTextColor();
        else
            EntityTypeTextColor = VisitzColors.BC_TextColor;
    }
}
