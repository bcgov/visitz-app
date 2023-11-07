namespace Visitz.Models;

public class NavItem
{
    private ImageSource selectedImageSource;
    private ImageSource unselectedImageSource;
    private double iconSize;

    public ImageSource SelectedImageSource
    { 
        get => selectedImageSource;
        set
        {
            selectedImageSource = value;

            if (selectedImageSource is FontImageSource icon)
            {
                icon.Color = Color;
                icon.Size = IconSize * 2;
            }
        }
    }

    public ImageSource UnselectedImageSource
    {
        get => unselectedImageSource;
        set
        {
            unselectedImageSource = value;

            if (unselectedImageSource is FontImageSource icon)
            {
                icon.Color = Color;
                icon.Size = IconSize * 2;
            }
        }
    }

    public string Text { get; set; }

    public Color Color { get; set; }

    public double IconSize
    {
        get => iconSize;
        set
        {
            iconSize = value;

            if (selectedImageSource is FontImageSource selectedIcon)
                selectedIcon.Size = iconSize * 2;

            if (unselectedImageSource is FontImageSource unselectedIcon)
                unselectedIcon.Size = iconSize * 2;
        }
    }

    public Type ContentViewType { get; set; }
}
