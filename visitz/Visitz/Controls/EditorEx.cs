using VisitzModel.Extensions;

namespace Visitz.Controls;

internal partial class EditorEx : Editor
{
    public static readonly BindableProperty CharacterCountProperty =
        BindableProperty.Create(nameof(CharacterCount), typeof(int), typeof(EditorEx),
            defaultBindingMode: BindingMode.OneWayToSource);

    public static readonly BindableProperty SuggestedMaxLengthProperty =
        BindableProperty.Create(nameof(SuggestedMaxLength), typeof(int), typeof(EditorEx));

    public int CharacterCount
    {
        get => (int)GetValue(CharacterCountProperty);
        set => SetValue(CharacterCountProperty, value);
    }

    public int SuggestedMaxLength
    {
        get => (int)GetValue(SuggestedMaxLengthProperty);
        set => SetValue(SuggestedMaxLengthProperty, value);
    }

    public event EventHandler EmojiEntered;

    public event EventHandler SuggestedMaxLengthExceeded;

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
        CharacterCount = newValue?.Length ?? 0;

        if (DoesCharacterCountExceedSuggestedMaxLength())
            SuggestedMaxLengthExceeded?.Invoke(this, new EventArgs());

        base.OnTextChanged(oldValue, newValue);
    }
}
