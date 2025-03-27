using VisitzModel.Models.Drafts;

namespace VisitzModel.Events;

public class DraftSaveStatusEventArgs(DraftSaveState state) : EventArgs
{
    public DraftSaveState State { get; } = state;
}
