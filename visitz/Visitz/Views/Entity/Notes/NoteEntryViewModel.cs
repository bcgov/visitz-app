using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Oidc;
using Oidc.Network;
using Realms;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.BaseClasses.Publishing;
using VisitzApi.Models;
using VisitzModel.Events;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Notes;

#nullable enable

namespace Visitz.Views.Entity.Notes;

public partial class NoteEntryViewModel : IcmRecordViewModel
{
    private static readonly int CharacterLimit = 16000;
    public static readonly string RemainingCharactersString = "{0}/" + CharacterLimit;

    [ObservableProperty]
    public partial NoteDraft NoteDraft { get; set; } = new();

    private string DraftOutput => NoteDraft.Draft?.Trim() ?? string.Empty;

    private bool _disposed;

    [ObservableProperty]
    public partial bool AllowPublish { get; set; }

    [ObservableProperty]
    public partial bool AllowDiscard { get; set; }

    [ObservableProperty]
    public partial int RemainingCharacters { get; set; } = CharacterLimit;

    [ObservableProperty]
    public partial bool InternetAvailable { get; set; } = NetworkHelper.InternetAvailable;

    public event EventHandler<DraftErrorEventArgs>? DraftError;

    public DraftSaveStateHandler SaveStateHandler { get; } = new();

    Realm? DraftRealm { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;

        DraftRealm = await VisitzRealms.GetNoteDraftsRealmAsync();
        NoteDraft = NoteDraft.FindByEntityId(DraftRealm, BusinessObject.FileNumber) ?? CreateNoteDraft();

        SaveStateHandler.Clear();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            Connectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;

            SaveStateHandler.Dispose();

            DraftRealm?.Dispose();
            DraftRealm = null;

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private NoteDraft CreateNoteDraft()
    {
        return new NoteDraft() { ParentEntityId = NoteDraft.MakeId(BusinessObject.FileNumber) };
    }

    [RelayCommand]
    public async Task PublishNotes()
    {
        if (UpdateAllowPublish())
        {
            var notePublishVm = ServiceProvider.GetService<NotePublishViewModel>();
            var logger = ServiceProvider.GetService<ILogger<PublishPage>>();

#pragma warning disable SS002 // DateTime.Now was referenced
            var now = DateTime.Now; // API system does not use UTC times
#pragma warning restore SS002 // DateTime.Now was referenced

            var info = await OidcSessionInfo.GetAsync();
            var submitNoteEntity = new SubmitNoteEntity
            {
                EntityNumber = BusinessObject.FileNumber,
                EntityType = BusinessObject.EntityType.GetDisplayString(),
                NotePeriod = NoteItem.NotePeriodFrom(now),
                Content = NoteItem.WrapContent(info.Idir, now, DraftOutput),
            };

            notePublishVm.Init(BusinessObject, submitNoteEntity);

            await Navigator.Navigation.PopModalAsync();
            await Navigator.Navigation.PushAsync(new PublishPage(notePublishVm, logger));
        }
    }

    public async Task EditorTextChanged(TextChangedEventArgs e)
    {
        if (string.Equals(e.OldTextValue, e.NewTextValue))
            // Early return required to prevent infinite loops due to "cancelling" events
            // by reassigning its previous value
            return;

        if (DraftRealm == null)
            return;

        SetDraftInfo();

        int length = e.NewTextValue?.Length ?? 0;

        if (length > 0 && !NoteDraft.IsManaged)
            DraftRealm.Write(() => DraftRealm.Add(NoteDraft));

        if (ContainEmojis(e))
        {
            CancelTextChangedEvent(e);
            DraftError?.Invoke(this, new DraftErrorEventArgs(LocalizedStrings.InvalidEntry));
            return;
        }
        else if (ExceedsCharacterLimit(e))
        {
            CancelTextChangedEvent(e);
            DraftError?.Invoke(this, new DraftErrorEventArgs(LocalizedStrings.CharacterLimitReached));
            return;
        }

        RemainingCharacters = CharacterLimit - length;
        AllowDiscard = NoteDraft.IsManaged;
        UpdateAllowPublish(e.NewTextValue);

        if (NoteDraft.IsManaged)
            await SaveStateHandler.Saving();
        else
            SaveStateHandler.Clear();
    }

    private static bool ExceedsCharacterLimit(TextChangedEventArgs e)
    {
        return e.NewTextValue?.Length > CharacterLimit;
    }

    private static bool ContainEmojis(TextChangedEventArgs e)
    {
        return e.NewTextValue?.ContainsUnicodeSurrogatesAndOtherSymbols() ?? false;
    }

    private void SetDraftInfo()
    {
        if (string.IsNullOrWhiteSpace(NoteDraft.DraftLocationBinding))
            NoteDraft.DraftLocationBinding = BusinessObject.DisplayName;

        if (NoteDraft.RelatedEntityTypeBinding == EntityType.Unknown)
            NoteDraft.RelatedEntityTypeBinding = BusinessObject.EntityType;

        if (NoteDraft.RelatedEntitySubtypeBinding == EntitySubtype.Unknown)
            NoteDraft.RelatedEntitySubtypeBinding = BusinessObject.EntitySubtype;
    }

    private void CancelTextChangedEvent(TextChangedEventArgs e)
    {
        NoteDraft?.DraftBinding = e.OldTextValue;
    }

    partial void OnInternetAvailableChanged(bool value)
    {
        UpdateAllowPublish();
    }

    private bool UpdateAllowPublish(string? draftText = null)
    {
        draftText ??= DraftOutput;
        AllowPublish = InternetAvailable && draftText?.Length > 0;
        return AllowPublish;
    }

    private void Current_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        InternetAvailable = NetworkHelper.InternetAvailable;
    }

    public async Task ResetDraftAsync()
    {
        if (NoteDraft == null || !NoteDraft.IsManaged || DraftRealm == null)
            return;

        await DraftRealm.WriteAsync(() => DraftRealm.Remove(NoteDraft));
        NoteDraft = CreateNoteDraft();
    }
}
