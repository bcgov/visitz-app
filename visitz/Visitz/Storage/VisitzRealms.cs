using Microsoft.Extensions.Logging;
using Realms;
using VisitzModel.Storage;
using VisitzModel.Utilities;

namespace Visitz.Storage;

internal static class VisitzRealms
{
    private static readonly EagerActionQueue icmDataQueue = new();

    private static async Task<byte[]> GetKey(string name) =>
        await VisitzKey.GetKey(name) ?? throw new InvalidOperationException($"Key for '{name}' missing");

    public static async Task<IcmData> GetIcmDataAsync() => new IcmData(await GetKey(IcmData.Name));

    public static async Task<NoteDrafts> GetNoteDraftsAsync() => new NoteDrafts(await GetKey(NoteDrafts.Name));

    public static async Task<SafetyAssessmentDrafts> GetSafetyAssessmentDraftAsync() =>
        new SafetyAssessmentDrafts(await GetKey(SafetyAssessmentDrafts.Name));

    public static async Task<AttachmentDrafts> GetAttachmentDraftsAsync() =>
        new AttachmentDrafts(await GetKey(AttachmentDrafts.Name));

    public static async Task<LogRealm> GetLogAsync() => new LogRealm(await GetKey(LogRealm.Name));

    public static async Task<PersonVisitDrafts> GetPersonVisitDraftsAsync() =>
        new PersonVisitDrafts(await GetKey(PersonVisitDrafts.Name));

    public static async Task<Realm> GetNoteDraftsRealmAsync() =>
        await (await GetNoteDraftsAsync()).GetAsync(ServiceProvider.GetService<ILogger<NoteDrafts>>());

    public static async Task<Realm> GetIcmDataRealmAsync() =>
        await (await GetIcmDataAsync()).GetAsync(ServiceProvider.GetService<ILogger<IcmData>>());

    public static async Task<Realm> GetSafetyAssessmentDraftRealmAsync() =>
        await (await GetSafetyAssessmentDraftAsync()).GetAsync(
            ServiceProvider.GetService<ILogger<SafetyAssessmentDrafts>>()
        );

    public static async Task<Realm> GetAttachmentDraftsRealmAsync() =>
        await (await GetAttachmentDraftsAsync()).GetAsync(ServiceProvider.GetService<ILogger<AttachmentDrafts>>());

    public static async Task<Realm> GetLogRealmAsync() =>
        await (await GetLogAsync()).GetAsync(ServiceProvider.GetService<ILogger<LogRealm>>());

    public static async Task<Realm> GetPersonVisitDraftsRealmAsync() =>
        await (await GetPersonVisitDraftsAsync()).GetAsync(ServiceProvider.GetService<ILogger<PersonVisitDrafts>>());

    public static Task EnqueueIcmDataActionAsync(Func<Realm, Task> task)
    {
        return icmDataQueue.EnqueueAsync(async () =>
        {
            using var realm = await GetIcmDataRealmAsync();
            await task(realm);
        });
    }
}
