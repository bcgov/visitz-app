using Visitz.FontIcons;

namespace Visitz.Controls;

internal class CloseButton : ImageButton
{
	public CloseButton() : base()
	{
		WidthRequest = 44;
		HeightRequest = 44;

		Source = MaterialIcons.Close.GetFilledMaterialIcon(Colors.White);

		Clicked += CloseButton_Clicked;
	}

	private async void CloseButton_Clicked(object sender, EventArgs e)
	{
		if (Navigator.CurrentOpenModal != null)
			await Navigator.Navigation.PopModalAsync();
		else
			await Navigator.Navigation.PopAsync();
	}
}
