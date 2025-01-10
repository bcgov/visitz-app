using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.Banners;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;

namespace Visitz.Views.Entity.ChildYouthVisits;

internal partial class ChildYouthVisitListViewModel : VisitzViewModel, ICaseloadItemHolder
{
    private bool _disposed;

    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public ObservableCollection<PersonVisit> inPersonVisitList;

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
        await LoadInPersonVisitData();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            // TODO
            _disposed = true;
        }

        base.Dispose(disposing);
    }

    private async Task LoadInPersonVisitData()
    {
        var icmData = await VisitzRealms.GetIcmDataRealmAsync();
        var persons = icmData.All<PersonVisit>()
                      .Where(person => person.ParentId == CaseloadItem.CaseIncidentNumber)
                      .OrderByDescending(person => person.DateOfVisit)
                      .ToList();
        InPersonVisitList = new ObservableCollection<PersonVisit>(persons);
        if (persons.Count != 0)
        {
            var lastVisit = persons.FirstOrDefault();
            if (lastVisit != null)
            {
                DateTimeOffset currentDate = DateTimeOffset.Now;
                DateTimeOffset nextVisitDate = lastVisit.DateOfVisit.AddDays(90);
                string dueDate = nextVisitDate.ToString("MMMM d, yyyy");
                var dateDifference = nextVisitDate - currentDate;
                SetBannerInfo(dateDifference.Days, dueDate);
            }
        }
        else
        {
            HasVisitData = false;
            ShowEmptyIcon = true;
        }
    }

    private void SetBannerInfo(int daysDifference, string dateInBanner)
    {
        if (daysDifference > 30 && daysDifference <= 90)
        {
            BannerLevel = AlertLevel.Info;
            BannerText = string.Format(
                LocalizedStrings.NextVisitDueBy, dateInBanner);
        }
        else if (daysDifference <= 30 && daysDifference > 5)
        {
            BannerLevel = AlertLevel.Warning;
            BannerText = string.Format(
                LocalizedStrings.VisitDueBy, dateInBanner);
        }
        else if (daysDifference <= 5 && daysDifference >= 0)
        {
            BannerLevel = AlertLevel.Danger;
            BannerText = string.Format(
                LocalizedStrings.VisitDueBy, dateInBanner);
        }
        else if (daysDifference < 0)
        {
            BannerLevel = AlertLevel.Critical;
            BannerText = string.Format(
                LocalizedStrings.OverdueVisitOn, dateInBanner);
        }
    }
}
