using CommunityToolkit.Maui;

namespace Visitz.Views;

internal partial class IndeterminateProgressView : ContentView
{
    static readonly uint s_animationDuration = 1500;

    CancellationTokenSource? _animationCts;

    readonly BoxView _progressIndicator;

    double ParentWidth => ((VisualElement)Parent).Width;

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
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            StopAnimation();

            _animationCts = new CancellationTokenSource();

            while (_animationCts != null && !_animationCts.IsCancellationRequested)
            {
                if (Parent == null || ParentWidth <= 0)
                {
                    await Task.Delay(50);
                    continue;
                }

                await Task.WhenAll(SizeAndMoveAsync(ParentWidth), GrowAndShrink());

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
        double indicatorWidth = Math.Min(80, containerWidth * 0.1);

        _progressIndicator.WidthRequest = indicatorWidth;
        _progressIndicator.TranslationX = -indicatorWidth;

        await _progressIndicator.TranslateToAsync(containerWidth, 0, s_animationDuration, Easing.CubicInOut);
    }

    async Task GrowAndShrink()
    {
        await _progressIndicator.ScaleXToAsync(3.0d, s_animationDuration / 2, Easing.Linear);
        await _progressIndicator.ScaleXToAsync(1.0d, s_animationDuration / 4, Easing.Linear);
    }
}
