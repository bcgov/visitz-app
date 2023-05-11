using System;
namespace Visitz
{
    /// <summary>
    /// A work around implementation that prevents the navigation collision and the subsequent crash.
    /// The implementation queues the navigation requests and works in a FIFO manner.
    /// </summary>
    public partial class AppShell : Shell
    {
        bool isNavigating;
        readonly List<NavigationRequest> navigationRequests = new();

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);
            isNavigating = true;
        }

        protected override void OnNavigated(ShellNavigatedEventArgs args)
        {
            base.OnNavigated(args);
            isNavigating = false;
            NavigateIfNeeded();
        }

        /// <summary>
        /// A method that acts as a replacement for `GoToAsync`
        /// </summary>
        public void GoToAsyncRequest(ShellNavigationState state, bool animate = true,
            IDictionary<string, object> parameters = null)
        {
            _ = MainThread.InvokeOnMainThreadAsync(() =>
            {
                NavigationRequest navReq = new(state, animate, parameters);
                navigationRequests.Add(navReq);
                NavigateIfNeeded();
            });
        }

        async void NavigateIfNeeded()
        {
            if (isNavigating || navigationRequests.Count < 1)
            {
                return;
            }
            NavigationRequest request = navigationRequests.First();
            navigationRequests.RemoveAt(0);
            await GoToAsync(request.state, request.animate, request.parameters ?? new Dictionary<string, object> { });
        }

        struct NavigationRequest
        {
            public ShellNavigationState state;
            public bool animate;
            public IDictionary<string, object> parameters;

            public NavigationRequest(ShellNavigationState state, bool animate,
                IDictionary<string, object> parameters)
            {
                this.state = state;
                this.animate = animate;
                this.parameters = parameters;
            }
        }
    }
}

