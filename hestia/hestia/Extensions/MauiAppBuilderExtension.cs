using System;
using hestia.Services.Authentication;
using hestia.Services.Networking;
using hestia.Services;
using hestia.Views;
using hestia.ViewModels;
using hestia.Routers;
using System.Reflection;
using hestia.Services.Localization;
using Microsoft.Extensions.Configuration;
using hestia.Models;

namespace hestia.Extensions
{
    /// <summary>
    /// MauiAppBuilder's added functionality
    /// </summary>
    public static class MauiAppBuilderExtension
    {
        /// <summary>
        /// Dependency injection setup
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>MauiAppBuilder</returns>
        public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {
            // This service is needed to inject IStringLocalizer into LocalizeExtension
            builder.Services.AddLocalization();

            builder.Services.AddSingleton<LocalizeExtension>();

            builder.Services.AddSingleton<LandingPage>();
            builder.Services.AddSingleton<LandingRouter>();
            builder.Services.AddSingleton<LandingViewModel>();

            builder.Services.AddTransient<DeviceAuthenticator>();
            builder.Services.AddTransient<DeviceAuthenticationRouter>();
            builder.Services.AddTransient<DeviceAuthenticationPage>();
            builder.Services.AddTransient<DeviceAuthenticationViewModel>();

            builder.Services.AddTransient<OpenIdAuthenticationRouter>();
            builder.Services.AddTransient<OpenIdAuthenticationPage>();
            builder.Services.AddTransient<OpenIdAuthenticationViewModel>();

            builder.Services.AddTransient<CasesAndIncidentsRouter>();
            builder.Services.AddTransient<CasesAndIncidentsPage>();
            builder.Services.AddTransient<CasesAndIncidentsViewModel>();

            builder.Services.AddTransient<CaseNotesRouter>();
            builder.Services.AddTransient<CaseNotesPage>();
            builder.Services.AddTransient<CaseNotesViewModel>();

            builder.Services.AddTransient<CaseIncidentDetailsPage>();
            builder.Services.AddTransient<CaseIncidentDetailsViewModel>();

            return builder;
        }
    }
}

