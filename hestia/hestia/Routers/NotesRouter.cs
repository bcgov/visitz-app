using System;
using hestia.Views;
using hestia.Models.BOs;

namespace hestia.Routers
{
    /// <summary>
    /// Use Router to move to different screens. Router handles navigation between screens.
    /// </summary>
	public class NotesRouter
    {
        public void RouteUsing(CaseloadItem bo)
        {
            routeToCaseIncidentDetailsPage(bo);
        }

        private void routeToCaseIncidentDetailsPage(CaseloadItem bo)
        {
            Routing.RegisterRoute(nameof(CaseIncidentDetailsPage), typeof(CaseIncidentDetailsPage));
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var navigationParameter = new Dictionary<string, object> { { "caseIncident", bo } };
                ((AppShell)Shell.Current).GoToAsyncRequest(nameof(CaseIncidentDetailsPage), parameters: navigationParameter);
            });
        }
    }
}

