using Foundation;
using UIKit;

namespace Visitz.Device
{
    public class KeyboardStateChangedEventArgs(bool isKeyboardOpen) : EventArgs
    {
        public bool IsKeyboardOpen { get; } = isKeyboardOpen;
    }
    public partial class SoftKeyboardOpenHandler : IDisposable
    {
        // Event to notify when keyboard state changes
        public event EventHandler<KeyboardStateChangedEventArgs> KeyboardStateChanged;

        private bool _isKeyboardOpen;
        private bool disposedValue;

        // Platform-specific subscription tokens will be initialized in platform-specific files.
        protected NSObject KeyboardShowToken { get; set; }
        protected NSObject KeyboardHideToken { get; set; }

        public SoftKeyboardOpenHandler()
        {
            // Subscribe to keyboard state changes
            SubscribeToKeyboardEvents();
        }

        // Method that will be called in platform-specific files to handle subscription
        partial void SubscribeToKeyboardEvents();

        // Method to handle the state of the keyboard (open/close)
        private void OnKeyboardStateChanged(bool isKeyboardOpen)
        {
            _isKeyboardOpen = isKeyboardOpen;
            KeyboardStateChanged?.Invoke(this, new KeyboardStateChangedEventArgs(isKeyboardOpen));
        }

        // Dispose logic for clean-up
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Unsubscribe from platform-specific events here.
                    UnsubscribeFromKeyboardEvents();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        partial void UnsubscribeFromKeyboardEvents();
    }
}


// using Foundation;
// using UIKit;

// namespace Visitz.Device
// {
//     // public class KeyboardStateChangedEventArgs(bool isKeyboardOpen) : EventArgs
//     // {
//     //     public bool IsKeyboardOpen { get; } = isKeyboardOpen;
//     // }
//     public class KeyboardStateChangedEventArgs(bool isKeyboardOpen, bool isPortrait) : EventArgs
//     {
//         public bool IsKeyboardOpen { get; } = isKeyboardOpen;
//         public bool IsPortrait { get; } = isPortrait;
//     }

//     public class SoftKeyboardOpenHandler : IDisposable
//     {
//         // Event to notify when keyboard state changes
//         public event EventHandler<KeyboardStateChangedEventArgs> KeyboardStateChanged;

//         private bool _isKeyboardOpen;
//         private bool disposedValue;

//         NSObject ObserveWillShowToken { get; set; }
//         NSObject ObserveWillHideToken { get; set; }
//         NSObject ObserveOrientationDidChangeToken { get; set; }


//         public SoftKeyboardOpenHandler()
//         {
//             // You can use the UIKeyboard notifications for iOS or platform-specific equivalents for other platforms
//             ObserveWillShowToken = UIKeyboard.Notifications.ObserveWillShow(OnKeyboardWillShow);
//             ObserveWillHideToken = UIKeyboard.Notifications.ObserveWillHide(OnKeyboardWillHide);

//             ObserveOrientationDidChangeToken = UIDevice.Notifications.ObserveOrientationDidChange((sender, args) => OnOrientationChanged());
//         }

//         private void OnKeyboardWillShow(object sender, UIKeyboardEventArgs e)
//         {
//             if (!_isKeyboardOpen && OrientationHelper.IsLandscape())
//             {
//                 _isKeyboardOpen = true;
//                 OnKeyboardStateChanged(true, OrientationHelper.IsPortrait());
//             }
//         }

//         private void OnKeyboardWillHide(object sender, UIKeyboardEventArgs e)
//         {
//             if (_isKeyboardOpen && OrientationHelper.IsLandscape())
//             {
//                 _isKeyboardOpen = false;
//                 OnKeyboardStateChanged(false, OrientationHelper.IsPortrait());
//             }
//         }

//         private void OnOrientationChanged()
//         {
//             // Check the orientation when it changes
//             bool isPortrait = OrientationHelper.IsPortrait();
//             if (isPortrait)
//             {
//                 // In portrait, even if the keyboard is open, ensure the radio buttons are shown
//                 if (_isKeyboardOpen)
//                 {
//                     OnKeyboardStateChanged(true, isPortrait);
//                 }
//                 else
//                 {
//                     OnKeyboardStateChanged(false, isPortrait);
//                 }
//             }
//             else
//             {
//                 // In landscape mode, respect the keyboard state
//                 OnKeyboardStateChanged(_isKeyboardOpen, isPortrait);
//             }
//         }

//        private void OnKeyboardStateChanged(bool isKeyboardOpen, bool isPortrait)
//         {
//             KeyboardStateChanged?.Invoke(this, new KeyboardStateChangedEventArgs(isKeyboardOpen, isPortrait));
//         }

//         // private bool IsLandscape()
//         // {
//         //     var orientation = UIDevice.CurrentDevice.Orientation;
//         //     return orientation == UIDeviceOrientation.LandscapeLeft || orientation == UIDeviceOrientation.LandscapeRight;
//         // }

//         // private bool IsPortrait()
//         // {
//         //     var orientation = UIDevice.CurrentDevice.Orientation;
//         //     return orientation == UIDeviceOrientation.Portrait || orientation == UIDeviceOrientation.PortraitUpsideDown;
//         // }

//         protected virtual void Dispose(bool disposing)
//         {
//             if (!disposedValue)
//             {
//                 if (disposing)
//                 {
//                     ObserveWillShowToken.Dispose();
//                     ObserveWillHideToken.Dispose();
//                     ObserveOrientationDidChangeToken.Dispose();
//                 }
//                 disposedValue = true;
//             }
//         }

//         public void Dispose()
//         {
//             // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
//             Dispose(disposing: true);
//             GC.SuppressFinalize(this);
//         }
//     }
// }
