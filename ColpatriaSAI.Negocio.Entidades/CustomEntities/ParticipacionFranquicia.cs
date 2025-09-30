using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ColpatriaSAI.Negocio.Entidades
{
    public partial class ParticipacionFranquicia
    {
        [DataMember]
        public string rangoParametros
        {
            get { return _rangoParametros; }
            set
            {
                _rangoParametros = "Desde: " + fecha_ini.ToShortDateString() + " Hasta: " + fecha_fin.Value.ToShortDateString();
            }
        }
        private string _rangoParametros;
    }
}
