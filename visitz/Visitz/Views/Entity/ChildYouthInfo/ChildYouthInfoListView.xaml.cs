using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ChildYouthInfoListView : IcmRecordContentView<ChildYouthInfoListViewModel>, IFocusDraftItem
{
    public IDraftItem? FocusedDraftItem
    {
        get => ViewModel.FocusedDraftItem;
        set => ViewModel.FocusedDraftItem = value;
    }

    public ChildYouthInfoListView(ChildYouthInfoListViewModel vm)
        : base(vm, LocalizedStrings.ChildYouthInfoTitle)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        bool animated = false;

        if (FocusedDraftItem is ContactEducationDraft)
            await MainScrollView.ScrollToAsync(EducationSection, ScrollToPosition.MakeVisible, animated);
        else if (FocusedDraftItem is ContactMedicalBehavioralDraft)
            await MainScrollView.ScrollToAsync(MedicalSection, ScrollToPosition.MakeVisible, animated);
        else if (FocusedDraftItem is ContactLanguageDraft)
            await MainScrollView.ScrollToAsync(LanguagesSection, ScrollToPosition.MakeVisible, animated);
    }
}
