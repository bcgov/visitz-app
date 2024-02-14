using Realms;
using VisitzModel.Storage;

namespace Visitz.Storage;

internal static class VisitzRealms
{
    private static async Task<byte[]> GetKey(string name) => 
        await VisitzKey.GetKey(VisitzRealmBase.GetRealmPath(name));

    public static async Task<IcmData> GetIcmDataAsync() => 
        new IcmData(await GetKey(IcmData.Name));

    public static async Task<NoteDrafts> GetNoteDraftsAsync() =>
        new NoteDrafts(await GetKey(NoteDrafts.Name));

    public static async Task<SafetyAssessmentDrafts> GetSafetyAssessmentDraftAsync() =>
        new SafetyAssessmentDrafts(await GetKey(SafetyAssessmentDrafts.Name));

    public static async Task<Realm> GetNoteDraftsRealmAsync() =>
        await (await GetNoteDraftsAsync()).GetAsync();

    public static async Task<Realm> GetIcmDataRealmAsync() =>
        await (await GetIcmDataAsync()).GetAsync();

    public static async Task<Realm> GetSafetyAssessmentDraftRealmAsync() =>
        await (await GetSafetyAssessmentDraftAsync()).GetAsync();
}
