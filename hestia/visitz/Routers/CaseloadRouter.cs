using System;
using visitz.Views;
using visitz.Models.BOs;

namespace visitz.Routers
{
    /// <summary>
    /// Use Router to move to different screens. Router handles navigation between screens.
    /// </summary>
	public class CaseloadRouter
    {
        public void RouteUsing(CaseloadItem bo)
        {
            routeToNotes(bo);
        }

        private void routeToNotes(CaseloadItem bo)
        {
            Routing.RegisterRoute(nameof(NotesPage), typeof(NotesPage));
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var navigationParameter = new Dictionary<string, object> { { "caseIncident", bo } };
                ((AppShell)Shell.Current).GoToAsyncRequest(nameof(NotesPage), parameters: navigationParameter);
            });
        }
    }
}

