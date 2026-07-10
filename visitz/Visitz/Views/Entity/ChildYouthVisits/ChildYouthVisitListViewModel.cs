using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.Banners;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.ChildYouthVisits;

#nullable enable

public partial class ChildYouthVisitListViewModel : IcmRecordViewModel, IRequestedEntitySection
{
    private bool _disposed;

    readonly ObservableRealmQueryMap realmQuery = new();

    public EntitySection RequestedSection { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<PersonVisit> PersonVisits { get; set; } = [];

    [ObservableProperty]
    public partial DateTimeOffset DateOfVisit { get; set; }

    [ObservableProperty]
    public partial string Type { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string VisitDescription { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string CreatedBy { get; set; } = string.Empty;

    [ObservableProperty]
    public partial AlertLevel BannerLevel { get; set; }

    [ObservableProperty]
    public partial string BannerText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool HasVisitData { get; set; } = true;

    [ObservableProperty]
    public partial bool ShowEmptyIcon { get; set; } = false;

    [ObservableProperty]
    public partial bool IsDraftAvailable { get; set; }

    [ObservableProperty]
    public partial string OpenAddVisitIconGlyph { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string OpenAddVisitText { get; set; } = string.Empty;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        Realm visitDraftRealm = await VisitzRealms.GetPersonVisitDraftsRealmAsync();

        realmQuery.ItemsChanged += RealmQuery_ItemsChanged;

        realmQuery.Subscribe(DataRealm, PersonVisit.GetVisitsByCaseId(DataRealm, BusinessObject.Id));

        realmQuery.Subscribe(
            visitDraftRealm,
            visitDraftRealm.All<PersonVisitDraft>().Where(visit => visit.RelatedEntityId == BusinessObject.Id)
        );

        OnIsDraftAvailableChanged(false);

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
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e
    )
    {
        if (e.Type == typeof(PersonVisit))
            UpdateVisitsList(e.Items, e.Changes);
        else if (e.Type == typeof(PersonVisitDraft))
            IsDraftAvailable = e.Items.Any();

        UpdatePersonVisitRelatedInfo(PersonVisits);
    }

    private void UpdateVisitsList(IRealmCollection<IRealmObject> items, ChangeSet? changes)
    {
        if (changes == null)
        {
            foreach (var item in items)
                PersonVisits.Add((PersonVisit)item);
        }
        else
        {
            foreach (int deleted in changes.DeletedIndices.Reverse())
                PersonVisits.RemoveAt(deleted);

            foreach (int inserted in changes.InsertedIndices)
                PersonVisits.Insert(inserted, (PersonVisit)items[inserted]);
        }
    }

    partial void OnIsDraftAvailableChanged(bool value)
    {
        OpenAddVisitIconGlyph = value ? MaterialIcons.Edit : MaterialIcons.Add;
        OpenAddVisitText = value ? LocalizedStrings.ContinueDraft : LocalizedStrings.AddVisit;
    }

    [RelayCommand]
    public async Task OpenVisitEntry()
    {
        var visitEntryView = ServiceProvider.GetService<ChildYouthVisitView>();
        visitEntryView.ViewModel.RowId = RowId;
        visitEntryView.ViewModel.EntityType = EntityType;

        await Navigator.Navigation.PushModalAsync(visitEntryView, ViewModalSize.Wide);
    }
}
