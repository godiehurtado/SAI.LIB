using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;


namespace ColpatriaSAI.Negocio.Componentes.Concursos
{
    public class EtapaConcursos
    {
        private SAI_Entities tabla = new SAI_Entities();


        public List<EtapaProducto> ListarEtapaProductoes()
        {
            return tabla.EtapaProductoes.Include("Concurso").Where(EtapaProducto => EtapaProducto.id > 0).ToList();
        }

        public List<EtapaProducto> ListarEtapaProductoesPorId(int id)
        {
            return tabla.EtapaProductoes.Include("Concurso").Where(EtapaProducto => EtapaProducto.id == id && EtapaProducto.id > 0).ToList();
        }


        public int InsertarEtapaProducto(EtapaProducto etapaproducto, string Username)
        {
            int resultado = 0;
            if (tabla.EtapaProductoes.Where(EtapaProducto => EtapaProducto.nombre == etapaproducto.nombre && EtapaProducto.concurso_id == etapaproducto.concurso_id).ToList().Count() == 0)
            {
                tabla.EtapaProductoes.AddObject(etapaproducto);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.EtapaProducto,
    SegmentosInsercion.Personas_Y_Pymes, null, etapaproducto);
                tabla.SaveChanges();
                resultado = etapaproducto.id;
            }
            return resultado;
        }

        public int ActualizarEtapaProducto(int id, EtapaProducto etapaproducto, string Username)
        {
            var etapaproductoActual = this.tabla.EtapaProductoes.Where(EtapaProducto => EtapaProducto.id == id).First();
            var pValorAntiguo = etapaproductoActual;
            etapaproductoActual.nombre = etapaproducto.nombre;

            if (id == 1)
            {
                var variableEtapa = this.tabla.Variables.Where(v => v.id == 140).First();
                variableEtapa.nombre = "% de ejecución Colquines Etapa " + etapaproducto.nombre + " vs Año Anterior";
            }

            if (id == 2)
            {
                var variableEtapa = this.tabla.Variables.Where(v => v.id == 141).First();
                variableEtapa.nombre = "% de ejecución Colquines Etapa " + etapaproducto.nombre + " vs Año Anterior";
            }

            if (id == 3)
            {
                var variableEtapa = this.tabla.Variables.Where(v => v.id == 142).First();
                variableEtapa.nombre = "% de ejecución Colquines Etapa " + etapaproducto.nombre + " vs Año Anterior";
            }

            if (id == 4)
            {
                var variableEtapa = this.tabla.Variables.Where(v => v.id == 143).First();
                variableEtapa.nombre = "% de ejecución Colquines Etapa " + etapaproducto.nombre + " vs Año Anterior";
            }

            if (id == 5)
            {
                var variableEtapa = this.tabla.Variables.Where(v => v.id == 144).First();
                variableEtapa.nombre = "% de ejecución Colquines Etapa " + etapaproducto.nombre + " vs Año Anterior";
            }
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.EtapaProducto,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, etapaproductoActual);

            return tabla.SaveChanges();
        }

        public string EliminarEtapaProducto(int id, EtapaProducto etapaproducto, string Username)
        {
            var etapaproductoActual = this.tabla.EtapaProductoes.Where(EtapaProducto => EtapaProducto.id == id && EtapaProducto.id > 0).First();
            tabla.DeleteObject(etapaproductoActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.EtapaProducto,
    SegmentosInsercion.Personas_Y_Pymes, etapaproductoActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "";
        }

    }
}





