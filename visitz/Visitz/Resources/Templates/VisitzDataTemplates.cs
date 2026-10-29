using Visitz.Extensions;

namespace Visitz.Resources.Templates;

public class VisitzDataTemplates
{
    public static DataTemplate TryGetDataTemplate(string name)
    {
        return Application.Current?.Resources.TryGetResource<DataTemplate>(name)
            ?? throw new InvalidOperationException($"DataTemplate '{name}' not found in resources");
    }

    public static DataTemplate ActionButtonMenuDataTemplate => TryGetDataTemplate(nameof(ActionButtonMenuDataTemplate));
}
