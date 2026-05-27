/*
    THIS FILE IS NOT AUTO-GENERATED.

    But it should be.

    TODO: Implement a Source Generator to generate this file from Colors.xaml.
 */

using Visitz.Extensions;

namespace Visitz.Resources.Styles;

public static class VisitzColors
{
    public static Color TryGetColor(string name)
    {
        return Application.Current.Resources.TryGetColor(name, null)
            ?? throw new InvalidOperationException($"Color '{name}' not found in resources");
    }

    public static readonly Color BC_Blue = TryGetColor(nameof(BC_Blue));
    public static readonly Color BC_Gold = TryGetColor(nameof(BC_Gold));
    public static readonly Color BC_TextColor = TryGetColor(nameof(BC_TextColor));
    public static readonly Color BC_TextColor_Lighter = TryGetColor(nameof(BC_TextColor_Lighter));
    public static readonly Color BC_Hyperlink = TryGetColor(nameof(BC_Hyperlink));
    public static readonly Color BC_Background_Dark = TryGetColor(nameof(BC_Background_Dark));
    public static readonly Color BC_Background_Light = TryGetColor(nameof(BC_Background_Light));
    public static readonly Color BC_InputControlsTextColor = TryGetColor(nameof(BC_InputControlsTextColor));
    public static readonly Color BC_Semantic_Error = TryGetColor(nameof(BC_Semantic_Error));
    public static readonly Color BC_Semantic_Success = TryGetColor(nameof(BC_Semantic_Success));
    public static readonly Color BC_Semantic_Info = TryGetColor(nameof(BC_Semantic_Info));
    public static readonly Color BC_Semantic_Warning = TryGetColor(nameof(BC_Semantic_Warning));
    public static readonly Color EmptyIconView_Color = TryGetColor(nameof(EmptyIconView_Color));

    public static readonly Color Default_Background = TryGetColor(nameof(Default_Background));

    public static readonly Color Semantic_Warning_LargeText = TryGetColor(nameof(Semantic_Warning_LargeText));

    // Primary uses color 'BC_Blue'
    public static readonly Color Primary = TryGetColor(nameof(Primary));

    // Secondary uses color 'BC_Background_Light'
    public static readonly Color Secondary = TryGetColor(nameof(Secondary));

    // Tertiary uses color 'BC_Background_Dark'
    public static readonly Color Tertiary = TryGetColor(nameof(Tertiary));

    // Production uses color 'Primary'
    public static readonly Color ProductionBuildColor = TryGetColor(nameof(ProductionBuildColor));

    // Other build colors arbitrarily chosen
    public static readonly Color BetaBuildColor = TryGetColor(nameof(BetaBuildColor));
    public static readonly Color TeamBuildColor = TryGetColor(nameof(TeamBuildColor));
    public static readonly Color DeveloperBuildColor = TryGetColor(nameof(DeveloperBuildColor));

    // BuildBarBackgroundColor uses one of the <build> colors
    public static readonly Color BuildBarBackgroundColor = TryGetColor(nameof(BuildBarBackgroundColor));

    public static readonly Color ItemSelectedColor = TryGetColor(nameof(ItemSelectedColor));

    public static readonly Color EntityNavBackgroundColor = TryGetColor(nameof(EntityNavBackgroundColor));
    public static readonly Color EntityNavSelectedColor = TryGetColor(nameof(EntityNavSelectedColor));

    // Tags Colors
    public static readonly Color EntityCaseTagBackground = TryGetColor(nameof(EntityCaseTagBackground));
    public static readonly Color EntityCaseTagText = TryGetColor(nameof(EntityCaseTagText));
    public static readonly Color EntityIncidentTagBackground = TryGetColor(nameof(EntityIncidentTagBackground));
    public static readonly Color EntityIncidentTagText = TryGetColor(nameof(EntityIncidentTagText));

    public static readonly Color ContactRelationshipTagBackground = TryGetColor(
        nameof(ContactRelationshipTagBackground)
    );
    public static readonly Color ContactRelationshipTagText = TryGetColor(nameof(ContactRelationshipTagText));

    public static readonly Color EntityMemoTagBackground = TryGetColor(nameof(EntityMemoTagBackground));
    public static readonly Color EntityMemoTagText = TryGetColor(nameof(EntityMemoTagText));

    public static readonly Color EntityServiceRequestTagBackground = TryGetColor(
        nameof(EntityServiceRequestTagBackground)
    );
    public static readonly Color EntityServiceRequestTagText = TryGetColor(nameof(EntityServiceRequestTagText));

    public static readonly Color PurpleTagBorder = TryGetColor(nameof(PurpleTagBorder));
    public static readonly Color FamilyTagColor = TryGetColor(nameof(FamilyTagColor));

