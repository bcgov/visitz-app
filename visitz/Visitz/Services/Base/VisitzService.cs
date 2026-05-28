using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Visitz.Services.Messages;

namespace Visitz.Services.Base;

public abstract class VisitzService
{
    public enum State
    {
        Unknown = 0,

        /// <summary>
        /// Service finished (erroneously or otherwise) or was never run.
        /// </summary>
        Stopped = 1,

        /// <summary>
        /// Service currently doing its task.
        /// </summary>
        Running = 4,
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

    public object ReturnPayload { get; protected set; }

    public Exception UncaughtException { get; protected set; }

    protected virtual ILogger Logger { get; set; } = ServiceProvider.GetService<ILogger<VisitzService>>();

    static readonly string LoggerTemplate = "{id} -> {stateMessage}";

    protected CancellationTokenSource CancelTokenSource { get; } = new();

    private void PublishCurrentState(State status)
    {
        Status = status;

        var stateMsg = new ServiceStateMessage()
        {
            ServiceId = GetId(),
            ServiceType = GetType(),
            Status = status,
            UncaughtException = UncaughtException,
            ReturnPayload = ReturnPayload,
        };

        if (status == State.Stopped)
        {
            stateMsg.Result = ResultCode;
            stateMsg.Message = ResultMessage;
        }

        WeakReferenceMessenger.Default.Send(stateMsg, GetId());
#if DEBUG
        Logger.LogDebug(LoggerTemplate, GetId(), stateMsg);
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
            if (ex is OperationCanceledException)
            {
                ResultCode = Result.Cancelled;
                Logger.LogDebug(LoggerTemplate, GetId(), "Service cancelled");
            }
            else
            {
                UncaughtException = ex;
                ResultCode = Result.Error;
                Logger.LogError(LoggerTemplate, GetId(), ex.ToString());
            }

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
