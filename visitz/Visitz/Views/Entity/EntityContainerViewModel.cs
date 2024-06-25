using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Resources.Styles;
using Visitz.ViewModels;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.Entity;

public partial class EntityContainerViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;

	[ObservableProperty]
	public Color entityTypeTextColor;

	partial void OnCaseloadItemChanged(CaseloadItem oldValue, CaseloadItem newValue)
	{
		if (newValue != null && newValue.EntityType.TryParseEntityType(out EntityType type))
			EntityTypeTextColor = type.GetTextColor();
		else
			EntityTypeTextColor = VisitzColors.BC_TextColor;
	}
}
