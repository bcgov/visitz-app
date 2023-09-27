using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Authentication.Keycloak;
using Visitz.Storage;

namespace Visitz.ViewModels
{
    public partial class DebugOptionsViewModel : VisitzViewModel
    {
        [ObservableProperty]
        public string idirOverride;

        [ObservableProperty]
        public bool showNoteItemViewDebugInfo;

        [ObservableProperty]
        public bool dryFireSubmitNotes;

        public override void PageStarted()
        {
            base.PageStarted();

            IdirOverride = DebugOptions.IdirOverride;
            ShowNoteItemViewDebugInfo = DebugOptions.ShowNoteItemViewDebugInfo;
            DryFireSubmitNotes = DebugOptions.DryFireSubmitNotes;
        }

        partial void OnIdirOverrideChanged(string value)
        {
            DebugOptions.IdirOverride = value;
        }

        partial void OnShowNoteItemViewDebugInfoChanged(bool value)
        {
            DebugOptions.ShowNoteItemViewDebugInfo = value;
        }

        partial void OnDryFireSubmitNotesChanged(bool value)
        {
            DebugOptions.DryFireSubmitNotes = value;
        }

        [RelayCommand]
        public void DeleteAccessToken()
        {
            if (DebugOptions.Enabled)
                TokenHolder.DeleteAccessToken();
        }

        [RelayCommand]
        public void DeleteRefreshToken()
        {
            if (DebugOptions.Enabled)
                TokenHolder.DeleteRefreshToken();
        }

        [RelayCommand]
        public async void ClearRealmData()
        {
            await DebugOptions.ClearRealmData();
        }

        [RelayCommand]
        public void DeleteEncryptionKey()
        {
            DebugOptions.DeleteEncryptionKey();
        }

        [RelayCommand]
        public async void Logout()
        {
            if (DebugOptions.Enabled)
                await VisitzSession.LogoutAsync();
        }
    }
}
