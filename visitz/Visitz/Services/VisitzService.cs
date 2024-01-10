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

            /// <summary>
            /// A service was requested to run but it was already running.
            /// </summary>
            NoOperation = 2,

            /// <summary>
            /// A service was started but was intentionally stopped before it could complete.
            /// </summary>
            Cancelled = 3,
        }

        public State Status { get; protected set; }

        // Services must explicitly set ResultCode when they complete their tasks.
        public Result ResultCode { get; protected set; } = Result.Error;

        public string ResultMessage { get; protected set; }

        public object Payload { get; set; }

        private void PublishCurrentState(State status)
        {
            Status = status;

            var stateMsg = new ServiceStateMessage()
            {
                ServiceId = GetId(),
                ServiceType = GetType(),
                Status = status,
            };

            if (status == State.Stopped)
            {
                stateMsg.Result = ResultCode;
                stateMsg.Message = ResultMessage;
            }

            WeakReferenceMessenger.Default.Send(stateMsg, GetId());
#if DEBUG
            string result = Status == State.Stopped 
                ? $" | Result: {ResultCode} | Message: {ResultMessage}"
                : "";

            Console.WriteLine($"{GetId()} -> {status}" + result);
#endif
        }

        public abstract string GetId();

        public async Task RunAsync()
        {
            PublishCurrentState(State.Running);

            try
            {
                await RunServiceAsync();
            }
            catch (Exception ex)
            {
                ResultCode = Result.Error;
                ResultMessage = ex.Message;

                throw;
            }
        }

        protected abstract Task RunServiceAsync();

        public async Task FinishAsync()
        {
            try
            {
                await FinishServiceAsync();
            }
            finally
            {
                PublishCurrentState(State.Stopped);
            }
        }

        protected virtual Task FinishServiceAsync()
        {
            return Task.CompletedTask;
        }
    }
}
