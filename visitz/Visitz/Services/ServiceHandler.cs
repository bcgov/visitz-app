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
            // Only allow 1 service per ID at a time. For now, ServiceHandler shouldn't need to worry about
            // queueing services or distributing workloads.
            if (!Services.ContainsKey(startMessage.ServiceId))
            {
                try
                {
                    await RunServiceAsync(startMessage);
                }
                finally
                {
                    Services.Remove(startMessage.ServiceId);
                }
            }
        }

        public async Task RunServiceAsync(StartServiceMessage startMessage)
        {
            var service = (VisitzService)ServiceProvider.Current.GetRequiredService(startMessage.ServiceType);
            service.Payload = startMessage.Payload;
            Services[startMessage.ServiceId] = service;

            try
            {
                await service.OnStartAsync();
                await service.OnRunAsync();
            }
            finally
            {
                await service.OnFinishAsync();
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
