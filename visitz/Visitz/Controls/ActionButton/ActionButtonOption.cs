using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Visitz.Resources.Styles;

namespace Visitz.Controls.ActionButton;

public class ActionButtonOption
{
    public required ICommand Command
    {
        get;
        set =>
            // Wrap with our own command to notify when commands are executed
            field = value
                is IAsyncRelayCommand asyncCommand
                ? new Command(async () =>
                {
                    await asyncCommand.ExecuteAsync(null);
                    Selected?.Invoke(this, EventArgs.Empty);
                })
                : new Command(() =>
                {
                    value.Execute(null);
                    Selected?.Invoke(this, EventArgs.Empty);
                });
    }

    public ImageSource? Icon
    {
        get;
        set
        {
            if (value is FontImageSource source)
            {
                // Override incoming value properties so callers do not need to worry about setting it themselves
                source.Color = VisitzColors.Primary;
                source.Size = VisitzDimensions.ButtonFontImageSourceSize;
            }
            field = value;
        }
    }

    public required string Text { get; set; }

    public event EventHandler? Selected;
}
