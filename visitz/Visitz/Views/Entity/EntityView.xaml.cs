using System.Reflection;
using Syncfusion.Maui.Toolkit.Popup;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity;

#nullable enable

public partial class EntityView : IcmRecordContentView<EntityViewModel>
{
    Grid? TabsGrid => TabView.Content as Grid;

    RowDefinition? TabsRow =>
        TabsGrid is not null && TabsGrid.RowDefinitions.Count > 0 ? TabsGrid?.RowDefinitions[0] : null;

    Thickness? TabBarPadding
    {
        get
        {
            if (TabsGrid?[0] is VisualElement element)
            {
                Type type = element.GetType();

                if (
                    type.GetProperty("TabHeaderPadding", typeof(Thickness)) is PropertyInfo info
                    && info.GetValue(element) is Thickness thickness
                )
                {
                    return thickness;
                }
            }

            return null;
        }
    }

    public EntityView()
        : base(ServiceProvider.GetService<EntityViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        ViewModel.TabListButtonWidth = TabBarPadding?.Left ?? -1;

        if (TabsRow is RowDefinition row)
        {
            row.SizeChanged += EntityView_SizeChanged;
            ViewModel.TabBarHeight = row.Height.Value;
        }
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            TabsRow?.SizeChanged -= EntityView_SizeChanged;

            disposed = true;
        }
        base.Dispose(disposing);
    }

    private void EntityView_SizeChanged(object? sender, EventArgs e)
    {
        if (sender is RowDefinition row)
        {
            ViewModel.TabBarHeight = row.Height.Value;
        }
    }

    private async void TabListButton_Clicked(object? sender, EventArgs e)
    {
        if (sender != null)
            TabPopup.ShowRelativeToView((View)sender, PopupRelativePosition.AlignBottom);
    }

    private void PopupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TabPopup.Dismiss();
    }
}