    public static readonly Color EntitySubTypeBackground = TryGetColor(nameof(EntitySubTypeBackground));
    public static readonly Color EntitySubTypeTagTextBackground = TryGetColor(nameof(EntitySubTypeTagTextBackground));

    public static readonly Color EntityUnknownTypeBackground = TryGetColor(nameof(EntityUnknownTypeBackground));
    public static readonly Color EntityUnknownTypeTagText = TryGetColor(nameof(EntityUnknownTypeTagText));

    // Family Information Colors
    public static readonly Color KeyPlayerInfoPurpleBackground = TryGetColor(nameof(KeyPlayerInfoPurpleBackground));
    public static readonly Color FamilyMemberInfoGrayBorder = TryGetColor(nameof(FamilyMemberInfoGrayBorder));

    public static readonly Color LightGrayText = TryGetColor(nameof(LightGrayText));
    public static readonly Color DarkSkyBlueBackground = TryGetColor(nameof(DarkSkyBlueBackground));
    public static readonly Color DarkSkyBlueBackgroundPointerOver = TryGetColor(
        nameof(DarkSkyBlueBackgroundPointerOver)
    );
    public static readonly Color DarkSkyBlueBackgroundPressed = TryGetColor(nameof(DarkSkyBlueBackgroundPressed));
    public static readonly Color BlackishText = TryGetColor(nameof(BlackishText));
    public static readonly Color LightGrayBackground = TryGetColor(nameof(LightGrayBackground));
    public static readonly Color SkyBlueText = TryGetColor(nameof(SkyBlueText));

    public static readonly Color AlertBannerInfoPrimary = TryGetColor(nameof(AlertBannerInfoPrimary));
    public static readonly Color AlertBannerInfoBackground = TryGetColor(nameof(AlertBannerInfoBackground));
    public static readonly Color AlertBannerWarningPrimary = TryGetColor(nameof(AlertBannerWarningPrimary));
    public static readonly Color AlertBannerWarningBackground = TryGetColor(nameof(AlertBannerWarningBackground));
    public static readonly Color AlertBannerDangerPrimary = TryGetColor(nameof(AlertBannerDangerPrimary));
    public static readonly Color AlertBannerDangerBackground = TryGetColor(nameof(AlertBannerDangerBackground));
    public static readonly Color AlertBannerCriticalPrimary = TryGetColor(nameof(AlertBannerCriticalPrimary));
    public static readonly Color AlertBannerCriticalBackground = TryGetColor(nameof(AlertBannerCriticalBackground));

    public static readonly Color SeparatorColor = TryGetColor(nameof(SeparatorColor));

    public static readonly Color CardGrayBg = TryGetColor(nameof(CardGrayBg));
    public static readonly Color Gray100 = TryGetColor(nameof(Gray100));
    public static readonly Color Gray200 = TryGetColor(nameof(Gray200));
    public static readonly Color Gray300 = TryGetColor(nameof(Gray300));
    public static readonly Color Gray400 = TryGetColor(nameof(Gray400));
    public static readonly Color Gray500 = TryGetColor(nameof(Gray500));
    public static readonly Color Gray600 = TryGetColor(nameof(Gray600));
    public static readonly Color Gray900 = TryGetColor(nameof(Gray900));
    public static readonly Color Gray950 = TryGetColor(nameof(Gray950));

    public static readonly Color Yellow100Accent = TryGetColor(nameof(Yellow100Accent));
    public static readonly Color Yellow200Accent = TryGetColor(nameof(Yellow200Accent));
    public static readonly Color Yellow300Accent = TryGetColor(nameof(Yellow300Accent));
    public static readonly Color Cyan100Accent = TryGetColor(nameof(Cyan100Accent));
    public static readonly Color Cyan200Accent = TryGetColor(nameof(Cyan200Accent));
    public static readonly Color Cyan300Accent = TryGetColor(nameof(Cyan300Accent));
    public static readonly Color Blue100Accent = TryGetColor(nameof(Blue100Accent));
    public static readonly Color Blue200Accent = TryGetColor(nameof(Blue200Accent));
    public static readonly Color Blue300Accent = TryGetColor(nameof(Blue300Accent));

    public static readonly Color UnpublishedDraftBackground = TryGetColor(nameof(UnpublishedDraftBackground));
    public static readonly Color UnpublishedDraftTextColor = TryGetColor(nameof(UnpublishedDraftTextColor));

    public static readonly Color ClearButtonColor = TryGetColor(nameof(ClearButtonColor));

    public static readonly Color DeceasedBackground = TryGetColor(nameof(DeceasedBackground));

    public static readonly Color IsActiveTagBackground = TryGetColor(nameof(IsActiveTagBackground));
}
