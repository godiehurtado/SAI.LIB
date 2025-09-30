using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ColpatriaSAI.Negocio.Entidades.Informacion
{
    [Serializable]
    public class InfoAplicacion
    {
        [DataMember(IsRequired = false)]
        public string NombreUsuario
        {
            get { return nombreUsuario; }
            set { nombreUsuario = value; }
        }
        private string nombreUsuario;

        [DataMember(IsRequired = false)]
        public string DireccionIP
        {
            get { return direccionIP; }
            set { direccionIP = value; }
        }
        private string direccionIP;
    }
}