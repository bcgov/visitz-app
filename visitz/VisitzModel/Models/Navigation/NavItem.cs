using CommunityToolkit.Mvvm.ComponentModel;

namespace VisitzModel.Models.Navigation;

public partial class NavItem : ObservableObject
{
    private double iconSize;

    public ImageSource? SelectedImageSource
    {
        get => field;
        set
        {
            field = value;

            if (field is FontImageSource icon)
            {
                icon.Color = Color;
                icon.Size = IconSize * 2;
            }
        }
    }

    public ImageSource? UnselectedImageSource
    {
        get => field;
        set
        {
            field = value;

            if (field is FontImageSource icon)
            {
                icon.Color = Color;
                icon.Size = IconSize * 2;
            }
        }
    }

    public string Text { get; set; } = string.Empty;

    public Color? Color { get; set; }

    public double IconSize
    {
        get => iconSize;
        set
        {
            iconSize = value;

            if (SelectedImageSource is FontImageSource selectedIcon)
                selectedIcon.Size = iconSize * 2;

            if (UnselectedImageSource is FontImageSource unselectedIcon)
                unselectedIcon.Size = iconSize * 2;
        }
    }

    public Type? ContentViewType { get; set; }

    [ObservableProperty]
    public partial int BadgeCount { get; set; }

    [ObservableProperty]
    public partial bool ShowBadge { get; set; } = false;

    partial void OnBadgeCountChanged(int oldValue, int newValue)
    {
        ShowBadge = newValue > 0;
    }
}
