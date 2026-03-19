namespace Visitz.Views.SegmentedButtons;

public class ItemActivatedEventArgs(SegmentedOptions segmentedOption) : EventArgs
{
    public SegmentedOptions SegmentedOption { get; set; } = segmentedOption;
}
