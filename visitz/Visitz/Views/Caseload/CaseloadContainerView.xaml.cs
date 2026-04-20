using CommunityToolkit.Mvvm.Messaging;
using Visitz.Extensions;
using Visitz.Views.BaseClasses;
using Visitz.Views.Entity;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Caseload;

public partial class CaseloadContainerView : BaseContentView
{
    IView CaseloadView;

    public CaseloadContainerView()
    {
        InitializeComponent();
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        RegisterReceivers();

        CaseloadView ??= ServiceProvider.GetService<CaseloadView>();
        await ContentStack.PushAsync((ContentView)CaseloadView);
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

    private void RegisterReceivers()
    {
        StrongReferenceMessenger.Default.Register<BusinessObjectSelectedMessage>(
            this,
            async (recipient, message) =>
            {
                await (recipient as CaseloadContainerView).OpenBusinessObject(message);
            }
        );

        StrongReferenceMessenger.Default.Register<EntityNavBackMessage>(
            this,
            async (recipient, message) =>
            {
                await (recipient as CaseloadContainerView).ContentStack.PopAsync();
            }
        );
    }

    private async Task OpenBusinessObject(BusinessObjectSelectedMessage message)
    {
        IBusinessObject item = message.Value;
        EntitySection section = message.Section;
        IDraftItem draftItem = message.DraftItem;

        var entityView = ServiceProvider.GetService<EntityView>();
        entityView.RowId = item.Id;
        entityView.EntityType = item.EntityType;
        entityView.SetRequestedSection(section, draftItem);

        try
        {
            await ContentStack.PushAsync(entityView);
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }
}
