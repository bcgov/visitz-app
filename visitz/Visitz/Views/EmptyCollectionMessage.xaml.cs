namespace Visitz.Views;

public partial class EmptyCollectionMessage : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(EmptyCollectionMessage)
    );

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public EmptyCollectionMessage()
    {
        InitializeComponent();
    }
}
