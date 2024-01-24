using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Visitz.ViewModels;

public abstract partial class PublishViewModel : VisitzViewModel
{
    public enum State
    {
        Waiting,
        Publishing,
        Published,
        PublishError,
        Cancelled,
    }

    public State CurrentState { get; private set; } = State.Waiting;

    [ObservableProperty]
    public string title;

    [ObservableProperty]
    public bool showPublishingIndicator;

    [ObservableProperty]
    public string publishingStatus;

    [ObservableProperty]
    public bool showRetrySection;

    [ObservableProperty]
    public bool showSuccessIcon;

    [ObservableProperty]
    public bool showErrorIcon;

    private void SetState(State state)
    {
        switch (state)
        {
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
            case State.Cancelled:
                SetFlags(showRetrySection: true);
                break;
        }
    }

    private void SetFlags(
        bool showPublishingIndicator = false,
        bool showRetrySection = false,
        bool showSuccessIcon = false,
        bool showErrorIcon = false)
    {
        ShowPublishingIndicator = showPublishingIndicator;
        ShowSuccessIcon = showSuccessIcon;
        ShowErrorIcon = showErrorIcon;
        ShowRetrySection = showRetrySection;
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

    public void PublishError(string errorText)
    {
        SetState(State.PublishError);

        PublishingStatus = errorText;
    }

    public void Cancel(string cancelText)
    {
        SetState(State.Cancelled);

        PublishingStatus = cancelText;
    }

    [RelayCommand]
    public static async Task Dismiss()
    {
        await Navigator.Navigation.PopAsync();
    }

    [RelayCommand]
    public abstract void Publish();
}
