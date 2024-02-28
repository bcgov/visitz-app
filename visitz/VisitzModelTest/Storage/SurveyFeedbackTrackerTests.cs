using VisitzModel.Storage;

namespace VisitzModelTest.Storage;

public class SurveyFeedbackTrackerTests
{
	private static SurveyFeedbackTracker MakeNewTracker()
	{
		return new(new LocalPreferencesMock());
	}

	[Fact]
	public void SurveyPrompted_IsFalse()
	{
		var tracker = MakeNewTracker();

		Assert.False(tracker.SurveyPrompted);
	}

	[Fact]
	public void SurveyPrompted_IsTrue()
	{
		var tracker = MakeNewTracker();

		tracker.SetHavePromptedSurvey();

		Assert.True(tracker.SurveyPrompted);
	}

	[Fact]
	public void LastOpenedSurveyUtc_IsDateEqual()
	{
		var tracker = MakeNewTracker();

		var expectedNow = DateTime.UtcNow;
		tracker.SetHavePromptedSurvey();

		Assert.Equal(expectedNow.Date, tracker.LastOpenedSurveyUtc.Date);
	}

	[Fact]
	public void TimesAppUnlocked_IsZero()
	{
		var tracker = MakeNewTracker();

		const int ZeroUnlocks = 0;

		Assert.Equal(ZeroUnlocks, tracker.TimesAppUnlocked);
	}

	[Fact]
	public void TimesAppUnlocked_IsFive()
	{
		var tracker = MakeNewTracker();

		const int FiveUnlocks = 5;

		for (int i = 0;  i < FiveUnlocks; i++)
			tracker.IncrementTimesAppUnlocked();

		Assert.Equal(FiveUnlocks, tracker.TimesAppUnlocked);
	}

	[Fact]
	public void UnlockedAppEnough_IsFalse()
	{
		var tracker = MakeNewTracker();

		Assert.False(tracker.UnlockedAppEnough);
	}

	[Fact]
	public void UnlockedAppEnough_IsTrue()
	{
		var tracker = MakeNewTracker();

		const int FiveUnlocks = 5;

		for (int i = 0; i < FiveUnlocks; i++)
			tracker.IncrementTimesAppUnlocked();

		Assert.True(tracker.UnlockedAppEnough);
	}

	[Fact]
	public void PublishedAnything_IsFalse()
	{
		var tracker = MakeNewTracker();

		Assert.False(tracker.PublishedAnything);
	}

	[Fact]
	public void PublishedAnything_IsTrue()
	{
		var tracker = MakeNewTracker();

		tracker.SetHasPublishedAnything();

		Assert.True(tracker.PublishedAnything);
	}

	[Fact]
	public void SurveyPromptedAndUnlockedAppEnoughAndPublishedAnythingAfterClearAll_AllFalse()
	{
		var tracker = MakeNewTracker();

		const int FiveUnlocks = 5;

		for (int i = 0; i < FiveUnlocks; i++)
			tracker.IncrementTimesAppUnlocked();

		tracker.SetHavePromptedSurvey();
		tracker.SetHasPublishedAnything();

		tracker.ClearAll();

		Assert.False(tracker.SurveyPrompted || tracker.UnlockedAppEnough || tracker.PublishedAnything);
	}
}

internal class LocalPreferencesMock : IPreferences
{
	private Dictionary<string, object> Pairs { get; } = [];

	public void Clear(string? sharedName = null)
	{
		Pairs.Clear();
	}

	public bool ContainsKey(string key, string? sharedName = null)
	{
		return Pairs.ContainsKey(key);
	}

	public T Get<T>(string key, T defaultValue, string? sharedName = null)
	{
		return ContainsKey(key) ? (T)Pairs[key] : defaultValue;
	}

	public void Remove(string key, string? sharedName = null)
	{
		Pairs.Remove(key);
	}

	public void Set<T>(string key, T value, string? sharedName = null)
	{
#pragma warning disable CS8601 // Possible null reference assignment.
		Pairs[key] = value;
#pragma warning restore CS8601 // Possible null reference assignment.
	}
}
