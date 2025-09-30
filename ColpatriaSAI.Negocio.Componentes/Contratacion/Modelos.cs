using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using System.Data.EntityClient;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Modelos
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado de los Modelos
        /// </summary>
        /// <returns>Lista de Modelos</returns>
        public List<Modelo> ListarModelos()
        {
            return tabla.Modeloes.Where(Modelo => Modelo.id > 0).ToList();
        }

        /// <summary>
        /// Obtiene listado de los Modelos por id
        /// </summary>
        /// <param name="id">Id del Nivel a consultar</param>
        /// <returns>Lista de objectos Modelos</returns>
        public List<Modelo> ListarModelosPorId(int id)
        {
            return tabla.Modeloes.Where(Modelo => Modelo.id == id && Modelo.id > 0).ToList();
        }

        public List<ModeloxMeta> ListarModelosxMetaPorIdModelo(int id)
        {
            return tabla.ModeloxMetas.Include("Modelo").Include("Meta").Include("FactorxNota").Where(ModeloxMeta => ModeloxMeta.modelo_id == id).ToList();
        }

        /// <summary>
        /// Obtiene listado de los participantes que pertenecen un determinado modelo
        /// </summary>
        /// <param name="id">Id del modelo</param>
        /// <returns>Lista de objectos ModeloxParticipante</returns>
        public List<ModeloxNodo> ListarModeloxParticipantesPorId(int id)
        {
            return tabla.ModeloxNodoes.Include("Modelo").Include("Nivel").Include("Zona").Include("Localidad").Include("JerarquiaDetalle")
                .Where(m => m.modelo_id == id).ToList();
        }

        /// <summary>
        /// Guarda un registro de Modelo
        /// </summary>
        /// <param name="modelo">Objecto Modelo a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarModelo(Modelo modelo, string Username)
        {
            int resultado = 0;

            if (modelo.id != 0 && modelo.id != null)
            {
                var modeloTemp = tabla.Modeloes.Where(m => m.id == modelo.id).FirstOrDefault();
                var pValorAntiguo = modeloTemp;
                modeloTemp.descripcion = modelo.descripcion;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Modelo,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, modeloTemp);
                tabla.SaveChanges();
                resultado = modelo.id;
            }
            else
            {
                if (tabla.Modeloes.Where(Modelo => Modelo.descripcion == modelo.descripcion).ToList().Count() == 0)
                {
                    tabla.Modeloes.AddObject(modelo);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Modelo,
    SegmentosInsercion.Personas_Y_Pymes, null, modelo);
                    tabla.SaveChanges();
                }

                resultado = tabla.Modeloes.Where(Modelo => Modelo.descripcion == modelo.descripcion).ToList()[0].id;
            }

            return resultado;
        }

        public string AsociarFactorToModelo(int modelo, int factor)
        {
            var modeloActual = this.tabla.Modeloes.Where(m => m.id == modelo).First();
            modeloActual.factorxnota_id = factor;
            return tabla.SaveChanges().ToString();
        }

        /// <summary>
        /// Guarda un registro de ModeloxMeta
        /// </summary>
        /// <param name="modeloxMeta">Objecto ModeloxMeta a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarModeloxMeta(ModeloxMeta modeloxMeta, string Username)
        {
            int resultado = 0;
            if (tabla.ModeloxMetas.Where(ModeloxMeta => ModeloxMeta.meta_id == modeloxMeta.meta_id && ModeloxMeta.peso == modeloxMeta.peso &&
                    ModeloxMeta.modelo_id == modeloxMeta.modelo_id).ToList().Count() == 0)
            {
                tabla.ModeloxMetas.AddObject(modeloxMeta);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ModeloxMeta,
    SegmentosInsercion.Personas_Y_Pymes, null, modeloxMeta);
                tabla.SaveChanges();
                resultado = modeloxMeta.id;
            }
            return resultado;
        }

        public int CargarModeloxMeta(List<ModelosContratacion> modeloxMeta, int esUltimaHoja)
        {
            int resultado = 0;

            var tabla = new System.Data.DataTable();
            tabla.Columns.Add("CodModelo", typeof(string)); tabla.Columns.Add("CodMeta", typeof(string));
            tabla.Columns.Add("NombreMeta", typeof(string)); tabla.Columns.Add("Peso", typeof(double)); tabla.Columns.Add("CodEscala", typeof(Int32));

            foreach (var item in modeloxMeta) tabla.Rows.Add(item.CodModelo, item.CodMeta, item.NombreMeta, item.Peso, item.CodEscala);

            var pTabla = new SqlParameter("@hoja", SqlDbType.Structured) { TypeName = "ModelosContratacion", Value = tabla };
            //pTabla.TypeName = "ModelosContratacion"; pTabla.Value = tabla;

            using (var conexion = ((EntityConnection)this.tabla.Connection).StoreConnection)
            {
                DbCommand comando = conexion.CreateCommand();
                comando.CommandText = "Contratacion_CargarInfo";
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.Add(pTabla);
                comando.Parameters.Add(new SqlParameter("esUltimaHoja", esUltimaHoja));

                bool abrirConexion = comando.Connection.State == ConnectionState.Closed;
                if (abrirConexion) { comando.Connection.Open(); comando.CommandTimeout = 1200; }

                try { resultado = comando.ExecuteNonQuery(); }
                catch { resultado = 0; }
            }
            return resultado;
        }

        /// <summary>
        /// Guarda un registro de ModeloxParticipante
        /// </summary>
        /// <param name="modeloxMeta">Objecto ModeloxParticipante a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarModeloxParticipante(ModeloxNodo modeloxPart, string Username)
        {
            int resultado = 0;
            if (tabla.ModeloxNodoes.Where(m => m.fechaIni == modeloxPart.fechaIni && m.fechaFin == modeloxPart.fechaFin &&
                    m.modelo_id == modeloxPart.modelo_id && m.nivel_id == modeloxPart.nivel_id && m.zona_id == modeloxPart.zona_id &&
                    m.localidad_id == modeloxPart.localidad_id && m.jerarquiaDetalle_id == modeloxPart.jerarquiaDetalle_id).ToList().Count() == 0)
            {
                tabla.ModeloxNodoes.AddObject(modeloxPart);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ModeloxNodo,
    SegmentosInsercion.Personas_Y_Pymes, null, modeloxPart);
                tabla.SaveChanges();
                resultado = modeloxPart.id;
            }
            return resultado;
        }

        /// <summary>
        /// Actualiza un registro de Modelo
        /// </summary>
        /// <param name="id">Id del Modelo a modificar</param>
        /// <param name="modelo">Objeto Modelo utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarModelo(int id, Modelo modelo, string Username)
        {
            var modeloActual = this.tabla.Modeloes.Where(Modelo => Modelo.id == id && Modelo.id > 0).First();
            var pValorAntiguo = modeloActual;
            modeloActual.descripcion = modelo.descripcion;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Modelo,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, modeloActual);
            return tabla.SaveChanges();
        }

        public int ActualizarModeloxMeta(int id, ModeloxMeta modeloxMeta, string Username)
        {
            var modeloxMetaActual = this.tabla.ModeloxMetas.Where(m => m.id == modeloxMeta.id).First();
            var pValorAntiguo = modeloxMetaActual;
            modeloxMetaActual.meta_id = modeloxMeta.meta_id;
            modeloxMetaActual.peso = modeloxMeta.peso;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ModeloxMeta,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, modeloxMetaActual);
            return tabla.SaveChanges();
        }

        /// <summary>
        /// Elimina un registro de Modelo
        /// </summary>
        /// <param name="id">Id del Modelo a eliminar</param>
        /// <param name="modelo">Objecto Modelo utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarModelo(int id, Modelo modelo, string Username)
        {
            var modeloActual = this.tabla.Modeloes.Where(Modelo => Modelo.id == id && Modelo.id > 0).First();
            tabla.DeleteObject(modeloActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Modelo,
    SegmentosInsercion.Personas_Y_Pymes, modeloActual, null);
                return tabla.SaveChanges().ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public int EliminarModeloxMetaPorModelos(List<int> codigosModelo, string Username)
        {
            foreach (var item in codigosModelo)
            {
                var modelosMetas = tabla.ModeloxMetas.Where(m => m.modelo_id == item).ToList();
                foreach (var modelometa in modelosMetas)
                {
                    tabla.ModeloxMetas.DeleteObject(modelometa);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ModeloxMeta,
    SegmentosInsercion.Personas_Y_Pymes, modelometa, null);
                }

            }
            return tabla.SaveChanges();
        }

        public string EliminarModeloxMeta(int id, ModeloxMeta modeloxMeta, string Username)
        {
            var modeloxMetaActual = this.tabla.ModeloxMetas.Where(m => m.id == id).First();
            tabla.DeleteObject(modeloxMetaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ModeloxMeta,
    SegmentosInsercion.Personas_Y_Pymes, modeloxMetaActual, null);
                return tabla.SaveChanges().ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string EliminarModeloxParticipante(int id, ModeloxNodo modeloxPart, string Username)
        {
            var modeloPartActual = this.tabla.ModeloxNodoes.Where(m => m.id == id).First();
            tabla.DeleteObject(modeloPartActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ModeloxNodo,
    SegmentosInsercion.Personas_Y_Pymes, modeloPartActual, null);
                return tabla.SaveChanges().ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

