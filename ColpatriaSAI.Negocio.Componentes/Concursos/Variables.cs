using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Concursos
{
   public class Variables
    {
        private SAI_Entities tabla = new SAI_Entities();


        public List<Variable> ListarVariables()
        {
            return tabla.Variables.Include("Tipovariable").Include("tabla").Include("OperacionAgregacion").Where(Variable => Variable.id > 0).OrderBy(v => v.nombre).ToList();
        }


        public List<Variable> ListarVariablesPorId(int idVariable)
        {
            return tabla.Variables.Where(Variable => Variable.id == idVariable && Variable.id > 0).ToList();
        }

        public List<Variable> ListarVariablesPremio()
        {
         
            return tabla.Variables.Include("Tipovariable").Include("tabla").Include("OperacionAgregacion").Where(Variable => Variable.id > 0 && Variable.tipoVariable_id == 1).ToList();

        }
    }

   public class Tipovariables
   {
       private SAI_Entities tabla = new SAI_Entities();

       public List<TipoVariable> ListarTipovariables()
       {
           return tabla.TipoVariables.Where(TipoVariable => TipoVariable.id > 0).ToList();
       }

   }


}
