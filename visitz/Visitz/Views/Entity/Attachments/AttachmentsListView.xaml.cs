using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsListView : ViewModelContentView, ICaseloadItemHolder
{
    new AttachmentsListViewModel ViewModel => base.ViewModel as AttachmentsListViewModel;
    public CaseloadItem CaseloadItem
    {
        get => ViewModel.CaseloadItem;
        set => ViewModel.CaseloadItem = value;
    }

	public AttachmentsListView() : base(ServiceProvider.GetService<AttachmentsListViewModel>())
	{
		InitializeComponent();
        BindingContext = ViewModel;
	}
}
