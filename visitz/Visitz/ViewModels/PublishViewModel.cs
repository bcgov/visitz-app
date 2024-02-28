using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Pages;

namespace Visitz.ViewModels;

public abstract partial class PublishViewModel : VisitzViewModel
{
    public static readonly int DismissDuration = 3000;

    public enum State
    {
        Cancelled,
        Waiting,
        Publishing,
        Published,
        PublishError,
        Refreshing,
        Refreshed,
        RefreshError,
        Completed,
    }

    public State CurrentState { get; private set; } = State.Waiting;

    [ObservableProperty]
    public string title;

    [ObservableProperty]
    public bool showPublishingIndicator;

    [ObservableProperty]
    public string publishingStatus;

    [ObservableProperty]
    public bool showRefreshingIndicator;

    [ObservableProperty]
    public string refreshingStatus;

    [ObservableProperty]
    public bool showRetrySection;

    [ObservableProperty]
    public bool showPublishSuccessIcon;

    [ObservableProperty]
    public bool showPublishErrorIcon;

    [ObservableProperty]
    public string publishErrorDetail;

    [ObservableProperty]
    public bool showRefreshSuccessIcon;

    [ObservableProperty]
    public bool showRefreshErrorIcon;

    [ObservableProperty]
    public string refreshErrorDetail;

    [ObservableProperty]
    public bool allowRetry;

    public event EventHandler OnCompleted;

    private void SetState(State state)
    {
        CurrentState = state;

        switch (state)
        {
            case State.Cancelled:
                SetFlags(showRetrySection: true);
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
                SetFlags(showPublishErrorIcon: true, showRetrySection: true);
                break;
            case State.Refreshing:
                SetFlags(
                    showPublishSuccessIcon: ShowPublishSuccessIcon,
                    showPublishErrorIcon: ShowPublishErrorIcon,
                    showRefreshingIndicator: true);
                break;
            case State.Refreshed:
                SetFlags(
                    showPublishSuccessIcon: ShowPublishSuccessIcon,
                    showPublishErrorIcon: ShowPublishErrorIcon,
                    showRefreshSuccessIcon: true);
                break;
            case State.RefreshError:
                SetFlags(
                    showPublishSuccessIcon: ShowPublishSuccessIcon,
                    showPublishErrorIcon: ShowPublishErrorIcon,
                    showRefreshErrorIcon: true,
                    showRetrySection: true,
                    allowRetry: false);
                break;
            case State.Completed:
                SetFlags(
                    showPublishSuccessIcon: ShowPublishSuccessIcon,
                    showPublishErrorIcon: ShowPublishErrorIcon,
                    showRefreshSuccessIcon: ShowRefreshSuccessIcon,
                    showRefreshErrorIcon: ShowRefreshErrorIcon);
                break;
        }
    }

    private void SetFlags(
        bool showPublishingIndicator = false,
        bool showRefreshingIndicator = false,
        bool showRetrySection = false,
        bool showPublishSuccessIcon = false,
        bool showPublishErrorIcon = false,
        bool showRefreshSuccessIcon = false,
        bool showRefreshErrorIcon = false,
        bool allowRetry = true)
    {
        ShowPublishingIndicator = showPublishingIndicator;
        ShowRefreshingIndicator = showRefreshingIndicator;
        ShowPublishSuccessIcon = showPublishSuccessIcon;
        ShowPublishErrorIcon = showPublishErrorIcon;
        ShowRetrySection = showRetrySection;
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

    public async Task Complete()
    {
        SetState(State.Completed);

        OnCompleted?.Invoke(this, EventArgs.Empty);

        await Task.Delay(DismissDuration);
        await Navigator.Navigation.PopAsync();

		await FeedbackSurveyPage.TryOpen();
    }

    [RelayCommand]
    public static async Task Dismiss()
    {
        await Navigator.Navigation.PopAsync();
    }

    [RelayCommand]
    public abstract void Publish();
}
