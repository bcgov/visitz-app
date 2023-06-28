using Visitz.Models;
using Visitz.Extensions;

namespace Visitz.Views;

public partial class ContactItemView : ContentView
{
    public ContactItemView()
	{
		InitializeComponent();
	}

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        UpdateUI();
    }

    private void UpdateUI()
    {
        var member = (FamilyMember)BindingContext;
        if (member?.KeyPlayer == "Y")
        {
            ContainerBorder.BackgroundColor = Application.Current.Resources.TryGetColor("KeyPlayerInfoPurpleBackground", Colors.Purple);
            ContainerBorder.Stroke = Application.Current.Resources.TryGetColor("EntitySubtypeTagPurpleText", Colors.Purple);
            RelationshipTagView.BackgroundColor = Application.Current.Resources.TryGetColor("EntitySubtypeTagPurpleText", Colors.Purple);
            RelationshipTagView.TextColor = Colors.White;
        }
        else
        {
            ContainerBorder.BackgroundColor = Colors.White;
            ContainerBorder.Stroke = Application.Current.Resources.TryGetColor("FamilyMemberInfoGrayBorder", Colors.Purple);
            RelationshipTagView.BackgroundColor = Application.Current.Resources.TryGetColor("EntitySubtypeTagPurpleBackground", Colors.Red);
            RelationshipTagView.TextColor = Application.Current.Resources.TryGetColor("EntitySubtypeTagPurpleText", Colors.Red);
        }
    }
}