using System.Collections.Concurrent;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Extensions;
using Visitz.Services.Messages;
using VisitzModel.Models.Caseload;

namespace Visitz.Services.Base;

#nullable enable

public partial class ServiceActivityListener : IRecipient<ServiceStateMessage>, IDisposable
{
    private bool _disposedValue;

    public ConcurrentDictionary<string, byte> RunningServiceIds { get; } = [];

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
            RunningServiceIds.TryAdd(message.ServiceId, 0);
        else
            RunningServiceIds.TryRemove(message.ServiceId, out _);

        if (!HasActivity && RunningServiceIds.Keys.Count > 0)
        {
            HasActivity = true;
            Started?.Invoke(this, EventArgs.Empty);
        }
        else if (HasActivity && RunningServiceIds.Keys.Count <= 0)
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
