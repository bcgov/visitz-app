using CommunityToolkit.Maui.Animations;

namespace Visitz.Animations;

public class VisibilityAnimation : BaseAnimation
{
    private const uint DefaultDuration = 250;

    public bool ShowView { get; }

    public uint Duration { get; } = DefaultDuration;

    public VisibilityAnimation(bool showView, uint duration = DefaultDuration, Easing easing = null)
        : base(DefaultDuration)
    {
        ShowView = showView;
        Duration = duration;
        Easing = easing ?? (showView ? Easing.CubicIn : Easing.CubicOut);
    }

    public override async Task Animate(VisualElement view, CancellationToken token = default)
    {
        if (ShowView)
        {
            view.IsVisible = ShowView;

            view.Opacity = 0.0;
            await view.FadeToAsync(1.0, Duration, Easing);
        }
        else
        {
            view.Opacity = 1.0;
            await view.FadeToAsync(0.0, Duration, Easing);

            view.IsVisible = ShowView;
        }
    }
}
