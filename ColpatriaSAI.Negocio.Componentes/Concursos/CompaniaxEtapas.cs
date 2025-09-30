using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Concursos
{
    public class CompaniaxEtapas
    {
        private SAI_Entities tabla = new SAI_Entities();

        #region COMPANIAXETAPA

        public List<CompaniaxEtapa> ListarCompaniaxEtapa()
        {
            return tabla.CompaniaxEtapas.Include("Compania").Include("EtapaProducto").Where(CompaniaxEtapa => CompaniaxEtapa.id > 0).ToList();
        }

        public List<CompaniaxEtapa> ListarCompaniaxEtapaPorId(int id)
        {
            return tabla.CompaniaxEtapas.Include("Compania").Include("EtapaProducto").Where(CompaniaxEtapa => CompaniaxEtapa.id == id && CompaniaxEtapa.id > 0).ToList();
        }

        public int InsertarCompaniaxEtapa(CompaniaxEtapa companiaxetapa, string Username)
        {
            tabla.CompaniaxEtapas.AddObject(companiaxetapa);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.CompaniaxEtapa,
    SegmentosInsercion.Personas_Y_Pymes, null, companiaxetapa);
            return tabla.SaveChanges();
        }

        public int ActualizarCompaniaxEtapa(int id, CompaniaxEtapa companiaxetapa, string Username)
        {
            var companiaxetapaActual = this.tabla.CompaniaxEtapas.Where(CompaniaxEtapa => CompaniaxEtapa.id == id).First();
            var pValorAntiguo = companiaxetapaActual;
            companiaxetapaActual.mes_inicial = companiaxetapa.mes_inicial;
            companiaxetapaActual.mes_final = companiaxetapa.mes_final;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.CompaniaxEtapa,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, companiaxetapaActual);
            return tabla.SaveChanges();
        }

        public string EliminarCompaniaxEtapa(int id, CompaniaxEtapa companiaxetapa, string Username)
        {
            var companiaxetapaActual = this.tabla.CompaniaxEtapas.Where(CompaniaxEtapa => CompaniaxEtapa.id == id && CompaniaxEtapa.id > 0).First();
            tabla.DeleteObject(companiaxetapaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.CompaniaxEtapa,
    SegmentosInsercion.Personas_Y_Pymes, companiaxetapaActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        #endregion

        #region PERSISTENCIAESPERADA

        public List<PersistenciaEsperada> ListarPersistenciaEsperada()
        {
            return tabla.PersistenciaEsperadas.Include("Concurso").Include("Plazo").Where(pe => pe.id > 0).ToList();
        }

        public List<PersistenciaEsperada> ListarPersistenciaEsperadaPorId(int id)
        {
            return tabla.PersistenciaEsperadas.Include("Concurso").Include("Plazo").Where(pe => pe.id == id && pe.id > 0).ToList();
        }

        public int InsertarPersistenciaEsperada(PersistenciaEsperada persistenciaesperada, string Username)
        {
            int resultado = 0;
            if (tabla.PersistenciaEsperadas.Where(PersistenciaEsperada => PersistenciaEsperada.tipoPersistencia == persistenciaesperada.tipoPersistencia && PersistenciaEsperada.concurso_id == persistenciaesperada.concurso_id
                && PersistenciaEsperada.valor == persistenciaesperada.valor && PersistenciaEsperada.plazo_id == persistenciaesperada.plazo_id).ToList().Count() == 0)
            {
                tabla.PersistenciaEsperadas.AddObject(persistenciaesperada);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.PersistenciaEsperada,
    SegmentosInsercion.Personas_Y_Pymes, null, persistenciaesperada);
                tabla.SaveChanges();
                resultado = persistenciaesperada.id;
            }
            return resultado;
        }

        public int ActualizarPersistenciaEsperada(int id, PersistenciaEsperada persistenciaesperada, string Username)
        {
            var persistenciaesperadaActual = this.tabla.PersistenciaEsperadas.Where(pe => pe.id == id).First();
            var pValorAntiguo = persistenciaesperadaActual;
            persistenciaesperadaActual.tipoPersistencia = persistenciaesperada.tipoPersistencia;
            persistenciaesperadaActual.valor = persistenciaesperada.valor;
            persistenciaesperadaActual.plazo_id = persistenciaesperada.plazo_id;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.PersistenciaEsperada,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, persistenciaesperadaActual);
            return tabla.SaveChanges();
        }

        public string EliminarPersistenciaEsperada(int id, PersistenciaEsperada persistenciaesperada, string Username)
        {
            var persistenciaesperadaActual = this.tabla.PersistenciaEsperadas.Where(pe => pe.id == id && pe.id > 0).First();
            tabla.DeleteObject(persistenciaesperadaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.PersistenciaEsperada,
    SegmentosInsercion.Personas_Y_Pymes, persistenciaesperadaActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        #endregion

        #region SINIESTRALIDADESPERADA

        public List<SiniestralidadEsperada> ListarSiniestralidadEsperada()
        {
            return tabla.SiniestralidadEsperadas.Include("Concurso").Include("Ramo").Where(se => se.id > 0).ToList();
        }

        public List<SiniestralidadEsperada> ListarSiniestralidadEsperadaPorId(int id)
        {
            return tabla.SiniestralidadEsperadas.Include("Concurso").Include("Ramo").Where(se => se.id == id && se.id > 0).ToList();
        }

        public int InsertarSiniestralidadEsperada(SiniestralidadEsperada siniestralidadesperada, string Username)
        {
            int resultado = 0;
            if (tabla.SiniestralidadEsperadas.Where(SiniestralidadEsperada => SiniestralidadEsperada.concurso_id == siniestralidadesperada.concurso_id && SiniestralidadEsperada.ramo_id == siniestralidadesperada.ramo_id
                && SiniestralidadEsperada.valor == siniestralidadesperada.valor).ToList().Count() == 0)
            {
                tabla.SiniestralidadEsperadas.AddObject(siniestralidadesperada);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.SiniestralidadEsperada,
    SegmentosInsercion.Personas_Y_Pymes, null, siniestralidadesperada);
                tabla.SaveChanges();
                resultado = siniestralidadesperada.id;
            }
            return resultado;
        }

        public int ActualizarSiniestralidadEsperada(int id, SiniestralidadEsperada siniestralidadesperada, string Username)
        {
            int resultado = 0;
            if (tabla.SiniestralidadEsperadas.Where(SiniestralidadEsperada => SiniestralidadEsperada.concurso_id == siniestralidadesperada.concurso_id && SiniestralidadEsperada.ramo_id == siniestralidadesperada.ramo_id
                && SiniestralidadEsperada.valor == siniestralidadesperada.valor).ToList().Count() == 1)
            {
                var siniestralidadesperadaActual = this.tabla.SiniestralidadEsperadas.Where(se => se.id == id).First();
                var pValorAntiguo = siniestralidadesperadaActual;
                siniestralidadesperadaActual.concurso_id = siniestralidadesperada.concurso_id;
                siniestralidadesperadaActual.ramo_id = siniestralidadesperada.ramo_id;
                siniestralidadesperadaActual.valor = siniestralidadesperada.valor;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.SiniestralidadEsperada,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, siniestralidadesperadaActual);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        public string EliminarSiniestralidadEsperada(int id, SiniestralidadEsperada siniestralidadesperada, string Username)
        {
            var siniestralidadesperadaActual = this.tabla.SiniestralidadEsperadas.Where(se => se.id == id && se.id > 0).First();
            tabla.DeleteObject(siniestralidadesperadaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.SiniestralidadEsperada,
    SegmentosInsercion.Personas_Y_Pymes, siniestralidadesperadaActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        #endregion

    }
}
