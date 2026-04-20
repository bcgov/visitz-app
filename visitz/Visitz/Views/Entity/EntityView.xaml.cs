using Visitz.Views.BaseClasses;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity;

#nullable enable

public partial class EntityView : IcmRecordContentView<EntityViewModel>
{
    public EntityView()
        : base(ServiceProvider.GetService<EntityViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    public void SetRequestedSection(EntitySection sectionToOpen, IDraftItem focusedDraftItem)
    {
        ViewModel.RequestedSection = sectionToOpen;
        ViewModel.FocusedDraftItem = focusedDraftItem;
    }
}
