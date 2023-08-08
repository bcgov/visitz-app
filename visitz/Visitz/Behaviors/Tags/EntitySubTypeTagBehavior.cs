using Visitz.Models;
using Visitz.Resources.Styles;
using Visitz.Views;

namespace Visitz.Behaviors;

public class EntitySubTypeTagBehavior : TagStyleBehavior
{
    protected override void ApplyTagStyle(TagView tag)
    {
        if (tag.BindingContext is not CaseloadItem item)
            return;

        tag.Text = item.CaseIncidentType;

        tag.BackgroundColor = Colors.Transparent;
        tag.BorderColor = VisitzColors.BC_Blue;
    }
}
