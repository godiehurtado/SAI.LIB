using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.ProcesoAutomatico
{
    public class AUT
    {
        private SAI_Entities contexto = new SAI_Entities();

        public List<Ejecuciones> TraerUltimaEjecucion()
        {
            List<Ejecuciones> EjecucionActual = (from ej in contexto.AUT_Ejecucion
                        join ejp in contexto.AUT_Ejecucion_Proceso
                        on ej.id_ejecucion equals ejp.id_ejecucion
                        join es1 in contexto.AUT_Estado_Ejecucion
                        on ej.estado_ejecucion equals es1.id_estado
                        join es2 in contexto.AUT_Estado_Ejecucion
                        on ejp.estado equals es2.id_estado
                        join pr in contexto.AUT_Proceso
                        on ejp.id_proceso equals pr.id
                        where ej.id_ejecucion == contexto.AUT_Ejecucion.Max(x => x.id_ejecucion)
                        select new Ejecuciones()
                        {
                            id_ejecucion = ej.id_ejecucion,
                            fechaIncioEjecucion = ej.fecha_hora_inicio,
                            fechaFinEjecucion = ej.fecha_hora_fin,
                            estadoEjecucion = es1.nombre_estado,
                            id_proceso = pr.id,
                            nombreProceso = pr.nombre_proceso,
                            fechaInicioProceso = ejp.fecha_inicio,
                            fechaFinProceso = ejp.fecha_fin,
                            estadoProceso = es2.nombre_estado,
                            detalleProceso = ejp.detalle
                        }).ToList();
            return EjecucionActual;
                        
        }

        public List<Ejecuciones> TraerEjecucion(DateTime fechaIni)
        {
            List<Ejecuciones> EjecucionActual = (from ej in contexto.AUT_Ejecucion
                                                 join ejp in contexto.AUT_Ejecucion_Proceso
                                                 on ej.id_ejecucion equals ejp.id_ejecucion
                                                 join es1 in contexto.AUT_Estado_Ejecucion
                                                 on ej.estado_ejecucion equals es1.id_estado
                                                 join es2 in contexto.AUT_Estado_Ejecucion
                                                 on ejp.estado equals es2.id_estado
                                                 join pr in contexto.AUT_Proceso
                                                 on ejp.id_proceso equals pr.id
                                                 where ej.id_ejecucion == (contexto.AUT_Ejecucion.Where(
                                                   x => x.fecha_hora_inicio.Year == fechaIni.Year 
                                                     && x.fecha_hora_inicio.Month == fechaIni.Month
                                                     && x.fecha_hora_inicio.Day == fechaIni.Day).Max(x => x.id_ejecucion))
                                                 select new Ejecuciones()
                                                 {
                                                     id_ejecucion = ej.id_ejecucion,
                                                     fechaIncioEjecucion = ej.fecha_hora_inicio,
                                                     fechaFinEjecucion = ej.fecha_hora_fin,
                                                     estadoEjecucion = es1.nombre_estado,
                                                     id_proceso = pr.id,
                                                     nombreProceso = pr.nombre_proceso,
                                                     fechaInicioProceso = ejp.fecha_inicio,
                                                     fechaFinProceso = ejp.fecha_fin,
                                                     estadoProceso = es2.nombre_estado,
                                                     detalleProceso = ejp.detalle
                                                 }).ToList();
            return EjecucionActual;

        }

        public List<AUT_Programacion_Proceso> TraerUltimasFechasProgramacion()
        {
            List<AUT_Programacion_Proceso> programas = (from pr in contexto.AUT_Programacion_Proceso                                                 
                                                 orderby pr.fecha_hora_inicio_ejecucion descending
                                                 select pr
                                                 ).Take(2).ToList();
            return programas;
        }

        public List<AUT_Programacion_Proceso> TraerFechasProgramacion()
        {
            List<AUT_Programacion_Proceso> programas = (from pr in contexto.AUT_Programacion_Proceso
                                                        orderby pr.fecha_hora_inicio_ejecucion descending
                                                        select pr
                                                 ).ToList();
            return programas;
        }

        public AUT_Programacion_Proceso InsertarProgramacion(AUT_Programacion_Proceso programacion)
        {
            contexto.AUT_Programacion_Proceso.AddObject(programacion);
            contexto.SaveChanges();
            return programacion;
        }

        public bool EliminarProgramacion(AUT_Programacion_Proceso programacion)
        {
            try
            {
                AUT_Programacion_Proceso programadelete = contexto.AUT_Programacion_Proceso.Where(x => x.fecha_hora_inicio_ejecucion == programacion.fecha_hora_inicio_ejecucion).FirstOrDefault();
                contexto.AUT_Programacion_Proceso.DeleteObject(programadelete);
                contexto.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<AUT_Proceso> TraerProcesos()
        {
            List<AUT_Proceso> procesos = (from pr in contexto.AUT_Proceso
                                          select pr).ToList();
            return procesos;
        }

        public AUT_Proceso ActualizarProceso(AUT_Proceso procesoold)
        {
            AUT_Proceso procesonew = contexto.AUT_Proceso.Where(x => x.id == procesoold.id).FirstOrDefault();

            if (procesonew != null)
            {
                procesonew.max_reintentos = procesoold.max_reintentos;
                procesonew.max_tiempo_ejecucion = procesoold.max_tiempo_ejecucion;
                procesonew.habilitado = procesoold.habilitado;
                procesonew.notificar_error = procesoold.notificar_error;
                procesonew.notificar_fin = procesoold.notificar_fin;
                procesonew.notificar_inicio = procesoold.notificar_inicio;

                contexto.SaveChanges();                
            }

            return procesonew;
        }

        public List<AUT_Proceso_Dependencia> TraerDependenciaPorProceso(int idProceso)
        {
            List<AUT_Proceso_Dependencia> dependencias = (from dp in contexto.AUT_Proceso_Dependencia
                                                          where dp.id == idProceso
                                                          select dp).ToList();
            return dependencias;
        }

        public List<AUT_Tipo_Accion_En_Error> TraerAccionesEnError()
        {
            List<AUT_Tipo_Accion_En_Error> acciones = (from ac in contexto.AUT_Tipo_Accion_En_Error
                                                       select ac).ToList();
            return acciones;
        }

        public AUT_Proceso_Dependencia InsertarProcesoDependencia(AUT_Proceso_Dependencia procesoDependencia)
        {
            contexto.AUT_Proceso_Dependencia.AddObject(procesoDependencia);
            contexto.SaveChanges();
            return procesoDependencia;
        }

        public AUT_Proceso_Dependencia ActualizarProcesoDependencia(AUT_Proceso_Dependencia procesoold, AUT_Proceso_Dependencia procesonew)
        {
            AUT_Proceso_Dependencia procesoRef = contexto.AUT_Proceso_Dependencia.Where(x => x.id == procesoold.id && x.id_proceso_requerido == procesoold.id_proceso_requerido).FirstOrDefault();
            if (procesoRef != null)
            {
                procesoRef.id_proceso_requerido = procesonew.id_proceso_requerido;
                procesoRef.en_error_proceso_requerido = procesonew.en_error_proceso_requerido;
                contexto.SaveChanges();
                return procesoRef;
            }
            else
            {
                return null;
            }
        }

        public bool EliminarProcesoDependencia(AUT_Proceso_Dependencia procesoDependencia)
        {
            try
            {
                AUT_Proceso_Dependencia procesoDel = contexto.AUT_Proceso_Dependencia.Where(x => x.id == procesoDependencia.id && x.id_proceso_requerido == procesoDependencia.id_proceso_requerido).FirstOrDefault();
                if (procesoDel != null)
                {
                    contexto.AUT_Proceso_Dependencia.DeleteObject(procesoDel);
                    contexto.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
