using Visitz.Views;

namespace Visitz.Routers
{
    /// <summary>
    /// Use Router to move to different screens. Router handles navigation between screen.
    /// </summary>
    public class LandingRouter
    {
        public void RouteTo(Route route)
        {
            UnRegisterRoutes();
            string routeStr = string.Empty;
            switch (route)
            {
                case Route.DeviceAuthentication:
                    Routing.RegisterRoute(nameof(AppLockPage), typeof(AppLockPage));
                    routeStr = nameof(AppLockPage);
                    break;
                case Route.OpenIdAuthentication:
                    Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
                    routeStr = nameof(LoginPage);
                    break;
                case Route.CasesAndIncidents:
                    Routing.RegisterRoute(nameof(CaseloadPage), typeof(CaseloadPage));
                    routeStr = nameof(CaseloadPage);
                    break;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ((VisitzShell)Shell.Current).GoToAsyncRequest(routeStr);
            });
        }

        void UnRegisterRoutes()
        {
            Routing.UnRegisterRoute(nameof(AppLockPage));
            Routing.UnRegisterRoute(nameof(LoginPage));
            Routing.UnRegisterRoute(nameof(CaseloadPage));
        }

        public enum Route
        {
            DeviceAuthentication,
            OpenIdAuthentication,
            CasesAndIncidents
        }
    }
}

