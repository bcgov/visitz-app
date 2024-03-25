using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Extensions.EntityTypes;

public static class MemoTypeExtensions
{
	public static string ToString(this MemoType memoType)
	{
		return memoType switch
		{
			MemoType.AfterHoursAction => MemoTypeStrings.AfterHoursAction,
			MemoType.AfterHoursFrom => MemoTypeStrings.AfterHoursFrom,
			MemoType.AfterHoursInfo => MemoTypeStrings.AfterHoursInfo,
			MemoType.AgreementWithYoungAdults => MemoTypeStrings.AgreementWithYoungAdults,
			MemoType.Cysn => MemoTypeStrings.Cysn,
			MemoType.CentralizedServicesHub => MemoTypeStrings.CentralizedServicesHub,
			MemoType.ProtocolInvestigation => MemoTypeStrings.ProtocolInvestigation,
			MemoType.Screening => MemoTypeStrings.Screening,
			MemoType.SupportNeedsRequest => MemoTypeStrings.SupportNeedsRequest,
			_ => throw new NotImplementedException(),
		};
	}

	public static MemoType ParseMemoType(this string str)
	{
		str = str.Trim();

		if (EntityTypeExtensions.Matches(str, MemoTypeStrings.AfterHoursAction))
			return MemoType.AfterHoursAction;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.AfterHoursFrom))
			return MemoType.AfterHoursFrom;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.AfterHoursInfo))
			return MemoType.AfterHoursInfo;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.AgreementWithYoungAdults))
			return MemoType.AgreementWithYoungAdults;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.Cysn))
			return MemoType.Cysn;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.CentralizedServicesHub))
			return MemoType.CentralizedServicesHub;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.ProtocolInvestigation))
			return MemoType.ProtocolInvestigation;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.Screening))
			return MemoType.Screening;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.SupportNeedsRequest))
			return MemoType.SupportNeedsRequest;
		else
			return MemoType.Unknown;
	}

	public static bool TryParseMemoType(this string str, out MemoType memoType)
	{
		memoType = ParseMemoType(str);
		return memoType > MemoType.Unknown && memoType <= MemoType.SupportNeedsRequest;
	}
}
