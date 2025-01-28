using Visitz.Resources.Styles;
using Visitz.Views.TagViews;
using VisitzModel.Models.People;

namespace Visitz.Behaviors.Tags;

public class ContactRelationshipTagBehavior : TagStyleBehavior
{
    protected override void ApplyTagStyle(TagView tag)
    {
        if (tag.BindingContext is not FamilyMember fam)
            return;

        tag.Text = fam.Relationship;

        if (fam.IsKeyPlayer)
        {
            tag.BackgroundColor = VisitzColors.ContactRelationshipTagText;
            tag.TextColor = Colors.White;
        }
        else
        {
            tag.BackgroundColor = VisitzColors.ContactRelationshipTagBackground;
            tag.TextColor = VisitzColors.ContactRelationshipTagText;
        }
    }
}
