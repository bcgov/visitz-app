using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.BaseClasses;

#nullable enable

public partial class IcmRecordContentView<T>(T viewModel, string title = "")
    : ViewModelContentView(viewModel, title),
        IIcmRecordInfo,
        IBusinessObjectHolder
    where T : IcmRecordViewModel
{
    public new T ViewModel => (T)base.ViewModel;

    public string RowId
    {
        get => ViewModel.RowId;
        set => ViewModel.RowId = value;
    }

    public EntityType EntityType
    {
        get => ViewModel.EntityType;
        set => ViewModel.EntityType = value;
    }

    public IBusinessObject? BusinessObject
    {
        get => ViewModel.BusinessObject;
        set => ViewModel.BusinessObject = value;
    }
}
