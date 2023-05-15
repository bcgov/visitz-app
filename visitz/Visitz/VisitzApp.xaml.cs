namespace Visitz;

public partial class VisitzApp : Application
{
    public VisitzApp()
    {
        InitializeComponent();

        MainPage = new VisitzShell();
    }
}

