using Visitz.Models;
using Visitz.Resources.Styles;
using Visitz.Views;

namespace Visitz.Behaviors;

class EntityTypeTagBehavior : TagStyleBehavior
{
    protected override void ApplyTagStyle(TagView tag)
    {
        if (tag.BindingContext is not CaseloadItem item)
            return;

        tag.Text = item.EntityType;

        if (item.EntityType == IcmEntity.Case)
        {
            tag.BackgroundColor = VisitzColors.EntityCaseTagBackground;
            tag.TextColor = VisitzColors.EntityCaseTagText;
            tag.IconName = "folder.png";
        }
        else if (item.EntityType == IcmEntity.Incident)
        {
            tag.BackgroundColor = VisitzColors.EntityIncidentTagBackground;
            tag.TextColor = VisitzColors.EntityIncidentTagText;
            tag.IconName = "incident_tag_icon.png";
        }
        else
        {
            tag.BackgroundColor = Colors.Transparent;
            tag.BorderColor = VisitzColors.BC_Blue;
            tag.TextColor = VisitzColors.BC_TextColor;
        }
    }
}
