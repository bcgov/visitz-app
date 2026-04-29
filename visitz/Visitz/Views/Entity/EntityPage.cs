using Microsoft.Extensions.Logging;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity;

public partial class EntityPage : VisitzPage<EntityPage, VisitzViewModel>
{
    public EntityView EntityView => (EntityView)base.Content;

    public EntityPage(ILogger<EntityPage> logger, EntityView entityView)
        : base(new(), logger)
    {
        Content = entityView;
    }
}
