using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
    public bool showSuccessIcon;

    [ObservableProperty]
    public bool showErrorIcon;

    [ObservableProperty]
    public string publishErrorDetail;

    public event EventHandler OnCompleted;

    private void SetState(State state)
    {
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
                SetFlags(showSuccessIcon: true);
                break;
            case State.PublishError:
                SetFlags(showErrorIcon: true, showRetrySection: true);
                break;
            case State.Refreshing:
                SetFlags();
                break;
            case State.Refreshed:
                SetFlags();
                break;
            case State.RefreshError:
                SetFlags();
                break;
            case State.Completed:
                SetFlags(showSuccessIcon: ShowSuccessIcon);
                break;
        }
    }

    private void SetFlags(
        bool showPublishingIndicator = false,
        bool showRefreshingIndicator = false,
        bool showRetrySection = false,
        bool showSuccessIcon = false,
        bool showErrorIcon = false)
    {
        ShowPublishingIndicator = showPublishingIndicator;
        ShowSuccessIcon = showSuccessIcon;
        ShowErrorIcon = showErrorIcon;
        ShowRefreshingIndicator = showRefreshingIndicator;
        ShowRetrySection = showRetrySection;
    }

    public void Cancel(string cancelText)
    {
        SetState(State.Cancelled);

        PublishingStatus = cancelText;
    }

    public void Wait(string waitingPrompt)
    {
        SetState(State.Waiting);

        PublishingStatus = waitingPrompt;
    }

    public void Publishing(string publishingPrompt)
    {
        SetState(State.Publishing);

        PublishingStatus = publishingPrompt;
    }

    public void Published(string publishedText)
    {
        SetState(State.Published);

        PublishingStatus = publishedText;
    }

    public void PublishError(string errorText, string errorDetails)
    {
        SetState(State.PublishError);

        PublishingStatus = errorText;
        PublishErrorDetail = errorDetails;
    }

    public async Task Complete()
    {
        SetState(State.Completed);

        OnCompleted?.Invoke(this, EventArgs.Empty);

        await Task.Delay(DismissDuration);
        await Navigator.Navigation.PopAsync();
    }

    [RelayCommand]
    public static async Task Dismiss()
    {
        await Navigator.Navigation.PopAsync();
    }

    [RelayCommand]
    public abstract void Publish();
}
