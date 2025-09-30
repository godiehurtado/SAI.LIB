using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;
using ColpatriaSAI.Negocio.Componentes.Concursos;
using System.Data.Objects;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Parametros
    {
        private SAI_Entities tabla = new SAI_Entities();

        #region PERSISTENCIAVIDA

        public List<ParametrosPersistenciaVIDA> ListarPPVIDA()
        {
            return tabla.ParametrosPersistenciaVIDAs.ToList();
        }

        public List<ParametrosPersistenciaVIDA> ListarPPVIDAPorId(int id)
        {
            return tabla.ParametrosPersistenciaVIDAs.Where(PPV => PPV.id == id && PPV.id > 0).ToList();
        }

        public int ActualizarPPV(int id, ParametrosPersistenciaVIDA parametrospvida, string Username)
        {
            int resultado = 0;
            var ppvActual = this.tabla.ParametrosPersistenciaVIDAs.Where(PPV => PPV.id == id && PPV.id > 0).First();
            var pValorAntiguo = ppvActual;
            ppvActual.factor = parametrospvida.factor;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username,
                tablasAuditadas.ParametrosPersistenciaVIDA, SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, ppvActual);
            resultado = tabla.SaveChanges();

            return resultado;
        }

        #endregion

        #region PARAMETROS APP

        public List<ParametrosApp> ListarParametrosApp()
        {
            return tabla.ParametrosApps.ToList();
        }

        public List<ParametrosApp> ListarParametrosAppPorId(int id)
        {
            return tabla.ParametrosApps.Where(PAP => PAP.id == id).ToList();
        }

        public int ActualizarParametrosApp(List<ParametrosApp> parametrosapp, string Username)
        {
            int resultado = 0;

            try
            {
                List<ParametrosApp> prapp = new List<ParametrosApp>();
                foreach (var item in parametrosapp)
                {
                    ParametrosApp papp = new ParametrosApp();
                    papp.id = item.id;
                    papp.valor = item.valor;
                    prapp.Add(papp);

                    var parametrosUpdate = this.tabla.ParametrosApps.Where(p => p.id == papp.id).First();
                    var pValorAntiguo = parametrosUpdate;
                    parametrosUpdate.valor = papp.valor;
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ParametrosApp, SegmentosInsercion.Personas_Y_Pymes,
                        pValorAntiguo, parametrosUpdate);

                }
                tabla.SaveChanges();

                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

    }
}
