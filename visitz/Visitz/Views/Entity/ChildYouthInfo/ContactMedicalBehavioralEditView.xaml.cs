using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ContactMedicalBehavioralEditView : DraftEditView<ContactMedicalBehavioralDraft>
{
    public ContactMedicalBehavioralEditView(ContactMedicalBehavioralEditViewModel viewModel)
        : base(viewModel, LocalizedStrings.MedicalBehavioural)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
