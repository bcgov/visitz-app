using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.FontIcons;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Notes;
using VisitzModel.Models.SafetyAssess;

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

    [ObservableProperty]
    public partial string IconGlyph { get; set; } = GetIconGlyph(draft);

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

    static string GetIconGlyph(IDraftItem item)
    {
        if (item is PersonVisitDraft)
            return MaterialIcons.Person_pin_circle;
        else if (item is NoteDraft)
            return MaterialIcons.Edit_document;
        else if (item is AssessmentDraft)
            return MaterialIcons.Assignment;
        else if (item is AttachmentDraft)
            return MaterialIcons.Attachment;
        else
            return MaterialIcons.Unknown_document;
    }
}
