namespace Visitz;

public partial class VisitzWindow : Window
{
	public VisitzWindow() { }

	public VisitzWindow(Page page) : base(page) { }

	protected override void OnCreated()
	{
		base.OnCreated();

#if WINDOWS
		ApplyDefaultWindowLayout(this);
#endif
	}

	private static partial Window ApplyDefaultWindowLayout(Window window);
}
