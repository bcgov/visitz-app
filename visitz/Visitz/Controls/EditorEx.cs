using System.Text.Json;
using VisitzModel.Extensions;

namespace Visitz.Controls;

internal partial class EditorEx : Editor
{
    public static readonly BindableProperty CharacterCountProperty = BindableProperty.Create(
        nameof(CharacterCount),
        typeof(int),
        typeof(EditorEx),
        defaultBindingMode: BindingMode.OneWayToSource
    );

    public static readonly BindableProperty CountStyleProperty = BindableProperty.Create(
        nameof(CountStyle),
        typeof(CharacterCountStyle),
        typeof(EditorEx)
    );

    public static readonly BindableProperty SuggestedMaxLengthProperty = BindableProperty.Create(
        nameof(SuggestedMaxLength),
        typeof(int),
        typeof(EditorEx)
    );

    public int CharacterCount
    {
        get => (int)GetValue(CharacterCountProperty);
        set => SetValue(CharacterCountProperty, value);
    }

    public CharacterCountStyle CountStyle
    {
        get => (CharacterCountStyle)GetValue(CountStyleProperty);
        set => SetValue(CountStyleProperty, value);
    }

    public int SuggestedMaxLength
    {
        get => (int)GetValue(SuggestedMaxLengthProperty);
        set => SetValue(SuggestedMaxLengthProperty, value);
    }

    public event EventHandler? EmojiEntered;

    public event EventHandler? SuggestedMaxLengthExceeded;

    public EditorEx()
    {
        SuggestedMaxLength = int.MinValue;
    }

    protected override void OnTextChanged(string oldValue, string newValue)
    {
        if (oldValue == newValue)
            return;
        else if (ShouldRestoreText(newValue))
        {
            Text = oldValue;
            EmojiEntered?.Invoke(this, EventArgs.Empty);
        }
        else if (AllowedToUpdate(oldValue))
            DoTextChanged(oldValue, newValue);
    }

    static bool ShouldRestoreText(string newValue)
    {
        return newValue?.ContainsUnicodeSurrogatesAndOtherSymbols() ?? false;
    }

    static bool AllowedToUpdate(string oldValue)
    {
        return !oldValue?.ContainsUnicodeSurrogatesAndOtherSymbols() ?? true;
    }

    bool DoesCharacterCountExceedSuggestedMaxLength()
    {
        return SuggestedMaxLength >= 0 && CharacterCount > SuggestedMaxLength;
    }

    void DoTextChanged(string oldValue, string newValue)
    {
        if (CountStyle == CharacterCountStyle.JsonForRestApi)
        {
            // Naive check. Default encoder escaping for JsonSerializer is aggressive
            // and escapes most non-English characters. It also tends to escape to
            // full unicode (e.g. ' => \u0027, “ => \u0028), dramatically misrepresenting
            // the actual character/byte count.
            string value = JsonSerializer.Serialize(newValue ?? "");
            CharacterCount = value[1..^1].Length;
        }
        else
            CharacterCount = newValue?.Length ?? 0;

        if (DoesCharacterCountExceedSuggestedMaxLength())
            SuggestedMaxLengthExceeded?.Invoke(this, new EventArgs());

        base.OnTextChanged(oldValue, newValue);
    }
}
