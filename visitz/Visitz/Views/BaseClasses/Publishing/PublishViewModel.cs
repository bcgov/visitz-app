using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VisitzModel;

namespace Visitz.Views.BaseClasses.Publishing;

public abstract partial class PublishViewModel : VisitzViewModel
{
    public static readonly int DismissDuration = 3000;

    public enum State
    {
        Unknown = 0,
        Cancelled = 1,
        Waiting = 2,
        Publishing = 3,
        Published = 4,
        PublishError = 5,
        Refreshing = 6,
        Refreshed = 7,
        RefreshError = 8,
        Completed = 9,
    }

    public State CurrentState { get; private set; } = State.Waiting;

    [ObservableProperty]
    public partial string Title { get; set; }

    [ObservableProperty]
    public partial bool ShowPublishingIndicator { get; set; }

    [ObservableProperty]
    public partial string PublishingStatus { get; set; }

    [ObservableProperty]
    public partial bool ShowRefreshingIndicator { get; set; }

    [ObservableProperty]
    public partial string RefreshingStatus { get; set; }

    [ObservableProperty]
    public partial bool ShowRetryButton { get; set; }

    [ObservableProperty]
    public partial bool ShowDismissButton { get; set; }

    [ObservableProperty]
    public partial bool ShowPublishSuccessIcon { get; set; }

    [ObservableProperty]
    public partial bool ShowPublishErrorIcon { get; set; }

    [ObservableProperty]
    public partial string? PublishErrorDetail { get; set; }

    [ObservableProperty]
    public partial bool ShowRefreshSuccessIcon { get; set; }

    [ObservableProperty]
    public partial bool ShowRefreshErrorIcon { get; set; }

    [ObservableProperty]
    public partial string? RefreshErrorDetail { get; set; }

    [ObservableProperty]
    public partial bool AllowRetry { get; set; }

    public event EventHandler? OnCompleted;

    private void SetState(State state)
    {
        CurrentState = state;

        switch (state)
        {
            case State.Cancelled:
                SetFlags(showRetryButton: true, showDismissButton: true);
                break;
            case State.Waiting:
                SetFlags();
                break;
            case State.Publishing:
                SetFlags(showPublishingIndicator: true);
                break;
            case State.Published:
                SetFlags(showPublishSuccessIcon: true);
                break;
            case State.PublishError:
                SetFlags(showPublishErrorIcon: true, showRetryButton: true, showDismissButton: true);
                break;
            case State.Refreshing:
                SetFlags(
                    showPublishSuccessIcon: ShowPublishSuccessIcon,
                    showPublishErrorIcon: ShowPublishErrorIcon,
                    showRefreshingIndicator: true
                );
                break;
            case State.Refreshed:
                SetFlags(
                    showPublishSuccessIcon: ShowPublishSuccessIcon,
                    showPublishErrorIcon: ShowPublishErrorIcon,
                    showRefreshSuccessIcon: true
                );
                break;
            case State.RefreshError:
                SetFlags(
                    showPublishSuccessIcon: ShowPublishSuccessIcon,
                    showPublishErrorIcon: ShowPublishErrorIcon,
                    showRefreshErrorIcon: true,
                    showRetryButton: true,
                    allowRetry: false,
                    showDismissButton: true
                );
                break;
            case State.Completed:
                SetFlags(
                    showPublishSuccessIcon: ShowPublishSuccessIcon,
                    showPublishErrorIcon: ShowPublishErrorIcon,
                    showRefreshSuccessIcon: ShowRefreshSuccessIcon,
                    showRefreshErrorIcon: ShowRefreshErrorIcon,
                    showDismissButton: true
                );
                break;
            case State.Unknown:
                ConsoleTrace.TraceMethod(this, $"Reached {nameof(State.Unknown)} enum");
                break;
            default:
                throw new NotImplementedException(nameof(state));
        }
    }

    private void SetFlags(
        bool showPublishingIndicator = false,
        bool showRefreshingIndicator = false,
        bool showRetryButton = false,
        bool showDismissButton = false,
        bool showPublishSuccessIcon = false,
        bool showPublishErrorIcon = false,
        bool showRefreshSuccessIcon = false,
        bool showRefreshErrorIcon = false,
        bool allowRetry = true
    )
    {
        ShowPublishingIndicator = showPublishingIndicator;
        ShowRefreshingIndicator = showRefreshingIndicator;
        ShowPublishSuccessIcon = showPublishSuccessIcon;
        ShowPublishErrorIcon = showPublishErrorIcon;
        ShowRetryButton = showRetryButton;
        ShowDismissButton = showDismissButton;
        ShowRefreshSuccessIcon = showRefreshSuccessIcon;
        ShowRefreshErrorIcon = showRefreshErrorIcon;
        AllowRetry = allowRetry;
    }

    public void Cancel(string cancelText)
    {
        SetState(State.Cancelled);

        PublishingStatus = cancelText;
        PublishErrorDetail = null;
    }

    public void Wait(string waitingPrompt)
    {
        SetState(State.Waiting);

        PublishingStatus = waitingPrompt;
        PublishErrorDetail = null;
    }

    public void Publishing(string publishingPrompt)
    {
        SetState(State.Publishing);

        PublishingStatus = publishingPrompt;
        PublishErrorDetail = null;
    }

    public void Published(string publishedText)
    {
        SetState(State.Published);

        PublishingStatus = publishedText;
        PublishErrorDetail = null;
    }

    public void PublishError(string errorText, string errorDetails)
    {
        if (CurrentState == State.PublishError)
            return;

        SetState(State.PublishError);

        PublishingStatus = errorText;
        PublishErrorDetail = errorDetails;
    }

    public void Refreshing(string refreshingStatus)
    {
        SetState(State.Refreshing);

        RefreshingStatus = refreshingStatus;
        RefreshErrorDetail = null;
    }

    public void Refreshed(string refreshingStatus)
    {
        SetState(State.Refreshed);

        RefreshingStatus = refreshingStatus;
        RefreshErrorDetail = null;
    }

    public void RefreshError(string refreshingError, string errorDetails)
    {
        SetState(State.RefreshError);

        RefreshingStatus = refreshingError;
        RefreshErrorDetail = errorDetails;
    }

    public void Complete()
    {
        SetState(State.Completed);

        OnCompleted?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    public abstract void Publish();
}
