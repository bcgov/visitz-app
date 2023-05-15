using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The base class for all the view models. Common functionality can be defined here.
    /// </summary>
	public partial class VisitzViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        [ObservableProperty]
        string title;

        public VisitzPage VisitzPage { get; set; }

        public bool IsNotBusy => !IsBusy;

        public virtual void PageCreated() { }

        public virtual void PageStarted() { }

        public virtual void PageStopped() { }

        public void SubscribeToWindow(Window window)
        {
            if (window == null)
                return;

            window.Resumed += Window_Resumed;
            window.Stopped += Window_Stopped;
            // TODO: window.Destroying & this.IDisposable?
        }

        public void UnsubscribeFromWindow(Window window)
        {
            if (window == null)
                return;

            window.Resumed -= Window_Resumed;
            window.Stopped -= Window_Stopped;
            // TODO: window.Destroying & this.IDisposable?
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

