namespace Visitz.Views.Banners;

public partial class InfoBanner : ContentView
{
	public static readonly BindableProperty TextProperty =
		BindableProperty.Create(nameof(Text), typeof(string), typeof(InfoBanner));

    public static readonly BindableProperty LevelProperty =
        BindableProperty.Create(nameof(Level), typeof(AlertLevel), typeof(InfoBanner), propertyChanged: LevelChanged);

	public string Text
	{
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

    public AlertLevel Level
    {
        get => (AlertLevel)GetValue(LevelProperty);
        set => SetValue(LevelProperty, value);
    }

	public InfoBanner()
	{
		InitializeComponent();
	}

    private static void LevelChanged(BindableObject boundObj, object oldVal, object newVal)
    {
        InfoBanner banner = (InfoBanner)boundObj;
        AlertLevel newLevel = (AlertLevel)newVal;

        switch (newLevel)
        {
            case AlertLevel.Info:
                // TODO ...
                break;
            case AlertLevel.Warning:
                break;
            case AlertLevel.Danger:
                break;
            case AlertLevel.Critical:
                break;
            case AlertLevel.Unknown:
            case AlertLevel.Success:
            default:
                throw new NotImplementedException(newLevel.ToString());
        }
    }
}
