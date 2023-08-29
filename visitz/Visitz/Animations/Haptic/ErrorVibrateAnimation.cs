using CommunityToolkit.Maui.Animations;

namespace Visitz.Animations.Haptic;

public class ErrorVibrateAnimation : BaseAnimation
{
    private static readonly double Distance = 10;
    private static readonly uint DurationMs = 10;

    public override async Task Animate(VisualElement view)
    {
        double originalX = view.X;

        // Refer to https://developer.apple.com/design/human-interface-guidelines/playing-haptics#Notification
        // The "Error" pattern. Two short bursts, a more powerful burst, then a longer & less powerful burst.
        await view.TranslateTo(originalX + Distance, view.Y, DurationMs, Easing.CubicIn);
        await view.TranslateTo(originalX - Distance, view.Y, DurationMs, Easing.Linear);
        await view.TranslateTo(originalX + Distance + Distance, view.Y, DurationMs, Easing.Linear);
        await view.TranslateTo(originalX, view.Y, DurationMs + DurationMs / 5, Easing.CubicOut);
    }
}
