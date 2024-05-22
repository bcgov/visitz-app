using VisitzModel.Extensions;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models;

public partial class NoteDraft
{
	public DateTimeOffset LastUpdatedBinding
	{
		get => IsValid ? LastUpdated : default;
		set
		{
			this.Commit(() => LastUpdated = value);
			RaisePropertyChanged(nameof(LastUpdated));
		}
	}

	public string DraftBinding
	{
		get => IsValid ? Draft : default;
		set
		{
			bool canSet = !value?.ContainsUnicodeSurrogatesAndOtherSymbols() ?? true;

			if (canSet)
			{
				this.Commit(() => Draft = value);
				RaisePropertyChanged(nameof(DraftBinding));
				LastUpdatedBinding = DateTimeOffset.Now;
			}
		}
	}

	public string DraftLocationBinding
	{
		get => IsValid ? DraftLocation : default;
		set
		{
			this.Commit(() => DraftLocation = value);
			RaisePropertyChanged(nameof(DraftLocation));
		}
	}
}
