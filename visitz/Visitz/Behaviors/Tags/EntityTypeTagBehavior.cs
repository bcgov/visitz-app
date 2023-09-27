using Visitz.FontIcons;
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
        tag.BorderColor = Colors.Transparent;

        if (item.EntityType == IcmEntity.Case)
        {
            tag.BackgroundColor = VisitzColors.EntityCaseTagBackground;
            tag.TextColor = VisitzColors.EntityCaseTagText;
            tag.ImageSource = MaterialIcons.Folder.GetUnfilledMaterialIcon(VisitzColors.EntityCaseTagText);
        }
        else if (item.EntityType == IcmEntity.Incident)
        {
            tag.BackgroundColor = VisitzColors.EntityIncidentTagBackground;
            tag.TextColor = VisitzColors.EntityIncidentTagText;
            tag.ImageSource = MaterialIcons.Warning.GetUnfilledMaterialIcon(VisitzColors.EntityIncidentTagText);
        }
        else if (item.EntityType == IcmEntity.Memo)
        {
            tag.BackgroundColor = VisitzColors.EntityMemoTagBackground;
            tag.TextColor = VisitzColors.EntityMemoTagText;
            tag.ImageSource = MaterialIcons.Note_alt.GetUnfilledMaterialIcon(VisitzColors.EntityMemoTagText);
        }
        else if (item.EntityType == IcmEntity.ServiceRequest)
        {
            tag.BackgroundColor = VisitzColors.EntityServiceRequestTagBackground;
            tag.TextColor = VisitzColors.EntityServiceRequestTagText;
            tag.ImageSource = MaterialIcons.Headset_mic
                .GetUnfilledMaterialIcon(VisitzColors.EntityServiceRequestTagText);
        }
        else
        {
            tag.BackgroundColor = Colors.Transparent;
            tag.BorderColor = VisitzColors.BC_Blue;
            tag.TextColor = VisitzColors.BC_TextColor;
            tag.ImageSource = null;
        }
    }
}
