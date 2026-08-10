using VisitzModel.Models.Navigation;

namespace Visitz.Views.Navigation;

public class NavItemSelectedEventArgs : EventArgs
{
    public required NavItem NavItem { get; set; }
}
