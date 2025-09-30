using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Runtime.Serialization;



namespace ColpatriaSAI.Negocio.Entidades.UserAdmin
{
    public class UserAdministrationEntities: ObjectContext
    {
        private ObjectSet<aspnet_Applications> _aspnet_Applications;
        private ObjectSet<aspnet_Users> _aspnet_Users;
        private ObjectSet<aspnet_Roles> _aspnet_Roles;


        public UserAdministrationEntities()
            : base("name=ASPNETDBEntities", "ASPNETDBEntities")
        {
            //_aspnet_Applications = base.CreateObjectSet<aspnet_Applications>();
            //_aspnet_Users = base.CreateObjectSet<aspnet_Users>();
            //_aspnet_Roles = base.CreateObjectSet<aspnet_Roles>();





        }
        public UserAdministrationEntities(string connectionString)
            : base(connectionString, "ASPNETDBEntities")
        {
            //_aspnet_Applications = base.CreateObjectSet<aspnet_Applications>();
            //_aspnet_Users = base.CreateObjectSet<aspnet_Users>();
            //_aspnet_Roles = base.CreateObjectSet<aspnet_Roles>();





        }

        public ObjectSet<aspnet_Users> Usuarios
        {
            get
            {

                return _aspnet_Users ?? (_aspnet_Users = base.CreateObjectSet<aspnet_Users>());

            }

        }

        public ObjectSet<aspnet_Applications> Aplicaciones
        {
            get
            {
                return _aspnet_Applications ?? (_aspnet_Applications = base.CreateObjectSet<aspnet_Applications>());

            }

        }

        public ObjectSet<aspnet_Roles> Roles
        {
            get
            {
                return _aspnet_Roles ?? (_aspnet_Roles = base.CreateObjectSet<aspnet_Roles>());

            }

        }


    }

    


}
