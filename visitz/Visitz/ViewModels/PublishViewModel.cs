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
                SetFlags();
                break;
            case State.PublishError:
                SetFlags(showRetrySection: true);
                break;
        }
    }

    private void SetFlags(
        bool showPublishingIndicator = false,
        bool showRetrySection = false)
    {
        ShowPublishingIndicator = showPublishingIndicator;
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

    [RelayCommand]
    public static async Task Dismiss()
    {
        await Navigator.Navigation.PopAsync();
    }

    [RelayCommand]
    public abstract void Publish();
}
