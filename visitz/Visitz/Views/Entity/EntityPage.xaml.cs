using Microsoft.Extensions.Logging;
using Visitz.Animations;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Interfaces;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity;

#nullable enable

public partial class EntityPage : VisitzPage<EntityPage, EntityPageViewModel>, IIcmRecordInfo, ISnackbarPresenter
{
    Task<EntityView>? _createEntityView;

    VisitzSnackbar? Snackbar { get; set; }

    public string RowId
    {
        get => ViewModel.RowId;
        set => ViewModel.RowId = value;
    }

    public EntityType EntityType
    {
        get => ViewModel.EntityType;
        set => ViewModel.EntityType = value;
    }

    public EntityPage(EntityPageViewModel viewModel, ILogger<EntityPage> logger)
        : base(viewModel, logger)
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    public void Init(
        string rowId,
        EntityType type,
        string displayName,
        string fileNumber,
        EntitySection? section = null,
        IDraftItem? draft = null
    )
    {
        RowId = rowId;
        EntityType = type;
        ViewModel.DisplayName = displayName;
        ViewModel.FileNumber = fileNumber;

        _createEntityView = MakeEntityViewAsync(rowId, type, section, draft);
    }

    static async Task<EntityView> MakeEntityViewAsync(
        string rowId,
        EntityType type,
        EntitySection? section,
        IDraftItem? draft
    )
    {
        var entity = ServiceProvider.GetService<EntityView>();

        entity.RowId = rowId;
        entity.EntityType = type;
        entity.ViewModel.RequestedSection = section;
        entity.ViewModel.FocusedDraftItem = draft;

        await entity.StartInitAsync();

        return entity;
    }

    protected override async Task OnFirstLoadAsync()
    {
        await base.OnFirstLoadAsync();

        if (_createEntityView == null)
        {
            Logger.LogError($"{_createEntityView} was null");
            return;
        }

        View view = await _createEntityView;
        view.Opacity = 0.0d;
        view.Loaded += EntityView_Loaded;

        MainContent.Add(view);
    }

    private async void EntityView_Loaded(object? sender, EventArgs e)
    {
        if (sender is not View view)
            return;

        await Task.WhenAll(
            Shimmer.FadeToAsync(0.0d, easing: Easing.Linear),
            view.FadeToAsync(1.0d, easing: Easing.Linear)
        );

        MainContent.Remove(Shimmer);
        view.Loaded -= EntityView_Loaded;
    }

    public void SetSnackbar(VisitzSnackbar? snackbar)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Snackbar?.ShouldClose -= Snackbar_ShouldClose;

            Snackbar = snackbar;
            SnackbarContainer.Content = Snackbar;
            SnackbarContainer.IsVisible = Snackbar != null;

            if (Snackbar != null)
            {
                Snackbar.ShouldClose += Snackbar_ShouldClose;
                _ = new VisibilityAnimation(showView: true, 150).Animate(Snackbar);
            }
        });
    }

    public void Snackbar_ShouldClose(object? sender, EventArgs e)
    {
        _ = AnimateCloseSnackbar();
    }

    private async Task AnimateCloseSnackbar()
    {
        if (Snackbar != null)
            await new VisibilityAnimation(showView: false, 150).Animate(Snackbar);

        SetSnackbar(null);
    }
}
