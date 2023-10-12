using CoreGraphics;
using Foundation;
using System.Runtime.InteropServices;
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

    NFloat KeyboardHeight = 0;

    void OnKeyboardShowing(object sender, UIKeyboardEventArgs args)
    {
        CGRect kbFrame = UIKeyboard.FrameEndFromNotification(args.Notification);
        KeyboardHeight = kbFrame.Height;

        SetBottomPadding(Page.Padding.Bottom + KeyboardHeight);
    }

    void OnKeyboardHiding(object sender, UIKeyboardEventArgs args)
    {
        SetBottomPadding(Page.Padding.Bottom - KeyboardHeight);
    }

    void SetBottomPadding(double bottomSize)
    {
        Page.Padding = new Thickness(Page.Padding.Left, Page.Padding.Top, Page.Padding.Right, bottomSize);
    }

}
