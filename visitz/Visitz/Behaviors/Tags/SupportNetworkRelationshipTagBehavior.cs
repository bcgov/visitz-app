using Visitz.Resources.Styles;
using Visitz.Views.TagViews;
using VisitzModel.Models.People;

namespace Visitz.Behaviors.Tags;

public class SupportNetworkRelationshipTagBehavior : TagStyleBehavior
{
    protected override void ApplyTagStyle(TagView tag)
    {
        if (tag.BindingContext is not SupportNetworkItem item)
            return;
        if (!string.IsNullOrWhiteSpace(item?.Relationship))
        {
            tag.Text = item.Relationship;
            tag.BackgroundColor = VisitzColors.BC_Gold;
            tag.TextColor = VisitzColors.BC_TextColor;
        }
    }
}
