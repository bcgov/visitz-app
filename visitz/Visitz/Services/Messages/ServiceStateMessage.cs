namespace Visitz.Services
{
    public class ServiceStateMessage : ServiceInfoMessage
    {
        public VisitzService.State Status { get; set; }
    }
}
