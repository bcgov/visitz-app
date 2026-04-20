using Visitz.Views.BaseClasses;

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
}
