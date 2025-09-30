using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Concursos
{
    public class MonedaxNegocios
    {
        private SAI_Entities tabla = new SAI_Entities();

        #region MONEDAXNEGOCIO

        public List<MonedaxNegocio> ListarMonedaxNegocio()
        {
            return tabla.MonedaxNegocios.Include("Compania").Include("LineaNegocio").Include("Producto").Include("Ramo").Include("Amparo").Include("Cobertura").Include("Moneda").Include("ActividadEconomica").Include("Plan").Include("ModalidadPago").Include("MaestroMonedaxNegocio").Include("Red").Include("Banco").Include("Combo").Include("Segmento").Include("TipoVehiculo").Include("Zona").Include("Localidad").Where(MonedaxNegocio => MonedaxNegocio.id > 0).ToList();
        }

        public List<MonedaxNegocio> ListarMonedaxNegocioPorId(int id)
        {
            return tabla.MonedaxNegocios.Include("Compania").Include("LineaNegocio").Include("Producto").Include("Ramo").Include("Amparo").Include("Cobertura").Include("Moneda").Include("ActividadEconomica").Include("Plan").Include("ModalidadPago").Include("MaestroMonedaxNegocio").Include("Red").Include("Banco").Include("Combo").Include("Segmento").Include("TipoVehiculo").Include("Zona").Include("Localidad").Where(MonedaxNegocio => MonedaxNegocio.id == id).ToList();
        }

        public int InsertarMonedaxNegocio(MonedaxNegocio monedaxnegocio, string Username)
        {
            int resultado = 0;
            if (monedaxnegocio.combo_id == 0)
            {
                if (tabla.MonedaxNegocios.Where(MonedaxNegocio => MonedaxNegocio.factor == monedaxnegocio.factor &&
                   MonedaxNegocio.producto_id == monedaxnegocio.producto_id && MonedaxNegocio.compania_id == monedaxnegocio.compania_id && MonedaxNegocio.lineaNegocio_id == monedaxnegocio.lineaNegocio_id
                   && MonedaxNegocio.ramo_id == monedaxnegocio.ramo_id && MonedaxNegocio.amparo_id == monedaxnegocio.amparo_id &&
                   MonedaxNegocio.plan_id == monedaxnegocio.plan_id && MonedaxNegocio.modalidadPago_id == monedaxnegocio.modalidadPago_id && MonedaxNegocio.actividadeconomica_id == monedaxnegocio.actividadeconomica_id
                   && MonedaxNegocio.maestromoneda_id == monedaxnegocio.maestromoneda_id && MonedaxNegocio.red_id == monedaxnegocio.red_id && MonedaxNegocio.banco_id == monedaxnegocio.banco_id
                   && MonedaxNegocio.segmento_id == monedaxnegocio.segmento_id && MonedaxNegocio.tipoVehiculo_id == monedaxnegocio.tipoVehiculo_id
                   && MonedaxNegocio.zona_id == monedaxnegocio.zona_id && MonedaxNegocio.localidad_id == monedaxnegocio.localidad_id
                   ).ToList().Count() == 0)
                {
                    tabla.MonedaxNegocios.AddObject(monedaxnegocio);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.MonedaxNegocio,
    SegmentosInsercion.Personas_Y_Pymes, monedaxnegocio, null);
                    tabla.SaveChanges();
                    resultado = monedaxnegocio.id;
                }
            }

            else
            {
                if (tabla.MonedaxNegocios.Where(MonedaxNegocio => MonedaxNegocio.combo_id == monedaxnegocio.combo_id).ToList().Count() == 0)
                {
                    tabla.MonedaxNegocios.AddObject(monedaxnegocio);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.MonedaxNegocio,
    SegmentosInsercion.Personas_Y_Pymes, null ,monedaxnegocio);
                    tabla.SaveChanges();
                    resultado = monedaxnegocio.id;
                }
            }
            return resultado;
        }

        public int ActualizarMonedaxNegocio(int id, MonedaxNegocio monedaxnegocio, string Username)
        {
            var monedaxnegocioActual = this.tabla.MonedaxNegocios.Where(MonedaxNegocio => MonedaxNegocio.id == id && MonedaxNegocio.id > 0).First();
            var pValorAntiguo = monedaxnegocioActual;
            monedaxnegocioActual.compania_id = monedaxnegocio.compania_id;
            monedaxnegocioActual.lineaNegocio_id = monedaxnegocio.lineaNegocio_id;
            monedaxnegocioActual.ramo_id = monedaxnegocio.ramo_id;
            monedaxnegocioActual.producto_id = monedaxnegocio.producto_id;
            monedaxnegocioActual.amparo_id = monedaxnegocio.amparo_id;
            monedaxnegocioActual.factor = monedaxnegocio.factor;
            monedaxnegocioActual.actividadeconomica_id = monedaxnegocio.actividadeconomica_id;
            monedaxnegocioActual.modalidadPago_id = monedaxnegocio.modalidadPago_id;
            monedaxnegocioActual.plan_id = monedaxnegocio.plan_id;
            monedaxnegocioActual.red_id = monedaxnegocio.red_id;
            monedaxnegocioActual.banco_id = monedaxnegocio.banco_id;
            monedaxnegocioActual.combo_id = monedaxnegocio.combo_id;
            monedaxnegocioActual.segmento_id = monedaxnegocio.segmento_id;
            monedaxnegocioActual.tipoVehiculo_id = monedaxnegocio.tipoVehiculo_id;
            monedaxnegocioActual.localidad_id = monedaxnegocio.localidad_id;
            monedaxnegocioActual.zona_id = monedaxnegocio.zona_id;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.MonedaxNegocio,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, monedaxnegocioActual);
            return tabla.SaveChanges();
        }

        public string EliminarMonedaxNegocio(int id, MonedaxNegocio monedaxnegocio, string Username)
        {
            var monedaxnegocioActual = this.tabla.MonedaxNegocios.Where(MonedaxNegocio => MonedaxNegocio.id == id).First();
            tabla.DeleteObject(monedaxnegocioActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.MonedaxNegocio,
    SegmentosInsercion.Personas_Y_Pymes, monedaxnegocioActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        #endregion

        #region TOPEMONEDA

        public List<TopeMoneda> ListarTopeMoneda()
        {
            return tabla.TopeMonedas.Include("Compania").Include("Concurso").Include("Producto").Include("Ramo").Where(tp => tp.id > 0).ToList();
        }

        public List<TopeMoneda> ListarTopeMonedaPorId(int id)
        {
            return tabla.TopeMonedas.Include("Compania").Include("Concurso").Include("Producto").Include("Ramo").Where(tp => tp.id == id).ToList();
        }

        public int InsertarTopeMoneda(TopeMoneda topemoneda, string Username)
        {
            int resultado = 0;
            if (tabla.TopeMonedas.Where(TopeMoneda => TopeMoneda.compania_id == topemoneda.compania_id &&
                    TopeMoneda.ramo_id == topemoneda.ramo_id && TopeMoneda.producto_id == topemoneda.producto_id
                    && TopeMoneda.maestromoneda_id == topemoneda.maestromoneda_id).ToList().Count() == 0)
            {
                tabla.TopeMonedas.AddObject(topemoneda);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.TopeMoneda,
    SegmentosInsercion.Personas_Y_Pymes, null, topemoneda);
                tabla.SaveChanges();
                resultado = topemoneda.id;
            }
            return resultado;
        }

        public int ActualizarTopeMoneda(int id, TopeMoneda topemoneda, string Username)
        {
            var topemonedaActual = this.tabla.TopeMonedas.Where(tp => tp.id == id && tp.id > 0).First();
            var pValorAntiguo = topemonedaActual;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.TopeMoneda,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, topemonedaActual);
            topemonedaActual.tope = topemoneda.tope;
            return tabla.SaveChanges();
        }

        public string EliminarTopeMoneda(int id, TopeMoneda topemoneda, string Username)
        {
            var topemonedaActual = this.tabla.TopeMonedas.Where(tp => tp.id == id && tp.id > 0).First();
            tabla.DeleteObject(topemonedaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.TopeMoneda,
    SegmentosInsercion.Personas_Y_Pymes, topemonedaActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }


        #endregion

        #region MAESTROMONEDAXNEGOCIO

        public List<MaestroMonedaxNegocio> ListarMaestroMonedaxNegocio()
        {
            return tabla.MaestroMonedaxNegocios.Include("Moneda").Where(mmn => mmn.id > 0).ToList();
        }

        public List<MaestroMonedaxNegocio> ListarMaestroMonedaxNegocioPorId(int id)
        {
            return tabla.MaestroMonedaxNegocios.Include("Moneda").Where(mmn => mmn.id == id).ToList();
        }

        public int InsertarMaestroMonedaxNegocio(MaestroMonedaxNegocio maestromonedaxnegocio, string Username)
        {
            int resultado = 0;

            if (tabla.MaestroMonedaxNegocios.Where(MMN => (MMN.moneda_id == maestromonedaxnegocio.moneda_id) &&
                                                  (MMN.fecha_inicial <= maestromonedaxnegocio.fecha_inicial && MMN.fecha_final > maestromonedaxnegocio.fecha_inicial) ||
                                                  (MMN.fecha_inicial < maestromonedaxnegocio.fecha_inicial && MMN.fecha_final > maestromonedaxnegocio.fecha_final) ||
                                                  (MMN.fecha_inicial > maestromonedaxnegocio.fecha_inicial && MMN.fecha_final < maestromonedaxnegocio.fecha_final) ||
                                                  (MMN.fecha_inicial < maestromonedaxnegocio.fecha_final && MMN.fecha_final > maestromonedaxnegocio.fecha_final)
                                                  ).ToList().Count() == 0)
            {
                tabla.MaestroMonedaxNegocios.AddObject(maestromonedaxnegocio);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.MaestroMonedaxNegocio,
    SegmentosInsercion.Personas_Y_Pymes, null, maestromonedaxnegocio);
                tabla.SaveChanges();
                resultado = maestromonedaxnegocio.id;
            }
            return resultado;
        }

        public int ActualizarMaestroMonedaxNegocio(int id, MaestroMonedaxNegocio maestromonedaxnegocio, string Username)
        {
            int resultado = 0;

            resultado = (tabla.MaestroMonedaxNegocios.Where(MMN => (MMN.moneda_id == maestromonedaxnegocio.moneda_id) &&
                                                    ((MMN.fecha_inicial <= maestromonedaxnegocio.fecha_inicial && MMN.fecha_final > maestromonedaxnegocio.fecha_inicial) ||
                                                    (MMN.fecha_inicial < maestromonedaxnegocio.fecha_inicial && MMN.fecha_final > maestromonedaxnegocio.fecha_final) ||
                                                    (MMN.fecha_inicial > maestromonedaxnegocio.fecha_inicial && MMN.fecha_final < maestromonedaxnegocio.fecha_final) ||
                                                    (MMN.fecha_inicial < maestromonedaxnegocio.fecha_final && MMN.fecha_final > maestromonedaxnegocio.fecha_final))
                                                    && (MMN.id != id)
                                                    ).ToList().Count());

            if (resultado == 0)
            {
                var maestromonedaxnegocioActual = this.tabla.MaestroMonedaxNegocios.Where(mmn => mmn.id == id && mmn.id > 0).First();
                var pValorAntiguo = maestromonedaxnegocioActual;
                maestromonedaxnegocioActual.moneda_id = maestromonedaxnegocio.moneda_id;
                maestromonedaxnegocioActual.fecha_inicial = maestromonedaxnegocio.fecha_inicial;
                maestromonedaxnegocioActual.fecha_final = maestromonedaxnegocio.fecha_final;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.MonedaxNegocio,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, maestromonedaxnegocioActual);
                tabla.SaveChanges();
                resultado = maestromonedaxnegocio.id;
            }
            return resultado;
        }

        public string EliminarMaestroMonedaxNegocio(int id, MaestroMonedaxNegocio maestromonedaxnegocio, string Username)
        {
            var maestromonedaxnegocioActual = this.tabla.MaestroMonedaxNegocios.Where(mmn => mmn.id == id && mmn.id > 0).First();
            tabla.DeleteObject(maestromonedaxnegocioActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.MonedaxNegocio,
    SegmentosInsercion.Personas_Y_Pymes, maestromonedaxnegocioActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

        public Dictionary<int, string> ListarSegmentoxUsuario(string userName)
        {
            Dictionary<int, string> resultadoSegmentoxUsuario = new Dictionary<int, string>();
            var list = from d in tabla.UsuarioxSegmentoes
                       join p in tabla.Segmentoes on d.segmento_id equals p.id
                       join x in tabla.Monedas on p.id equals x.segmento_id
                       where (d.userName == userName)
                       select new
                       {
                           nombreMoneda = x.nombre,
                           idMoneda = x.id
                       };

            foreach (var item in list)
            {
                resultadoSegmentoxUsuario.Add(item.idMoneda, item.nombreMoneda);
            }

            return resultadoSegmentoxUsuario;
        }

        public List<UsuarioxSegmento> ListarSegmentodelUsuario()
        {
            return tabla.UsuarioxSegmentoes.Include("Segmento").Where(uds => uds.id > 0).ToList();
        }

        #endregion

        #region TOPEXEDAD

        public List<TopexEdad> ListarTopexEdad()
        {
            return tabla.TopexEdads.Include("Compania").Include("Producto").Include("Ramo").Where(te => te.id > 0).ToList();
        }

        public List<TopexEdad> ListarTopexEdadPorId(int id)
        {
            return tabla.TopexEdads.Include("Compania").Include("Producto").Include("Ramo").Where(te => te.id == id).ToList();
        }

        public int InsertarTopexEdad(TopexEdad topexedad, string Username)
        {
            int resultado = 0;
            if (tabla.TopexEdads.Where(TopexEdad => TopexEdad.compania_id == topexedad.compania_id &&
                    TopexEdad.ramo_id == topexedad.ramo_id && TopexEdad.producto_id == topexedad.producto_id
                    && TopexEdad.maestromoneda_id == topexedad.maestromoneda_id).ToList().Count() == 0)
            {
                tabla.TopexEdads.AddObject(topexedad);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.TopexEdad,
    SegmentosInsercion.Personas_Y_Pymes, null, topexedad);
                tabla.SaveChanges();
                resultado = topexedad.id;
            }
            return resultado;
        }

        public int ActualizarTopexEdad(int id, TopexEdad topexedad, string Username)
        {
            var topexedadActual = this.tabla.TopexEdads.Where(te => te.id == id && te.id > 0).First();
            var pValorAntiguo = topexedadActual;
            topexedadActual.tope = topexedad.tope;
            topexedadActual.edad = topexedad.edad;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.TopexEdad,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, topexedadActual);
            return tabla.SaveChanges();
        }

        public string EliminarTopexEdad(int id, TopexEdad topexedad, string Username)
        {
            var topexedadActual = this.tabla.TopexEdads.Where(te => te.id == id && te.id > 0).First();
            tabla.DeleteObject(topexedadActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.TopexEdad,
    SegmentosInsercion.Personas_Y_Pymes, topexedadActual, null);
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




