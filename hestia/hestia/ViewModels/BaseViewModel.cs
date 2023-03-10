using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace hestia.ViewModels
{
    /// <summary>
    /// The base class for all the view models. Common functionality can be defined here.
    /// </summary>
	public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        [ObservableProperty]
        string title;

        public bool IsNotBusy => !IsBusy;
    }
}

