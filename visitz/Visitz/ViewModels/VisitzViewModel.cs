using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Pages;
using VisitzModel;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The base class for all the view models. Common functionality can be defined here.
    /// </summary>
	public partial class VisitzViewModel : ObservableObject
    {
        public VisitzPage VisitzPage { get; set; }

        public virtual void PageCreated() 
        {
            ConsoleTrace.TraceMethod(this);
        }

        public virtual void PageStarted()
        {
            ConsoleTrace.TraceMethod(this);
        }

        public virtual void PageStopped()
        {
            ConsoleTrace.TraceMethod(this);
        }

        public virtual void PageDestroyed()
        {
            ConsoleTrace.TraceMethod(this);
        }

        public void AttachToLifecycle(Window window)
        {
            ConsoleTrace.TraceMethod(this, $"window = '{window}'");

            (Application.Current as VisitzApp).AppResumed += Window_Resumed;

            if (window == null)
                return;

            window.Activated += Window_Activated;
            window.Resumed += Window_Resumed;
            window.Stopped += Window_Stopped;
            window.Deactivated += Window_Deactivated;
        }

        public void DetachFromLifecycle(Window window)
        {
            ConsoleTrace.TraceMethod(this, $"window = '{window}'");

            (Application.Current as VisitzApp).AppResumed -= Window_Resumed;

            if (window == null)
                return;

            window.Activated -= Window_Activated;
            window.Resumed -= Window_Resumed;
            window.Stopped -= Window_Stopped;
            window.Deactivated -= Window_Deactivated;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            ConsoleTrace.TraceMethod(this);
            PageStarted();
        }

        public void Window_Resumed(object sender, EventArgs e)
        {
            ConsoleTrace.TraceMethod(this);
            PageStarted();
        }

        public void Window_Stopped(object sender, EventArgs e)
        {
            ConsoleTrace.TraceMethod(this);
            PageStopped();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            ConsoleTrace.TraceMethod(this);
            PageStopped();
        }
    }
}

