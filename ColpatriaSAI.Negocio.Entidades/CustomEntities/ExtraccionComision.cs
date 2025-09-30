using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColpatriaSAI.Negocio.Entidades.CustomEntities
{
    public class ExtraccionComision
    {
        public int id { get; set; }
        public string usuario { get; set; }
		public DateTime fecha { get; set; }
		public int estadoExtraccion_id { get; set; }
		public int año { get; set; }
		public int mes { get; set; }
		public int dia { get; set; }
		public int tipoLiquidacion { get; set; }
		public string CodigoExt { get; set; }
	}
}
