using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.Banners;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Storage;

namespace Visitz.Views.Entity.ChildYouthVisits;

internal partial class ChildYouthVisitListViewModel : VisitzViewModel, ICaseloadItemHolder
{
    private static readonly int InfoDayRange = 90;
    private static readonly int WarningDayRange = 30;
    private static readonly int DangerDayRange = 5;
    private static readonly int CriticalDayRange = 0;
    private bool _disposed;
    readonly ObservableRealmQueryMap realmQuery = new();

    [ObservableProperty]
    ObservableCollection<PersonVisit> personVisits = [];

    [ObservableProperty]
    public PersonVisit personVisitItem;

    [ObservableProperty]
    public CaseloadItem caseloadItem;

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
        realmQuery.ItemsChanged += RealmQuery_ItemsChanged;

        realmQuery.Subscribe(icmDataRealm, icmDataRealm.All<PersonVisit>()
                .Where(person => person.ParentId == CaseloadItem.CaseIncidentNumber)
                .OrderByDescending(person => person.DateOfVisit));
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
        {
            DateTimeOffset currentDate = DateTimeOffset.UtcNow;
            DateTimeOffset nextVisitDate = lastVisit.DateOfVisit.AddDays(InfoDayRange);
            string dueDate = nextVisitDate.ToString("MMMM d, yyyy");
            var dateDifference = nextVisitDate - currentDate;
            SetBannerInfo(dateDifference.Days, dueDate);
        }
    }

    private void SetBannerInfo(int daysDifference, string dateInBanner)
    {
        if (daysDifference > WarningDayRange)
        {
            BannerLevel = AlertLevel.Info;
            BannerText = string.Format(
                LocalizedStrings.NextVisitDueBy, dateInBanner);
        }
        else if (daysDifference > DangerDayRange)
        {
            BannerLevel = AlertLevel.Warning;
            BannerText = string.Format(
                LocalizedStrings.VisitDueBy, dateInBanner);
        }
        else if (daysDifference >= CriticalDayRange)
        {
            BannerLevel = AlertLevel.Danger;
            BannerText = string.Format(
                LocalizedStrings.VisitDueBy, dateInBanner);
        }
        else
        {
            BannerLevel = AlertLevel.Critical;
            BannerText = string.Format(
                LocalizedStrings.OverdueVisitOn, dateInBanner);
        }
    }

    private void RealmQuery_ItemsChanged(object sender, (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
    {
        if (e.Changes == null)
        {
            foreach (var item in e.Items)
                PersonVisits.Add(item as PersonVisit);
        }
        else
        {
            foreach (int deleted in e.Changes.DeletedIndices)
                PersonVisits.RemoveAt(deleted);

            foreach (int modified in e.Changes.ModifiedIndices)
                PersonVisits[modified] = e.Items[modified] as PersonVisit;

            foreach (int inserted in e.Changes.InsertedIndices)
                PersonVisits.Insert(inserted, e.Items[inserted] as PersonVisit);
        }
        UpdatePersonVisitRelatedInfo(PersonVisits);
        if (e.Type == typeof(PersonVisitDrafts))
            UpdateOpenAddVisitText(e.Items.Any());
    }

    private void UpdateOpenAddVisitText(bool draftAvailable)
	{
		OpenAddVisitText = draftAvailable ? LocalizedStrings.ContinueDraft : LocalizedStrings.AddVisit;
	}

    [RelayCommand]
    public async Task OpenVisitEntry(PersonVisit personVisitObj = null)
    {
        var visitEntryView = ServiceProvider.GetService<ChildYouthVisitView>();
        visitEntryView.CaseloadItem = CaseloadItem;
        visitEntryView.ViewModel.PersonVisitItem = personVisitObj;
        if (personVisitObj != null)
            visitEntryView.ViewModel.IsUpdatingEnabled = false;
        else
            visitEntryView.ViewModel.IsUpdatingEnabled = true;

        await Navigator.Navigation.PushModalAsync(visitEntryView, ViewModalSize.Wide);
    }
}
