using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Models.BOs;

namespace Visitz.Controls;

public partial class ContactItemView : ContentView
{
    public static readonly BindableProperty FamilyMemberProperty =
        BindableProperty.Create(nameof(FamilyMember), typeof(string), typeof(ContactItemView));

    public FamilyMember FamilyMember
    {
        get => (FamilyMember)GetValue(FamilyMemberProperty);
        set => SetValue(FamilyMemberProperty, value);
    }

    public ContactItemView()
	{
		InitializeComponent();
        BindingContext = this;
	}
}