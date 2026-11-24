using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ContactEducationEditView : DraftEditView<ContactEducationDraft>
{
    public ContactEducationEditView(ContactEducationEditViewModel viewModel)
        : base(viewModel, LocalizedStrings.ContactEducation)
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
