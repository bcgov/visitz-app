
namespace Visitz.Views.SegmentedButtons;

public readonly struct SegmentedOptions(string id, string text, ImageSource imageSource)
{
    public static readonly SegmentedOptions Empty = new();

    public readonly string Id { get; } = id;

    public readonly string Text { get; } = text;

    public readonly ImageSource ImageSource { get; } = imageSource;

    public override readonly bool Equals(object obj)
    {
        return obj is SegmentedOptions opts &&
               Id == opts.Id &&
               Text == opts.Text &&
               EqualityComparer<ImageSource>.Default.Equals(ImageSource, opts.ImageSource);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Id, Text, ImageSource);
    }

    public static bool operator ==(SegmentedOptions a, SegmentedOptions b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(SegmentedOptions a, SegmentedOptions b)
    {
        return !a.Equals(b);
    }
}
