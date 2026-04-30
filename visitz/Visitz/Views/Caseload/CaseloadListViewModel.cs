using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Oidc;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Models;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Caseload;

#nullable enable

public partial class CaseloadListViewModel : VisitzViewModel
{
    bool _disposed;

    DraftIndicatorHelper DraftIndicatorHelper { get; } = new();

    OidcSessionInfo? SessionInfo { get; set; }

    ObservableRealmQueryMap QueryMap { get; } = new();

    ObservableCollection<CaseRecord> CaseRecords { get; set; } = [];

    ObservableCollection<IncidentRecord> IncidentRecords { get; set; } = [];

    ObservableCollection<IBusinessObject> AllItems { get; set; } = [];

    [ObservableProperty]
    public ObservableCollection<CaseloadItemViewModel> filteredItems = [];

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        await DraftIndicatorHelper.InitTask;
        SessionInfo = await OidcSessionInfo.GetAsync();

        CaseRecords.CollectionChanged += SupportingRecordsCollection_CollectionChanged;
        IncidentRecords.CollectionChanged += SupportingRecordsCollection_CollectionChanged;
        AllItems.CollectionChanged += AllItems_CollectionChanged;

        var dataRealm = await VisitzRealms.GetIcmDataRealmAsync();

        QueryMap.ItemsChanged += QueryMap_ItemsChanged;
        QueryMap.Subscribe(dataRealm, dataRealm.All<CaseRecord>());
        QueryMap.Subscribe(dataRealm, dataRealm.All<IncidentRecord>());
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            QueryMap.Dispose();

            AllItems.CollectionChanged -= AllItems_CollectionChanged;
            CaseRecords.CollectionChanged -= SupportingRecordsCollection_CollectionChanged;
            IncidentRecords.CollectionChanged -= SupportingRecordsCollection_CollectionChanged;

            DraftIndicatorHelper.Dispose();

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    void QueryMap_ItemsChanged(object? sender, (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e)
    {
        if (e.Type == typeof(CaseRecord))
            UpdateList(CaseRecords, e.Items, e.Changes);
        else if (e.Type == typeof(IncidentRecord))
            UpdateList(IncidentRecords, e.Items, e.Changes);
    }

    void UpdateList<TRecord>(
        ObservableCollection<TRecord> records,
        IRealmCollection<IRealmObject> items,
        ChangeSet? changes
    )
        where TRecord : IBusinessObject
    {
        if (changes == null)
        {
            records.AddAll(items.Cast<TRecord>());
        }
        else
        {
            foreach (var deleteIndex in changes.DeletedIndices.Reverse())
                records.RemoveAt(deleteIndex);

            foreach (var insertIndex in changes.InsertedIndices)
                records.Insert(insertIndex, (TRecord)items.ElementAt(insertIndex));
        }
    }

    void SupportingRecordsCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            IEnumerable<IBusinessObject> newItems = e.NewItems.Cast<IBusinessObject>();

            foreach (var item in newItems)
                AllItems.Add(item);
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            IEnumerable<IBusinessObject> oldItems = e.OldItems.Cast<IBusinessObject>();

            foreach (var item in oldItems)
                AllItems.Remove(item);
        }
        else
        {
            Logger.LogInformation($"Unhandled CollectionChanged action: '{e.Action}'");
        }
    }

    private void AllItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(SessionInfo);

        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            IEnumerable<IBusinessObject> newItems = e.NewItems.Cast<IBusinessObject>();

            foreach (var item in newItems)
                FilteredItems.Add(new CaseloadItemViewModel(DraftIndicatorHelper, item, SessionInfo));
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            IEnumerable<IBusinessObject> oldItems = e.OldItems.Cast<IBusinessObject>();

            foreach (var item in oldItems)
                if (FilteredItems.FirstOrDefault(vm => vm.BusinessObject == item) is CaseloadItemViewModel vm)
                    FilteredItems.Remove(vm);
        }
        else
        {
            Logger.LogInformation($"Unhandled CollectionChanged action: '{e.Action}'");
        }
    }
}
