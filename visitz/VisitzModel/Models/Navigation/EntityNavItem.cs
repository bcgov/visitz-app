using CommunityToolkit.Mvvm.ComponentModel;

namespace VisitzModel.Models.Navigation;

public partial class EntityNavItem : NavItem
{
    public EntitySection Section { get; set; } = EntitySection.Details;

    [ObservableProperty]
    public partial bool HasDraft { get; set; }
}
