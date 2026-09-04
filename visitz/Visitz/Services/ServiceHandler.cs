using System.Collections.Concurrent;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzModel;
using VisitzModel.Extensions;

namespace Visitz.Services;

public class ServiceHandler : IRecipient<StartServiceMessage>
{
    readonly ConcurrentDictionary<string, VisitzService> Services = [];

    ILogger<ServiceHandler> Logger { get; } = ServiceProvider.GetService<ILogger<ServiceHandler>>();

    public ServiceHandler()
    {
        WeakReferenceMessenger.Default.Register(this);
    }

    public void Receive(StartServiceMessage message)
    {
        Logger.TraceMethod(this);
        _ = TryRunServiceAsync(message);
    }

    private VisitzService MakeAndTrackService(StartServiceMessage startMessage)
    {
        var service = (VisitzService)ServiceProvider.Current.GetRequiredService(startMessage.ServiceType);

        Services[startMessage.ServiceId] = service;
        service.Payload = startMessage.Payload;

        Logger.TraceMethod(this);

        return service;
    }

    /// <summary>
    /// Tries to run a service if it isn't already running (according to the internal tracking dictionary).
    /// </summary>
    /// <param name="startMessage"></param>
    /// <returns></returns>
    public async Task<VisitzService.Result> TryRunServiceAsync(StartServiceMessage startMessage)
    {
        // Only allow 1 service per ID at a time. For now, ServiceHandler shouldn't need to worry about
        // queueing services or distributing workloads.
        if (!Services.ContainsKey(startMessage.ServiceId))
        {
            var service = MakeAndTrackService(startMessage);

            try
            {
                await RunServiceAsync(service);
                return service.ResultCode;
            }
            finally
            {
                Services.TryRemove(startMessage.ServiceId, out var _);
            }
        }
        else
            return VisitzService.Result.NoOperation;
    }

    private async Task RunServiceAsync(VisitzService service)
    {
        try
        {
            await service.RunAsync();
        }
#if DEBUG
        catch (Exception ex)
        {
            ConsoleTrace.TraceMethod(this, ex);

            throw;
        }
#endif
        finally
        {
            await service.FinishAsync();
        }
    }

    public VisitzService.State GetServiceState(string serviceId)
    {
        return Services.TryGetValue(serviceId, out VisitzService? service)
            ? service.Status
            : VisitzService.State.Stopped;
    }

    public bool IsAnyServiceRunning(string serviceIdContains)
    {
        string? key = Services.Keys.FirstOrDefault(key => key.Contains(serviceIdContains));
        return key is not null
            && Services.TryGetValue(key, out var service)
            && service.Status == VisitzService.State.Running;
    }
}
