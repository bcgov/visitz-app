using Microsoft.Extensions.Logging;

namespace Visitz;

/// <summary>
/// The program that gets invoked before anything else by the .NET runtime.
/// </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        return VisitzApp.Create();
    }
}

