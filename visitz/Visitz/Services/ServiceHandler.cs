using CommunityToolkit.Mvvm.Messaging;
using Visitz.Services.Messages;

namespace Visitz.Services
{
    public class ServiceHandler : IRecipient<StartServiceMessage>
    {
        readonly IDictionary<string, VisitzService> Services = new Dictionary<string, VisitzService>();

        public ServiceHandler()
        {
            WeakReferenceMessenger.Default.Register(this);
        }

        public void Receive(StartServiceMessage message)
        {
            _ = RunServiceAsync(message);
        }

        public async Task TryRunServiceAsync(StartServiceMessage startMessage)
        {
            await RunServiceAsync(startMessage);
        }

        public async Task RunServiceAsync(StartServiceMessage startMessage)
        {
            // Only allow 1 service per ID at a time. For now, ServiceHandler shouldn't need to worry about
            // queueing services or distributing workloads.
            if (!Services.ContainsKey(startMessage.ServiceId))
            {
                try
                {
                    var service = (VisitzService)ServiceProvider.Current.GetRequiredService(startMessage.ServiceType);
                    service.Payload = startMessage.Payload;
                    Services[startMessage.ServiceId] = service;
                
                    await service.OnStartAsync();
                    await service.OnRunAsync();
                    await service.OnFinishAsync();
                }
                finally
                {
                    Services.Remove(startMessage.ServiceId);
                }
            }
        }

        public VisitzService.State GetServiceState(string serviceId)
        {
            return Services.TryGetValue(serviceId, out VisitzService service) 
                ? service.Status 
                : VisitzService.State.Stopped;
        }
    }
}
