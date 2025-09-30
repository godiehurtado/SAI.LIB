using System;
using System.Collections.Generic;
using System.Linq;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using ColpatriaSAI.Negocio.Validadores.ModeloComision;
using ComponenteMatrizComisionVariable = ColpatriaSAI.Negocio.Componentes.Comision.Variable;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Modelo
{
    public class Modelo
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

        public List<Entidades.ModeloComision> ListarModelosComision()
        {
            return _dbcontext
                    .ModeloComisions
                    .ToList();
        }


        public List<LiquidacionComisionClass> ListarLiquidacionComision()
        {
            List<LiquidacionComision> ListLiqCom = new List<LiquidacionComision>();
            ListLiqCom = _dbcontext.LiquidacionComisions.ToList();

            List<LiquidacionComisionClass> LliqComision = new List<LiquidacionComisionClass>();

            for (int i = ListLiqCom.Count - 1; i >=0; i--)
            {
                LiquidacionComisionClass data = new LiquidacionComisionClass();
                if (ListLiqCom.Count == 1)
                {

                    data.idLiqComision = ListLiqCom[i].id.ToString() + "-" + "[Anterior a] " + ListLiqCom[i].fecha.ToString("dd/MM/yyyy");
                    data.Fecha = "[Anterior a] " + ListLiqCom[i].fecha.ToString("dd/MM/yyyy");

                    LliqComision.Add(data);
                    
                }
                else if ( i==0 )
                {

                    data.idLiqComision = ListLiqCom[i].id.ToString() + "-" + "[Anterior a] " + ListLiqCom[i].fecha.ToString("dd/MM/yyyy");
                    data.Fecha = "[Anterior a] " + ListLiqCom[i].fecha.ToString("dd/MM/yyyy");

                    LliqComision.Add(data);
                    
                }else
                {
                    data.idLiqComision = ListLiqCom[i].id.ToString() + "-" + ListLiqCom[i].fecha.ToString("dd/MM/yyyy"); 
                    data.Fecha = ListLiqCom[i].fecha.ToString("dd/MM/yyyy") + " - " + ListLiqCom[i - 1].fecha.ToString("dd/MM/yyyy");
                    LliqComision.Add(data);
                }

            }

            return LliqComision.ToList();
        }


        public List<Entidades.Canal> ListarCanalesDetalle()
        {
            return _dbcontext
                    .Canals
                    .Include("CanalDetalles")
                    .Where(x => x.id > 0)
                    .ToList();
        }

        public List<Entidades.TipoIntermediario> ListarTipoIntermediario()
        {
            return _dbcontext
                      .TipoIntermediarios
                      .ToList();
        }

        public List<Entidades.CanalDetalleTipoIntermediario> ListarCanalDetalleTipoIntermediario()
        {
            return _dbcontext
                .CanalDetalleTipoIntermediarios
                .Where(x => x.Estado_Id == 1)
                .ToList();
        }
        
        public Entidades.ModeloComision ObtenerModeloComisionXId(int modeloComisionId)
        {
            return _dbcontext
                    .ModeloComisions
                    .Include("CanalDetalleTipoIntermediarios")
                    .Include("CanalDetalles")
                    .Include("CanalDetalles.Canal")                    
                    .Where(x => x.id == modeloComisionId)
                    .FirstOrDefault();
        }

        public ResultadoOperacionBD InsertarModeloComision(Entidades.ModeloComision objetoDB, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                if (this.ValidarCanalDetalles(objetoDB))
                {
                    Entidades.ModeloComision tmpdbobj = new Entidades.ModeloComision()
                            {
                                codigo = objetoDB.nombre.Substring(0, 4),
                                fechadesde = objetoDB.fechadesde,
                                fechahasta = objetoDB.fechahasta,
                                nombre = objetoDB.nombre,
                                tope = objetoDB.tope,
                                descripcion = objetoDB.descripcion
                            };

                    foreach (var itemCanalDetalle in objetoDB.CanalDetalles)
                    {
                        var canalDetalle = _dbcontext.CanalDetalles.
                            Where(c => c.canal_id == itemCanalDetalle.canal_id && c.id == itemCanalDetalle.id).FirstOrDefault();
                        tmpdbobj.CanalDetalles.Add(canalDetalle);
                    }

                    foreach (var itemTipoInter in objetoDB.CanalDetalleTipoIntermediarios)
                    {
                        var TipoIntermediario = _dbcontext.CanalDetalleTipoIntermediarios.
                            Where(x => x.Id == itemTipoInter.Id).FirstOrDefault();
                        tmpdbobj.CanalDetalleTipoIntermediarios.Add(TipoIntermediario);
                    }

                    if (!ValidadorModeloComision.EditableSegunVigenciaModelo(objetoDB.fechadesde))
                    {
                        result.RegistrosAfectados = 0;
                        result.Resultado = ResultadoOperacion.Error;
                        result.MensajeError = "No se puede crear el modelo. El inicio de la vigencia no puede ser el mes actual";
                    }
                    else
                    {
                        _dbcontext.ModeloComisions.AddObject(tmpdbobj);
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ModeloComision,
                       SegmentosInsercion.Personas_Y_Pymes, null, tmpdbobj);
                        result.RegistrosAfectados = 1;
                        result.Resultado = ResultadoOperacion.Exito;
                        _dbcontext.SaveChanges();
                    }
                }
                else
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede crear un modelo con canales de otro modelo vigente";
                }
            }
            catch (Exception ex)
            {
                result.RegistrosAfectados = 0;
                result.Resultado = ResultadoOperacion.Error;
                result.MensajeError = ex.Message;
            }
            return result;
        }

        private bool ValidarCanalDetalles(Entidades.ModeloComision obj, int? modeloId = null)
        {
            int[] tcds = obj.CanalDetalleTipoIntermediarios.Select(x => x.Id).ToArray();
            bool res = false;
            String sql = "select mc.* from ModeloComisionTipoIntermediario mcti"
                    + " inner join ModeloComision mc on mcti.modelo_id = mc.id";
            if (modeloId.HasValue)
            {
                sql += " and mc.id <> " + modeloId.Value.ToString();
            }
            sql += " where mcti.CanalDetalleTipoIntermediario_Id in (";
            for (int i = 0; i < obj.CanalDetalleTipoIntermediarios.Count; i++)
            {
                sql += obj.CanalDetalleTipoIntermediarios[i].Id;
                if (i < obj.CanalDetalleTipoIntermediarios.Count - 1)
                {
                    sql += ",";
                }
            }
            sql += ")";
            sql += " AND mc.fechahasta between @ini and @fin";           
            sql += ")";
            
            var query = _dbcontext.ExecuteStoreQuery<Entidades.ModeloComision>(sql,
                new SqlParameter("@ini", obj.fechadesde),
                new SqlParameter("@fin", obj.fechahasta));
            return query.Count() == 0;
            //return true; // Siempre retorna true debido a error
        }

        public ResultadoOperacionBD ActualizarModeloComision(Entidades.ModeloComision objetoDB, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var query = _dbcontext
                            .ModeloComisions
                            .Include("CanalDetalles")
                            .Include("CanalDetalleTipoIntermediarios")
                            .Where(x =>
                                    x.id == objetoDB.id);

                if (query.Count() != 1)
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "Modelo no existente.";
                }
                // CAAM : 16-04.2020 _ Validaciones Removidas para dejar editar 
                // los factores mas fluidamente x solicitud de negocio - PM11454
                /*else if (!ValidadorModeloComision.EditableSegunVigenciaModelo(query.First().fechadesde))
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede modificar el modelo. El modelo se encuentra vigente";
                }
                else if (!ValidadorModeloComision.EditableSegunVigenciaModelo(objetoDB.fechadesde))
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede modificar el modelo. El inicio de la vigencia no puede ser el mes actual";
                }*/
                else
                {
                    // CAAM : 16-04.2020 _ Validaciones Removidas para dejar editar 
                    // los factores mas fluidamente x solicitud de negocio - PM11454
                    //if (this.ValidarCanalDetalles(objetoDB,objetoDB.id))
                    //{
                    var tmpobj = query.First();
                    
                     string pValorAntiguo = Utilidades.Auditoria.CrearDescripcionAuditoria(
                      new
                      {
                          INICIO = tmpobj.codigo,
                          Nombre = tmpobj.nombre,
                          FechaDesde = tmpobj.fechadesde ,
                          FechaHasta = tmpobj.fechahasta ,
                          Codigo = tmpobj.codigo ,
                          Tope = tmpobj.tope,
                          Descripcion = tmpobj.descripcion,
                          FIN = tmpobj.descripcion

                      }); ;

                    tmpobj.nombre = objetoDB.nombre;
                    tmpobj.fechadesde = objetoDB.fechadesde;
                    tmpobj.fechahasta = objetoDB.fechahasta;
                    tmpobj.codigo = objetoDB.id.ToString();
                    tmpobj.tope = objetoDB.tope;
                    tmpobj.descripcion = objetoDB.descripcion;

               

                    // Log Auditoria
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ModeloComision,
                   SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, tmpobj);

                       

                    // CAAM : 16-04.2020 _ Validaciones Removidas para dejar editar 
                    // los factores mas fluidamente x solicitud de negocio - PM11454

                    //foreach (var todel in tmpobj.CanalDetalles.ToList())
                    //    {
                    //        var qtodel = _dbcontext.CanalDetalles.Where(x => x.id == todel.id).First();
                    //        if (objetoDB.CanalDetalles.Where(x => x.id == qtodel.id).Count() == 0)
                    //        {
                    //            tmpobj.CanalDetalles.Remove(qtodel);
                    //        }
                    //    }
                    //    foreach (var itemCanalDetalle in objetoDB.CanalDetalles)
                    //    {
                    //        var canalDetalle = _dbcontext.CanalDetalles.
                    //            FirstOrDefault(c => c.canal_id == itemCanalDetalle.canal_id && c.id == itemCanalDetalle.id);
                    //        if (tmpobj.CanalDetalles.Where(x => x.id == canalDetalle.id).Count() == 0)
                    //        {
                    //            tmpobj.CanalDetalles.Add(canalDetalle);
                    //        }
                    //    }
                    //    foreach (var tointer in tmpobj.CanalDetalleTipoIntermediarios.ToList())
                    //    {
                    //        var qinter = _dbcontext.CanalDetalleTipoIntermediarios.Where(x => x.Id == tointer.Id).First();
                    //        if (objetoDB.CanalDetalleTipoIntermediarios.Where(x => x.Id == qinter.Id).Count() == 0)
                    //        {
                    //            tmpobj.CanalDetalleTipoIntermediarios.Remove(qinter);
                    //        }
                    //    }
                    //    foreach (var itemTipoIntermediario in objetoDB.CanalDetalleTipoIntermediarios)
                    //    {
                    //        var TipoInter = _dbcontext.CanalDetalleTipoIntermediarios.
                    //            FirstOrDefault(c => c.Id == itemTipoIntermediario.Id );
                    //        if (tmpobj.CanalDetalleTipoIntermediarios.Where(x => x.Id == TipoInter.Id).Count() == 0)
                    //        {
                    //            tmpobj.CanalDetalleTipoIntermediarios.Add(TipoInter);
                    //        }
                    //    }

                        result.RegistrosAfectados = 1;
                        result.Resultado = ResultadoOperacion.Exito;
                        _dbcontext.SaveChanges();
                    // CAAM : 16-04.2020 _ Validaciones Removidas para dejar editar 
                    // los factores mas fluidamente x solicitud de negocio - PM11454
                    //}
                    //else
                    //{
                    //    result.RegistrosAfectados = 0;
                    //    result.Resultado = ResultadoOperacion.Error;
                    //    result.MensajeError = "No se puede crear un modelo con canales de otro modelo vigente";
                    //}
                }

            }
            catch (Exception ex)
            {
                result.RegistrosAfectados = 0;
                result.Resultado = ResultadoOperacion.Error;
                result.MensajeError = ex.Message;
            }
            return result;
        }

        private ResultadoOperacionBD EliminarCanalDetalleXModeloComision(Entidades.ModeloComision objetoDB, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                if (!ValidadorModeloComision.EditableSegunVigenciaModelo(objetoDB.fechadesde))
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede modificar el modelo. El modelo se encuentra vigente";
                }
                else
                {
                    foreach (var itemCanalDetalle in objetoDB.CanalDetalles)
                    {
                        var canalDetalle = _dbcontext.CanalDetalles.
                            FirstOrDefault(c => c.canal_id == itemCanalDetalle.canal_id && c.id == itemCanalDetalle.id);
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ModeloComisionCanalDetalle,
                   SegmentosInsercion.Personas_Y_Pymes, canalDetalle, null);
                        objetoDB.CanalDetalles.Remove(canalDetalle);
                    }
                    result.RegistrosAfectados = 1;
                    result.Resultado = ResultadoOperacion.Exito;
                    _dbcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.RegistrosAfectados = 0;
                result.Resultado = ResultadoOperacion.Error;
                result.MensajeError = ex.Message;
            }
            return result;
        }

        private ResultadoOperacionBD EliminarTipoIntermediarioXModeloComision(Entidades.ModeloComision objetoDB, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                if (!ValidadorModeloComision.EditableSegunVigenciaModelo(objetoDB.fechadesde))
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede modificar el modelo. El modelo se encuentra vigente";
                }
                else
                {
                    foreach (var itemTipoInter in objetoDB.CanalDetalleTipoIntermediarios)
                    {
                        var tipoIntermediario = _dbcontext.CanalDetalleTipoIntermediarios.
                            FirstOrDefault(c => c.Id == itemTipoInter.Id);
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ModeloComisionTipoIntermediario,
                   SegmentosInsercion.Personas_Y_Pymes, tipoIntermediario, null);
                        objetoDB.CanalDetalleTipoIntermediarios.Remove(tipoIntermediario);
                    }
                    result.RegistrosAfectados = 1;
                    result.Resultado = ResultadoOperacion.Exito;
                    _dbcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.RegistrosAfectados = 0;
                result.Resultado = ResultadoOperacion.Error;
                result.MensajeError = ex.Message;
            }
            return result;
        }
        public ResultadoOperacionBD EliminarModeloComision(int modeloId, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var querymodel = _dbcontext.ModeloComisions
                     .Include("CanalDetalles")
                     .Include("CanalDetalleTipoIntermediarios")
                     .Include("ExcepcionFijaVariables")
                     .Include("ExcepcionPenalizacions")
                     .Include("FactorComisionFijas")
                     .Include("FactorComisionVariableNetoes")
                     .Include("FactorComisionVariableNuevoes")
                     .Include("MatrizComisionVariables")
                     .Include("MatrizComisionVariables.RangosXNeto")
                     .Include("MatrizComisionVariables.RangosYNuevo")
                     .Where(x => x.id == modeloId).First();

                if (!ValidadorModeloComision.EditableSegunVigenciaModelo(querymodel.fechadesde))
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede eliminar el modelo. El modelo se encuentra vigente";
                }
                else
                {
                    result.RegistrosAfectados = 0;
                    foreach (var item in querymodel.ExcepcionFijaVariables.ToList())
                    {
                        _dbcontext.ExcepcionFijaVariables.DeleteObject(item);
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ExcepcionFijaVariable,
                        SegmentosInsercion.Personas_Y_Pymes, item, null);
                        result.RegistrosAfectados++;
                    }
                    foreach (var item in querymodel.ExcepcionPenalizacions.ToList())
                    {
                        _dbcontext.ExcepcionPenalizacions.DeleteObject(item); new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ExcepcionPenalizacion,
                        SegmentosInsercion.Personas_Y_Pymes, item, null);
                        result.RegistrosAfectados++;
                    }
                    foreach (var item in querymodel.FactorComisionFijas.ToList())
                    {
                        _dbcontext.FactorComisionFijas.DeleteObject(item); new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.FactorComisionFija,
                        SegmentosInsercion.Personas_Y_Pymes, item, null);
                        result.RegistrosAfectados++;
                    }
                    foreach (var item in querymodel.FactorComisionVariableNetoes.ToList())
                    {
                        _dbcontext.FactorComisionVariableNetoes.DeleteObject(item); new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.FactorComisionVariableNeto,
                        SegmentosInsercion.Personas_Y_Pymes, item, null);
                        result.RegistrosAfectados++;
                    }
                    foreach (var item in querymodel.FactorComisionVariableNuevoes.ToList())
                    {
                        _dbcontext.FactorComisionVariableNuevoes.DeleteObject(item); new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.FactorComisionVariableNuevo,
                        SegmentosInsercion.Personas_Y_Pymes, item, null); result.RegistrosAfectados++;
                    }
                    foreach (var item in querymodel.CanalDetalles.ToList())
                    {
                        querymodel.CanalDetalles.Remove(item); new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ModeloComisionCanalDetalle,
                        SegmentosInsercion.Personas_Y_Pymes, item, null); result.RegistrosAfectados++;
                    }
                    foreach (var item in querymodel.CanalDetalleTipoIntermediarios.ToList())
                    {
                        querymodel.CanalDetalleTipoIntermediarios.Remove(item); new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ModeloComisionCanalDetalle,
                        SegmentosInsercion.Personas_Y_Pymes, item, null); result.RegistrosAfectados++;
                    }
                    foreach (var item in querymodel.MatrizComisionVariables.ToList())
                    {
                        _dbcontext.MatrizComisionVariables.DeleteObject(item); new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.MatrizComisionVariable,
                        SegmentosInsercion.Personas_Y_Pymes, item, null); result.RegistrosAfectados++;
                    }
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ModeloComision,
                    SegmentosInsercion.Personas_Y_Pymes, querymodel, null);
                    _dbcontext.ModeloComisions.DeleteObject(querymodel);
                    result.Resultado = ResultadoOperacion.Exito;
                    _dbcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.RegistrosAfectados = 0;
                result.Resultado = ResultadoOperacion.Error;
                result.MensajeError = ex.Message;
            }
            return result;
        }

        internal List<Entidades.ModeloComision> ListarModeloComisionVigentes()
        {
            return _dbcontext.ModeloComisions
                .Where(x => x.fechadesde <= DateTime.Now && DateTime.Now < x.fechahasta).ToList();
        }

        public ResultadoOperacionBD DuplicarModeloComision(int modeloId, Entidades.ModeloComision objetoDB, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                int modeloNuevoID = 0;
                var querymodel = _dbcontext.ModeloComisions
                    .Include("CanalDetalles")
                    .Include("ExcepcionFijaVariables")
                    .Include("ExcepcionPenalizacions")
                    .Include("FactorComisionFijas")
                    .Include("FactorComisionVariableNetoes")
                    .Include("FactorComisionVariableNuevoes")
                    .Include("MatrizComisionVariables")
                    .Include("MatrizComisionVariables.RangosXNeto")
                    .Include("MatrizComisionVariables.RangosYNuevo")
                    .Where(x => x.id == modeloId).First();

                result.RegistrosAfectados = 0;
                objetoDB.id = 0;
                if (InsertarModeloComision(objetoDB, Username).Resultado == ResultadoOperacion.Exito)
                {

                    modeloNuevoID = _dbcontext.ModeloComisions.ToList().Last().id;
                    objetoDB = ObtenerModeloComisionXId(modeloNuevoID);

                    foreach (var item in querymodel.ExcepcionFijaVariables.ToList())
                    {
                        Entidades.ExcepcionFijaVariable itemtemp = new ExcepcionFijaVariable()
                        {
                            tipoexcepcionfijavariable_id = item.tipoexcepcionfijavariable_id,
                            numerocontrato = item.numerocontrato,
                            participante_id = item.participante_id,
                            porcentajecomision = item.porcentajecomision,
                            fechavigencia = item.fechavigencia,
                            modelo_id = modeloNuevoID,
                            ModeloComision = objetoDB,
                            Participante = item.Participante
                        };
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ExcepcionFijaVariable,
SegmentosInsercion.Personas_Y_Pymes, null, itemtemp);
                        _dbcontext.ExcepcionFijaVariables.AddObject(itemtemp);
                        result.RegistrosAfectados++;
                    }
                    foreach (var item in querymodel.ExcepcionPenalizacions.ToList())
                    {
                        Entidades.ExcepcionPenalizacion itemtemp = new ExcepcionPenalizacion()
                        {
                            numerocontrato = item.numerocontrato,
                            participanteorigen_id = item.participanteorigen_id,
                            participantedestino_id = item.participantedestino_id,
                            Participante = item.Participante,
                            Participante1 = item.Participante1,
                            aplica = item.aplica,
                            modelo_id = objetoDB.id,
                            ModeloComision = objetoDB
                        };
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ExcepcionPenalizacion,
SegmentosInsercion.Personas_Y_Pymes, null, itemtemp);
                        _dbcontext.ExcepcionPenalizacions.AddObject(itemtemp);
                        result.RegistrosAfectados++;
                    }
                    foreach (var item in querymodel.FactorComisionFijas.ToList())
                    {
                        FactorComisionFija itemtemp = new FactorComisionFija()
                        {
                            compania_id = item.compania_id,
                            Compania = item.Compania,
                            ramoDetalle_id = item.ramoDetalle_id,
                            RamoDetalle = item.RamoDetalle,
                            productoDetalle_id = item.productoDetalle_id,
                            ProductoDetalle = item.ProductoDetalle,
                            planDetalle_id = item.planDetalle_id,
                            PlanDetalle = item.PlanDetalle,
                            tipocontrato_id = item.tipocontrato_id,
                            TipoContrato = item.TipoContrato,
                            modelo_id = objetoDB.id,
                            ModeloComision = objetoDB,
                            edadmaxima = item.edadmaxima,
                            edadminima = item.edadminima,
                            factor = item.factor,
                            estadoBeneficiario_id = item.estadoBeneficiario_id
                        };
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.FactorComisionFija,
SegmentosInsercion.Personas_Y_Pymes, null, itemtemp);
                        _dbcontext.FactorComisionFijas.AddObject(itemtemp);
                        result.RegistrosAfectados++;
                    }
                    foreach (var item in querymodel.FactorComisionVariableNetoes.ToList())
                    {
                        FactorComisionVariableNeto itemtemp = new FactorComisionVariableNeto()
                        {
                            compania_id = item.compania_id,
                            Compania = item.Compania,
                            ramoDetalle_id = item.ramoDetalle_id,
                            RamoDetalle = item.RamoDetalle,
                            productoDetalle_id = item.productoDetalle_id,
                            ProductoDetalle = item.ProductoDetalle,
                            planDetalle_id = item.planDetalle_id,
                            PlanDetalle = item.PlanDetalle,
                            tipocontrato_id = item.tipocontrato_id,
                            TipoContrato = item.TipoContrato,
                            modelo_id = objetoDB.id,
                            ModeloComision = objetoDB,
                            factor = item.factor,
                            estadoBeneficiario_id = item.estadoBeneficiario_id
                        };
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.FactorComisionVariableNeto,
SegmentosInsercion.Personas_Y_Pymes, null, itemtemp);
                        _dbcontext.FactorComisionVariableNetoes.AddObject(itemtemp);
                        result.RegistrosAfectados++;
                    }
                    foreach (var item in querymodel.FactorComisionVariableNuevoes.ToList())
                    {
                        FactorComisionVariableNuevo itemtemp = new FactorComisionVariableNuevo()
                        {
                            compania_id = item.compania_id,
                            Compania = item.Compania,
                            ramoDetalle_id = item.ramoDetalle_id,
                            RamoDetalle = item.RamoDetalle,
                            productoDetalle_id = item.productoDetalle_id,
                            ProductoDetalle = item.ProductoDetalle,
                            planDetalle_id = item.planDetalle_id,
                            PlanDetalle = item.PlanDetalle,
                            tipocontrato_id = item.tipocontrato_id,
                            TipoContrato = item.TipoContrato,
                            modelo_id = objetoDB.id,
                            ModeloComision = objetoDB,
                            factor = item.factor,
                            estadoBeneficiario_id = item.estadoBeneficiario_id
                        };
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.FactorComisionVariableNuevo,
SegmentosInsercion.Personas_Y_Pymes, null, itemtemp);
                        _dbcontext.FactorComisionVariableNuevoes.AddObject(itemtemp);
                        result.RegistrosAfectados++;
                    }
                    if (querymodel.MatrizComisionVariables.Any())
                    {
                        List<RangosXNeto> lRangosx = new ComponenteMatrizComisionVariable.Matriz().ListarRangosXNetoMatrizComisionVariableXModelo(modeloId);
                        List<RangosYNuevo> lrangosy = new ComponenteMatrizComisionVariable.Matriz().ListarRangosYNuevoMatrizComisionVariableXModelo(modeloId);

                        new ComponenteMatrizComisionVariable.Matriz().AdicionarRangosMatriz(modeloNuevoID, lRangosx, lrangosy, Username);
                        result.RegistrosAfectados++;
                    }
                    _dbcontext.SaveChanges();
                    result.Resultado = ResultadoOperacion.Exito;
                }

            }
            catch (Exception ex)
            {
                result.RegistrosAfectados = 0;
                result.Resultado = ResultadoOperacion.Error;
                result.MensajeError = ex.Message;
            }
            return result;

        }

    }
}
