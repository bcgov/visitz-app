using CommunityToolkit.Mvvm.ComponentModel;
using Oidc;
using Realms;
using System.Collections.ObjectModel;
using VisitzModel.Models;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Caseload;

#nullable enable

public partial class CaseloadLister : ObservableObject, IDisposable
{
    Realm Realm { get; }

    Func<IEnumerable<IBusinessObject>, IEnumerable<IBusinessObject>> Filter { get; }

    OidcSessionInfo SessionInfo { get; }

    readonly ObservableRealmQueryMap queryMap = new();

    bool disposedValue;

    readonly List<CaseRecord> cases = [];

    readonly List<IncidentRecord> incidents = [];

    [ObservableProperty]
    public ObservableCollection<CaseloadItemViewModel> records = [];

    [ObservableProperty]
    public DraftIndicatorHelper indicatorHelper;

    public CaseloadLister(
        Realm realm,
        DraftIndicatorHelper indicatorHelper,
        OidcSessionInfo sessionInfo,
        Func<IEnumerable<IBusinessObject>, IEnumerable<IBusinessObject>> filter)
    {
        Realm = realm;
        Filter = filter;
        SessionInfo = sessionInfo;
        IndicatorHelper = indicatorHelper;
        Setup();
    }

    void Setup()
    {
        queryMap.ItemsChanged += QueryMap_ItemsChanged;

        queryMap.Subscribe(Realm, Realm.All<CaseRecord>());
        queryMap.Subscribe(Realm, Realm.All<IncidentRecord>());
        // TODO: query Memos and Service Requests when we decide to support them
    }

    void QueryMap_ItemsChanged(
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e)
    {
        if (e.Type == typeof(CaseRecord))
            UpdateItems(cases, e.Items, e.Changes);
        else if (e.Type == typeof(IncidentRecord))
            UpdateItems(incidents, e.Items, e.Changes);

        ApplyWithFilter();
    }

    public void ApplyWithFilter()
    {
        var caseRecords = cases
            .Cast<IBusinessObject>()
            .Where(rec => rec.IsValid);

        var incidentRecords = incidents
            .Cast<IBusinessObject>()
            .Where(rec => rec.IsValid);

        var combined = caseRecords.Concat(incidentRecords);

        foreach (var item in Records)
            item.Dispose();
        Records.Clear();

        combined = Filter(combined);

        foreach (var record in combined)
            Records.Add(new CaseloadItemViewModel(IndicatorHelper, record, SessionInfo));
    }

    static void UpdateItems<T>(
        IList<T> list,
        IRealmCollection<IRealmObject> items,
        ChangeSet? changes) where T : IBusinessObject
    {
        if (changes == null)
        {
            list.Clear();

            foreach (var item in items)
                list.Add((T)item);
        }
        else
        {
            foreach (var index in changes.DeletedIndices.Reverse())
                list.RemoveAt(index);

            foreach (var index in changes.InsertedIndices)
                list.Insert(index, (T)items.ElementAt(index));
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                queryMap?.Dispose();

            foreach (var item in Records)
                item?.Dispose();

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
