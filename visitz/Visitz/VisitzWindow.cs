namespace Visitz;

public partial class VisitzWindow : Window
{
	private static readonly double ValidityTimeoutMinutes = 5.0d;

	private DateTime? LastDeactivated { get; set; }

	public event EventHandler ActivatedWhenInvalid;

	public VisitzWindow() { }

	public VisitzWindow(Page page) : base(page) { }

	protected override void OnCreated()
	{
		base.OnCreated();

#if WINDOWS
		ApplyDefaultWindowLayout(this);
#endif
	}

#if WINDOWS
	private static partial Window ApplyDefaultWindowLayout(Window window);
#endif

	protected override void OnActivated()
	{
		base.OnActivated();

		if (LastDeactivated is DateTime last)
		{
			var validUntil = last.AddMinutes(ValidityTimeoutMinutes);
			var isValid = DateTime.UtcNow <= validUntil;

			if (!isValid)
				ActivatedWhenInvalid?.Invoke(this, EventArgs.Empty);

			LastDeactivated = null;
		}
	}

	protected override void OnDeactivated()
	{
		base.OnDeactivated();

		LastDeactivated = DateTime.UtcNow;
	}
}
