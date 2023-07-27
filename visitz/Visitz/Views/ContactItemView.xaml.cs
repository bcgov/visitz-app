using Visitz.Extensions;
using Visitz.Models;

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
        var familyMember = BindingContext as FamilyMember;

        CellPhoneTagView.IsVisible = familyMember.CellPhone?.Length > 0;
        HomePhoneTagView.IsVisible = familyMember.HomePhone?.Length > 0;
        PhoneRow.IsVisible = CellPhoneTagView.IsVisible || HomePhoneTagView.IsVisible;

        if (familyMember.IsKeyPlayer)
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