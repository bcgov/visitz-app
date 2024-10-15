using CommunityToolkit.Mvvm.ComponentModel;
using VisitzModel;

namespace Visitz.Views.BaseClasses
{
	/// <summary>
	/// The base class for all the view models. Common functionality can be defined here.
	/// </summary>
	public partial class VisitzViewModel : ObservableObject
	{
		bool created;

		public void OnCreate()
		{
			if (!created)
			{
				Create();
				created = true;
			}
		}

		public virtual void Create()
		{
			ConsoleTrace.TraceMethod(this);
		}

		public virtual void Destroy()
		{
			ConsoleTrace.TraceMethod(this);
		}
	}
}
