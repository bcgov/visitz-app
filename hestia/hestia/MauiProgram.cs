using Microsoft.Extensions.Logging;

namespace hestia;

/// <summary>
/// The program that gets invoked before anything else by the .NET runtime.
/// </summary>
public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		return HestiaApp.Create();
	}
}

