using CommunityToolkit.Mvvm.ComponentModel;
using VisitzModel;

namespace Visitz.Views.BaseClasses
{
    /// <summary>
    /// The base class for all the view models. Common functionality can be defined here.
    /// </summary>
    public partial class VisitzViewModel : ObservableObject, IDisposable
    {
        bool _disposedValue;

        public Task InitTask { get; private set; }

        public virtual Task StartInitAsync()
        {
            InitTask ??= InitAsync();

            return InitTask;
        }

        protected virtual Task InitAsync()
        {
            ConsoleTrace.TraceMethod(this);

            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            if (disposing)
                ConsoleTrace.TraceMethod(this);

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
