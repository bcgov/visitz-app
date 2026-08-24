using System.ComponentModel;
using System.Web;
using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Device;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.FormControls;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.Details;

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
            KeyPlayer = null;
            RecordInfoItems.Clear();
            ContactInfoItems.Clear();

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
#if IOS
                    ValueColor = Colors.Blue,
                    ValueTextDecorations = TextDecorations.Underline,
                    TapAction = () => TryDial(KeyPlayer?.HomePhoneFormatted),
#endif
                },
                new()
                {
                    IconGlyph = MaterialIcons.Phone,
                    FontFamily = MaterialIcons.RoundedUnfilled.FontFamily,
                    Label = LocalizedStrings.CellNumber,
                    Value = KeyPlayer.CellPhoneFormatted,
#if IOS
                    ValueColor = Colors.Blue,
                    ValueTextDecorations = TextDecorations.Underline,
                    TapAction = () => TryDial(KeyPlayer?.CellPhoneFormatted),
#endif
                },
                new()
                {
                    IconGlyph = MaterialIcons.Home,
                    FontFamily = MaterialIcons.RoundedUnfilled.FontFamily,
                    Label = LocalizedStrings.Address,
                    Value = KeyPlayer.PrimaryAddressBinding,
                    ValueColor = Colors.Blue,
                    ValueTextDecorations = TextDecorations.Underline,
                    TapAction = async () =>
                    {
                        try
                        {
                            if (KeyPlayer?.PrimaryAddressBinding.Trim().Length > 0)
                                await MapsHelper.OpenAddress(HttpUtility.UrlEncode(KeyPlayer.PrimaryAddressBinding));
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
                        }
                    },
                },
            ];
        }
    }

    void TryDial(string? phoneNumber)
    {
        try
        {
            if (phoneNumber?.Trim().Length > 0)
                PhoneDialer.Default.Open(phoneNumber);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            _ = Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }
}
