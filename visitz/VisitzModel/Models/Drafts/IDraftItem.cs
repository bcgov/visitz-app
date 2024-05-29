using Realms;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models.Drafts;

public interface IDraftItem : IRealmObject
{
	string RelatedEntityId { get; set; }

	DateTimeOffset DraftCreated { get; set; }

	DateTimeOffset LastUpdated { get; set; }

	string Preview { get; }

	string DraftLocation { get; set; }

	EntityType RelatedEntityType { get; set; }

	EntitySubtype RelatedEntitySubtype { get; set; }
}
