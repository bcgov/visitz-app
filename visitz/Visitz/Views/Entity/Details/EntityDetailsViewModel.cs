using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.FormControls;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.Details;

#nullable enable

public partial class EntityDetailsViewModel : IcmRecordViewModel
{
    [ObservableProperty]
    public partial IcmContact? KeyPlayer { get; set; }

    [ObservableProperty]
    public partial List<InfoItem> RecordInfoItems { get; set; } = [];

    [ObservableProperty]
    public partial List<InfoItem> ContactInfoItems { get; set; } = [];

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        KeyPlayer = BusinessObject.GetKeyPlayer();

        SetupRecordInfo();
        BusinessObject.SubscribePropertyChanged(BusinessObject_PropertyChanged);
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            KeyPlayer = null;

            BusinessObject.UnsubscribePropertyChanged(BusinessObject_PropertyChanged);

            disposed = true;
        }
        base.Dispose(disposing);
    }

    void BusinessObject_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IBusinessObject.LocalState))
            SetupRecordInfo();
    }

    void SetupRecordInfo()
    {
        RecordInfoItems =
        [
            new()
            {
                IconGlyph = MaterialIcons.Barcode,
                FontFamily = MaterialIcons.RoundedUnfilled.FontFamily,
                Label = LocalizedStrings.Id,
                Value = BusinessObject.FileNumberBinding,
            },
            new()
            {
                IconGlyph = MaterialIcons.Assignment_ind,
                FontFamily = MaterialIcons.RoundedUnfilled.FontFamily,
                Label = LocalizedStrings.AssignedTo,
                Value = BusinessObject.DisplayAssignees,
            },
            new()
            {
                IconGlyph = MaterialIcons.Calendar_today,
                FontFamily = MaterialIcons.RoundedUnfilled.FontFamily,
                Label = LocalizedStrings.OpenDate,
                Value = BusinessObject.DisplayDate,
            },
            new()
            {
                IconGlyph = MaterialIcons.Corporate_fare,
                FontFamily = MaterialIcons.RoundedUnfilled.FontFamily,
                Label = LocalizedStrings.ServiceOffice,
                Value = BusinessObject.ServiceOfficeBinding,
            },
        ];

        if (KeyPlayer != null)
        {
            ContactInfoItems =
            [
                new()
                {
                    IconGlyph = MaterialIcons.Phone,
                    FontFamily = MaterialIcons.RoundedUnfilled.FontFamily,
                    Label = LocalizedStrings.HomeNumber,
                    Value = KeyPlayer.HomePhoneFormatted,
                },
                new()
                {
                    IconGlyph = MaterialIcons.Phone,
                    FontFamily = MaterialIcons.RoundedUnfilled.FontFamily,
                    Label = LocalizedStrings.CellNumber,
                    Value = KeyPlayer.CellPhoneFormatted,
                },
                new()
                {
                    IconGlyph = MaterialIcons.Home,
                    FontFamily = MaterialIcons.RoundedUnfilled.FontFamily,
                    Label = LocalizedStrings.Address,
                    Value = KeyPlayer.PrimaryAddress,
                },
            ];
        }
    }
}
