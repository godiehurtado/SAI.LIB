using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Produccion
{
   public class Beneficiarios
    {
        private SAI_Entities tabla = new SAI_Entities();

           
        public List<Beneficiario> ListarBeneficiarios()
        {
            return tabla.Beneficiarios.Include("Cliente").Where(Beneficiario => Beneficiario.id > 0).ToList();
        }

    
        public List<Beneficiario> ListarBeneficiariosPorId(int idBeneficiario)
        {
            return tabla.Beneficiarios.Where(Beneficiario => Beneficiario.id == idBeneficiario && Beneficiario.id > 0).ToList();
        }
             
    }

     public class Clientes
        {
            private SAI_Entities tabla = new SAI_Entities();

            public List<Cliente> ListarClientes()
            {
            return tabla.Clientes.Where(Cliente => Cliente.id > 0).ToList();
            }

        }


}
