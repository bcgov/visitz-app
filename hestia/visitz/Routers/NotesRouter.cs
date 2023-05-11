using System;
using visitz.Views;
using visitz.Models.BOs;

namespace visitz.Routers
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

