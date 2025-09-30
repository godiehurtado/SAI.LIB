using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;

namespace ColpatriaSAI.Negocio.Componentes.Seguridad
{
    public class Rol
    {
        private SAI_Entities _dbcontext;

        public void EliminarPermisosRol(string rol)
        {
            _dbcontext = new SAI_Entities();
            var query = _dbcontext.SiteMaps.Where(x => x.Roles.Contains(rol));
            foreach (var module in query)
            {
                string[] t = module.Roles.Split(',');
                String rolestokeep = String.Empty;
                for (int i = 0; i < t.Length; i++)
                {
                    if (t[i].Trim() != rol)
                    {
                        rolestokeep += t[i] + ",";
                    }
                }
                if (rolestokeep.EndsWith(","))
                {
                    rolestokeep = rolestokeep.Substring(0, rolestokeep.Length - 1);
                }
                module.Roles = rolestokeep;
            }
            _dbcontext.SaveChanges();
        }
    }
}
