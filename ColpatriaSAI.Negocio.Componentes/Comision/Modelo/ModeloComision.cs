using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponenteComisionFija = ColpatriaSAI.Negocio.Componentes.Comision.Fija;
using ComponenteComisionVariable = ColpatriaSAI.Negocio.Componentes.Comision.Variable;
using ComponenteModeloComision = ColpatriaSAI.Negocio.Componentes.Comision.Modelo;
using ColpatriaSAI.Negocio.Entidades;
using ColpatriaSAI.Datos;
using Entidades = ColpatriaSAI.Negocio.Entidades;
using System.Data.Entity;
using ComponenteMatrizComisionVariable = ColpatriaSAI.Negocio.Componentes.Comision.Variable;
using System.Data;
using Componentes = ColpatriaSAI.Negocio.Componentes;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Modelo
{

    public class ModeloComision : IModeloComision
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

        #region Comision Fija
        public List<Entidades.FactorComisionFija> ListarFactorComisionFijaXModeloId(int modeloId)
        {
            return (new ComponenteComisionFija.FactorComisionFija().ListarFactorComisionFijaXModeloId(modeloId));
        }

        public ResultadoOperacionBD InsertarFactorComisionFija(Entidades.FactorComisionFija objetoDb, string Username)
        {
            return (new ComponenteComisionFija.FactorComisionFija().InsertarFactorComisionFija(objetoDb, Username));
        }

        public ResultadoOperacionBD EliminarFactorComisionFija(Entidades.FactorComisionFija objetoDb, string Username)
        {
            return (new ComponenteComisionFija.FactorComisionFija().EliminarFactorComisionFija(objetoDb, Username));
        }

        public FactorComisionFija ObtenerFactorComisionFijaXId(int factorId)
        {
            return (new ComponenteComisionFija.FactorComisionFija().ObtenerFactorComisionFijaXId(factorId));
        }

        public ResultadoOperacionBD ActualizarFactorComisionFija(FactorComisionFija objetoDb, string Username)
        {
            return (new ComponenteComisionFija.FactorComisionFija().ActualizarFactorComisionFija(objetoDb, Username));
        }

        public Boolean ValidarEdadMaxima(Int32 edad)
        {
            return new ComponenteComisionFija.FactorComisionFija().ValidarEdad(edad);
        }
        #endregion


        #region Modelo Comisión

        public List<Entidades.ModeloComision> ListarModelosComision()
        {
            return (new ComponenteModeloComision.Modelo()).ListarModelosComision();
        }

        public Entidades.ModeloComision ObtenerModeloComisionXId(int modeloComisionId)
        {
            return (new ComponenteModeloComision.Modelo()).ObtenerModeloComisionXId(modeloComisionId);
        }

        public List<Entidades.Canal> ListarCanalesDetalle()
        {
            return (new ComponenteModeloComision.Modelo()).ListarCanalesDetalle();
        }

        public ResultadoOperacionBD InsertarModeloComision(Entidades.ModeloComision objetoDB, string Username)
        {
            return (new ComponenteModeloComision.Modelo()).InsertarModeloComision(objetoDB, Username);
        }

        public ResultadoOperacionBD ActualizarModeloComision(Entidades.ModeloComision objetoDB, string Username)
        {
            return (new ComponenteModeloComision.Modelo()).ActualizarModeloComision(objetoDB, Username);
        }

        public ResultadoOperacionBD EliminarModeloComision(int modeloId, string Username)
        {
            return (new ComponenteModeloComision.Modelo()).EliminarModeloComision(modeloId, Username);
        }

        public List<Entidades.ModeloComision> ListarModeloComisionVigentes()
        {
            return (new ComponenteModeloComision.Modelo()).ListarModeloComisionVigentes();
        }


        public ResultadoOperacionBD DuplicarModeloComision(int modeloId, Entidades.ModeloComision objetoDB, string Username)
        {
            return (new ComponenteModeloComision.Modelo()).DuplicarModeloComision(modeloId, objetoDB, Username);
        }
        #endregion

        #region Excepcion Penalización

        public List<Entidades._ExcepcionPenalizacion> ListarExcepcionesPenalizacionXModeloComision(int modeloComisionId)
        {
            List<_ExcepcionPenalizacion> excepciones = (from ex in _dbcontext.ExcepcionPenalizacions
                                                        join pao in _dbcontext.Participantes
                                                        on ex.participanteorigen_id equals pao.id
                                                        join pad in _dbcontext.Participantes
                                                        on ex.participantedestino_id equals pad.id
                                                        where ex.modelo_id == modeloComisionId
                                                        select new _ExcepcionPenalizacion()
                                                        {
                                                            NUMERO_CONTRATO = ex.numerocontrato,
                                                            CLAVE_ORIGEN = (int)ex.participanteorigen_id,
                                                            CLAVE_DESTINO = (int)ex.participantedestino_id,
                                                            MODELO = modeloComisionId,
                                                            EXCEPCION = ex.aplica,
                                                            DESCRIPCION_ERROR = "",
                                                            NOMBRE_ASESOR_ORIGEN = pao.nombre.Trim() + " " + pao.apellidos.Trim() + "/" + pao.clave.Trim(),
                                                            NOMBRE_ASESOR_DESTINO = pad.nombre.Trim() + " " + pad.apellidos.Trim() + "/" + pad.clave.Trim()
                                                        }).ToList();
            return excepciones;
        }

        public Entidades.ExcepcionPenalizacion ObtenerExcepcionPenalizacionXId(int penalizacionId)
        {
            return _dbcontext.ExcepcionPenalizacions
                .Include("Participante")
                .Include("Participante1")
                .Where(c => c.id == penalizacionId).FirstOrDefault();
        }

        public ResultadoOperacionBD InsertarExcepcionPenalizacion(Entidades.ExcepcionPenalizacion objetoDB, string Username)
        {
            return (new Excepciones.ExcepcionPenalizacion()).InsertarExcepcionPenalizacion(objetoDB, Username);
        }

        public ResultadoOperacionBD ActualizarExcepcionPenalizacion(Entidades.ExcepcionPenalizacion objetoDB, string Username)
        {
            return (new Excepciones.ExcepcionPenalizacion()).ActualizarExcepcionPenalizacion(objetoDB, Username);
        }

        public ResultadoOperacionBD EliminarExcepcionPenalizacion(Entidades.ExcepcionPenalizacion objetoDb, string Username)
        {
            return (new Excepciones.ExcepcionPenalizacion()).EliminarExcepcionPenalizacion(objetoDb, Username);
        }

        public List<Participante> ListarParticipantes()
        {
            return _dbcontext.Participantes.ToList();
        }

        public List<_ExcepcionPenalizacion> _lisErrores()
        {
            return (new ColpatriaSAI.Negocio.Componentes.Comision.Modelo.Excepciones.ExcepcionPenalizacion()._lisErrores());
        }


        public List<_ExcepcionPenalizacion> CargueExcepcionesPenalizacion(List<_ExcepcionPenalizacion> penalizacion)
        {
            return (new ColpatriaSAI.Negocio.Componentes.Comision.Modelo.Excepciones.ExcepcionPenalizacion().CargueExcepcionPenalizacion(penalizacion));
        }

        public ResultadoOperacionBD CambiarEstadoExcepcionPenalizacion(int excepcionId, int modeloId, string Username) {
            return (new Excepciones.ExcepcionPenalizacion()).CambiarEstadoExcepcionPenalizacion(excepcionId, modeloId, Username);
        }
        #endregion

        #region Excepcion Fija/Variable

        public List<Entidades.ExcepcionFijaVariable> ListarExcepcionesFijaVariableXModeloComision(int modeloComisionId)
        {
            return _dbcontext
                    .ExcepcionFijaVariables
                    .Include("Participante")
                    .Where(me => me.modelo_id == modeloComisionId)
                    .ToList();
        }

        public Entidades.ExcepcionFijaVariable ObtenerExcepcionFijaVariableXId(int excepcionFijaVariableId)
        {
            return _dbcontext.ExcepcionFijaVariables
                .Include("Participante")
                .Where(c => c.id == excepcionFijaVariableId).FirstOrDefault();
        }

        public ResultadoOperacionBD InsertarExcepcionFijaVariable(Entidades.ExcepcionFijaVariable objetoDB, string Username)
        {
            return (new Excepciones.ExcepcionFijaVariable()).InsertarExcepcionFijaVariable(objetoDB, Username);
        }

        public ResultadoOperacionBD ActualizarExcepcionFijaVariable(Entidades.ExcepcionFijaVariable objetoDB, string Username)
        {
            return (new Excepciones.ExcepcionFijaVariable()).ActualizarExcepcionFijaVariable(objetoDB, Username);
        }

        public ResultadoOperacionBD EliminarExcepcionFijaVariable(Entidades.ExcepcionFijaVariable objetoDb, string Username)
        {
            return (new Excepciones.ExcepcionFijaVariable()).EliminarExcepcionFijaVariable(objetoDb, Username);
        }

        public ResultadoOperacionBD CambiarEstadoExcepcionFijaVariable(int excepcionId, int modeloId, string Username)
        {
            return (new Excepciones.ExcepcionFijaVariable()).CambiarEstadoExcepcionFijaVariable(excepcionId, modeloId, Username);
        }

        public List<ResultadoOperacionBD> CargarExcepcionesMasivo(DataTable pData, out string pErrores, string Username,int modelo)
        {
            List<ResultadoOperacionBD> lresult = new List<ResultadoOperacionBD>();
            DataTable Errores = new DataTable();
            pErrores = string.Empty;
            ResultadoOperacionBD lRegTotales = new ResultadoOperacionBD() { MensajeError = "Registro Totales", Resultado = ResultadoOperacion.Exito, RegistrosAfectados = 0 };
            ResultadoOperacionBD lRegNoProcesados = new ResultadoOperacionBD() { MensajeError = "Registro No Procesados", Resultado = ResultadoOperacion.Error, RegistrosAfectados = 0 };
            ResultadoOperacionBD lRegProcesados = new ResultadoOperacionBD() { MensajeError = "Registro Procesados", Resultado = ResultadoOperacion.Exito, RegistrosAfectados = 0 }; ;
            if (pData.Rows.Count > 0)
            {
                lRegTotales.RegistrosAfectados = pData.Rows.Count;
                Errores = pData.Clone();
                foreach (DataRow lRow in pData.Rows)
                {
                    string TipoExcepcion = (lRow["TipoExcepcion"] == DBNull.Value ? string.Empty : lRow["TipoExcepcion"].ToString());
                    string lReferencia = (lRow["Excepcion para"] == DBNull.Value ? string.Empty : lRow["Excepcion para"].ToString());
                    string lClaveAsesor = (lRow["Clave"] == DBNull.Value ? string.Empty : lRow["Clave"].ToString());
                    string lFechaInicioVigencia = (lRow["FechaInicioVigencia (dd/MM/yyyy)"] == DBNull.Value ? string.Empty : lRow["FechaInicioVigencia (dd/MM/yyyy)"].ToString());
                    string lFechaFinVigencia = (lRow["FechaFinVigencia (dd/MM/yyyy)"] == DBNull.Value ? string.Empty : lRow["FechaFinVigencia (dd/MM/yyyy)"].ToString());
                    string lPorcentaje = (lRow["Porcentaje Comision"] == DBNull.Value ? string.Empty : lRow["Porcentaje Comision"].ToString());
                    string lTipoContrato = (lRow["Excepción Por"] == DBNull.Value ? string.Empty : lRow["Excepción Por"].ToString());

                    if (string.IsNullOrEmpty(TipoExcepcion) 
                        || string.IsNullOrEmpty(lReferencia) 
                        || string.IsNullOrEmpty(lClaveAsesor)
                        || string.IsNullOrEmpty(lFechaInicioVigencia) 
                        || string.IsNullOrEmpty(lFechaFinVigencia) 
                        || string.IsNullOrEmpty(lPorcentaje) 
                        || string.IsNullOrEmpty(lTipoContrato))
                    {
                        lRegNoProcesados.RegistrosAfectados += 1;
                        lRow["Error"] = "Informacion faltante";
                        Errores.ImportRow(lRow);
                    }
                    else if (Convert.ToDateTime(lFechaInicioVigencia).Date > Convert.ToDateTime(lFechaFinVigencia).Date)
                    {
                        lRegNoProcesados.RegistrosAfectados += 1;
                        lRow["Error"] = "La fecha de inicio de vigencia no puede ser superior a la fecha del fin de vigencia";
                        Errores.ImportRow(lRow);
                    }
                    else
                    {
                        ExcepcionFijaVariable lExepcion = new ExcepcionFijaVariable();

                        lExepcion.tipoexcepcionfijavariable_id = (string.IsNullOrEmpty(TipoExcepcion) ? Convert.ToByte(1) : (TipoExcepcion.ToUpper().Contains("BLOQUEO") ? Convert.ToByte(3) : (TipoExcepcion.ToUpper().Equals("COMISION TOTAL") ? Convert.ToByte(1) : (TipoExcepcion.ToUpper().Equals("COMISION FIJA") ? Convert.ToByte(2) : Convert.ToByte(1)))));

                        lExepcion.numerocontrato = lReferencia;
                        Participante lp = new Participante();
                        List<Participante> lAsesores = _dbcontext.Participantes.Where(x=>x.clave == lClaveAsesor).ToList();
                        if (lAsesores.Any())
                        {
                            if (lAsesores.Count > 0)
                            {
                                lp = lAsesores.Where(x => x.clave.Trim() == lClaveAsesor).FirstOrDefault();
                                lExepcion.participante_id = lp.id;
                                lExepcion.fechavigencia = Convert.ToDateTime(lFechaInicioVigencia);
                                lExepcion.fechafinvigencia = Convert.ToDateTime(lFechaFinVigencia);
                                lExepcion.porcentajecomision = double.Parse(lPorcentaje);
                                Entidades.ModeloComision tmpmdc = _dbcontext.ModeloComisions.Where(x => x.id.Equals(modelo)).FirstOrDefault();
                                lExepcion.modelo_id = modelo;
                                lExepcion.activo = true;
                                lExepcion.excepcionpor = (byte)(lTipoContrato.ToUpper().Contains("BLOQUEO") ? Entidades.ExcepcionPor.BloqueoPago : lTipoContrato.ToUpper().Contains("CLAVE") ? Entidades.ExcepcionPor.PorClave : lTipoContrato.ToUpper().Contains("GRUPO") ? Entidades.ExcepcionPor.GrupoAsociativo : (lTipoContrato.ToUpper().StartsWith("CONTRATO") ? Entidades.ExcepcionPor.Contrato : Entidades.ExcepcionPor.Subcontrato));
                                
                                var lListaExcepciones = ListarExcepcionesFijaVariableXModeloComision(tmpmdc.id).Where(x => x.numerocontrato == lExepcion.numerocontrato && x.participante_id == lExepcion.participante_id && lExepcion.fechavigencia < x.fechafinvigencia).ToList();
                                if (lListaExcepciones.Any())
                                {
                                    lRegNoProcesados.RegistrosAfectados += 1;
                                    lRow["Error"] = "Ya existe un excepcion con estas caracteristicas";
                                    Errores.ImportRow(lRow);
                                }
                                else
                                {
                                    if (lExepcion.porcentajecomision > tmpmdc.tope)
                                    {
                                        lRegNoProcesados.RegistrosAfectados += 1;
                                        lRow["Error"] = "El porcentaje excede el tope del modelo";
                                        Errores.ImportRow(lRow);
                                    }
                                    else
                                    {
                                        ResultadoOperacionBD linserto = InsertarExcepcionFijaVariable(lExepcion, Username);
                                        if (linserto.Resultado == ResultadoOperacion.Exito)
                                        {
                                            lRegProcesados.RegistrosAfectados += 1;
                                        }
                                        else
                                        {
                                            lRegNoProcesados.RegistrosAfectados += 1;
                                            lRow["Error"] = linserto.MensajeError;
                                            Errores.ImportRow(lRow);
                                        }
                                    }                                    
                                }
                            }
                            else
                            {
                                lRegNoProcesados.RegistrosAfectados += 1;
                                lRow["Error"] = "No se encontró la clave " + lClaveAsesor;
                                Errores.ImportRow(lRow);
                            }
                        }
                        else
                        {
                            lRegNoProcesados.RegistrosAfectados += 1;
                            lRow["Error"] = "No se encontró la clave " + lClaveAsesor;
                            Errores.ImportRow(lRow);
                        }
                    }

                }
            }

            lresult.Add(lRegTotales);
            lresult.Add(lRegNoProcesados);
            lresult.Add(lRegProcesados);
            pErrores = RetornaErrores(Errores);
            return lresult;
        }

        private string RetornaErrores(DataTable pData)
        {
            string lResult = string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (DataRow dr in pData.Rows)
            {
                foreach (DataColumn dc in pData.Columns)
                    sb.Append(FormatCSV(dr[dc.ColumnName].ToString()) + ",");

                sb.Remove(sb.Length - 1, 1);
                sb.AppendLine();
            }
            lResult = sb.ToString();
            return lResult;

        }

        private static string FormatCSV(string linea)
        {
            try
            {
                if (linea == null)
                    return string.Empty;

                bool ContieneCaracteres = false;
                bool ContieneComas = false;
                int len = linea.Length;
                for (int i = 0; i < len && (ContieneComas == false || ContieneCaracteres == false); i++)
                {
                    char ch = linea[i];
                    if (ch == '"')
                        ContieneCaracteres = true;
                    else if (ch == ',')
                        ContieneComas = true;
                }

                if (ContieneCaracteres && ContieneComas)
                    linea = linea.Replace("\"", "\"\"");

                if (ContieneComas)
                    return "\"" + linea + "\"";
                else
                    return linea;
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region Comision Variable
        #region Factores Netos

        public List<FactorComisionVariableNeto> ListarFactorNetoComisionVariableXModeloId(int modeloId)
        {
            return (new ComponenteComisionVariable.FactorNeto()).ListarFactorNetoComisionVariableXModeloId(modeloId);
        }

        public ResultadoOperacionBD InsertarFactorNetoComisionVariable(FactorComisionVariableNeto dbobj, string Username)
        {
            return (new ComponenteComisionVariable.FactorNeto().InsertarFactorNeto(dbobj, Username));
        }

        public ResultadoOperacionBD EliminarFactorNetoComisionVariable(FactorComisionVariableNeto dbobj, string Username)
        {
            return (new ComponenteComisionVariable.FactorNeto().EliminarFactorNeto(dbobj, Username));
        }

        public FactorComisionVariableNeto ObtenerFactorNetoComisionVariableXId(int factorId)
        {
            return (new ComponenteComisionVariable.FactorNeto().ObtenerFactorNetoComisionVariableXId(factorId));
        }

        public ResultadoOperacionBD ActualizarFactorNetoComisionVariable(FactorComisionVariableNeto dbobj, string Username)
        {
            return (new ComponenteComisionVariable.FactorNeto().ActualizarFactorNeto(dbobj, Username));
        }
        #endregion

        #region Factores Nuevos
        public List<FactorComisionVariableNuevo> ListarFactorNuevoComisionVariableXModeloId(int modeloId)
        {
            return (new ComponenteComisionVariable.FactorNuevo()).ListarFactorNuevoComisionVariableXModeloId(modeloId);
        }

        public ResultadoOperacionBD InsertarFactorNuevoComisionVariable(FactorComisionVariableNuevo dbobj, string Username)
        {
            return (new ComponenteComisionVariable.FactorNuevo().InsertarFactorNuevo(dbobj, Username));
        }

        public ResultadoOperacionBD EliminarFactorNuevoComisionVariable(FactorComisionVariableNuevo dbobj, string Username)
        {
            return (new ComponenteComisionVariable.FactorNuevo().EliminarFactorNuevo(dbobj, Username));
        }

        public FactorComisionVariableNuevo ObtenerFactorNuevoComisionVariableXId(int factorId)
        {
            return (new ComponenteComisionVariable.FactorNuevo().ObtenerFactorNuevoComisionVariableXId(factorId));
        }

        public ResultadoOperacionBD ActualizarFactorNuevoComisionVariable(FactorComisionVariableNuevo dbobj, string Username)
        {
            return (new ComponenteComisionVariable.FactorNuevo().ActualizarFactorNuevo(dbobj, Username));
        }
        #endregion
        #endregion

        #region Matriz Comision Variable

        public ResultadoOperacionBD AdicionarRangosMatriz(int modeloId, List<Entidades.RangosXNeto> ejex, List<Entidades.RangosYNuevo> ejey, string Username)
        {
            return (new ComponenteMatrizComisionVariable.Matriz()).AdicionarRangosMatriz(modeloId, ejex, ejey, Username);
        }

        public ResultadoOperacionBD EliminarMatrizComisionVariableXModeloId(int modeloId, string Username)
        {
            return (new ComponenteMatrizComisionVariable.Matriz()).EliminarMatrizComisionVariableXModeloId(modeloId, Username);
        }

        public List<Entidades.MatrizComisionVariable> ListarValoresMatrizComisionVariableXModelo(int modeloId)
        {
            return (new ComponenteMatrizComisionVariable.Matriz()).ListarValoresMatrizComisionVariableXModelo(modeloId);
        }

        public List<Entidades.RangosYNuevo> ListarRangosYNuevoMatrizComisionVariableXModelo(int modeloId)
        {
            return (new ComponenteMatrizComisionVariable.Matriz()).ListarRangosYNuevoMatrizComisionVariableXModelo(modeloId);
        }

        public List<Entidades.RangosXNeto> ListarRangosXNetoMatrizComisionVariableXModelo(int modeloId)
        {
            return (new ComponenteMatrizComisionVariable.Matriz()).ListarRangosXNetoMatrizComisionVariableXModelo(modeloId);

        }

        public ResultadoOperacionBD ActualizarFactorMatrizComisionVariable(Entidades.MatrizComisionVariable dbobj, string Username)
        {
            return (new ComponenteMatrizComisionVariable.Matriz()).ActualizarFactorMatrizComisionVariable(dbobj, Username);
        }
        #endregion

        //public List<LiquidacionComision> ListarLiquidacionComision()
        //{
        //    return (new ComponenteModeloComision.Modelo()).ListarLiquidacionComision);
        //}


        public List<LiquidacionComisionClass> ListarLiquidacionComision()
        {
            return (new ComponenteModeloComision.Modelo()).ListarLiquidacionComision();
        }


        public List<Entidades.TipoIntermediario> ListarTipoIntermediario()
        {
            return (new ComponenteModeloComision.Modelo()).ListarTipoIntermediario();
        }

        public List<Entidades.CanalDetalleTipoIntermediario> ListarCanalDetalleTipoIntermediario()
        {
            return (new ComponenteModeloComision.Modelo()).ListarCanalDetalleTipoIntermediario();
        }
    }
}
