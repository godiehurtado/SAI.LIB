using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace ColpatriaSAI.Negocio.Entidades
{
    [Serializable]
    public class TempList
    {
        [DataMember(IsRequired = false)]
        public int id { get; set; }

        [DataMember(IsRequired = false)]
        public string nombre { get; set; }

        [DataMember(IsRequired = false)]
        public int resultado { get; set; }

    }

    [DataContract(IsReference = true)]
    public partial class NodoArbol
    {
        [DataMember]
        public string data { get; set; }

        [DataMember]
        public Atributo attr { get; set; }

        [DataMember]
        public string state { get; set; }

        [DataMember]
        public List<NodoArbol> children { get; set; }
    }

    [DataContract(IsReference = true)]
    public partial class Atributo
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string rel { get; set; }
        [DataMember]
        public string cod { get; set; }
    }

    [DataContract(IsReference = true)]
    public partial class DetalleLiquidacionEncabezado
    {
        [DataMember]
        public string concursoNombre { get; set; }
        [DataMember]
        public string reglaNombre { get; set; }
        [DataMember]
        public Nullable<DateTime> fecha_liquidacion { get; set; }
        [DataMember]
        public Nullable<DateTime> fecha_inicial { get; set; }
        [DataMember]
        public Nullable<DateTime> fecha_final { get; set; }
        [DataMember]
        public Nullable<int> tipoConcurso_id { get; set; }
        [DataMember]
        public Nullable<int> regla_id { get; set; }   
    }    

    [DataContract(IsReference = true)]
    public class PresupuestoDetalles
    {
        [DataMember]
        public String id { get; set; }
        /*[DataMember]
        public String Nombre_Nodo { get; set; }*/
        [DataMember]
        public String CodNivel { get; set; }
        /*[DataMember]
        public String Nombre_participante { get; set; }*/
        [DataMember]
        public String Codigo_meta { get; set; }
        [DataMember]
        public String Año { get; set; }
        [DataMember]
        public Double Enero { get; set; }
        [DataMember]
        public Double Febrero { get; set; }
        [DataMember]
        public Double Marzo { get; set; }
        [DataMember]
        public Double Abril { get; set; }
        [DataMember]
        public Double Mayo { get; set; }
        [DataMember]
        public Double Junio { get; set; }
        [DataMember]
        public Double Julio { get; set; }
        [DataMember]
        public Double Agosto { get; set; }
        [DataMember]
        public Double Septiembre { get; set; }
        [DataMember]
        public Double Octubre { get; set; }
        [DataMember]
        public Double Noviembre { get; set; }
        [DataMember]
        public Double Diciembre { get; set; }
    }

    [DataContract(IsReference = true)]
    public class ModelosContratacion
    {
        [DataMember]
        public String CodModelo { get; set; }
        [DataMember]
        public String CodMeta { get; set; }
        [DataMember]
        public String NombreMeta { get; set; }
        [DataMember]
        public Double Peso { get; set; }
        [DataMember]
        public int CodEscala { get; set; }
    }

    [DataContract(IsReference = true)]
    public class ModeloxMetaTemp
    {
        [DataMember]
        public string campo1 { get; set; }
        [DataMember]
        public double campo2 { get; set; }
    }

    [DataContract(IsReference = true)]
    public class NotificacionProceso
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public int? tipo { get; set; }
        [DataMember]
        public string mensaje { get; set; }
    }
}