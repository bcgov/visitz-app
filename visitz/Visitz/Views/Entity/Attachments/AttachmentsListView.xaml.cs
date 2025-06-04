using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsListView : ViewModelContentView, IBusinessObjectHolder
{
    new AttachmentsListViewModel ViewModel => base.ViewModel as AttachmentsListViewModel;

    public IBusinessObject BusinessObject
    {
        get => ViewModel.BusinessObject;
        set => ViewModel.BusinessObject = value;
    }

    public AttachmentsListView() : base(ServiceProvider.GetService<AttachmentsListViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
