namespace Visitz.Services
{
    public class ServiceStateMessage : ServiceInfoMessage
    {
        public VisitzService.State Status { get; set; }

        public VisitzService.Result Result { get; set; }

        public string Message { get; set; }
    }
}
