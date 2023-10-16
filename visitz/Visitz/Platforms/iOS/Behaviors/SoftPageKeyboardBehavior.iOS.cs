using CoreGraphics;
using Foundation;
using UIKit;

namespace Visitz.Behaviors;

partial class SoftPageKeyboardBehavior
{
    NSObject ObserveShowToken { get; set; }

    NSObject ObserveHideToken { get; set; }

    partial void Attach()
    {
        ObserveShowToken = UIKeyboard.Notifications.ObserveWillShow(OnKeyboardShowing);
        ObserveHideToken = UIKeyboard.Notifications.ObserveWillHide(OnKeyboardHiding);
    }

    partial void Detach()
    {
        ObserveShowToken.Dispose();
        ObserveShowToken = null;

        ObserveHideToken.Dispose();
        ObserveHideToken = null;
    }

    void OnKeyboardShowing(object sender, UIKeyboardEventArgs args)
    {
        CGRect beginFrame = UIKeyboard.FrameBeginFromNotification(args.Notification);
        CGRect endFrame = UIKeyboard.FrameEndFromNotification(args.Notification);

        if (beginFrame.Y != endFrame.Y)
            SetBottomPadding(Page.Padding.Bottom + endFrame.Height);
    }

    void OnKeyboardHiding(object sender, UIKeyboardEventArgs args)
    {
        SetBottomPadding(InitialBottomPadding);

    }

    void SetBottomPadding(double bottomSize)
    {
        Page.Padding = new Thickness(Page.Padding.Left, Page.Padding.Top, Page.Padding.Right, bottomSize);
    }

}
