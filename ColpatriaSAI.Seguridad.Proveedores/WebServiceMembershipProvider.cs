using System;
using System.Web.Configuration;
using System.Web.Security;

namespace ColpatriaSAI.Seguridad.Proveedores
{
    /// <summary>
    /// un proveedor de suscripciones que utiliza un servicio web para acceder a un almacén de datos a distancia

    /// </summary>
    sealed public class WebServiceMembershipProvider : System.Web.Security.MembershipProvider
    {
        private MembershipProvider.MembershipProvider service;

        private bool _EnablePasswordReset;
        private bool _EnablePasswordRetrieval;
        private int _MaxInvalidPasswordAttempts;
        private int _MinRequiredNonalphanumericCharacters;
        private int _MinRequiredPasswordLength;
        private int _PasswordAttemptWindow;
        private System.Web.Security.MembershipPasswordFormat _PasswordFormat;
        private string _PasswordStrengthRegularExpression;
        private bool _RequiresQuestionAndAnswer;
        private bool _RequiresUniqueEmail;

        private string _ApplicationName;
        private string _RemoteProviderName;

        /// <summary>
        /// un constructor
        /// </summary>
        public WebServiceMembershipProvider()
        {
            service = new MembershipProvider.MembershipProvider();


        }

        /// <summary>
        /// Convertir un usuario
        /// </summary>
        /// <param name="user">un objeto user </param>
        /// <returns>un objeto user convertido</returns>
        static private ColpatriaSAI.Seguridad.Proveedores.MembershipProvider.MembershipUser ConvertUser(System.Web.Security.MembershipUser user)
        {
            if (user == null) return null;
            ColpatriaSAI.Seguridad.Proveedores.MembershipProvider.MembershipUser membershipUser = new ColpatriaSAI.Seguridad.Proveedores.MembershipProvider.MembershipUser();
            membershipUser.Comment = user.Comment;
            membershipUser.CreationDate = user.CreationDate;
            membershipUser.Email = user.Email;
            membershipUser.IsApproved = user.IsApproved;
            membershipUser.IsLockedOut = user.IsLockedOut;
            membershipUser.IsOnline = user.IsOnline;
            membershipUser.LastActivityDate = user.LastActivityDate;
            membershipUser.LastLockoutDate = user.LastLockoutDate;
            membershipUser.LastLoginDate = user.LastLoginDate;
            membershipUser.LastPasswordChangedDate = user.LastPasswordChangedDate;
            membershipUser.PasswordQuestion = user.PasswordQuestion;
            membershipUser.ProviderName = user.ProviderName;
            membershipUser.ProviderUserKey = user.ProviderUserKey;
            membershipUser.UserName = user.UserName;
            return membershipUser;
        }

        /// <summary>
        /// convertir an usuario
        /// </summary>
        /// <param name="user">un objeto user</param>
        /// <returns>un objeto user convertido</returns>
        private System.Web.Security.MembershipUser ConvertUser(ColpatriaSAI.Seguridad.Proveedores.MembershipProvider.MembershipUser user)
        {
            if (user == null) return null;
            System.Web.Security.MembershipUser membershipUser =
              new System.Web.Security.MembershipUser("WebServiceMembershipProvider",
                user.UserName,
                user.ProviderUserKey,
                user.Email,
                user.PasswordQuestion,
                user.Comment,
                user.IsApproved,
                user.IsLockedOut,
                user.CreationDate,
                user.LastLoginDate,
                user.LastActivityDate,
                user.LastPasswordChangedDate,
                user.LastLockoutDate);
            return membershipUser;
        }

