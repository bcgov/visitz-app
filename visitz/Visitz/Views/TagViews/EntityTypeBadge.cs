using Visitz.Resources.Styles;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.TagViews;

public class EntityTypeBadge : TagView
{
	const double DefaultCornerRadius = 4;
	const double DefaultStrokeThickness = 0;
	readonly Thickness DefaultPadding = new(5, 0);

	public EntityTypeBadge()
	{
		CornerRadius = DefaultCornerRadius;
		StrokeThickness = DefaultStrokeThickness;
		Padding = DefaultPadding;
	}

	protected override void OnBindingContextChanged()
	{
		base.OnBindingContextChanged();

		if (BindingContext is CaseloadItem item)
			ApplyCaseloadItem(item);
	}

	void ApplyCaseloadItem(CaseloadItem item)
	{
		if (item.EntityType.TryParseEntityType(out EntityType type))
		{
			BackgroundColor = type.GetBackgroundColor();
			TextColor = type.GetTextColor();
		}
		else
		{
			BackgroundColor = Colors.Transparent;
			TextColor = VisitzColors.BC_TextColor;
		}

		Text = item.TypeInitials;
	}
}
