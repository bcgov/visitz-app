using CommunityToolkit.Maui;

namespace Visitz.Behaviors;

public partial class GridRotateLaneBehavior : Behavior<Grid>
{
    Grid Grid { get; set; } = [];

    ColumnDefinitionCollection? HorizontalColumnDefinitions { get; set; }

    RowDefinitionCollection? VerticalRowDefinitions { get; set; }

    [BindableProperty(PropertyChangedMethodName = nameof(OrientationChanged))]
    public partial ItemsLayoutOrientation Orientation { get; set; } = ItemsLayoutOrientation.Horizontal;

    protected override void OnAttachedTo(Grid bindable)
    {
        base.OnAttachedTo(bindable);

        Grid = bindable;
        BindingContext = bindable.BindingContext;

        HorizontalColumnDefinitions = Grid.ColumnDefinitions;
        VerticalRowDefinitions = Grid.RowDefinitions;

        ApplyDirection();
    }

    protected override void OnDetachingFrom(Grid bindable)
    {
        base.OnDetachingFrom(bindable);

        Grid = [];
        BindingContext = null;
    }

    static void OrientationChanged(BindableObject bindable, object _, object __)
    {
        ((GridRotateLaneBehavior)bindable).ApplyDirection();
    }

    void ApplyDirection()
    {
        if (Orientation == ItemsLayoutOrientation.Horizontal)
            ApplyHorizontal();
        else if (Orientation == ItemsLayoutOrientation.Vertical)
            ApplyVertical();
    }

    void ApplyHorizontal()
    {
        Grid.RowDefinitions = [new() { Height = GridLength.Star }];
        Grid.ColumnDefinitions = HorizontalColumnDefinitions ?? [];

        for (int i = 0; i < Grid.Children.Count; i++)
        {
            IView child = Grid.Children[i];
            Grid.SetRow(child, 0);
            Grid.SetColumn(child, i);

            if (HorizontalColumnDefinitions == null)
                Grid.ColumnDefinitions.Add(new() { Width = GridLength.Star });
        }
    }

    void ApplyVertical()
    {
        Grid.RowDefinitions = VerticalRowDefinitions ?? [];
        Grid.ColumnDefinitions = [new() { Width = GridLength.Star }];

        for (int i = 0; i < Grid.Children.Count; i++)
        {
            IView child = Grid.Children[i];
            Grid.SetRow(child, i);
            Grid.SetColumn(child, 0);

            if (VerticalRowDefinitions == null)
                Grid.RowDefinitions.Add(new() { Height = GridLength.Star });
        }
    }
}