        /// <summary>
        /// crear una lista de usuarios
        /// </summary>
        /// <param name="list">una lista de usuarios</param>
        /// <returns>auna lista de usuarios convertida</returns>
        private System.Web.Security.MembershipUserCollection BuildUserCollection(ColpatriaSAI.Seguridad.Proveedores.MembershipProvider.MembershipUser[] list)
        {
            if (list == null) return null;
            System.Web.Security.MembershipUserCollection collection = new System.Web.Security.MembershipUserCollection();
            foreach (ColpatriaSAI.Seguridad.Proveedores.MembershipProvider.MembershipUser user in list)
            {
                collection.Add(ConvertUser(user));
            }
            return collection;
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
        /// requiere implementacion
        /// </summary>
        /// <param name="name">un nombre de proveedor</param>
        /// <param name="config">una coleccion de items</param>
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

            _EnablePasswordRetrieval = ProviderUtility.GetBooleanValue(config, "enablePasswordRetrieval", false);
            _EnablePasswordReset = ProviderUtility.GetBooleanValue(config, "enablePasswordReset", true);
            _RequiresQuestionAndAnswer = ProviderUtility.GetBooleanValue(config, "requiresQuestionAndAnswer", true);
            _RequiresUniqueEmail = ProviderUtility.GetBooleanValue(config, "requiresUniqueEmail", true);
            _MaxInvalidPasswordAttempts = ProviderUtility.GetIntValue(config, "maxInvalidPasswordAttempts", 5, false, 0);
            _PasswordAttemptWindow = ProviderUtility.GetIntValue(config, "passwordAttemptWindow", 10, false, 0);
            _MinRequiredPasswordLength = ProviderUtility.GetIntValue(config, "minRequiredPasswordLength", 7, false, 0x80);
            _MinRequiredNonalphanumericCharacters = ProviderUtility.GetIntValue(config, "minRequiredNonalphanumericCharacters", 1, true, 0x80);
            _PasswordStrengthRegularExpression = config["passwordStrengthRegularExpression"];

            if (config["passwordFormat"] != null)
            {
                _PasswordFormat = (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), config["passwordFormat"]);
            }
            else
            {
                _PasswordFormat = MembershipPasswordFormat.Hashed;
            }

            _RemoteProviderName = config["remoteProviderName"];

            try
            {
                base.Initialize(name, config);
            }

