using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oidc.Network;
using Realms;
using System.ComponentModel;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.BaseClasses.Publishing;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitViewModel : VisitzViewModel, IBusinessObjectHolder
{
    public static readonly string VisitTypeGroup = "VisitTypeGroup";
    public static readonly string VisitDetailGroup = "VisitDetailGroup";
    public static readonly int CharacterLimit = 4000;
    public static readonly string RemainingCharactersString = "{0}/" + CharacterLimit;

    private bool _disposed;

    Realm Realm { get; set; }

    Realm DraftRealm { get; set; }

    CaseRecord Case { get; set; }

    [ObservableProperty]
    PersonVisitDraft draft;

    public IBusinessObject BusinessObject { get; set; }

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
    public bool allowDiscard;

    [ObservableProperty]
    public bool allowPublish;

    [ObservableProperty]
    public int remainingCharacters = CharacterLimit;

    [ObservableProperty]
    public int characterCount;

    [ObservableProperty]
    public PersonVisit personVisitItem;

    [ObservableProperty]
    public bool showFullForm = true;

    public DraftSaveStateHandler SaveStateHandler { get; } = new();

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        Realm = await VisitzRealms.GetIcmDataRealmAsync();
        DraftRealm = await VisitzRealms.GetPersonVisitDraftsRealmAsync();
        Case = Realm.All<CaseRecord>().Where(@case => @case.FileNumber == BusinessObject.FileNumber).First();

        if (PersonVisitItem == null && IsUpdatingEnabled)
            Draft = PersonVisitDraft.GetDraft(DraftRealm, Case.Id) ?? new(Case);

        SaveStateHandler.Clear();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            Draft = null;
            PersonVisitItem = null;

            SaveStateHandler.Dispose();

            DraftRealm?.Dispose();
            DraftRealm = null;

            Realm?.Dispose();
            Realm = null;

            _disposed = true;
        }

        base.Dispose(disposing);
    }

    partial void OnPersonVisitItemChanged(PersonVisit oldValue, PersonVisit newValue)
    {
        if (oldValue != null)
            oldValue.PropertyChanged -= PersonVisitItem_PropertyChanged;

        if (newValue != null)
        {
            newValue.PropertyChanged += PersonVisitItem_PropertyChanged;
            UpdateAllowPublish();
        }
    }

    private async void PersonVisitItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        IsVisitTypeSelected = PersonVisitItem.VisitDetails?.Count > 0;

        if (!IsUpdatingEnabled)
            return;

        await HandleDraft();
        UpdateAllowPublish();
    }

    [RelayCommand]
    public async Task PublishInPersonVisit()
    {
        if (!AllowPublish)
            return;

        await Navigator.Navigation.PopModalAsync();

        var publishVm = ServiceProvider.GetService<ChildYouthVisitPublishViewModel>();
        publishVm.BusinessObject = BusinessObject;

        await Navigator.Navigation.PushAsync(new PublishPage(publishVm));
    }

    public void DiscardDraft()
    {
        string id = Draft.RelatedEntityId;
        Draft = null;

        DraftRealm.Write(() => DraftRealm.DeleteByIds<PersonVisitDraft>([id]));
    }

    private void UpdateAllowPublish()
    {
        AllowPublish = NetworkHelper.InternetAvailable
            && PersonVisitItem?.VisitDetails.Count > 0
            && PersonVisitItem?.VisitDescription?.Length > 0
            && CharacterCount <= CharacterLimit;
    }

    TaskCompletionSource DraftInitTcs;
    private async Task HandleDraft()
    {
        if (Draft == null)
            return;

        if (DraftInitTcs != null)
            await DraftInitTcs.Task;

        if (!PersonVisitItem.IsManaged)
        {
            DraftInitTcs = new();

            Draft = await PersonVisitDraft.Upsert(
                DraftRealm,
                Case.Id,
                PersonVisitItem,
                BusinessObject.DisplayName);

            DraftInitTcs.TrySetResult();
        }
        else if (Draft?.IsValid ?? false)
            Draft.LastUpdatedBinding = DateTimeOffset.Now;

        await SaveStateHandler.Saving();
    }

    partial void OnCharacterCountChanged(int value)
    {
        RemainingCharacters = CharacterLimit - value;
    }

    partial void OnDraftChanged(PersonVisitDraft value)
    {
        PersonVisitItem = value?.Visit;
        AllowDiscard = value?.IsManaged ?? false;
    }
}
