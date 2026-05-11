using System.Collections;
using CommunityToolkit.Maui;
using Syncfusion.Maui.Toolkit.Popup;
using Visitz.Behaviors;
using Visitz.Resources.Styles;

namespace Visitz.Controls;

#nullable enable

public partial class SelectionPopup : SfPopup
{
    [BindableProperty]
    public partial IEnumerable? ItemsSource { get; set; }

    [BindableProperty]
    public partial object? SelectedItem { get; set; }

    [BindableProperty]
    public partial DataTemplate? ItemDataTemplate { get; set; }

    [BindableProperty]
    public partial bool StickySelection { get; set; }

    public SelectionPopup()
    {
        AnimationDuration = 75;
        ShowHeader = false;
        AutoSizeMode = PopupAutoSizeMode.Both;
        ShowOverlayAlways = false;

        PopupStyle = new()
        {
            PopupBackground = VisitzColors.Gray100,
            HasShadow = true,
            CornerRadius = 5,
        };

        ContentTemplate = new DataTemplate(() =>
        {
            VerticalStackLayout vsl = new()
            {
                MinimumHeightRequest = 10,
                MinimumWidthRequest = 10,
                HorizontalOptions = LayoutOptions.Start,
            };

            vsl.Behaviors.Add(MakeSelectionBehavior());

            vsl.SetBinding(
                BindableLayout.ItemTemplateProperty,
                static (SelectionPopup popup) => popup.ItemDataTemplate,
                source: this
            );

            return vsl;
        });
    }

    SelectionLayoutBehavior MakeSelectionBehavior()
    {
        SelectionLayoutBehavior slb = new();

        slb.SetBinding(
            SelectionLayoutBehavior.StickySelectionProperty,
            static (SelectionPopup popup) => popup.StickySelection,
            source: this
        );
        slb.SetBinding(
            SelectionLayoutBehavior.ItemsSourceProperty,
            static (SelectionPopup popup) => popup.ItemsSource,
            source: this
        );
        slb.SetBinding(
            SelectionLayoutBehavior.SelectedItemProperty,
            static (SelectionPopup popup) => popup.SelectedItem,
            source: this
        );

        return slb;
    }
}
