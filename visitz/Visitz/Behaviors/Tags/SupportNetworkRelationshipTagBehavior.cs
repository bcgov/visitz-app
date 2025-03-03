using Visitz.Resources.Styles;
using Visitz.Views.Entity.SupportNetwork;
using Visitz.Views.TagViews;

namespace Visitz.Behaviors.Tags;

public class SupportNetworkRelationshipTagBehavior : TagStyleBehavior
{
    protected override void ApplyTagStyle(TagView tag)
    {
        if (tag.BindingContext is not SupportNetworkItemUi itemUi)
            return;
        var item = itemUi.SupportNetwork;
        if (!string.IsNullOrWhiteSpace(item?.Relationship))
        {
            tag.Text = item.Relationship;
            tag.BackgroundColor = VisitzColors.BC_Gold;
            tag.TextColor = VisitzColors.BC_TextColor;
            tag.FontSize = 12;
        }
    }
}
