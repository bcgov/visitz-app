using VisitzModel.Models;

namespace Visitz.Views.Navigation;

public class NavItemSelectedEventArgs : EventArgs
{
	public NavItem NavItem { get; set; }
}
