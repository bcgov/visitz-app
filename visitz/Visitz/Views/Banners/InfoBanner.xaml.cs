using Visitz.FontIcons;
using Visitz.Resources.Styles;

namespace Visitz.Views.Banners;

public partial class InfoBanner : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(InfoBanner)
    );

    public static readonly BindableProperty LevelProperty = BindableProperty.Create(
        nameof(Level),
        typeof(AlertLevel),
        typeof(InfoBanner),
        propertyChanged: LevelChanged
    );

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
                banner.BackgroundColor = VisitzColors.AlertBannerInfoBackground;
                banner.IconLabel.Text = MaterialIcons.Info;
                banner.IconLabel.TextColor = VisitzColors.AlertBannerInfoPrimary;
                banner.Border.Stroke = VisitzColors.AlertBannerInfoPrimary;
                break;
            case AlertLevel.Warning:
                banner.BackgroundColor = VisitzColors.AlertBannerWarningBackground;
                banner.IconLabel.Text = MaterialIcons.Error;
                banner.IconLabel.TextColor = VisitzColors.AlertBannerWarningPrimary;
                banner.Border.Stroke = VisitzColors.AlertBannerWarningPrimary;
                break;
            case AlertLevel.Danger:
                banner.BackgroundColor = VisitzColors.AlertBannerDangerBackground;
                banner.IconLabel.Text = MaterialIcons.Error;
                banner.IconLabel.TextColor = VisitzColors.AlertBannerDangerPrimary;
                banner.Border.Stroke = VisitzColors.AlertBannerDangerPrimary;
                break;
            case AlertLevel.Critical:
                banner.BackgroundColor = VisitzColors.AlertBannerCriticalBackground;
                banner.IconLabel.Text = MaterialIcons.Error;
                banner.IconLabel.TextColor = VisitzColors.AlertBannerCriticalPrimary;
                banner.Border.Stroke = VisitzColors.AlertBannerCriticalPrimary;
                break;
            case AlertLevel.Unknown:
            case AlertLevel.Success:
            default:
                throw new NotImplementedException(newLevel.ToString());
        }
    }
}
