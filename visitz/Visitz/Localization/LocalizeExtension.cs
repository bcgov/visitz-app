using Microsoft.Extensions.Localization;
using Visitz.Resources.Localization;

namespace Visitz.Localization;

[RequireService([typeof(IStringLocalizer<LocalizedStrings>)])]
[ContentProperty(nameof(Key))]
// We will use this name in XML like so: Text="{local:Localize hello_world}"
public class LocalizeExtension : IMarkupExtension
{
    // Generic LocalizedStrings name has to match your .resx filename
    private IStringLocalizer<LocalizedStrings> Localizer { get; }

    public string Key { get; set; } = string.Empty;

    public LocalizeExtension()
    {
        // Have to inject like this because LocalizeExtension constructor
        // has to be parameterless in order to be used in XAML
        Localizer = ServiceProvider.GetService<IStringLocalizer<LocalizedStrings>>();
    }

    public object ProvideValue()
    {
        string localizedText = Localizer[Key];
        return localizedText;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue();

    public string Localize(string key)
    {
        return Localizer[key];
    }
}
