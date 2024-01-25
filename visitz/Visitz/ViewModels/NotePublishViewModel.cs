using CommunityToolkit.Mvvm.Messaging;
using Visitz.Models;
using Visitz.Services;

namespace Visitz.ViewModels
{
    public partial class NotePublishViewModel : PublishViewModel, IRecipient<ServiceStateMessage>, ICaseloadItemHolder
    {
        public CaseloadItem CaseloadItem { get; set; }

        public override void Publish()
        {
            throw new NotImplementedException();
        }

        public void Receive(ServiceStateMessage message)
        {
            throw new NotImplementedException();
        }
    }
}

