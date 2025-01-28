using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oidc.Network;
using Realms;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Events;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.InPersonVisits;

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
    public string visitDescription;

    [ObservableProperty]
    public DateTimeOffset dateOfVisit;

    [ObservableProperty]
    public bool isUpdatingEnabled = true;

    [ObservableProperty]
    public bool hideElements = true;

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

    public event EventHandler<DraftErrorEventArgs> DraftError;

    protected override async Task InitAsync()
    {
        await base.InitAsync();
        PersonVisitItem ??= new();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _disposed = true;
        }
        base.Dispose(disposing);
    }
    partial void OnPersonVisitItemChanged(PersonVisit oldValue, PersonVisit newValue)
    {
        if (oldValue != null)
            oldValue.PropertyChanged -= PersonVisitItem_PropertyChanged;
        
        if (newValue != null)
            newValue.PropertyChanged += PersonVisitItem_PropertyChanged;
    }

    private void PersonVisitItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        IsVisitTypeSelected = !string.IsNullOrWhiteSpace(PersonVisitItem.VisitDetailsValue);
        UpdateAllowPublish();
    }

    [RelayCommand]
    public async Task PublishInPersonVisit()
    {
    }

    private void UpdateAllowPublish()
    {
        AllowPublish = NetworkHelper.InternetAvailable
            && PersonVisitItem.VisitDetailsGroup != null
            && PersonVisitItem.VisitDetailsValue != null
            && PersonVisitItem.VisitDescription?.Length > 0;
    }

    private static bool ContainEmojis(TextChangedEventArgs e)
    {
        return e.NewTextValue?.ContainsUnicodeSurrogatesAndOtherSymbols() ?? false;
    }

    private static bool ExceedsCharacterLimit(TextChangedEventArgs e)
    {
        return e.NewTextValue?.Length > CharacterLimit;
    }

    public void EditorTextChanged(TextChangedEventArgs e)
    {
        if (string.Equals(e.OldTextValue, e.NewTextValue))
            // Early return required to prevent infinite loops due to "cancelling" events
            // by reassigning its previous value
            return;

        int length = e.NewTextValue?.Length ?? 0;
        if (ContainEmojis(e))
        {
            CancelTextChangedEvent(e);
            DraftError?.Invoke(this, new DraftErrorEventArgs(LocalizedStrings.InvalidEntry));
            return;
        }
        else if (ExceedsCharacterLimit(e))
        {
            CancelTextChangedEvent(e);
            DraftError?.Invoke(this, new DraftErrorEventArgs(LocalizedStrings.CharacterLimitReached));
            return;
        }
        RemainingCharacters = CharacterLimit - length;

    }

    private void CancelTextChangedEvent(TextChangedEventArgs e)
    {
        PersonVisitItem.VisitDescription = e.OldTextValue;
    }
}
