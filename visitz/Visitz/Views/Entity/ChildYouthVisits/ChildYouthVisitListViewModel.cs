using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.Banners;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.ChildYouthVisits;

internal partial class ChildYouthVisitListViewModel : VisitzViewModel, IBusinessObjectHolder, IRequestedEntitySection
{
    private bool _disposed;

    readonly ObservableRealmQueryMap realmQuery = new();

    public EntitySection RequestedSection { get; set; }

    [ObservableProperty]
    ObservableCollection<PersonVisit> personVisits = [];

    [ObservableProperty]
    public IBusinessObject businessObject;

    [ObservableProperty]
    public DateTimeOffset dateOfVisit;

    [ObservableProperty]
    public string type;

    [ObservableProperty]
    public string visitDescription;

    [ObservableProperty]
    public string createdBy;

    [ObservableProperty]
    public AlertLevel bannerLevel;

    [ObservableProperty]
    public string bannerText;

    [ObservableProperty]
    public bool hasVisitData = true;

    [ObservableProperty]
    public bool showEmptyIcon = false;

    [ObservableProperty]
    public string openAddVisitText;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        Realm icmDataRealm = await VisitzRealms.GetIcmDataRealmAsync();
        Realm visitDraftRealm = await VisitzRealms.GetPersonVisitDraftsRealmAsync();

        realmQuery.ItemsChanged += RealmQuery_ItemsChanged;

        realmQuery.Subscribe(icmDataRealm, PersonVisit.GetVisitsByCaseId(icmDataRealm, BusinessObject.Id));

        realmQuery.Subscribe(
            visitDraftRealm,
            visitDraftRealm.All<PersonVisitDraft>().Where(visit => visit.RelatedEntityId == BusinessObject.Id)
        );

        if (RequestedSection == EntitySection.ChildYouthVisitsEntry)
            await OpenVisitEntry();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            realmQuery.ItemsChanged -= RealmQuery_ItemsChanged;
            realmQuery.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private void UpdatePersonVisitRelatedInfo(ObservableCollection<PersonVisit> personVisits)
    {
        HasVisitData = personVisits.Count > 0;
        ShowEmptyIcon = !HasVisitData;
        if (personVisits.FirstOrDefault() is PersonVisit lastVisit)
            SetBannerInfo(lastVisit);
    }

    private void SetBannerInfo(PersonVisit personVisit)
    {
        string dateInBanner = personVisit.DueDate.ToString("MMMM d, yyyy");
        var threshold = personVisit.CurrentDueDateThreshold;
        switch (threshold)
        {
            case VisitDaysThreshold.Info:
                BannerLevel = AlertLevel.Info;
                BannerText = string.Format(LocalizedStrings.NextVisitDueBy, dateInBanner);
                break;
            case VisitDaysThreshold.Warning:
                BannerLevel = AlertLevel.Warning;
                BannerText = string.Format(LocalizedStrings.VisitDueBy, dateInBanner);
                break;
            case VisitDaysThreshold.Danger:
                BannerLevel = AlertLevel.Danger;
                BannerText = string.Format(LocalizedStrings.VisitDueBy, dateInBanner);
                break;
            case VisitDaysThreshold.Critical:
                BannerLevel = AlertLevel.Critical;
                BannerText = string.Format(LocalizedStrings.OverdueVisitOn, dateInBanner);
                break;
            default:
                BannerLevel = AlertLevel.Critical;
                BannerText = string.Format(LocalizedStrings.OverdueVisitOn, dateInBanner);
                break;
        }
    }

    private void RealmQuery_ItemsChanged(
        object sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e
    )
    {
        if (e.Type == typeof(PersonVisit))
            UpdateVisitsList(e.Items, e.Changes);
        else if (e.Type == typeof(PersonVisitDraft))
            UpdateOpenAddVisitText(e.Items.Any());

        UpdatePersonVisitRelatedInfo(PersonVisits);
    }

    private void UpdateVisitsList(IRealmCollection<IRealmObject> items, ChangeSet changes)
    {
        if (changes == null)
        {
            foreach (var item in items)
                PersonVisits.Add(item as PersonVisit);
        }
        else
        {
            foreach (int deleted in changes.DeletedIndices.Reverse())
                PersonVisits.RemoveAt(deleted);

            foreach (int inserted in changes.InsertedIndices)
                PersonVisits.Insert(inserted, items[inserted] as PersonVisit);
        }
    }

    private void UpdateOpenAddVisitText(bool draftAvailable)
    {
        OpenAddVisitText = draftAvailable ? LocalizedStrings.ContinueDraft : LocalizedStrings.AddVisit;
    }

    [RelayCommand]
    public async Task OpenVisitEntry(PersonVisit personVisitObj = null)
    {
        var visitEntryView = ServiceProvider.GetService<ChildYouthVisitView>();
        visitEntryView.BusinessObject = BusinessObject;
        visitEntryView.ViewModel.PersonVisitItem = personVisitObj;
        visitEntryView.ViewModel.IsUpdatingEnabled = personVisitObj == null;
        visitEntryView.ViewModel.HideElements = personVisitObj == null;

        await Navigator.Navigation.PushModalAsync(visitEntryView, ViewModalSize.Wide);
    }
}
