using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ContactEducationEditView : IcmRecordContentView<ContactEducationEditViewModel>
{
    public ContactEducationEditView(ContactEducationEditViewModel viewModel)
        : base(viewModel, LocalizedStrings.ContactEducation)
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    public void Init(string rowId, EntityType entityType, ContactEducationDraft? draftToLoad)
    {
        Init(rowId, entityType);
        ViewModel.Draft = draftToLoad;
    }
}
