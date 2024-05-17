using Realms;

namespace VisitzModel.Models.Drafts;

public interface IDraftItem : IRealmObject
{
	DateTime LastUpdated { get; set; }

	string Preview { get; set; }

	string Name { get; set; }
}
