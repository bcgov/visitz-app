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

	public static bool TryParseMemoType(this string str, out MemoType memoType)
	{
		str = str.Trim();

		if (EntityTypeExtensions.Matches(str, MemoTypeStrings.AfterHoursAction))
			memoType = MemoType.AfterHoursAction;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.AfterHoursFrom))
			memoType = MemoType.AfterHoursFrom;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.AfterHoursInfo))
			memoType = MemoType.AfterHoursInfo;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.AgreementWithYoungAdults))
			memoType = MemoType.AgreementWithYoungAdults;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.Cysn))
			memoType = MemoType.Cysn;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.CentralizedServicesHub))
			memoType = MemoType.CentralizedServicesHub;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.ProtocolInvestigation))
			memoType = MemoType.ProtocolInvestigation;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.Screening))
			memoType = MemoType.Screening;
		else if (EntityTypeExtensions.Matches(str, MemoTypeStrings.SupportNeedsRequest))
			memoType = MemoType.SupportNeedsRequest;
		else
			memoType = MemoType.Unknown;

		return memoType > MemoType.Unknown && memoType <= MemoType.SupportNeedsRequest;
	}
}
