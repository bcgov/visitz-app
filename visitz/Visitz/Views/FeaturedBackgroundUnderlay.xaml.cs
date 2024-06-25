using Visitz.Storage;
using Visitz.Views.BaseClasses;

namespace Visitz.Views;

public partial class FeaturedBackgroundUnderlay : BaseContentView
{
	public enum DisplayOptions
	{
		Unknown = 0,
		Clear = 1,
		TextReadable = 2,
	}

	private static readonly int ImageCacheValidityDuration = 30;
	private static readonly double ClearOpacity = 0.0;
	private static readonly double TextReadableOpacity = 0.35;

	public static readonly BindableProperty ImageDisplayOptionsProperty =
		BindableProperty.Create(nameof(ImageDisplayOptions), typeof(DisplayOptions),
			typeof(FeaturedBackgroundUnderlay), DisplayOptions.Clear,
			propertyChanged: (boundObj, oldVal, newVal) =>
			{
				var featuredBg = (FeaturedBackgroundUnderlay)boundObj;
				var newDisplayOptions = (DisplayOptions)newVal;

				var opacity = newDisplayOptions.Equals(DisplayOptions.TextReadable)
					? TextReadableOpacity : ClearOpacity;

				featuredBg.OverlayBoxView.Opacity = opacity;
			});

	public DisplayOptions ImageDisplayOptions { get; set; }

	public FeaturedBackgroundUnderlay()
	{
		InitializeComponent();
	}

	protected override async void Creating()
	{
		base.Creating();

		FeatureImage.Source = new UriImageSource()
		{
			Uri = new Uri(await BcGovAlbum.GetFeaturedPictureUri()),
			CacheValidity = TimeSpan.FromDays(ImageCacheValidityDuration),
		};
	}
}
