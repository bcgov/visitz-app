using System.ComponentModel;
using Realms;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.BaseClasses;

public partial class DraftEditView<TDraftItem>(DraftEditViewModel<TDraftItem> viewModel, string title)
    : IcmRecordContentView<DraftEditViewModel<TDraftItem>>(viewModel, title)
    where TDraftItem : IDraftItem, IRealmObject, INotifyPropertyChanged, new()
{
    public void Init(string rowId, EntityType entityType, TDraftItem? draftToLoad)
    {
        Init(rowId, entityType);
        ViewModel.Draft = draftToLoad;
    }
}
