using Microsoft.Maui.Controls.Foldable;

namespace Visitz.Views.Navigation;

#nullable enable

public partial class ContentViewNavigationStack : ContentView
{
    readonly Stack<TwoPaneView> _viewStack = new();

    public double MinWideModeWidth { get; set; }

    public int PaneCount
    {
        get
        {
            if (_viewStack.Count > 0)
            {
                int panes = _viewStack.Count * 2;

                // -2 to account for a potentially half-full *Two*PaneView
                return panes - 2 + _viewStack.Peek().Children.Count;
            }
            else
                return 0;
        }
    }

    TwoPaneView NewTwoPane(ContentView? view = null)
    {
        return new TwoPaneView()
        {
            TallModeConfiguration = TwoPaneViewTallModeConfiguration.SinglePane,
            WideModeConfiguration = TwoPaneViewWideModeConfiguration.SinglePane,
            MinWideModeWidth = MinWideModeWidth,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Pane1 = view,
        };
    }

    public void Push(ContentView view, GridLength? paneLength = null)
    {
        GridLength length = paneLength ?? GridLength.Star;

        view.HorizontalOptions = LayoutOptions.Fill;
        view.VerticalOptions = LayoutOptions.Fill;

        if (_viewStack.Count <= 0)
            _viewStack.Push(NewTwoPane());

        TwoPaneView twoPane = _viewStack.Peek();

        if (twoPane.Pane1 == null)
        {
            twoPane.Pane1 = view;
            twoPane.Pane1Length = length;
            twoPane.PanePriority = TwoPaneViewPriority.Pane1;
            twoPane.WideModeConfiguration = TwoPaneViewWideModeConfiguration.SinglePane;
        }
        else if (twoPane.Pane2 == null)
        {
            twoPane.Pane2 = view;
            twoPane.Pane2Length = length;
            twoPane.PanePriority = TwoPaneViewPriority.Pane2;
            twoPane.WideModeConfiguration = TwoPaneViewWideModeConfiguration.LeftRight;
        }
        else
            _viewStack.Push(NewTwoPane(view));

        Content = _viewStack.Peek();
    }

    public ContentView? Pop()
    {
        TwoPaneView twoPane = _viewStack.Peek();

        if (twoPane.Pane2 is ContentView view2)
        {
            twoPane.Pane2 = null;
            twoPane.PanePriority = TwoPaneViewPriority.Pane1;
            twoPane.WideModeConfiguration = TwoPaneViewWideModeConfiguration.SinglePane;
            return view2;
        }
        else if (twoPane.Pane1 is ContentView view1)
        {
            twoPane.Pane1 = null;
            _viewStack.Pop();
            Content = _viewStack.Peek();
            return view1;
        }
        else
        {
            _viewStack.Pop();
            Content = _viewStack.Peek();
            return null;
        }
    }
}
