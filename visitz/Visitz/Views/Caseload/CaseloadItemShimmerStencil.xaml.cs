namespace Visitz.Views.Caseload;

public partial class CaseloadItemShimmerStencil : ContentView
{
    const double _minWidth = 50.0d;
    const double _maxWidth = 200.0d;

    public CaseloadItemShimmerStencil()
    {
        InitializeComponent();

#pragma warning disable SCS0005 // Weak random number generator.
        // Not using random for cryptography
        MainLabelBox.WidthRequest = Math.Clamp(new Random().NextDouble() * _maxWidth + _minWidth, _minWidth, _maxWidth);
#pragma warning restore SCS0005 // Weak random number generator.
    }
}
