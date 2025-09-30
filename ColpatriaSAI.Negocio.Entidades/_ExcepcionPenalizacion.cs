using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ColpatriaSAI.Negocio.Entidades
{
    [DataContract(IsReference = true)]
    public class _ExcepcionPenalizacion
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string NUMERO_CONTRATO { get; set; }
        [DataMember]
        public int CLAVE_ORIGEN { get; set; }
        [DataMember]
        public int CLAVE_DESTINO { get; set; }
        [DataMember]
        public int MODELO { get; set; }
        [DataMember]
        public string EXCEPCION { get; set; }
        [DataMember]
        public string DESCRIPCION_ERROR { get; set; }
        [DataMember]
        public string NOMBRE_ASESOR_ORIGEN { get; set; }
        [DataMember]
        public string NOMBRE_ASESOR_DESTINO { get; set; }
        [DataMember]
        public bool ACTIVO { get; set; }
    }

}
