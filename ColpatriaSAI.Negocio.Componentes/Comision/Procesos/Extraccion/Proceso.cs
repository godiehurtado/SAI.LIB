using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using Microsoft.SqlServer.Dts.Runtime;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Procesos.Extraccion
{
    public class Proceso
    {
        private SAI_Entities _dbcontext = new SAI_Entities();

        public List<CPRO_ProcesosExtraccion> ListarProcesosExtracion()
        {
            return _dbcontext.CPRO_ProcesosExtraccion.ToList();
        }

        internal ResultadoProcesoExtraccion EjecutarProceso(byte procesoId, DateTime fechaInicio, DateTime fechaFin, string codigoExtraccion)
        {
            EstadoEjecucionProceso resstatus = EstadoEjecucionProceso.Completo;
            string currentUserName = "uinversion/juanmrojasr";
            DateTime currentTime = DateTime.Now;
            switch (resstatus)
            {
                case EstadoEjecucionProceso.Completo:
                    _dbcontext.CPRO_HistorialEjecucionProceso.AddObject(
                        new CPRO_HistorialEjecucionProceso()
                        {
                            usuario = currentUserName,
                            fecha = currentTime,
                            procesoExtraccion_id = procesoId
                        });
                    var dbobj = _dbcontext.CPRO_ProcesosExtraccion.Where(x => x.id == procesoId).First();
                    dbobj.ultimaEjecucion = currentTime;
                    string pkgLocation = dbobj.packageFileName;
                    Microsoft.SqlServer.Dts.Runtime.Package pkg;
                    Application app;
                    DTSExecResult pkgResults;
                    app = new Application();
                    pkg = app.LoadPackage(pkgLocation, null);
                    pkgResults = pkg.Execute();
                    break;
                case EstadoEjecucionProceso.Abortado:
                    break;
                case EstadoEjecucionProceso.CompletoConErrores:
                    break;
                case EstadoEjecucionProceso.ErrorDesconocido:
                    break;
                default:
                    break;
            }
            _dbcontext.SaveChanges();
            return new ResultadoProcesoExtraccion() { Estado = resstatus, RegistrosAfectados = 0 };
        }
    }
}
