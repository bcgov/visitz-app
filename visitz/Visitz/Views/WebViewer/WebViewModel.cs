using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.WebViewer;

public partial class WebViewModel : VisitzViewModel
{
    [ObservableProperty]
    public Uri authUri;
}
