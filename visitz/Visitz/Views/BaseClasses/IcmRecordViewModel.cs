using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Storage;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.BaseClasses;

public partial class IcmRecordViewModel : VisitzViewModel, IIcmRecordInfo, IBusinessObjectHolder
{
    protected Realm? DataRealm { get; private set; }

    public string RowId { get; set; } = string.Empty;

    public EntityType EntityType { get; set; }

    [ObservableProperty]
    public partial IBusinessObject BusinessObject { get; set; } = new CaseRecord();

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        ArgumentException.ThrowIfNullOrEmpty(RowId);

        if (EntityType == EntityType.Unknown)
            throw new ArgumentException($"EntityType should not be {EntityType.Unknown}");

        DataRealm =
            await VisitzRealms.GetIcmDataRealmAsync()
            ?? throw new InvalidOperationException("Couldn't open IcmData Realm");

        BusinessObject =
            IBusinessObject.GetByIdType(DataRealm, RowId, EntityType)
            ?? throw new InvalidOperationException(
                $"Unable to retrieve BusinessObject (id '{RowId}', type '{EntityType}')"
            );
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            BusinessObject = new CaseRecord();
            DataRealm?.Dispose();

            disposed = true;
        }

        base.Dispose(disposing);
    }
}
