using VisitzModel.Models.Drafts;

namespace Visitz.Views.Entity.ChildYouthInfo;

internal class ChildYouthInfoListItemSelector<TDraftItem, TReadItem> : DataTemplateSelector
    where TDraftItem : IDraftItem
{
    public DataTemplate? DraftTemplate { get; set; }

    public DataTemplate? DownloadedDataTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        return ((ChildYouthInfoListItemViewModel<TDraftItem, TReadItem>)item).DraftItem != null
            ? DraftTemplate
            : DownloadedDataTemplate;
    }
}
