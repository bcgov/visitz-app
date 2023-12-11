using Visitz.FontIcons;
using Visitz.Models;
using Visitz.Resources.Styles;
using Visitz.Views;

namespace Visitz.Behaviors.Tags;

public class EntityTypeTagBehavior : TagStyleBehavior
{
    protected override void ApplyTagStyle(TagView tag)
    {
        if (tag.BindingContext is not CaseloadItem item)
            return;

        tag.BorderColor = Colors.Transparent;
        tag.BackgroundColor = Colors.Transparent;

        if (string.IsNullOrWhiteSpace(tag.Text))
            tag.Text = item.FullType;

        if (item.EntityType == IcmEntity.Case)
        {
            tag.TextColor = VisitzColors.EntityCaseTagText;
            tag.ImageSource = MaterialIcons.Folder.GetFilledMaterialIcon();
        }
        else if (item.EntityType == IcmEntity.Incident)
        {
            tag.TextColor = VisitzColors.EntityIncidentTagText;
            tag.ImageSource = MaterialIcons.Warning.GetFilledMaterialIcon();
        }
        else if (item.EntityType == IcmEntity.Memo)
        {
            tag.TextColor = VisitzColors.EntityMemoTagText;
            tag.ImageSource = MaterialIcons.Note_alt.GetFilledMaterialIcon();
        }
        else if (item.EntityType == IcmEntity.ServiceRequest)
        {
            tag.TextColor = VisitzColors.EntityServiceRequestTagText;
            tag.ImageSource = MaterialIcons.Headset_mic.GetFilledMaterialIcon();
        }
        else
        {
            tag.TextColor = VisitzColors.BC_TextColor;
            tag.ImageSource = null;
        }
    }
}
