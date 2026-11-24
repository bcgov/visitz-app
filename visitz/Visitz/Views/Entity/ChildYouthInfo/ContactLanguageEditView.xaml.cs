using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ContactLanguageEditView : DraftEditView<ContactLanguageDraft>
{
    public ContactLanguageEditView(ContactLanguageEditViewModel viewModel)
        : base(viewModel, LocalizedStrings.ContactLanguages)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
