using Visitz.Services.Base;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;

namespace Visitz.Views.Drafts;

public partial class DraftsListItem : BaseContentView
{
    readonly ServiceActivityListener _activityListener = new();

    public DraftsListItem()
    {
        InitializeComponent();
        _activityListener.Started += ActivityListener_Started;
        _activityListener.Stopped += ActivityListener_Stopped;
    }

    protected override async void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is DraftListItemViewModel item)
        {
            using var realm = await VisitzRealms.GetIcmDataRealmAsync();
            if (item.Draft.GetRelatedBusinessObjectFrom(realm) is IBusinessObject record)
                _activityListener.RegisterForMessages(record);
        }
        else
        {
            _activityListener.UnregisterFromMessages();
        }

        UpdateDownloadActivityIndicator();
    }

    void ActivityListener_Started(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(UpdateDownloadActivityIndicator);
    }

    void ActivityListener_Stopped(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(UpdateDownloadActivityIndicator);
    }

    private void UpdateDownloadActivityIndicator()
    {
        try
        {
            DownloadActivity.IsRunning = _activityListener.HasActivity;
            DownloadActivity.IsVisible = _activityListener.HasActivity;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, $"Couldn't update network activity UI for {nameof(DraftsListItem)}");
        }
    }
}
