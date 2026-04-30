using Visitz.Views.BaseClasses;

namespace Visitz.Extensions;

#nullable enable

public static class ContentViewExtensions
{
    public static ContentPage WrapPageForModal(this ContentView contentView, ViewModalSize size = ViewModalSize.Wide)
    {
        var page = ServiceProvider.GetService<WrapperPage>();
        page.SetContent(contentView, size);
        return page;
    }
}
