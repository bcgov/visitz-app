namespace VisitzModel.Storage;

public class SurveyFeedbackTracker(IPreferences prefs)
{
	private static readonly string FeedbackSurveyKey = "FeedbackSurvey.";
	private static readonly string SurveyPromptedKey = FeedbackSurveyKey + "SurveyPrompted";
	private static readonly string TimesAppUnlockedKey = FeedbackSurveyKey + "TimesAppUnlocked";
	private static readonly string HavePublishedAnythingKey = FeedbackSurveyKey + "HavePublishedAnything";
	private static readonly string LastOpenedSurveyUtcKey = FeedbackSurveyKey + "LastOpenedSurveyUtc";

	private static readonly int TimesAppUnlockedThreshold = 5;

	IPreferences Preferences { get; set; } = prefs;

	public bool SurveyPrompted
	{
		get => Preferences.Get(SurveyPromptedKey, false);
		private set => Preferences.Set(SurveyPromptedKey, value);
	}

	public int TimesAppUnlocked
	{
		get => Preferences.Get(TimesAppUnlockedKey, 0);
		private set => Preferences.Set(TimesAppUnlockedKey, value);
	}

	public bool UnlockedAppEnough => TimesAppUnlocked >= TimesAppUnlockedThreshold;

	public bool HavePublishedAnything
	{
		get => Preferences.Get(HavePublishedAnythingKey, false);
		private set => Preferences.Set(HavePublishedAnythingKey, value);
	}

	public DateTime LastOpenedSurveyUtc
	{
		get => Preferences.Get(LastOpenedSurveyUtcKey, DateTime.MinValue);
		private set => Preferences.Set(LastOpenedSurveyUtcKey, value);
	}

	public void SetHavePromptedSurvey()
	{
		SurveyPrompted = true;
		LastOpenedSurveyUtc = DateTime.UtcNow;
	}

	public void IncrementTimesAppUnlocked()
	{
		TimesAppUnlocked++;
	}

	public void SetHasPublishedAnything()
	{
		HavePublishedAnything = true;
	}

	public void ClearAll()
	{
		Preferences.Remove(SurveyPromptedKey);
		Preferences.Remove(TimesAppUnlockedKey);
		Preferences.Remove(HavePublishedAnythingKey);
		Preferences.Remove(LastOpenedSurveyUtcKey);
	}
}
