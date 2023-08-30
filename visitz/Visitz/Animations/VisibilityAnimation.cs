using CommunityToolkit.Maui.Animations;

namespace Visitz.Animations
{
    public class VisibilityAnimation : BaseAnimation
    {
        public bool ShowView { get; }

        public uint Duration { get; } = 250;

        public VisibilityAnimation(bool isVisible)
        {
            ShowView = isVisible;
            Easing = Easing.Linear;
        }

        public VisibilityAnimation(bool showView, uint duration, Easing easing) : base(duration)
        {
            ShowView = showView;
            Duration = duration;
            Easing = easing;
        }

        public override async Task Animate(VisualElement view)
        {
            if (ShowView)
            {
                view.IsVisible = ShowView;
                
                view.Opacity = 0.0;
                await view.FadeTo(1.0, Duration, Easing);
            }
            else
            {
                view.Opacity = 1.0;
                await view.FadeTo(0.0, Duration, Easing);

                view.IsVisible = ShowView;
            }
        }
    }
}
