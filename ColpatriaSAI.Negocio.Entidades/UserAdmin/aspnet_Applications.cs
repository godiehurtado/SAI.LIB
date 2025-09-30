using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ColpatriaSAI.Negocio.Entidades.UserAdmin
{
    [DataContract(IsReference = true)] 
   public class aspnet_Applications
    {

        public string ApplicationName { get; set; }
        public string LoweredApplicationName { get; set; }
        public Guid ApplicationId { get; set; }
        public string Description { get; set; }
    }
}
