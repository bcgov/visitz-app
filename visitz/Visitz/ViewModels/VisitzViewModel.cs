using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Pages;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The base class for all the view models. Common functionality can be defined here.
    /// </summary>
	public partial class VisitzViewModel : ObservableObject
    {
        public VisitzPage VisitzPage { get; set; }

        public IDictionary<string, object> Parameters => VisitzPage.Parameters;

        protected async Task NavigateTo<T>(IDictionary<string, object> parameters = null) where T : VisitzPage
        {
            await VisitzPage.NavigateTo<T>(VisitzPage, parameters);
        }

        public virtual void PageCreated() { }

        public virtual void PageStarted() { }

        public virtual void PageStopped() { }

        public virtual void PageDestroyed() { }

        public void SubscribeToWindow(Window window)
        {
            if (window == null)
                return;

            window.Resumed += Window_Resumed;
            window.Stopped += Window_Stopped;
        }

        public void UnsubscribeFromWindow(Window window)
        {
            if (window == null)
                return;

            window.Resumed -= Window_Resumed;
            window.Stopped -= Window_Stopped;
        }

        public void Window_Resumed(object sender, EventArgs e)
        {
            PageStarted();
        }

        public void Window_Stopped(object sender, EventArgs e)
        {
            PageStopped();
        }
    }
}

