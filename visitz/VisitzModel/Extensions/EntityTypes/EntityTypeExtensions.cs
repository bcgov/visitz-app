using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Extensions.EntityTypes;

public static class EntityTypeExtensions
{
	public static string ToString(this EntityType entityType)
	{
		return entityType switch
		{
			EntityType.Case => EntityTypeStrings.Case,
			EntityType.Incident => EntityTypeStrings.Incident,
			EntityType.Memo => EntityTypeStrings.Memo,
			EntityType.ServiceRequest => EntityTypeStrings.ServiceRequest,
			_ => throw new NotImplementedException(),
		};
	}

	public static bool Matches(string str, string expected)
	{
		return str.Equals(expected, StringComparison.InvariantCultureIgnoreCase);
	}

	public static EntityType ParseEntityType(this string str)
	{
		str = str.Trim();

		if (Matches(str, EntityTypeStrings.Case))
			return EntityType.Case;
		else if (Matches(str, EntityTypeStrings.Incident))
			return EntityType.Incident;
		else if (Matches(str, EntityTypeStrings.Memo))
			return EntityType.Memo;
		else if (Matches(str, EntityTypeStrings.ServiceRequest))
			return EntityType.ServiceRequest;
		else
			return EntityType.Unknown;
	}

	public static bool TryParseEntityType(this string str, out EntityType entityType)
	{
		entityType = ParseEntityType(str);
		return entityType > EntityType.Unknown && entityType <= EntityType.ServiceRequest;
	}
}
