using Visitz.Extensions;
using Visitz.Models;
using Visitz.Resources.Styles;

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
            ContainerBorder.BackgroundColor = VisitzColors.KeyPlayerInfoPurpleBackground;
            ContainerBorder.Stroke = VisitzColors.ContactRelationshipTagText;
        }
        else
        {
            ContainerBorder.BackgroundColor = Colors.White;
            ContainerBorder.Stroke = VisitzColors.FamilyMemberInfoGrayBorder;
        }
    }
}