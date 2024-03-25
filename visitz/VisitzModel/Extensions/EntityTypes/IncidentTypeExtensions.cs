using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Extensions.EntityTypes;

public static class IncidentTypeExtensions
{
	public static string ToString(this IncidentType incidentType)
	{
		return incidentType switch
		{
			IncidentType.ChildProtection => IncidentTypeStrings.ChildProtection,
			IncidentType.ProtocolInvestigation => IncidentTypeStrings.ProtocolInvestigation,
			IncidentType.ReferralAndInquiry => IncidentTypeStrings.ReferralAndInquiry,
			_ => throw new NotImplementedException(),
		};
	}

	public static IncidentType ParseIncidentType(this string str)
	{
		str = str.Trim();

		if (EntityTypeExtensions.Matches(str, IncidentTypeStrings.ChildProtection))
			return IncidentType.ChildProtection;
		else if (EntityTypeExtensions.Matches(str, IncidentTypeStrings.ProtocolInvestigation))
			return IncidentType.ProtocolInvestigation;
		else if (EntityTypeExtensions.Matches(str, IncidentTypeStrings.ReferralAndInquiry))
			return IncidentType.ReferralAndInquiry;
		else
			return IncidentType.Unknown;
	}

	public static bool TryParseIncidentType(this string str, out IncidentType incidentType)
	{
		incidentType = ParseIncidentType(str);
		return incidentType > IncidentType.Unknown && incidentType <= IncidentType.ReferralAndInquiry;
	}
}
