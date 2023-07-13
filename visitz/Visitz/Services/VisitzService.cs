using CommunityToolkit.Mvvm.Messaging;

namespace Visitz.Services
{
    public abstract class VisitzService
    {
        public enum State
        {
            /// <summary>
            /// Service finished (erroneously or otherwise) or was never run.
            /// </summary>
            Stopped = 1,

            /// <summary>
            /// Service currently doing its task.
            /// </summary>
            Running = 4
        }

        public enum Result
        {
            /// <summary>
            /// A service completed its task correctly.
            /// </summary>
            Successful = 0,

            /// <summary>
            /// A service was unable to complete its tasks.
            /// </summary>
            Error = 1,
        }

        public State Status { get; protected set; }

        public object Payload { get; set; }

        private void PublishCurrentState(State status)
        {
            Status = status;

            WeakReferenceMessenger.Default.Send(
                new ServiceStateMessage()
                {
                    ServiceId = GetId(),
                    ServiceType = GetType(),
                    Status = status,
                }, 
                GetId()
            );

#if DEBUG
            Console.WriteLine($"{GetId()} -> {status}");
#endif
        }

        public abstract string GetId();

        public virtual async Task OnRunAsync()
        {
            PublishCurrentState(State.Running);
            await RunAsync();
        }

        public virtual async Task OnFinishAsync()
        {
            try
            {
                await FinishAsync();
            }
            finally
            {
                PublishCurrentState(State.Stopped);
            }
        }

        protected virtual Task RunAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual Task FinishAsync()
        {
            return Task.CompletedTask;
        }
    }
}
