using Visitz.FontIcons;
using Visitz.Models;
using Visitz.Resources.Styles;
using Visitz.Views;

namespace Visitz.Behaviors;

public class EntityTypeTagBehavior : TagStyleBehavior
{
    protected override void ApplyTagStyle(TagView tag)
    {
        if (tag.BindingContext is not CaseloadItem item)
            return;

        tag.BorderColor = Colors.Transparent;
        tag.BackgroundColor = Colors.Transparent;
        tag.TextTransform = TextTransform.Uppercase;
        tag.Text = item.FullType;

        if (item.EntityType == IcmEntity.Case)
        {
            tag.TextColor = VisitzColors.EntityCaseTagText;
            tag.ImageSource = MaterialIcons.Folder.GetFilledMaterialIcon(VisitzColors.EntityCaseTagText);
        }
        else if (item.EntityType == IcmEntity.Incident)
        {
            tag.TextColor = VisitzColors.EntityIncidentTagText;
            tag.ImageSource = MaterialIcons.Warning.GetFilledMaterialIcon(VisitzColors.EntityIncidentTagText);
        }
        else if (item.EntityType == IcmEntity.Memo)
        {
            tag.TextColor = VisitzColors.EntityMemoTagText;
            tag.ImageSource = MaterialIcons.Note_alt.GetFilledMaterialIcon(VisitzColors.EntityMemoTagText);
        }
        else if (item.EntityType == IcmEntity.ServiceRequest)
        {
            tag.TextColor = VisitzColors.EntityServiceRequestTagText;
            tag.ImageSource = MaterialIcons.Headset_mic
                .GetFilledMaterialIcon(VisitzColors.EntityServiceRequestTagText);
        }
        else
        {
            tag.TextColor = VisitzColors.BC_TextColor;
            tag.ImageSource = null;
        }
    }
}
