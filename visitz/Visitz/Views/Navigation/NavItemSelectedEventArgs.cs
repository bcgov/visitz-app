using VisitzModel.Models.Navigation;

namespace Visitz.Views.Navigation;

public class NavItemSelectedEventArgs : EventArgs
{
	public NavItem NavItem { get; set; }
}
