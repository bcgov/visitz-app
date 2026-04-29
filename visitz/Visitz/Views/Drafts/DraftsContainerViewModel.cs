using CommunityToolkit.Mvvm.Messaging;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Drafts;

public partial class DraftsContainerViewModel : VisitzViewModel
{
    protected override async Task InitAsync()
    {
        await base.InitAsync();
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);

            disposed = true;
        }

        base.Dispose(disposing);
    }
}
