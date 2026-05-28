using CommunityToolkit.Mvvm.Messaging;
using Visitz.Extensions;
using VisitzModel.Models.Caseload;

namespace Visitz.Services.Base;

#nullable enable

internal partial class ServiceActivityListener : IRecipient<ServiceStateMessage>, IDisposable
{
    private bool _disposedValue;

    public HashSet<string> RunningServiceIds { get; } = [];

    public bool HasActivity { get; private set; }

    public bool IsRegistered { get; private set; }

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public void RegisterForMessages(IBusinessObject businessObject)
    {
        UnregisterFromMessages();
        businessObject.RegisterActivityListeners(this);
        IsRegistered = true;
    }

    public void UnregisterFromMessages()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
        IsRegistered = false;
    }

    public void Receive(ServiceStateMessage message)
    {
        if (message.IsRunning)
            RunningServiceIds.Add(message.ServiceId);
        else
            RunningServiceIds.Remove(message.ServiceId);

        if (!HasActivity && RunningServiceIds.Count > 0)
        {
            HasActivity = true;
            Started?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            HasActivity = false;
            Stopped?.Invoke(this, EventArgs.Empty);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                UnregisterFromMessages();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
