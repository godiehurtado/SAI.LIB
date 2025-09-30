using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Threading.Tasks;

namespace ColpatriaSAI.Negocio.Componentes.LiqFranquicias
{

    public class AnticiposFranquicias
    {
        private static SAI_Entities contexto = new SAI_Entities();

        public List<ColpatriaSAI.Negocio.Entidades.AnticipoFranquicia> ListarAnticipoFranquicias()
        {
            return contexto.AnticipoFranquicias.Include("Localidad").Include("Compania").OrderByDescending(a => a.Id).ToList();
        }

        public ColpatriaSAI.Negocio.Entidades.AnticipoFranquicia AnticipoFranquiciaPorId(int id)
        {
            return contexto.AnticipoFranquicias.Single(e => e.Id == id);
        }

        /// <summary>
        /// Método que permite anular un anticipo
        /// </summary>
        /// <param name="idAnticipo">Código único del anticipo a anular</param>
        /// <returns>retorna el resultado de la operación</returns>
        public int AnularAnticipo(int idAnticipo)
        {
            int resultado = 0;
            try
            {
                AnticipoFranquicia anticipo = contexto.AnticipoFranquicias.Where(an => an.Id == idAnticipo).First();
                anticipo.estado = "3";
                resultado = contexto.SaveChanges();
            }
            catch
            {
                resultado = -1;
            }
            return resultado;

        }

        public int ActualizarAnticipoFranquicia(int idAnticipo, AnticipoFranquicia anticipo, string UserName)
        {
            int resultado = 0;
            try
            {
                AnticipoFranquicia anticipoActual = contexto.AnticipoFranquicias.Where(a => a.Id == idAnticipo).First();
                anticipoActual.localidad_id = anticipo.localidad_id;
                anticipoActual.fecha_anticipo = anticipo.fecha_anticipo;
                anticipoActual.compania_id = anticipo.compania_id;
                anticipoActual.descripcion = anticipo.descripcion;
                anticipoActual.valorAnti = anticipo.valorAnti;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, UserName, tablasAuditadas.AnticipoFranquicia, SegmentosInsercion.Personas_Y_Pymes, null, anticipoActual);
                resultado = contexto.SaveChanges();
            }
            catch { resultado = -1; }
            return resultado;
        }

