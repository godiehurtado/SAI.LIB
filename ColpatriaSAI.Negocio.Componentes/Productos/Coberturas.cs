using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Productos
{
    public class Coberturas
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<Cobertura> ListarCobertura()
        {
            return tabla.Coberturas.Where(Cobertura => Cobertura.id > 0).OrderBy(p => p.nombre).ToList();
        }

        public List<Cobertura> ListarCoberturasPorId(int idCobertura)
        {
            return tabla.Coberturas.Where(Cobertura => Cobertura.id == idCobertura && Cobertura.id > 0).ToList();
        }

    }
}

