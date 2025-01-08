using CommunityToolkit.Mvvm.ComponentModel;
using VisitzModel;

namespace Visitz.Views.BaseClasses
{
	/// <summary>
	/// The base class for all the view models. Common functionality can be defined here.
	/// </summary>
	public partial class VisitzViewModel : ObservableObject, IDisposable
	{
		bool created;
        bool _disposedValue;

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

        [Obsolete("Use Dispose instead")]
		public virtual void Destroy()
		{
			ConsoleTrace.TraceMethod(this);
		}

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            if (disposing)
            {
                ConsoleTrace.TraceMethod(this);
#pragma warning disable CS0618 // Type or member is obsolete
                Destroy(); // Used until all other references are removed
#pragma warning restore CS0618 // Type or member is obsolete
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
