using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ContactEducationEditView : IcmRecordContentView<ContactEducationEditViewModel>
{
    public ContactEducationEditView(ContactEducationEditViewModel viewModel)
        : base(viewModel, LocalizedStrings.ContactEducation)
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
