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

	public static bool TryParseIncidentType(this string str, out IncidentType incidentType)
	{
		str = str.Trim();

		if (EntityTypeExtensions.Matches(str, IncidentTypeStrings.ChildProtection))
			incidentType = IncidentType.ChildProtection;
		else if (EntityTypeExtensions.Matches(str, IncidentTypeStrings.ProtocolInvestigation))
			incidentType = IncidentType.ProtocolInvestigation;
		else if (EntityTypeExtensions.Matches(str, IncidentTypeStrings.ReferralAndInquiry))
			incidentType = IncidentType.ReferralAndInquiry;
		else
			incidentType = IncidentType.Unknown;

		return incidentType > IncidentType.Unknown && incidentType <= IncidentType.ReferralAndInquiry;
	}
}
