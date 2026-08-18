using Visitz.Resources.Localization;

namespace Visitz.Device;

internal class MapsHelper
{
    public static async Task OpenAddress(string address)
    {
#if WINDOWS
        string selection = await Navigator.CurrentOpenPage.DisplayActionSheetAsync(
            LocalizedStrings.OpenInMap,
            LocalizedStrings.Cancel,
            null,
            LocalizedStrings.GoogleMaps,
            LocalizedStrings.AppleMaps,
            LocalizedStrings.OpenStreetMap,
            LocalizedStrings.BingMaps
        );

        string mapEngine;

        if (selection == LocalizedStrings.AppleMaps)
            mapEngine = "https://maps.apple.com/place?address=";
        else if (selection == LocalizedStrings.BingMaps)
            mapEngine = "https://www.bing.com/maps/search?q=";
        else if (selection == LocalizedStrings.GoogleMaps)
            mapEngine = "https://www.google.com/maps/search/?api=1&query=";
        else if (selection == LocalizedStrings.OpenStreetMap)
            mapEngine = "https://www.openstreetmap.org/search?query=";
        else
            return;

        await Browser.Default.OpenAsync(mapEngine + address);
#elif IOS
        await Launcher.Default.OpenAsync($"maps://?q={address}");
#endif
    }
}