            catch
            {

            }

        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        public override bool EnablePasswordReset
        {
            get { return _EnablePasswordReset; }
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        public override bool EnablePasswordRetrieval
        {
            get { return _EnablePasswordRetrieval; }
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        public override int MaxInvalidPasswordAttempts
        {
            get { return _MaxInvalidPasswordAttempts; }
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _MinRequiredNonalphanumericCharacters; }
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        public override int MinRequiredPasswordLength
        {
            get { return _MinRequiredPasswordLength; }
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        public override int PasswordAttemptWindow
        {
            get { return _PasswordAttemptWindow; }
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _PasswordFormat; }
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        public override string PasswordStrengthRegularExpression
        {
            get { return _PasswordStrengthRegularExpression; }
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        public override bool RequiresQuestionAndAnswer
        {
            get { return _RequiresQuestionAndAnswer; }
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        public override bool RequiresUniqueEmail
        {
            get { return _RequiresUniqueEmail; }
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="username">un Username</param>
        /// <param name="oldPassword">original password</param>
        /// <param name="newPassword">nuevo password</param>
        /// <returns>true or false</returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return service.ChangePassword(_RemoteProviderName, _ApplicationName, username, oldPassword, newPassword);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="username">un username</param>
        /// <param name="password">elpassword</param>
        /// <param name="newPasswordQuestion">nueva question</param>
        /// <param name="newPasswordAnswer">nueva answer</param>
        /// <returns>true or false</returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password,
          string newPasswordQuestion, string newPasswordAnswer)
        {
            return service.ChangePasswordQuestionAndAnswer(_RemoteProviderName, _ApplicationName, username, password,
              newPasswordQuestion, newPasswordAnswer);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="username">requiere implementacion</param>
        /// <param name="password">requiere implementacion</param>
        /// <param name="email">requiere implementacion</param>
        /// <param name="passwordQuestion">requiere implementacion</param>
        /// <param name="passwordAnswer">requiere implementacion</param>
        /// <param name="isApproved">requiere implementacion</param>
        /// <param name="providerUserKey">requiere implementacion</param>
        /// <param name="status">requiere implementacion</param>
        /// <returns>un objeto usuario</returns>
        public override System.Web.Security.MembershipUser CreateUser(string username, string password, string email,
          string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey,
          out MembershipCreateStatus status)
        {
            ColpatriaSAI.Seguridad.Proveedores.MembershipProvider.MembershipCreateStatus newStatus;
            System.Web.Security.MembershipUser user = ConvertUser(service.CreateUser(_RemoteProviderName, _ApplicationName, username, password, email,
              passwordQuestion, passwordAnswer, isApproved, providerUserKey, out newStatus));
            status = (MembershipCreateStatus)Enum.Parse(typeof(MembershipCreateStatus), newStatus.ToString());
            return user;
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="username">requiere implementacion</param>
        /// <param name="deleteAllRelatedData">requiere implementacion</param>
        /// <returns>requiere implementacion</returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return service.DeleteUser(_RemoteProviderName, _ApplicationName, username, deleteAllRelatedData);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="emailToMatch">requiere implementacion</param>
        /// <param name="pageIndex">requiere implementacion</param>
        /// <param name="pageSize">requiere implementacion</param>
        /// <param name="totalRecords">requiere implementacion</param>
        /// <returns>requiere implementacion</returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return BuildUserCollection(service.FindUsersByEmail(_RemoteProviderName, _ApplicationName, emailToMatch, pageIndex,
              pageSize, out totalRecords));
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="emailToMatch">requiere implementacion</param>
        /// <param name="pageIndex">requiere implementacion</param>
        /// <param name="pageSize">requiere implementacion</param>
        /// <param name="totalRecords">requiere implementacion</param>
        /// <returns>requiere implementacion</returns>
        public MembershipUserCollection FindUsersByAllFilter(string filter, int pageIndex, int pageSize, out int totalRecords)
        {
            return BuildUserCollection(service.FindUsersByName(_RemoteProviderName, _ApplicationName, filter, pageIndex,
              pageSize, out totalRecords)); 
                
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="usernameToMatch">requiere implementacion</param>
        /// <param name="pageIndex">requiere implementacion</param>
        /// <param name="pageSize">requiere implementacion</param>
        /// <param name="totalRecords">requiere implementacion</param>
        /// <returns>requiere implementacion</returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex,
          int pageSize, out int totalRecords)
        {
            return BuildUserCollection(service.FindUsersByName(_RemoteProviderName, _ApplicationName, usernameToMatch,
              pageIndex, pageSize, out totalRecords));
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="pageIndex">requiere implementacion</param>
        /// <param name="pageSize">requiere implementacion</param>
        /// <param name="totalRecords">requiere implementacion</param>
        /// <returns>requiere implementacion</returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize,
          out int totalRecords)
        {
            return BuildUserCollection(service.GetAllUsers(_RemoteProviderName, _ApplicationName, pageIndex, pageSize,
            out totalRecords));
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <returns>requiere implementacion</returns>
        public override int GetNumberOfUsersOnline()
        {
            return service.GetNumberOfUsersOnline(_RemoteProviderName, _ApplicationName);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="username">requiere implementacion</param>
        /// <param name="answer">requiere implementacion</param>
        /// <returns>requiere implementacion</returns>
        public override string GetPassword(string username, string answer)
        {
            return service.GetPassword(_RemoteProviderName, _ApplicationName, username, answer);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="username">requiere implementacion</param>
        /// <param name="userIsOnline">requiere implementacion</param>
        /// <returns>requiere implementacion</returns>
        public override System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline)
        {
            return ConvertUser(service.GetUserByUserName(_RemoteProviderName, _ApplicationName, username, userIsOnline));
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="providerUserKey">requiere implementacion</param>
        /// <param name="userIsOnline">requiere implementacion</param>
        /// <returns>requiere implementacion</returns>
        public override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return ConvertUser(service.GetUserByKey(_RemoteProviderName, _ApplicationName, providerUserKey, userIsOnline));
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="email">requiere implementacion</param>
        /// <returns>requiere implementacion</returns>
        public override string GetUserNameByEmail(string email)
        {
            return service.GetUserNameByEmail(_RemoteProviderName, _ApplicationName, email);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="username">requiere implementacion</param>
        /// <param name="answer">requiere implementacion</param>
        /// <returns>requiere implementacion</returns>
        public override string ResetPassword(string username, string answer)
        {
            return service.ResetPassword(_RemoteProviderName, _ApplicationName, username, answer);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="userName">requiere implementacion</param>
        /// <returns>requiere implementacion</returns>
        public override bool UnlockUser(string userName)
        {
            return service.UnlockUser(_RemoteProviderName, _ApplicationName, userName);
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="user">requiere implementacion</param>
        public override void UpdateUser(System.Web.Security.MembershipUser user)
        {
            service.UpdateUser(_RemoteProviderName, _ApplicationName, ConvertUser(user));
        }

        /// <summary>
        /// requiere implementacion
        /// </summary>
        /// <param name="username">requiere implementacion</param>
        /// <param name="password">requiere implementacion</param>
        /// <returns>requiere implementacion</returns>
        public override bool ValidateUser(string username, string password)
        {
            return service.ValidateUser(_RemoteProviderName, _ApplicationName, username, password);
        }
    }
}
