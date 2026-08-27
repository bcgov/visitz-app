namespace Visitz.Storage;

/// <summary>Convenience class for interacting with the curated list of BC Gov pictures.</summary>
/// <remarks>
/// Currently, the pictures are collected from:
/// <list type="bullet">
///     <item>
///         <term>BC Gov Ministry of Transportation Flickr Album</term>
///         <description>https://www.flickr.com/photos/bcgovphotos/albums/72157683219327092</description>
///     </item>
///     <item>
///         <term>BC Gov PictureBC Flickr Album</term>
///         <description>https://www.flickr.com/photos/bcgovphotos/albums/72157689014058504</description>
///     </item>
/// </list>
/// </remarks>
public class BcGovAlbum
{
    private static readonly string PicturesFilepath = "PicturesList.txt";

    public static async Task<string[]> GetPictureUris()
    {
        await using var stream = await FileSystem.OpenAppPackageFileAsync(PicturesFilepath);
        using var reader = new StreamReader(stream);

        var contents = await reader.ReadToEndAsync();
        return contents.Trim().Split(Environment.NewLine);
    }

    public static async Task<string> GetRandomPictureUri()
    {
        var r = new Random();
        var pictures = await GetPictureUris();

        // Suppressing this instance of SCS0005 since we're not using randomization for security.
#pragma warning disable SCS0005 // Weak random number generator.
        return pictures[r.Next(pictures.Length)];
#pragma warning restore SCS0005 // Weak random number generator.
    }

    public static string GetFeaturedPictureUri()
    {
        // Currently just hardcoding for a specific picture.
        // TODO: When time allows, implement some kind of schedule to change the featured picture,
        // with a proper fallback mechanism if the new picture can't be downloaded.

        // Temporary override. The server we were downloading from started to deliver pictures in a corrupted way
        // so it's time to embed them in the app instead.
        return "bella_coola_river.jpg";
    }
}
