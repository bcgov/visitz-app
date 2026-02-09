using VisitzModel.Extensions;

namespace VisitzModel.Models.Caseload;

public partial class BoLocalState
{
    public bool ShouldDownloadDuringRefreshBinding
    {
        get => IsValid && ShouldDownloadDuringRefresh;
        set
        {
            this.Commit(() => ShouldDownloadDuringRefresh = value);
            RaisePropertyChanged(nameof(ShouldDownloadDuringRefresh));
        }
    }

    public DateTimeOffset LastOpenedBinding
    {
        get => IsValid ? LastOpened : default;
        set
        {
            this.Commit(() => LastOpened = value);
            RaisePropertyChanged(nameof(LastOpened));
        }
    }
}
