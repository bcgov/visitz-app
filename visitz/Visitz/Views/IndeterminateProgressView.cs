using CommunityToolkit.Maui;

namespace Visitz.Views;

internal partial class IndeterminateProgressView : ContentView
{
    static readonly uint s_animationDuration = 1500;

    CancellationTokenSource? _animationCts;

    readonly BoxView _progressIndicator;

    [BindableProperty(PropertyChangedMethodName = nameof(IsRunningChanged))]
    public partial bool IsRunning { get; set; }

    [BindableProperty]
    public partial Color Color { get; set; }

    public IndeterminateProgressView()
    {
        Content = _progressIndicator = new();
        HeightRequest = 4.0d;
        HorizontalOptions = LayoutOptions.Start;

        _progressIndicator.SetBinding(
            BoxView.ColorProperty,
            static (IndeterminateProgressView progress) => progress.Color,
            source: this
        );
    }

    static void IsRunningChanged(object boundObj, object _, object newVal)
    {
        if (boundObj is IndeterminateProgressView view && newVal is bool isRunning)
        {
            if (isRunning)
                view.StartAnimation();
            else
                view.StopAnimation();
        }
    }

    public void StartAnimation()
    {
        StopAnimation();

        _animationCts = new CancellationTokenSource();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            while (_animationCts != null && !_animationCts.IsCancellationRequested)
            {
                double containerWidth = ((VisualElement)Parent).Width;

                if (containerWidth <= 0)
                {
                    await Task.Delay(50);
                    continue;
                }

                await Task.WhenAll(SizeAndMoveAsync(containerWidth), GrowAndShrink());

                await Task.Delay(200);
            }
        });
    }

    public void StopAnimation()
    {
        _animationCts?.Cancel();
        _animationCts?.Dispose();
        _animationCts = null;

        _progressIndicator.CancelAnimations();
        _progressIndicator.TranslationX = 0;
    }

    async Task SizeAndMoveAsync(double containerWidth)
    {
        double indicatorWidth = Math.Min(80, containerWidth * 0.3);

        _progressIndicator.WidthRequest = indicatorWidth;
        _progressIndicator.TranslationX = -indicatorWidth;

        await _progressIndicator.TranslateToAsync(containerWidth, 0, s_animationDuration, Easing.CubicInOut);
    }

    async Task GrowAndShrink()
    {
        await _progressIndicator.ScaleXToAsync(3.0d, s_animationDuration / 2, Easing.Linear);
        await _progressIndicator.ScaleXToAsync(0.5d, s_animationDuration / 4, Easing.Linear);
    }
}
