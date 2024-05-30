namespace VisitzModel.Models.Navigation;

public class EntityNavItem : NavItem
{
	public EntitySection Section { get; set; } = EntitySection.Details;

	public bool HasDraft { get; set; }
}
