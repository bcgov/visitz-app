using Oidc.Network;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.Caseload;
using VisitzModel.Models.Caseload;

namespace Visitz.Extensions;

internal static class IBusinessObjectExtensions
{
    extension(IBusinessObject businessObject)
    {
        /// <summary>
        /// Prompt the user to ask if they want to download dependent information
        /// for this business object. Does not initiate a download.
        /// </summary>
        /// <param name="businessObject"></param>
        /// <returns></returns>
        public async Task<bool> PromptCanDownloadDependentData()
        {
            if (!NetworkHelper.InternetAvailable)
            {
                await Navigator.CurrentOpenPage.DisplayAlertAsync(
                    LocalizedStrings.NoInternet,
                    LocalizedStrings.NeedInternetToViewRecord,
                    LocalizedStrings.Ok
                );
                return false;
            }
            else
            {
                string msg = string.Format(
                    LocalizedStrings.MarkForDownload,
                    businessObject.EntityType,
                    businessObject.DisplayName.Trim()
                );

                return await Navigator.CurrentOpenPage.DisplayAlertAsync(
                    LocalizedStrings.DownloadRecordInformation,
                    msg,
                    LocalizedStrings.Download,
                    LocalizedStrings.Cancel
                );
            }
        }

        /// <summary>
        /// Start the download service to get all depdendent data for a business object.
        /// </summary>
        /// <param name="businessObject"></param>
        /// <returns>ID of the service that was started</returns>
        public async Task<VisitzService.Result> DownloadDependentData()
        {
            businessObject.LocalState.ShouldDownloadDuringRefreshBinding = true;

            try
            {
                var handler = ServiceProvider.GetService<ServiceHandler>();
                var msg = GetAllDataForRecordService.MakeStartMessage(businessObject);

                return await handler.TryRunServiceAsync(msg);
            }
            catch
            {
                businessObject.LocalState.ShouldDownloadDuringRefreshBinding = false;
                throw;
            }
        }
    }
}
