using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ColpatriaSAI.Negocio.Entidades;
using System.Data;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Administracion
{
    [ServiceContract]
    public interface IAdminComision
    {

        [OperationContract]
        List<ConfigParametros> ObtenerParametros(String[] parametros);

    }
}
