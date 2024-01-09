namespace Visitz.Views.SegmentedButtons;

public class ItemDeactivatedEventArgs(SegmentedOptions segmentedOption) : EventArgs
{
    public SegmentedOptions SegmentedOption { get; set; } = segmentedOption;
}
