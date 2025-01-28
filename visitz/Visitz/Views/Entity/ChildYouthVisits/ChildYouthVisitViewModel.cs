using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oidc.Network;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Models;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitViewModel : VisitzViewModel, ICaseloadItemHolder
{
    public static readonly string VisitTypeGroup = "VisitTypeGroup";
    public static readonly string VisitDetailGroup = "VisitDetailGroup";
    private static readonly int CharacterLimit = 4000;
    public static readonly string RemainingCharactersString = "{0}/" + CharacterLimit;
    private bool _disposed;
    Realm Realm { get; set; }
    public CaseloadItem CaseloadItem { get; set; }

    [ObservableProperty]
    public bool isVisitTypeSelected;

    [ObservableProperty]
    public DateTime maxDate = DateTimeExtensions.LocalNow;

    [ObservableProperty]
    public bool privateChecked;

    [ObservableProperty]
    public bool exemptionToPrivateVisitChecked;

    [ObservableProperty]
    public bool notPrivateChecked;

    [ObservableProperty]
    public bool childDeclinedToMeetChecked;

    [ObservableProperty]
    public bool otherChecked;

    [ObservableProperty]
    public bool planningMeetingChecked;

    [ObservableProperty]
    public bool relationalVisitChecked;

    [ObservableProperty]
    public bool visitAge0To5Checked;

    [ObservableProperty]
    public bool visitInHomeChecked;

    [ObservableProperty]
    public bool visitInTheHomeChecked;

    [ObservableProperty]
    public bool visitMedicalOrSupportNeedsChecked;

    [ObservableProperty]
    public bool visitNotInHomeChecked;

    [ObservableProperty]
    public bool visitWithCaregiverChecked;

    [ObservableProperty]
    public string selectedVisitType;

    [ObservableProperty]
    public string selectedVisitDetail;

    [ObservableProperty]
    public string visitDescription;

    [ObservableProperty]
    public DateTimeOffset dateOfVisit;

    [ObservableProperty]
    public bool isUpdatingEnabled = true;

    [ObservableProperty]
    public bool allowDiscard = true;

    [ObservableProperty]
    public bool allowPublish;

    [ObservableProperty]
    private NetworkAccess networkAccess = Connectivity.Current.NetworkAccess;

    [ObservableProperty]
    public int remainingCharacters = CharacterLimit;

    [ObservableProperty]
    public PersonVisit personVisitItem;

    protected override async Task InitAsync()
    {
        await base.InitAsync();
        await InitVisitDraft();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private async Task InitVisitDraft()
    {
        Realm = await VisitzRealms.GetPersonVisitDraftsRealmAsync();
    }

    partial void OnSelectedVisitTypeChanged(string value)
    {
        IsVisitTypeSelected = !string.IsNullOrWhiteSpace(value);
    }

    private void PersonVisitItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        UpdateAllowPublish();
        var x =0;
    }

    partial void OnPersonVisitItemChanged(PersonVisit value)
    {
        if (value != null)
        {
            SelectedVisitType = value.VisitDetailsValue;
            SelectedVisitDetail = value.VisitDetailsGroup;
        }
    }

    [RelayCommand]
    public async Task PublishInPersonVisit()
    {
    }

    private void UpdateAllowPublish()
    {
        AllowPublish = NetworkHelper.InternetAvailable 
            && SelectedVisitDetail != null 
            && SelectedVisitType != null 
            && VisitDescription.Length > 0;
    }

    public async Task EditorTextChanged(TextChangedEventArgs e)
    {
        if (string.Equals(e.OldTextValue, e.NewTextValue))
            // Early return required to prevent infinite loops due to "cancelling" events
            // by reassigning its previous value
            return;

        int length = e.NewTextValue?.Length ?? 0;
        RemainingCharacters = CharacterLimit - length;
    }
}