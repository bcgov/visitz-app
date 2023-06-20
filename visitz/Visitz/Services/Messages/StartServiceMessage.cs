using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visitz.Services.Messages
{
    public class StartServiceMessage : ServiceInfoMessage
    {
        public object Payload { get; set; }
    }
}
