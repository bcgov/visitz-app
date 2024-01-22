namespace Visitz.Views.Entity;

public partial class ChildInOutCareItem : ContentView
{
	public ChildInOutCareItem()
	{
		InitializeComponent();
        PropertyChanged += ChildInOutCareItem_PropertyChanged;
	}

    private void ChildInOutCareItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        ConsoleTrace.TraceMethod(this, $"property: '{e.PropertyName}'");
    }
}