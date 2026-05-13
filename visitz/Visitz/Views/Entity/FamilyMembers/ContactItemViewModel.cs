using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Resources.Styles;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.FamilyMembers;

#nullable enable

public partial class ContactItemViewModel : VisitzViewModel
{
    [ObservableProperty]
    public partial IcmContact Contact { get; set; }

    [ObservableProperty]
    public partial bool Expanded { get; set; }

    [ObservableProperty]
    public partial Color? TagBgColor { get; set; }

    [ObservableProperty]
    public partial Color? TagTextColor { get; set; }

    [ObservableProperty]
    public partial string ExpandedChevronGlyph { get; set; } = MaterialIcons.Keyboard_arrow_down;

    [ObservableProperty]
    public partial string DeceasedText { get; set; } = LocalizedStrings.Deceased;

    public ContactItemViewModel(IcmContact icmContact)
    {
        Contact = icmContact;
    }

    [RelayCommand]
    public void ItemTapped()
    {
        Expanded = !Expanded;
    }

    partial void OnContactChanged(IcmContact value)
    {
        TagBgColor = value.IsKeyPlayer
            ? VisitzColors.ContactRelationshipTagText
            : VisitzColors.ContactRelationshipTagBackground;

        TagTextColor = value.IsKeyPlayer ? Colors.White : VisitzColors.ContactRelationshipTagText;
    }

    partial void OnExpandedChanged(bool value)
    {
        ExpandedChevronGlyph = value ? MaterialIcons.Keyboard_arrow_up : MaterialIcons.Keyboard_arrow_down;
    }
}
