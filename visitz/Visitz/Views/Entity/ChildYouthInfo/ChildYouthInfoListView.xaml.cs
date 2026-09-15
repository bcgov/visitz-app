using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ChildYouthInfoListView : IcmRecordContentView<ChildYouthInfoListViewModel>
{
    public ChildYouthInfoListView(ChildYouthInfoListViewModel vm)
        : base(vm, LocalizedStrings.ChildYouthInfoTitle)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
