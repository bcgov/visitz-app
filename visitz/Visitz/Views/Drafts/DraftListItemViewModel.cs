using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;

namespace Visitz.Views.Drafts;

public partial class DraftListItemViewModel(IDraftItem draft) : VisitzViewModel, IComparable<DraftListItemViewModel>
{
    bool _disposed;

    ObservableQuery<IBusinessObject>? _relatedRecordQuery;

    public IDraftItem Draft { get; private set; } = draft;

    [ObservableProperty]
    public partial bool RelatedEntityAvailable { get; set; }

    [ObservableProperty]
    public partial bool RelatedEntityDownloaded { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        var realm = await VisitzRealms.GetIcmDataRealmAsync();
        var query = IBusinessObject.GetQueryableByRelaxedIdType(realm, Draft.RelatedEntityId, Draft.RelatedEntityType);

        _relatedRecordQuery = new(
            realm,
            query,
            itemsChanged: (items, _) =>
            {
                RelatedEntityAvailable = items.Any();
                RelatedEntityDownloaded =
                    items.FirstOrDefault() is IBusinessObject businessObject
                    && businessObject.LocalState is BoLocalState state
                    && state.ShouldDownloadDuringRefresh;
            }
        );
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _relatedRecordQuery?.Dispose();
            _relatedRecordQuery = null;

            _disposed = true;
        }

        base.Dispose(disposing);
    }

    public int CompareTo(DraftListItemViewModel? other)
    {
        return Draft.CompareTo(other?.Draft);
    }
}
