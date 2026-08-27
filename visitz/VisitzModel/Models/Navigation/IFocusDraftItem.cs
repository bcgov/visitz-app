using VisitzModel.Models.Drafts;

namespace VisitzModel.Models.Navigation;

public interface IFocusDraftItem
{
    IDraftItem? FocusedDraftItem { get; set; }
}
