/*
    THIS FILE IS NOT AUTO-GENERATED.

    But it should be.

    TODO: Implement a Source Generator to generate this file from Dimensions.xaml.
 */

using Visitz.Extensions;

namespace Visitz.Resources.Styles;

internal static class VisitzDimensions
{
    public static double TryGetDimension(string name, double? fallback = null)
    {
        return Application.Current.Resources.TryGetDimension(name, fallback)
            ??
            throw new InvalidOperationException($"Dimension '{name}' not found in resources");
    }

    public static readonly double TopAppBarHeight = TryGetDimension(nameof(TopAppBarHeight));
}
