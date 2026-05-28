using CommunityToolkit.Mvvm.Messaging;
using Oidc.Network;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Attachments;
using Visitz.Services.Base;
using Visitz.Services.CallDetails;
using Visitz.Services.Caseload;
using Visitz.Services.Notes;
using Visitz.Services.People;
using Visitz.Services.SafetyAssessments;
using Visitz.Services.Visits;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Extensions;

#nullable enable

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
            businessObject.LocalState?.ShouldDownloadDuringRefreshBinding = true;

            try
            {
                var handler = ServiceProvider.GetService<ServiceHandler>();
                var msg = GetAllDataForRecordService.MakeStartMessage(businessObject);

                return await handler.TryRunServiceAsync(msg);
            }
            catch
            {
                businessObject.LocalState?.ShouldDownloadDuringRefreshBinding = false;
                throw;
            }
        }

        public void RegisterActivityListeners(IRecipient<ServiceStateMessage> recipient)
        {
            Register(recipient, GetAttachmentsService.MakeId(businessObject.EntityType, businessObject.Id));
            Register(recipient, GetContactsService.MakeId(businessObject.EntityType, businessObject.Id));
            Register(recipient, GetSupportNetworkService.MakeId(businessObject.EntityType, businessObject.Id));

            if (businessObject.EntityType == EntityType.Case)
            {
                Register(recipient, GetVisitsService.MakeId(businessObject.Id));
            }

            if (businessObject.EntityType == EntityType.Incident)
            {
                Register(recipient, GetIncidentConcernsService.MakeId(businessObject.Id));
                Register(recipient, GetSafetyAssessmentsService.MakeId(new(businessObject)));
            }

            if (businessObject.EntityType is EntityType.Case or EntityType.Incident or EntityType.ServiceRequest)
            {
                Register(recipient, GetNotesService.MakeId(businessObject.FileNumber));
            }

            if (businessObject.EntityType is EntityType.Incident or EntityType.Memo or EntityType.ServiceRequest)
            {
                Register(recipient, GetCallInformationService.MakeId(businessObject.EntityType, businessObject.Id));
                Register(
                    recipient,
                    GetAdditionalInformationService.MakeId(businessObject.EntityType, businessObject.Id)
                );
            }
        }
    }

    static void Register(IRecipient<ServiceStateMessage> recipient, string token)
    {
        WeakReferenceMessenger.Default.Register(recipient, token);
    }
}
