namespace Visitz.Behaviors;

public class AutoAddColumnBehavior : Behavior<Grid>
{
    protected override void OnAttachedTo(Grid bindable)
    {
        base.OnAttachedTo(bindable);

        bindable.ChildAdded += Bindable_ChildAdded;
    }

    protected override void OnDetachingFrom(Grid bindable)
    {
        bindable.ChildAdded -= Bindable_ChildAdded;

        base.OnDetachingFrom(bindable);
    }

    private void Bindable_ChildAdded(object sender, ElementEventArgs e)
    {
        Grid grid = (Grid)sender;

        grid.AddColumnDefinition(new ColumnDefinition());
        grid.SetColumn((IView)e.Element, grid.Children.Count - 1);
    }
}
