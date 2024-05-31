using Visitz.Resources.Localization;
using VisitzModel.Extensions;
using VisitzModel.Formats;
using VisitzModel.Utilities;

namespace Visitz.Views;

public partial class LastUpdatedBanner : ContentView
{
	public static readonly BindableProperty LastUpdatedProperty =
		BindableProperty.Create(nameof(LastUpdated), typeof(DateTime?), typeof(LastUpdatedBanner),
			propertyChanged: SetUpdatedText);

	public static readonly BindableProperty FallbackTextProperty =
		BindableProperty.Create(nameof(FallbackText), typeof(string), typeof(LastUpdatedBanner),
			propertyChanged: SetUpdatedText, defaultValue: LocalizedStrings.NA);

	private static void SetUpdatedText(object boundObj, object _, object newVal)
	{
		var thiz = (LastUpdatedBanner)boundObj;

		thiz.SetLastUpdated(newVal as DateTime?);
	}

	public DateTime? LastUpdated
	{
		get => (DateTime?)GetValue(LastUpdatedProperty);
		set => SetValue(LastUpdatedProperty, value);
	}

	public string FallbackText
	{
		get => (string)GetValue(FallbackTextProperty);
		set => SetValue(FallbackTextProperty, value);
	}

	public LastUpdatedBanner()
	{
		InitializeComponent();
		SetLastUpdated(null);
	}

	private void SetLastUpdated(DateTime? lastUpdated)
	{
		LastUpdatedLabel.Text = lastUpdated is DateTime last
			? last.ToString(IcmDateFormats.BasicTimestamp)
			: LastUpdatedLabel.Text = FallbackText;
	}
}
