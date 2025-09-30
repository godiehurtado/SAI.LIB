

namespace ColpatriaSAI.Seguridad.Proveedores
{
    /// <summary>
    /// A Role Provider that uses a webserice to talk to a remote credential store
    /// </summary>
    sealed public class WebServiceRoleProvider : System.Web.Security.RoleProvider
    {
        private ColpatriaSAI.Seguridad.Proveedores.RoleProvider.RoleProvider service;

        private string _ApplicationName;
        private string _RemoteProviderName;

        /// <summary>
        /// Create an instance of this class
        /// </summary>
        public WebServiceRoleProvider()
        {
            service = new RoleProvider.RoleProvider();
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                return _ApplicationName;
            }
            set
            {
                _ApplicationName = value;
            }
        }

        /// <summary>
        /// handle the Initiate override and extract our parameters
        /// </summary>
        /// <param name="name">name of the provider</param>
        /// <param name="config">configuration collection</param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config["roleProviderUri"] != null)
            {
                service.Url = config["roleProviderUri"];
            }

            _ApplicationName = config["applicationName"];
            if (string.IsNullOrEmpty(_ApplicationName))
            {
                _ApplicationName = ProviderUtility.GetDefaultAppName();
            }

            _RemoteProviderName = config["remoteProviderName"];

            base.Initialize(name, config);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="usernames">a list of usernames</param>
        /// <param name="roleNames">a list of roles</param>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            service.AddUsersToRoles(_RemoteProviderName, _ApplicationName, usernames, roleNames);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="roleName">a role name</param>
        public override void CreateRole(string roleName)
        {
            service.CreateRole(_RemoteProviderName, _ApplicationName, roleName);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="roleName">a role</param>
        /// <param name="throwOnPopulatedRole">get upset of users are in a role</param>
        /// <returns></returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {            
            return service.DeleteRole(_RemoteProviderName, _ApplicationName, roleName, throwOnPopulatedRole);
        }

        /// <summary>
        /// required implemention
        /// </summary>
        /// <param name="roleName">a role</param>
        /// <param name="usernameToMatch">a username to look for in the role</param>
        /// <returns></returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return service.FindUsersInRole(_RemoteProviderName, _ApplicationName, roleName, usernameToMatch);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <returns></returns>
        public override string[] GetAllRoles()
        {
            return service.GetAllRoles(_RemoteProviderName, _ApplicationName);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="username">un username</param>
        /// <returns>una lista de roles</returns>
        public override string[] GetRolesForUser(string username)
        {
            return service.GetRolesForUser(_RemoteProviderName, _ApplicationName, username);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="roleName">un role</param>
        /// <returns>una lista de  users</returns>
        public override string[] GetUsersInRole(string roleName)
        {
            return service.GetUsersInRole(_RemoteProviderName, _ApplicationName, roleName);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="username">un username</param>
        /// <param name="roleName">un role</param>
        /// <returns>true or false</returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            return service.IsUserInRole(_RemoteProviderName, _ApplicationName, username, roleName);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="usernames">una lista de usernames</param>
        /// <param name="roleNames">una lista de roles</param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            service.RemoveUsersFromRoles(_RemoteProviderName, _ApplicationName, usernames, roleNames);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="roleName">un role</param>
        /// <returns>true or false</returns>
        public override bool RoleExists(string roleName)
        {
            return service.RoleExists(_RemoteProviderName, _ApplicationName, roleName);
        }
    }
}
