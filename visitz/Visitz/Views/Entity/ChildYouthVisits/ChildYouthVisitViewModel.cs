using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Models;
using VisitzModel.Resources.Localization;
using VisitzModel.Messaging;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitViewModel : VisitzViewModel, ICaseloadItemHolder
{
    public static readonly string VisitTypeGroup = "VisitTypeGroup";
    public static readonly string VisitDetailGroup = "VisitDetailGroup";
    private static readonly int CharacterLimit = 4000;
    public static readonly string RemainingCharactersString = "{0}/" + CharacterLimit;
    private bool _disposed;
    
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
    public bool showPrivateTypeVisitDetails = false;

    [ObservableProperty]
    public bool showExemptionTypeVisitDetails = false;

    [ObservableProperty]
    public bool showNotPrivateTypeVisitDetails = false;

    [ObservableProperty]
    public string visitDescription;

    [ObservableProperty]
    public DateTimeOffset dateOfVisit;

    [ObservableProperty]
    public bool isUpdatingEnabled = true;

    [ObservableProperty]
    public string selectedPrivateVisitDetailGroup;

    [ObservableProperty]
    public string selectedNotPrivateVisitDetailGroup;

    [ObservableProperty]
    public string selectedExcemptPrivateVisitDetailGroup;

    [ObservableProperty]
    public bool allowDiscard = true;

    [ObservableProperty]
    public int remainingCharacters = CharacterLimit;
    Realm Realm { get; set; }
    public CaseloadItem CaseloadItem { get; set; }

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
        // PersonVisit = PersonVisit.FindByEntityId(Realm, CaseloadItem.CaseIncidentNumber) ?? CreateNoteDraft();
    }

    partial void OnPersonVisitItemChanged(PersonVisit value)
    {
        if (value != null)
        {
            SelectedVisitType = value.VisitDetailsValue;
            SelectedVisitDetail = value.VisitDetailsGroup;
        }
    }
}