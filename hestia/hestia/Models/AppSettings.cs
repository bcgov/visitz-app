using System;
namespace hestia.Models
{
    /// <summary>
    /// Environment variables model following appSettings.json structure.
    /// </summary>
	public class AppSettings
	{
		public string AuthenticationDomain { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
    }
}

