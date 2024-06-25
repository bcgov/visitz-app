using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.ViewModels;

namespace Visitz.Views.Caseload;

internal partial class DataRefreshViewModel : VisitzViewModel
{
	[ObservableProperty]
	public bool superMessageVisible = false;

	[ObservableProperty]
	public string superMessage;

	[ObservableProperty]
	public StackOrientation orientation = StackOrientation.Vertical;

	public override void Create()
	{
		base.Create();

		SetConnectivityMessage(Connectivity.Current.NetworkAccess);
		Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
	}

	public override void Destroy()
	{
		base.Destroy();

		Connectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
	}

	private void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
	{
		SetConnectivityMessage(e.NetworkAccess);
	}

	private void SetConnectivityMessage(NetworkAccess access)
	{
		SuperMessage = access == NetworkAccess.Internet ? "" : LocalizedStrings.Offline;
	}

	[RelayCommand]
	public static void RefreshData()
	{
		WeakReferenceMessenger.Default.Send(GetAllDataForOfflineService.MakeStartMessage());
	}

	partial void OnSuperMessageChanged(string value)
	{
		SuperMessageVisible = !string.IsNullOrWhiteSpace(value);
	}
}
