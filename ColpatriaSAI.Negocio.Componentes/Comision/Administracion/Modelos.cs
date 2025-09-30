using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Administracion
{
    public class ConfigParametros
    {
        public int id { get; set; }
        public string parametro { get; set; }
        public string valor { get; set; }
        public string descripcion { get; set; }
    }

    public class parmbhExtractNoveades
    {
        public int NovNcode { get; set; }
        public string NovCDescription { get; set; }
        public string NovCtType { get; set; }
        public string Nca_Cdescription { get; set; }
    }
}
