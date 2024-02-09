using System.Runtime.CompilerServices;

namespace Oidc.Util;

public static class ConsoleTrace
{
    private static int traceCount = 0;

    public static void TraceMethod(
        object caller,
        string message = null,
        string prepend = null,
        [CallerMemberName] string memberName = "")
    {
        TraceMethod(caller.GetType(), message, prepend, memberName);
    }

    public static void TraceMethod(
        Type callerType,
        string message = null,
        string prepend = null,
        [CallerMemberName] string memberName = "")
    {
#if DEBUG
        if (prepend != null)
            prepend += ": ";

        if (message != null)
            message = " -> " + message;

        WriteLine($"({traceCount++}) {prepend}{callerType.Name}.{memberName}(){message}");
#endif
    }

    private static void WriteLine(string line)
    {
#if WINDOWS
        System.Diagnostics.Debug.WriteLine(line);
#else
        Console.WriteLine(line);
#endif
    }
}
