using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Negocio.Validadores.ModeloComision;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Variable
{
    class Matriz
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

        public ResultadoOperacionBD EliminarMatrizComisionVariableXModeloId(int modeloId, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var querymodel = _dbcontext.ModeloComisions.Where(x => x.id == modeloId).First();
                if (!ValidadorModeloComision.EditableSegunVigenciaModelo(querymodel.fechadesde))
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede modificar el modelo. El modelo se encuentra vigente";
                }
                else
                {
                    result.RegistrosAfectados = 0;
                    var queryRangos = _dbcontext.MatrizComisionVariables
                        .Include("RangosXNeto")
                        .Include("RangosYNuevo")
                        .Where(x => x.modelo_id == modeloId).ToList();
                    foreach (var punto in queryRangos)
                    {
                        _dbcontext.MatrizComisionVariables.DeleteObject(punto);
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.MatrizComisionVariable,
    SegmentosInsercion.Personas_Y_Pymes, punto); result.RegistrosAfectados++;
                    }
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

        public ResultadoOperacionBD AdicionarRangosMatriz(int modeloId, List<Entidades.RangosXNeto> ejex, List<Entidades.RangosYNuevo> ejey, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            bool hasErrors = false;
            List<string> errores = new List<string>();
            try
            {
                var querymodel = _dbcontext.ModeloComisions.Where(x => x.id == modeloId).First();
                if (!ValidadorModeloComision.EditableSegunVigenciaModelo(querymodel.fechadesde))
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede modificar el modelo. El modelo se encuentra vigente";
                }
                else if (ejey.Count != ejex.Count && ejex.Count > 0)
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "La matriz no es válida. No es cuadrática.";
                }
                else if (_dbcontext.MatrizComisionVariables.Where(x => x.modelo_id == modeloId).Count() > 0)
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede adicionar la matriz al modelo. El modelo ya tiene definida una matriz";
                }
                else
                {
                    result.RegistrosAfectados = 0;
                    List<Entidades.RangosXNeto> tejex = new List<Entidades.RangosXNeto>();
                    List<Entidades.RangosYNuevo> tejey = new List<Entidades.RangosYNuevo>();
                    for (int i = 0; i < ejex.Count; i++)
                    {
                        Entidades.RangosXNeto tmpdbobjrxn = new Entidades.RangosXNeto()
                        {
                            rangomin = ejex[i].rangomin,
                            rangomax = ejex[i].rangomax
                        };
                        Entidades.RangosYNuevo tmpdbobjryn = new Entidades.RangosYNuevo()
                        {
                            rangomin = ejey[i].rangomin,
                            rangomax = ejey[i].rangomax
                        };
                        if (tmpdbobjrxn.rangomin < tmpdbobjrxn.rangomax
                            && tmpdbobjryn.rangomin < tmpdbobjryn.rangomax
                            && ((i > 0
                                    && tmpdbobjrxn.rangomin == ejex[i - 1].rangomax + 1
                                    && tmpdbobjryn.rangomin == ejey[i - 1].rangomax + 1)
                                || (i == 0 && tmpdbobjryn.rangomin == 1)
                                )
                            )
                        {
                            _dbcontext.RangosXNetos.AddObject(tmpdbobjrxn);
                            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.RangosXNetos,
SegmentosInsercion.Personas_Y_Pymes, null, tmpdbobjrxn);
                            _dbcontext.RangosYNuevos.AddObject(tmpdbobjryn);
                            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.RangosYNuevos,
SegmentosInsercion.Personas_Y_Pymes, null, tmpdbobjryn);
                            tejex.Add(tmpdbobjrxn);
                            tejey.Add(tmpdbobjryn);
                            result.RegistrosAfectados += 2;
                        }
                        else
                        {
                            hasErrors = true;
                            if (tmpdbobjrxn.rangomin > tmpdbobjrxn.rangomax)
                            {
                                errores.Add(string.Format("El rango mínimo es mayor al máximo en el eje x{0}.", i));
                            }
                            if (tmpdbobjryn.rangomin > tmpdbobjryn.rangomax)
                            {
                                errores.Add(string.Format("El rango mínimo es mayor al máximo en el eje y{0}.", i));
                            }
                            if (i > 0 && tmpdbobjrxn.rangomin != ejex[i - 1].rangomax + 1)
                            {
                                errores.Add(string.Format("El valor del rango inferior x{0} debe ser el siguiente entero del rango superior x{0}.", i));
                            }
                            if (i > 0 && tmpdbobjryn.rangomin != ejey[i - 1].rangomax + 1)
                            {
                                errores.Add(string.Format("El valor del rango inferior y{0} debe ser el siguiente entero del rango superior y{0}.", i));
                            }
                            if (i == 0 && tmpdbobjryn.rangomin != 1)
                            {
                                errores.Add("El valor del rango inferior y0 debe ser 0.");
                            }
                        }
                    }
                    if (!hasErrors)
                    {
                        for (int i = 0; i < tejex.Count; i++)
                        {
                            for (int j = 0; j < tejey.Count; j++)
                            {
                                Entidades.MatrizComisionVariable tmpdbobjMatrizh = new Entidades.MatrizComisionVariable();
                                tmpdbobjMatrizh.modelo_id = modeloId;
                                tmpdbobjMatrizh.RangosXNeto = tejex[j];
                                tmpdbobjMatrizh.RangosYNuevo = tejey[i];
                                _dbcontext.MatrizComisionVariables.AddObject(tmpdbobjMatrizh);
                                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.MatrizComisionVariable,
SegmentosInsercion.Personas_Y_Pymes, null, tmpdbobjMatrizh);
                                result.RegistrosAfectados++;
                            }
                        }
                        result.Resultado = ResultadoOperacion.Exito;
                        _dbcontext.SaveChanges();
                    }
                    else
                    {
                        result.RegistrosAfectados = 0;
                        result.Resultado = ResultadoOperacion.Error;
                        result.MensajeError = "La matriz no es válida. ";
                        foreach (var item in errores)
                        {
                            result.MensajeError += item;
                        }
                    }
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

        public List<Entidades.MatrizComisionVariable> ListarValoresMatrizComisionVariableXModelo(int modeloId)
        {
            return _dbcontext.MatrizComisionVariables
                .Include("RangosXNeto")
                .Include("RangosYNuevo")
                .Where(x => x.modelo_id == modeloId).ToList();
        }

        public List<Entidades.RangosXNeto> ListarRangosXNetoMatrizComisionVariableXModelo(int modeloId)
        {
            var res = _dbcontext.MatrizComisionVariables.Where(x => x.modelo_id == modeloId)
                .Join(_dbcontext.RangosXNetos,
                    mcv => mcv.rangoxnetos_id,
                    rxn => rxn.id,
                    (mcv, rxn) => new
                    {
                        id = rxn.id,
                        rangomin = rxn.rangomin,
                        rangomax = rxn.rangomax
                    }
                ).Distinct().ToList();
            List<Entidades.RangosXNeto> retval = new List<Entidades.RangosXNeto>();
            foreach (var item in res)
            {
                retval.Add(new Entidades.RangosXNeto()
                {
                    id = item.id,
                    rangomin = item.rangomin,
                    rangomax = item.rangomax
                });
            }
            return retval;
        }

        public List<Entidades.RangosYNuevo> ListarRangosYNuevoMatrizComisionVariableXModelo(int modeloId)
        {
            var res = _dbcontext.MatrizComisionVariables.Where(x => x.modelo_id == modeloId)
                .Join(_dbcontext.RangosYNuevos,
                    mcv => mcv.rangoynuevos_id,
                    ryn => ryn.id,
                    (mcv, ryn) => new
                    {
                        id = ryn.id,
                        rangomin = ryn.rangomin,
                        rangomax = ryn.rangomax
                    }
                ).Distinct().ToList();
            List<Entidades.RangosYNuevo> retval = new List<Entidades.RangosYNuevo>();
            foreach (var item in res)
            {
                retval.Add(new Entidades.RangosYNuevo()
                {
                    id = item.id,
                    rangomin = item.rangomin,
                    rangomax = item.rangomax
                });
            }
            return retval;
        }

        public ResultadoOperacionBD ActualizarFactorMatrizComisionVariable(Entidades.MatrizComisionVariable dbobj, string Username)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                var querymodel = _dbcontext.ModeloComisions.Where(x => x.id == dbobj.modelo_id).First();
                if (!ValidadorModeloComision.EditableSegunVigenciaModelo(querymodel.fechadesde))
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "No se puede modificar el modelo. El modelo se encuentra vigente";
                }
                else if (!EsValidoFactorComisionVariableVsTopeModelo(dbobj))
                {
                    result.RegistrosAfectados = 0;
                    result.Resultado = ResultadoOperacion.Error;
                    result.MensajeError = "El porcentaje definido más el máximo factor de comisión fija no puede ser mayor que el tope del modelo.";
                }
                else
                {
                    var query = _dbcontext.MatrizComisionVariables
                        .Where(x => x.modelo_id == dbobj.modelo_id
                            && x.rangoxnetos_id == dbobj.rangoxnetos_id
                            && x.rangoynuevos_id == dbobj.rangoynuevos_id);
                    if (query.Count() == 1)
                    {
                        var pValorAntiguo = query.First().factor;
                        query.First().factor = dbobj.factor;
                        new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.MatrizComisionVariable,
 SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, dbobj.factor);
                        result.Resultado = ResultadoOperacion.Exito;
                        result.RegistrosAfectados = 1;
                        _dbcontext.SaveChanges();
                    }
                    else
                    {
                        result.RegistrosAfectados = 0;
                        result.Resultado = ResultadoOperacion.Error;
                        result.MensajeError = "No se encontró el factor de la matriz de comisión variable.";
                    }
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

        private bool EsValidoFactorComisionVariableVsTopeModelo(MatrizComisionVariable dbobj)
        {
            var query = _dbcontext.FactorComisionFijas.Where(x => x.modelo_id == dbobj.modelo_id);
            double maxFactorComisionFija = query.Count() == 0 ? 0 : query.Max(x => x.factor);
            double topeModelo = _dbcontext.ModeloComisions.Where(x => x.id == dbobj.modelo_id).Select(x => x.tope).FirstOrDefault();
            return dbobj.factor + maxFactorComisionFija <= topeModelo;
        }
    }
}
