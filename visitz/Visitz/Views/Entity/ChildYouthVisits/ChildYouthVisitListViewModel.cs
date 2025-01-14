using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.Banners;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;

namespace Visitz.Views.Entity.ChildYouthVisits;

internal partial class ChildYouthVisitListViewModel : VisitzViewModel, ICaseloadItemHolder
{
    private static readonly int InfoDayRange = 90;
    private static readonly int WarningDayRange = 30;
    private static readonly int DangerDayRange = 5;
    private static readonly int CriticalDayRange = 0;
    private bool _disposed;

    Realm icmDataRealm;

    ObservableRealmQueryMap realmQuery = new();

    [ObservableProperty]
    ObservableCollection<PersonVisit> personVisits = [];

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

    protected override async Task InitAsync()
    {
        await base.InitAsync();
        icmDataRealm = await VisitzRealms.GetIcmDataRealmAsync();
        realmQuery.ItemsChanged += RealmQuery_ItemsChanged;

        realmQuery.Subscribe(icmDataRealm, icmDataRealm.All<PersonVisit>()
                .Where(person => person.ParentId == CaseloadItem.CaseIncidentNumber)
                .OrderByDescending(person => person.DateOfVisit));

    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            if (realmQuery != null)
            {
                realmQuery.ItemsChanged -= RealmQuery_ItemsChanged;
                realmQuery.Dispose();
                realmQuery = null;
            }
            _disposed = true;
        }

        base.Dispose(disposing);
    }

    private void UpdatePersonVisitRelatedInfo(ObservableCollection<PersonVisit> personVisits)
    {
        HasVisitData = personVisits.Count > 0;
        ShowEmptyIcon = !HasVisitData;
        if (HasVisitData)
        {
            var lastVisit = personVisits.FirstOrDefault();
            if (lastVisit != null)
            {
                DateTimeOffset currentDate = DateTimeOffset.UtcNow;
                DateTimeOffset nextVisitDate = lastVisit.DateOfVisit.AddDays(InfoDayRange);
                string dueDate = nextVisitDate.ToString("MMMM d, yyyy");
                var dateDifference = nextVisitDate - currentDate;
                SetBannerInfo(dateDifference.Days, dueDate);
            }
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
    }
}
