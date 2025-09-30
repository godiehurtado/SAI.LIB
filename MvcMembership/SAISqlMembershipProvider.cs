using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace MvcMembership
{
    public class SAISqlMembershipProvider: System.Web.Security.SqlMembershipProvider
    {
        public MembershipUserCollection FindUsersByAllFilter(string filter,int pageIndex,int pageSize,out int totalRecords)
        {
            return this.FindUsersByEmail(filter, pageIndex, pageSize, out totalRecords);
        }
    }

    
}