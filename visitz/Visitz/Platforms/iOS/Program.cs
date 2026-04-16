using System.Runtime.InteropServices;
using System.Runtime.Loader;
using UIKit;

namespace Visitz;

public class Program
{
    // This is the main entry point of the application.
    static void Main(string[] args)
    {
        LoadRealmAssembly();

        // if you want to use a different Application Delegate class from "AppDelegate"
        // you can specify it here.
        try
        {
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);

            throw;
        }
    }

    // FIXME This function is a workaround for a .NET 10 breaking change that
    // affects the ways assemblies are loaded.
    // https://github.com/realm/realm-dotnet/issues/3711#issuecomment-3691192652
    // This workaround can be removed once the fix is applied in Realm.
    static void LoadRealmAssembly()
    {
        var alc = AssemblyLoadContext.GetLoadContext(typeof(Realms.Realm).Assembly) ?? AssemblyLoadContext.Default;

        alc.ResolvingUnmanagedDll += (assembly, libraryName) =>
        {
            if (libraryName == "realm-wrappers")
            {
                return NativeLibrary.Load(
                    "@rpath/realm-wrappers.framework/realm-wrappers",
                    assembly,
                    DllImportSearchPath.ApplicationDirectory
                );
            }

            return IntPtr.Zero;
        };
    }
}
