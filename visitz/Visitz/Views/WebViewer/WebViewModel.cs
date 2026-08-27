using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.WebViewer;

public partial class WebViewModel : VisitzViewModel
{
    [ObservableProperty]
    public partial Uri AuthUri { get; set; }
}
