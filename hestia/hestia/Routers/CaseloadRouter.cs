using System;
using hestia.Views;
using hestia.Models.BOs;

namespace hestia.Routers
{
    /// <summary>
    /// Use Router to move to different screens. Router handles navigation between screens.
    /// </summary>
	public class CaseloadRouter
    {
        public void RouteUsing(ListCaseIncident2 bo)
        {
            routeToCaseNotes(bo);
        }

        private void routeToCaseNotes(ListCaseIncident2 bo)
        {
            Routing.RegisterRoute(nameof(CaseNotesPage), typeof(CaseNotesPage));
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var navigationParameter = new Dictionary<string, object> { { "caseIncident", bo } };
                ((AppShell)Shell.Current).GoToAsyncRequest(nameof(CaseNotesPage), parameters: navigationParameter);
            });
        }
    }
}

