/*
    THIS FILE IS NOT AUTO-GENERATED.

    But it should be.

    TODO: Implement a Source Generator to generate this file from Dimensions.xaml.
 */

using Visitz.Extensions;

namespace Visitz.Resources.Styles;

internal static class VisitzShadows
{
    public static Shadow TryGetShadow(string name, Shadow? fallback = null)
    {
        return Application.Current?.Resources.TryGetShadow(name, fallback)
            ?? throw new InvalidOperationException($"Shadow '{name}' not found in resources");
    }

    public static readonly Shadow RestingLevel1 = TryGetShadow(nameof(RestingLevel1));
    public static readonly Shadow RestingLevel2 = TryGetShadow(nameof(RestingLevel2));
    public static readonly Shadow RestingLevel3 = TryGetShadow(nameof(RestingLevel3));
    public static readonly Shadow Level4 = TryGetShadow(nameof(Level4));
    public static readonly Shadow Level5 = TryGetShadow(nameof(Level5));
}
