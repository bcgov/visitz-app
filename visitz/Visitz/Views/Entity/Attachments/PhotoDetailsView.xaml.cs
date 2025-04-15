using CommunityToolkit.Mvvm.Messaging;
using Visitz.Services;
using Visitz.Services.Attachments;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;

namespace Visitz.Views.Entity.Attachments;

public partial class PhotoDetailsView : ViewModelContentView, ICaseloadItemHolder, IRecipient<ServiceStateMessage>
{
    new PhotoDetailsViewModel ViewModel => base.ViewModel as PhotoDetailsViewModel;

    public Attachment Attachment
    {
        get => ViewModel.Attachment;
        set => ViewModel.Attachment = value;
    }
    public CaseloadItem CaseloadItem
    {
        get => ViewModel.CaseloadItem;
        set => ViewModel.CaseloadItem = value;
    }

    public PhotoDetailsView() : base(ServiceProvider.GetService<PhotoDetailsViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override void Creating()
    {
        base.Creating();

        var attachment = ViewModel.Attachment;

        if (attachment.Draft is AttachmentDraft draft)
        {
            string id = SubmitAttachmentService.MakeId(draft.RelatedEntityId, attachment.Id);
            WeakReferenceMessenger.Default.Register(this, id);
        }
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

    private void CloseButton_Closing(object sender, Controls.ClosingEventArgs e)
    {
        Unregister();
    }
}
