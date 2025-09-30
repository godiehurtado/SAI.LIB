using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ColpatriaSAI.Negocio.Entidades.UserAdmin
{
    [DataContract(IsReference = true)] 
    public class aspnet_Users
    {

        public Guid ApplicationId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string LoweredUserName { get; set; }
        public string MobileAlias { get; set; }
        public Boolean IsAnonymous { get; set; }
        public DateTime LastActivityDate { get; set; }
        public ISet<aspnet_Applications> aspnet_Applications { get; set; }
        public aspnet_Users()
 {

     this.aspnet_Applications = new HashSet<aspnet_Applications>();
 }
    }


  
}
