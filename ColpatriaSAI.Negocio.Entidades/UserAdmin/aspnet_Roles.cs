using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;




namespace ColpatriaSAI.Negocio.Entidades.UserAdmin
{
    [DataContract(IsReference = true)] 
    public class aspnet_Roles
    {
        [DataMember]
        public Guid ApplicationId  { get; set; }
        [DataMember]
       public Guid RoleId  { get; set; }
        [DataMember]
        public string RoleName { get; set; }
        [DataMember]
        public string LoweredRoleName { get; set; }
        [DataMember]
       public string Description { get; set; }
        [DataMember]
        public ISet<aspnet_Users> aspnet_Users { get; set; }
        [DataMember]
        public ISet<aspnet_Applications> aspnet_Applications { get; set; }
        public aspnet_Roles()
        {

            this.aspnet_Users = new HashSet<aspnet_Users>();
        }

    }
}
