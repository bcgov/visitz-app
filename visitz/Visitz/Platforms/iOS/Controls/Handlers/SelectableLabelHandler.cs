using Microsoft.Maui.Handlers;
using UIKit;

namespace Visitz.Controls.Handlers;

public partial class SelectableLabelHandler() : ViewHandler<SelectableLabel, UITextView>(s_mapper, null)
{
    static readonly IPropertyMapper<SelectableLabel, SelectableLabelHandler> s_mapper = new PropertyMapper<
        SelectableLabel,
        SelectableLabelHandler
    >(ViewMapper)
    {
        [nameof(SelectableLabel.Text)] = MapProperties,
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

    static void MapProperties(SelectableLabelHandler handler, SelectableLabel label)
    {
        handler.PlatformView.Text = label.Text;
        handler.PlatformView.TextColor = UIColor.Black;
    }
}
