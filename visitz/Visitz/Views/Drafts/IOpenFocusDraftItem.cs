using VisitzModel.Models.Navigation;

namespace Visitz.Views.Drafts;

internal interface IOpenFocusDraftItem : IFocusDraftItem
{
    Task OpenFocusedDraftAsync();
}
