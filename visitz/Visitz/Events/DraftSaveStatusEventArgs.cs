namespace Visitz.Events;

public class DraftSaveStatusEventArgs(bool draftSaved, bool savingDraft) : EventArgs
{
    public bool DraftSaved { get; set; } = draftSaved;

    public bool SavingDraft { get; set; } = savingDraft;
}
