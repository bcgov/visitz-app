using System;
using Android.App;
using Android.Content.PM;
using Android.Webkit;

namespace visitz.Platforms.Android
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(new[] { global::Android.Content.Intent.ActionView },
              Categories = new[] {
                global::Android.Content.Intent.CategoryDefault,
                global::Android.Content.Intent.CategoryBrowsable
              },
              DataScheme = CALLBACK_SCHEME)]
    public class WebAuthenticationCallbackActivity : Microsoft.Maui.Authentication.WebAuthenticatorCallbackActivity
    {
        const string CALLBACK_SCHEME = "visitz";
    }
}

