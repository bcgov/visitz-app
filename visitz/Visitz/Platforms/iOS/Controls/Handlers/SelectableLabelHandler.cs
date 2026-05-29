using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace Visitz.Controls.Handlers;

public partial class SelectableLabelHandler() : ViewHandler<SelectableLabel, UITextView>(s_mapper, null)
{
    static readonly IPropertyMapper<SelectableLabel, SelectableLabelHandler> s_mapper = new PropertyMapper<
        SelectableLabel,
        SelectableLabelHandler
    >(ViewMapper)
    {
        [nameof(SelectableLabel.BackgroundColor)] = MapBackgroundColor,
        [nameof(SelectableLabel.Text)] = MapText,
        [nameof(ITextStyle.TextColor)] = MapTextColor,
        [nameof(ITextStyle.Font)] = MapFont,
    };

    protected override UITextView CreatePlatformView()
    {
        return new()
        {
            Editable = false,
            Selectable = true,
            ScrollEnabled = false,
            TextContainerInset = UIEdgeInsets.Zero,
        };
    }

    protected override void ConnectHandler(UITextView platformView)
    {
        base.ConnectHandler(platformView);
        platformView.Text = VirtualView.Text;
    }

    static void MapBackgroundColor(SelectableLabelHandler handler, SelectableLabel label)
    {
        handler.PlatformView.BackgroundColor = label.BackgroundColor.ToPlatform();
    }

    static void MapText(SelectableLabelHandler handler, SelectableLabel label)
    {
        handler.PlatformView.Text = label.Text;
    }

    static void MapTextColor(SelectableLabelHandler handler, SelectableLabel label)
    {
        handler.PlatformView.TextColor = label.TextColor.ToPlatform();
    }

    static void MapFont(SelectableLabelHandler handler, SelectableLabel label)
    {
        var fontManager = handler.GetRequiredService<IFontManager>();
        handler.PlatformView.UpdateFont(label, fontManager);
    }
}
