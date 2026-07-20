using System.Windows.Input;
using CommunityToolkit.Maui;
using Visitz.Resources.Localization;
using VisitzModel.Models.Drafts;

namespace Visitz.Views.FormControls;

#nullable enable

public partial class FormAppBar : ContentView, IDisposable
{
    bool _disposedValue;

    NetworkAccess _networkAccess;

    [BindableProperty]
    public partial bool AllowDiscard { get; set; } = true;

    [BindableProperty(PropertyChangedMethodName = nameof(AllowPublish_Changed))]
    public partial bool AllowPublish { get; set; } = true;

    /// <summary>
    /// A combination of allowing publish and other internal checks
    /// (like network availability). Code outside this class should use
    /// <see cref="AllowPublish"/> instead.
    /// </summary>
    [BindableProperty]
    public partial bool EnablePublish { get; set; } = true;

    [BindableProperty]
    public partial ICommand DiscardCommand { get; set; }

    [BindableProperty]
    public partial ICommand PublishCommand { get; set; }

    [BindableProperty]
    public partial bool IsReadOnly { get; set; }

    [BindableProperty]
    public partial DraftSaveState DraftSaveState { get; set; } = DraftSaveState.None;

    public FormAppBar()
    {
        InitializeComponent();
        _networkAccess = Connectivity.Current.NetworkAccess;

        Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        SizeChanged += FormAppBar_SizeChanged;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Connectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
                SizeChanged -= FormAppBar_SizeChanged;
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    void Current_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        _networkAccess = e.NetworkAccess;
        UpdateEnablePublish();
    }

    static void AllowPublish_Changed(BindableObject obj, object _, object __)
    {
        if (obj is FormAppBar appBar)
            appBar.UpdateEnablePublish();
    }

    void UpdateEnablePublish()
    {
        EnablePublish = _networkAccess == NetworkAccess.Internet && AllowPublish;
    }

    private void FormAppBar_SizeChanged(object? sender, EventArgs e)
    {
        InternetInfoView.ShowText = Width >= 650;
        DraftSavedView.ShowText = Width >= 600;

        if (Width >= 475)
        {
            DiscardButton.Text = LocalizedStrings.Discard;
            PublishButton.Text = LocalizedStrings.PublishToIcm;
        }
        else
        {
            DiscardButton.Text = string.Empty;
            PublishButton.Text = string.Empty;
        }
    }
}
