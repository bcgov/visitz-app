using Visitz.FontIcons;
using Visitz.Resources.Styles;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Extensions;

public static class EntityTypeUiExtensions
{
    public static string GetIconGlyph(this EntityType entityType)
    {
        return entityType switch
        {
            EntityType.Unknown => MaterialIcons.Question_mark,
            EntityType.Case => MaterialIcons.Folder,
            EntityType.Incident => MaterialIcons.Warning,
            EntityType.Memo => MaterialIcons.Note_alt,
            EntityType.ServiceRequest => MaterialIcons.Headset_mic,
            _ => throw new NotImplementedException(),
        };
    }

    public static Color GetTextColor(this EntityType entityType)
    {
        return entityType switch
        {
            EntityType.Unknown => VisitzColors.EntityUnknownTypeTagText,
            EntityType.Case => VisitzColors.EntityCaseTagText,
            EntityType.Incident => VisitzColors.EntityIncidentTagText,
            EntityType.Memo => VisitzColors.EntityMemoTagText,
            EntityType.ServiceRequest => VisitzColors.EntityServiceRequestTagText,
            _ => throw new NotImplementedException(),
        };
    }

    public static Color GetBackgroundColor(this EntityType entityType)
    {
        return entityType switch
        {
            EntityType.Unknown => VisitzColors.EntityUnknownTypeBackground,
            EntityType.Case => VisitzColors.EntityCaseTagBackground,
            EntityType.Incident => VisitzColors.EntityIncidentTagBackground,
            EntityType.Memo => VisitzColors.EntityMemoTagBackground,
            EntityType.ServiceRequest => VisitzColors.EntityServiceRequestTagBackground,
            _ => throw new NotImplementedException(),
        };
    }

    public static FontImageSource GetIcon(this EntityType entityType)
    {
        if (entityType > EntityType.ServiceRequest)
            throw new NotImplementedException();

        return new FontImageSource()
        {
            Glyph = entityType.GetIconGlyph(),
            FontFamily = MaterialIcons.RoundedFilled.FontFamily,
            Color = entityType.GetTextColor(),
        };
    }
}
