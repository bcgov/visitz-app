using Foundation;
using UIKit;

namespace Visitz.Device
{
    public class KeyboardStateChangedEventArgs(bool isKeyboardOpen) : EventArgs
    {
        public bool IsKeyboardOpen { get; } = isKeyboardOpen;
    }

    public class SoftKeyboardOpenHandler : IDisposable
    {
        // Event to notify when keyboard state changes
        public event EventHandler<KeyboardStateChangedEventArgs> KeyboardStateChanged;

        private bool _isKeyboardOpen;
        private bool disposedValue;

        NSObject ObserveWillShowToken { get; set; }
    	NSObject ObserveWillHideToken { get; set; }

        
        // You can handle the keyboard events based on platform-specific code
        public SoftKeyboardOpenHandler()
        {
            // You can use the UIKeyboard notifications for iOS or platform-specific equivalents for other platforms
            ObserveWillShowToken = UIKeyboard.Notifications.ObserveWillShow(OnKeyboardWillShow);
            ObserveWillHideToken = UIKeyboard.Notifications.ObserveWillHide(OnKeyboardWillHide);
        }

        private void OnKeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            if (!_isKeyboardOpen)
            {
                _isKeyboardOpen = true;
                OnKeyboardStateChanged(true);
            }
        }

        private void OnKeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            if (_isKeyboardOpen)
            {
                _isKeyboardOpen = false;
                OnKeyboardStateChanged(false);
            }
        }

        // Method to raise the KeyboardStateChanged event
        protected virtual void OnKeyboardStateChanged(bool isKeyboardOpen)
        {
            KeyboardStateChanged?.Invoke(this, new KeyboardStateChangedEventArgs(isKeyboardOpen));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ObserveWillShowToken.Dispose();
                    ObserveWillHideToken.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
