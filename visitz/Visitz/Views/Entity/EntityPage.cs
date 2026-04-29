using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Shimmer;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity;

public partial class EntityPage(ILogger<EntityPage> logger) : VisitzPage<EntityPage, VisitzViewModel>(new(), logger)
{
    Task<EntityView> _createEntityView;

    public void Init(string rowId, EntityType type, EntitySection? section = null, IDraftItem? draft = null)
    {
        _createEntityView = MakeEntityViewAsync(rowId, type, section, draft);
        ControlTemplate = new(() =>
        {
            return new SfShimmer()
            {
                Type = ShimmerType.Shopping,
                WidthRequest = 300,
                HeightRequest = 300,
            };
        });
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

    protected override async Task OnCreatedAsync()
    {
        await base.OnCreatedAsync();

        View view = await _createEntityView;

        ControlTemplate = null;
        Content = view;
    }
}
