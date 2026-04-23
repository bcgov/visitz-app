using CommunityToolkit.Mvvm.Messaging;
using Visitz.Services;
using Visitz.Services.Attachments;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Entity.Attachments;

public partial class PhotoDetailsView : ViewModelContentView, IBusinessObjectHolder, IRecipient<ServiceStateMessage>
{
    new PhotoDetailsViewModel ViewModel => base.ViewModel as PhotoDetailsViewModel;

    public Attachment Attachment
    {
        get => ViewModel.Attachment;
        set => ViewModel.Attachment = value;
    }

    public IBusinessObject BusinessObject
    {
        get => ViewModel.BusinessObject;
        set => ViewModel.BusinessObject = value;
    }

    public bool IsDownloadedAttachment
    {
        get => ViewModel.IsDownloadedAttachment;
        set => ViewModel.IsDownloadedAttachment = value;
    }

    public PhotoDetailsView()
        : base(ServiceProvider.GetService<PhotoDetailsViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override Task InitAsync()
    {
        var task = base.InitAsync();

        if (ViewModel.Attachment?.Draft is not null)
        {
            string id = SubmitAttachmentService.MakeId(BusinessObject.EntityType, BusinessObject.Id);

            WeakReferenceMessenger.Default.Register(this, id);
        }

        return task;
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            Unregister();
            disposed = true;
        }
        base.Dispose(disposing);
    }

    void Unregister()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    public void Receive(ServiceStateMessage message)
    {
        if (message.FinishedSuccess)
        {
            Navigator.Navigation.RemovePage(Navigator.CurrentOpenPage);
            Unregister();
        }
    }

    private void CloseButton_Closing(object? sender, Controls.ClosingEventArgs e)
    {
        Unregister();
    }
}
