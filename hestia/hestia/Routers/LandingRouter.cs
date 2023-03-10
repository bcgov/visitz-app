using System;
using hestia.Views;

namespace hestia.Routers
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
                    Routing.RegisterRoute(nameof(DeviceAuthenticationPage), typeof(DeviceAuthenticationPage));
                    routeStr = nameof(DeviceAuthenticationPage);
                    break;
                case Route.OpenIdAuthentication:
                    Routing.RegisterRoute(nameof(OpenIdAuthenticationPage), typeof(OpenIdAuthenticationPage));
                    routeStr = nameof(OpenIdAuthenticationPage);
                    break;
                case Route.CasesAndIncidents:
                    Routing.RegisterRoute(nameof(CasesAndIncidentsPage), typeof(CasesAndIncidentsPage));
                    routeStr = nameof(CasesAndIncidentsPage);
                    break;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ((AppShell)Shell.Current).GoToAsyncRequest(routeStr);
            });
        }

        void UnRegisterRoutes()
        {
            Routing.UnRegisterRoute(nameof(DeviceAuthenticationPage));
            Routing.UnRegisterRoute(nameof(OpenIdAuthenticationPage));
            Routing.UnRegisterRoute(nameof(CasesAndIncidentsPage));
        }

        public enum Route
        {
            DeviceAuthentication,
            OpenIdAuthentication,
            CasesAndIncidents
        }
    }
}

