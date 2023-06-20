using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visitz.Services;

namespace Visitz.VisitzConfig
{
    public static class VisitzServicesConfig
    {
        public static MauiAppBuilder ConfigureVisitzServices(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<ServiceHandler>();

            return builder;
        }
    }
}
