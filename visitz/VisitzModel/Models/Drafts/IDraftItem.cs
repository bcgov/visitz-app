using Realms;

namespace VisitzModel.Models.Drafts;

public interface IDraftItem : IRealmObject
{
	DateTimeOffset DraftCreated { get; set; }

	DateTimeOffset LastUpdated { get; set; }

	string Preview { get; }

	string DraftLocation { get; set; }
}
