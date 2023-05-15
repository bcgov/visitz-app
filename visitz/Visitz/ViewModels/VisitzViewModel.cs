using CommunityToolkit.Mvvm.ComponentModel;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The base class for all the view models. Common functionality can be defined here.
    /// </summary>
	public partial class VisitzViewModel : ObservableObject, IVisitzLifecycle
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        [ObservableProperty]
        string title;

        public bool IsNotBusy => !IsBusy;

        public void Window_Activated(object sender, EventArgs e) { }

        public void Window_Created(object sender, EventArgs e) { }

        public void Window_Deactivated(object sender, EventArgs e) { }

        public void Window_Resumed(object sender, EventArgs e)
        {
            PageStarted();
        }

        public void Window_Stopped(object sender, EventArgs e) { }

        public virtual void PageCreated() { }

        public virtual void PageStarted() { }
    }
}

