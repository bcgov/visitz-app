using VisitzModel.Events;
using VisitzModel.Extensions;
using VisitzModel.Storage;
using VisitzModelTest.Mocks;

namespace VisitzModelTest.Storage;

public class LastUpdatedPrefsTests
{
    static readonly string ArbitraryKey = nameof(LastUpdatedPrefsTests) + ".testkey";

    static LastUpdatedPrefs GetLastUpdatedPrefsMock()
    {
        return new(new LocalPreferencesMock());
    }

    [Fact]
    public void DefaultValueReturnedWhenRequestedKeyMissing()
    {
        var luPrefs = GetLastUpdatedPrefsMock();
        var minDate = DateTime.MinValue;

        Assert.Equal(minDate, luPrefs.Get(ArbitraryKey, minDate));
    }

    [Fact]
    public void NullFromGetOverloadReturnedWhenRequestedKeyMissing()
    {
        var luPrefs = GetLastUpdatedPrefsMock();

        Assert.Null(luPrefs.Get(ArbitraryKey));
    }

    [Fact]
    public void ValueFromGetOverloadStoresAndRetrievesTheSame()
    {
        var luPrefs = GetLastUpdatedPrefsMock();
        var localNow = DateTimeExtensions.LocalNow;

        luPrefs.Set(ArbitraryKey, localNow);

        Assert.Equal(localNow, luPrefs.Get(ArbitraryKey));
    }

    [Fact]
    public void LocalTimeStoresAndRetrievesTheSame()
    {
        var luPrefs = GetLastUpdatedPrefsMock();
        var localNow = DateTimeExtensions.LocalNow;

        luPrefs.Set(ArbitraryKey, localNow);

        Assert.Equal(localNow, luPrefs.Get(ArbitraryKey, DateTime.MinValue));
    }

    [Fact]
    public void NotifiedWhenValueSet()
    {
        var luPrefs = GetLastUpdatedPrefsMock();
        var localNow = DateTimeExtensions.LocalNow;

        Assert.Raises<LastUpdatedChangedEventArgs>(
            handler => luPrefs.LastUpdatedChanged += handler,
            handler => luPrefs.LastUpdatedChanged -= handler,
            () => luPrefs.Set(ArbitraryKey, localNow)
        );
    }

    [Fact]
    public void ValueIsCorrectWhenNotified()
    {
        var luPrefs = GetLastUpdatedPrefsMock();
        var localNow = DateTimeExtensions.LocalNow;
        DateTime valueAfterEvent = DateTime.MinValue;

        luPrefs.LastUpdatedChanged += (sender, args) =>
        {
            valueAfterEvent = args.NewLastUpdatedValue;
        };

        luPrefs.Set(ArbitraryKey, localNow);

        Assert.Equal(localNow, valueAfterEvent);
    }

    [Fact]
    public void ValueIsRetrievableFromEventArgs()
    {
        var luPrefs = GetLastUpdatedPrefsMock();
        var localNow = DateTimeExtensions.LocalNow;
        DateTime? valueAfterEvent = null;

        luPrefs.LastUpdatedChanged += (sender, args) =>
        {
            if (sender is LastUpdatedPrefs prefs)
                valueAfterEvent = prefs.Get(args.Id);
        };

        luPrefs.Set(ArbitraryKey, localNow);

        Assert.Equal(localNow, valueAfterEvent);
    }
}
