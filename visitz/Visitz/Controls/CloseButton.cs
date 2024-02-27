#if IOS
using Microsoft.Maui.Controls.PlatformConfiguration;
#endif

using Visitz.FontIcons;

namespace Visitz.Controls;

internal class CloseButton : ImageButton
{
	public static readonly BindableProperty FontIconColorProperty =
		BindableProperty.Create(nameof(FontIconColor), typeof(Color), typeof(CloseButton),
			propertyChanged: (boundObj, oldVal, newVal) =>
			{
				var closeButton = (CloseButton)boundObj;
				var newColor = (Color)newVal;

				if (closeButton.Source is FontImageSource fis)
					fis.Color = newColor;
			});

	public Color FontIconColor
	{
		get => (Color)GetValue(FontIconColorProperty);
		set => SetValue(FontIconColorProperty, value);
	}

	public CloseButton() : base()
	{
		WidthRequest = 44;
		HeightRequest = 44;

		Source = MaterialIcons.Close.GetFilledMaterialIcon(Colors.White);
#if IOS
		(On<iOS>().Element.Source as FontImageSource).Size = WidthRequest * 2;
#endif

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
