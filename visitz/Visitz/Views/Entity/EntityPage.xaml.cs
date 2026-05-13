using Microsoft.Extensions.Logging;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity;

#nullable enable

public partial class EntityPage : VisitzPage<EntityPage, EntityPageViewModel>, IIcmRecordInfo
{
    Task<EntityView>? _createEntityView;

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
}
