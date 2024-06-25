using Visitz.FontIcons;

namespace Visitz.Views.Caseload;

public partial class DataRefreshButton : ViewModelContentView
{
	new DataRefreshViewModel ViewModel => base.ViewModel as DataRefreshViewModel;

	public StackOrientation Orientation
	{
		get => ViewModel.Orientation;
		set => ViewModel.Orientation = value;
	}

	public DataRefreshButton() : base(ServiceProvider.GetService<DataRefreshViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;

		SetIconByNetworkAccess(Connectivity.Current.NetworkAccess);
		Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
	}

	protected override void Destroying()
	{
		Connectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;

		base.Destroying();
	}

	private void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
	{
		SetIconByNetworkAccess(e.NetworkAccess);
	}

	private void SetIconByNetworkAccess(NetworkAccess networkAccess)
	{
		RefreshButton.Text = networkAccess == NetworkAccess.Internet
			? MaterialIcons.Download_for_offline
			: MaterialIcons.File_download_off;
	}
}
