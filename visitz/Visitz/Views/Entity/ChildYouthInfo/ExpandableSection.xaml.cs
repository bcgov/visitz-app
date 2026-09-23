using System.Collections;
using CommunityToolkit.Maui;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ExpandableSection : BaseContentView
{
    [BindableProperty]
    public partial bool IsExpanded { get; set; } = true;

    [BindableProperty]
    public partial string HeaderGlyph { get; set; } = string.Empty;

    [BindableProperty]
    public partial string HeaderText { get; set; } = string.Empty;

    [BindableProperty]
    public partial IEnumerable? ItemsSource { get; set; }

    [BindableProperty]
    public partial DataTemplate? ItemTemplate { get; set; }

    public ExpandableSection()
    {
        InitializeComponent();
    }
}
