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
            return builder;
        }
    }
}

