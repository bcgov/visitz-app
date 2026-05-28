namespace Visitz.Device;

public class KeyboardStateChangedEventArgs(bool isKeyboardOpen) : EventArgs
{
    public bool IsKeyboardOpen { get; } = isKeyboardOpen;
}

public partial class SoftKeyboardOpenHandler : IDisposable
{
    public event EventHandler<KeyboardStateChangedEventArgs> KeyboardStateChanged;

    private bool _isKeyboardOpen;
    private bool disposedValue;

    public SoftKeyboardOpenHandler()
    {
        SubscribeToKeyboardEvents();
    }

    partial void SubscribeToKeyboardEvents();

    partial void UnsubscribeFromKeyboardEvents();

    private void OnKeyboardStateChanged(bool isKeyboardOpen)
    {
        _isKeyboardOpen = isKeyboardOpen;
        KeyboardStateChanged?.Invoke(this, new KeyboardStateChangedEventArgs(isKeyboardOpen));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
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
}
