using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Visitz.Services.Messages;
using VisitzModel.Extensions;

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

    public string? ResultMessage { get; protected set; }

    public object Payload { get; set; } = new();

    public object? ReturnPayload { get; protected set; }

    public Exception? UncaughtException { get; protected set; }

    protected ILogger Logger { get; }

#if DEBUG
    static readonly string LoggerTemplate = "Id '{id}' -> {stateMessage}";
#endif

    protected CancellationTokenSource CancelTokenSource { get; } = new();

    public VisitzService()
    {
        Logger = ServiceProvider.GetService<ILoggerFactory>().CreateLogger(GetType());
    }

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
            stateMsg.Message = ResultMessage ?? string.Empty;
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
#if DEBUG
                Logger.LogDebug(LoggerTemplate, GetId(), "Service cancelled");
#endif
            }
            else
            {
                UncaughtException = ex;
                ResultCode = Result.Error;
                Logger.LogException(ex, $"Id '{GetId()}'");
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