        /// <summary>
        /// Metodo que Actualiza el anticipo de una franquicias
        /// </summary>
        /// <param name="id">id del anticipo franquicia</param>
        /// <param name="anticipoFranquicia">anticipo franquicia que contiene la localidad para encontrar el participante</param>
        /// <param name="usuario">usuario que hace el anticipo</param>
        /// <returns></returns>
        public int ActualizarAnticipoFranquicias(int id, Negocio.Entidades.AnticipoFranquicia anticipoFranquicia, string usuario)
        {
            var participante = (from l in contexto.Localidads
                                join P in contexto.Participantes
                                    on l.clavePago equals P.clave
                                where l.id == anticipoFranquicia.localidad_id
                                select new
                                {
                                    tipodoc = P.tipoDocumento_id,
                                    doc = P.documento,
                                    clave = l.clavePago
                                }).FirstOrDefault();




            //----- grabamos el registro en pagofranquicias
            //generar la entidad de pago franquicia
            Entidades.PagoFranquicia pagoFranquicia = new PagoFranquicia();
            pagoFranquicia.fechaPago = DateTime.Now;
            pagoFranquicia.anticipo_id = id;
            pagoFranquicia.usuario = usuario;
            // generar la entidad para el detallepagofranquicia
            Entidades.DetallePagosFranquicia detallePagosFranquicia = new DetallePagosFranquicia();
            detallePagosFranquicia.compania_id = anticipoFranquicia.compania_id;
            if (participante != null)
            {
                detallePagosFranquicia.clave = participante.clave;
                detallePagosFranquicia.tipoDocumento_id = participante.tipodoc;
                detallePagosFranquicia.documento = participante.doc;
            }
            detallePagosFranquicia.numeroNegocio = "";
            detallePagosFranquicia.ramo_id = 1;
            detallePagosFranquicia.fechaRecaudo = DateTime.Now;
            var firstOrDefault = contexto.ConceptoPagoes.FirstOrDefault(e => e.compania_id == anticipoFranquicia.compania_id);
            if (firstOrDefault != null)
                detallePagosFranquicia.concepto_id =
                    Convert.ToInt32(firstOrDefault.
                                        codigoFranquicias);

            detallePagosFranquicia.totalParticipacion = anticipoFranquicia.valorAnti;
            detallePagosFranquicia.porcentajeComision = 1;
            detallePagosFranquicia.porcentajeParticipacion = 100;
            var orDefault = contexto.Companias.FirstOrDefault(e => e.id == anticipoFranquicia.compania_id);
            if (orDefault != null)
                detallePagosFranquicia.descripcion = "Anticipo " + orDefault.nombre;

            pagoFranquicia.DetallePagosFranquicias.Add(detallePagosFranquicia);
            // se guarda el pago y se genera el archivo plano
            GuardarPagliqFranquicias(pagoFranquicia);
            //-----------------------------------------------------------------------


            //------- se actualiza el estado de anticipo franquicia
            Negocio.Entidades.AnticipoFranquicia anticipofranquicia = contexto.AnticipoFranquicias.Single(e => e.Id == id);



            anticipofranquicia.localidad_id = anticipoFranquicia.localidad_id;
            anticipofranquicia.fecha_anticipo = anticipoFranquicia.fecha_anticipo;
            anticipofranquicia.descripcion = anticipoFranquicia.usuarioEjecutoAnti;
            anticipofranquicia.usuarioEjecutoAnti = anticipoFranquicia.usuarioEjecutoAnti;
            anticipofranquicia.valorAnti = anticipoFranquicia.valorAnti;
            anticipofranquicia.estado = anticipoFranquicia.estado;
            anticipofranquicia.compania_id = anticipoFranquicia.compania_id;

            contexto.AnticipoFranquicias.Attach(anticipofranquicia);
            contexto.ObjectStateManager.ChangeObjectState(anticipofranquicia, EntityState.Modified);


            return contexto.SaveChanges();

            //---------------------------------------------------------
        }
        public int GuardarPagliqFranquicias(Entidades.PagoFranquicia pagoFranquicias)
        {
            int respuesta = 0;
            if (contexto == null)
            {
                contexto = new SAI_Entities();

            }
            lock (contexto)
            {

                contexto.PagoFranquicias.AddObject(pagoFranquicias);

                try
                {
                    respuesta = contexto.SaveChanges();


                    //--------esto se usa para guardar archivo plano
                    StreamWriter sw = null;
                    StringBuilder stringBuilderDetlitot = new StringBuilder();
                    StringBuilder mensajeError = new StringBuilder();

                    foreach (var detpagofra in pagoFranquicias.DetallePagosFranquicias)
                    {
                        stringBuilderDetlitot.Append(TraerLinea(detpagofra));
                    }

                    string path = @"C:\" + "AnticiposPagosLiqFranquica" + pagoFranquicias.liquidacionFranquicia_id.ToString() + "-" + DateTime.Now.ToString("MMM-dd-yyyy") + ".txt"; //Path.GetTempFileName();
                    try
                    {


                        FileStream fs = File.Open(path,
                                                           FileMode.Create,
                                                           FileAccess.Write);

                        sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                        sw.Write(stringBuilderDetlitot.ToString());
                        sw.Close();
                        sw = null;
                        // retorna 11-- genero bien en la tabla y el archivo plano
                        return 11; //"El archivo se ha generado con exito: " + "PagosLiqFranquica" + pagoFranquicias.liquidacionFranquicia_id.ToString() + "-" + DateTime.Now + ".txt";
                    }
                    catch (IOException fe)
                    {
                        // retorna -11--  genero bien en la tabla y MAL el archivo plano
                        return 10;
                        //string sDir = Directory.GetCurrentDirectory();
                        //string s = Path.Combine(sDir, "PagosLiqFranquica" + idLiqFranquica.ToString() + "-" + DateTime.Now + ".txt");
                        //return s;
                    }
                    //--------------------------------------------------



                }
                catch (Exception)
                {
                    // retorna -11--  genero MAL  la tabla y MAL el archivo plano
                    return 0;
                }

            }

            return respuesta;

        }
        /// <summary>
        /// Metodo que devuelve string  de los valores de los campos de detalle pagos franquicias separados por comas 
        /// </summary>
        /// <param name="detallePagosFranquicia">DetallePagosFranquicia que contiene los valores</param>
        /// <returns></returns>
        public string TraerLinea(Negocio.Entidades.DetallePagosFranquicia detallePagosFranquicia)
        {
            StringBuilder stringBuilderDetli = new StringBuilder();

            Type t = detallePagosFranquicia.GetType();
            PropertyInfo[] pi = t.GetProperties();
            for (int index = 0; index < pi.Length; index++)
            {
                if (pi[index].Name != "ChangeTracker" && pi[index].Name != "PagoFranquicia" && pi[index].Name != "Compania" && pi[index].Name != "Ramo")
                {


                    stringBuilderDetli.Append(pi[index].GetValue(detallePagosFranquicia, null));
                    if (index < pi.Length - 1 && pi[index + 1] != null && pi[index].Name != "ChangeTracker" && pi[index + 1].Name != "PagoFranquicia" && pi[index + 1].Name != "Compania" && pi[index + 1].Name != "Ramo")
                    {
                        stringBuilderDetli.Append(",");
                    }
                }

            }

            stringBuilderDetli.Append("\r\n");

            return stringBuilderDetli.ToString();
        }



        public int InsertarAnticipoFranquicias(ColpatriaSAI.Negocio.Entidades.AnticipoFranquicia anticipoFranquicia, string Username)
        {
            contexto.AnticipoFranquicias.ApplyChanges(anticipoFranquicia);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.AnticipoFranquicia,
    SegmentosInsercion.Personas_Y_Pymes, null, anticipoFranquicia);
            contexto.ObjectStateManager.ChangeObjectState(anticipoFranquicia, EntityState.Added);
            return contexto.SaveChanges();
        }
    }
}

