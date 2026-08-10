using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.TagViews;

public class EntityTypeBadge : TagView
{
    public static readonly BindableProperty EntityTypeProperty = BindableProperty.Create(
        nameof(EntityType),
        typeof(EntityType),
        typeof(EntityTypeBadge),
        propertyChanged: (bound, _, _) =>
        {
            if (bound is EntityTypeBadge badge)
                badge.ApplyEntityTypes();
        }
    );

    public static readonly BindableProperty EntitySubtypeProperty = BindableProperty.Create(
        nameof(EntitySubtype),
        typeof(EntitySubtype),
        typeof(EntityTypeBadge),
        propertyChanged: (bound, _, _) =>
        {
            if (bound is EntityTypeBadge badge)
                badge.ApplyEntityTypes();
        }
    );

    public EntityType EntityType
    {
        get => (EntityType)GetValue(EntityTypeProperty);
        set => SetValue(EntityTypeProperty, value);
    }

    public EntitySubtype EntitySubtype
    {
        get => (EntitySubtype)GetValue(EntitySubtypeProperty);
        set => SetValue(EntitySubtypeProperty, value);
    }

    const double DefaultCornerRadius = 4;
    const double DefaultStrokeThickness = 0;
    readonly Thickness DefaultPadding = new(5, 0);

    public EntityTypeBadge()
    {
        CornerRadius = DefaultCornerRadius;
        StrokeThickness = DefaultStrokeThickness;
        Padding = DefaultPadding;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is IBusinessObject item)
        {
            EntityType = item.EntityType;
            EntitySubtype = item.EntitySubtype;
        }
    }

    void ApplyEntityTypes()
    {
        BackgroundColor = EntityType.GetBackgroundColor();
        TextColor = EntityType.GetTextColor();
        Text = EntitySubtype.GetDisplayInitials();
    }
}
