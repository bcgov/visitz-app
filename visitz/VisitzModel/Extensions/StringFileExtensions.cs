namespace VisitzModel.Extensions;

public static class StringFileExtensions
{
    public static string GetFileExtension(this string filename)
    {
        return new FileInfo(filename).Extension;
    }
}
