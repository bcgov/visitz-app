using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Resources.Styles;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.FamilyMembers;

#nullable enable

public partial class ContactItemViewModel : VisitzViewModel
{
    [ObservableProperty]
    public IcmContact contact;

    [ObservableProperty]
    public bool expanded;

    [ObservableProperty]
    public string tagText;

    [ObservableProperty]
    public Color tagBgColor;

    [ObservableProperty]
    public Color tagTextColor;

    [RelayCommand]
    public void ItemTapped()
    {
        Expanded = !Expanded;
    }

    partial void OnContactChanged(IcmContact value)
    {
        TagText = value.Relationship;

        TagBgColor = value.IsKeyPlayer
            ? VisitzColors.ContactRelationshipTagText
            : VisitzColors.ContactRelationshipTagBackground;

        TagTextColor = value.IsKeyPlayer
            ? Colors.White
            : VisitzColors.ContactRelationshipTagText;
    }
}
