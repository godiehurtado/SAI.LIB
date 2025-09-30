using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Net;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Franquicias
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<Entidades.Localidad> ListarFranquicias()
        {
            var data = tabla.Localidads.Include("ParticipacionFranquicias").Include("Excepcions").Where(e => e.tipo_localidad_id == 2).OrderBy(e => e.nombre).ToList();
            return data;
        }

        public int InsertarFranquicia(Entidades.Localidad localidad, string Username)
        {
            tabla.Localidads.AddObject(localidad);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Localidad,
    SegmentosInsercion.Personas_Y_Pymes, null, localidad);
            return tabla.SaveChanges();
        }

        public List<Entidades.Localidad> ListarFranquiciaPorId(int idFranquicia)
        {
            return tabla.Localidads.Where(e => e.id == idFranquicia).ToList();
        }


        public int ActualizarFranquicia(int id, Entidades.Localidad franquicia, string Username)
        {
            var entidadeContext = tabla.Localidads.FirstOrDefault(x => x.id == id);
            var pValorAntiguo = entidadeContext;
            entidadeContext.nombre = franquicia.nombre;
            entidadeContext.zona_id = franquicia.zona_id;
            entidadeContext.tipo_localidad_id = franquicia.tipo_localidad_id;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Localidad,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, entidadeContext);


            return tabla.SaveChanges();
        }

        public int EliminarFranquicia(int id, Entidades.Localidad sitemap, string Username)
        {
            tabla.DeleteObject(tabla.Localidads.FirstOrDefault(x => x.id == id));
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Localidad,
    SegmentosInsercion.Personas_Y_Pymes, sitemap, null);
            return tabla.SaveChanges();
        }

        public Entidades.DetallePartFranquicia DetalleFranquiciaPorIdFryIdDetFr(int idFranquicia, int iddetpartfra)
        {
            DetallePartFranquicia detalleparticipacionFranquicia = tabla.DetallePartFranquicias.Include("ParticipacionFranquicia").FirstOrDefault(e => e.part_franquicia_id == idFranquicia && e.id == iddetpartfra);
            return detalleparticipacionFranquicia;
        }

        /// <summary>
        /// Metodo que devuelve participacion franquicia con el detalle
        /// </summary>
        /// <param name="idFranquicia"></param>
        /// <returns>Entidades.ParticipacionFranquicia</returns>
        public Entidades.ParticipacionFranquicia DetalleFranquicia(int idFranquicia)
        {
            ParticipacionFranquicia participacionFranquicia = tabla.ParticipacionFranquicias.Include("Localidad").FirstOrDefault(e => e.id == idFranquicia);

            var detallePartFranquicias = from n in tabla.DetallePartFranquicias.Include("Compania").Include("Ramo").Include("Producto").Include("Plan").Include("TipoVehiculo").Include("ParticipacionFranquicia").AsParallel()
                                         where (n.part_franquicia_id == idFranquicia)
                                         select n;

            Parallel.ForEach(detallePartFranquicias, detallePartFranquicia =>
            {

                Negocio.Entidades.DetallePartFranquicia detallePartFranquiciases = new Negocio.Entidades.DetallePartFranquicia();

                detallePartFranquiciases.compania_id = detallePartFranquicia.compania_id;

                detallePartFranquiciases.ramo_id = detallePartFranquicia.ramo_id;

                if (detallePartFranquicia.producto_id != null)
                {
                    detallePartFranquiciases.producto_id = (int)detallePartFranquicia.producto_id;
                }

            });

            return participacionFranquicia;

        }

        public List<DetallePartFranquicia> DetalleFranquicias(int idFranquicia)
        {
            List<DetallePartFranquicia> detalleParts = tabla.DetallePartFranquicias.Include("Compania").Include("Ramo").Include("Producto").Include("Plan")
                .Include("TipoVehiculo").Include("ParticipacionFranquicia").Include("Amparo").Where(e => e.part_franquicia_id == idFranquicia).ToList();
            return detalleParts;
        }

        public List<Entidades.DetallePartFranquicia> DetalleFranquiciaPorPartFranqId(int idPartFranquicia)
        {
            List<Entidades.DetallePartFranquicia> detallePartFranquicia = tabla.DetallePartFranquicias.Include("ParticipacionFranquicia").Include("Compania").Include("Ramo").Include("Producto").Include("Plan").AsParallel().Where(e => e.part_franquicia_id == idPartFranquicia).ToList();
            return detallePartFranquicia;


        }

        /// <summary>
        /// Metodo que devuelve una lista de participacion franquicias por localidad
        /// </summary>
        /// <param name="idlocalidad"></param>
        /// <returns>Lista de ParticipacionFranquicia</returns>
        public List<Entidades.ParticipacionFranquicia> ListarPartFranquiciasPorlocalidad(int idlocalidad)
        {

            List<Entidades.Localidad> localidades = new List<Localidad>();
            Entidades.Localidad localidad = new Localidad();
            List<Entidades.ParticipacionFranquicia> participacionFranquicias = new List<ParticipacionFranquicia>();

            localidades = tabla.Localidads.Where(e => e.id == idlocalidad).ToList();

            localidad = localidades[0];
            participacionFranquicias = tabla.ParticipacionFranquicias.Where(e => e.Localidad_id == idlocalidad).ToList();

            foreach (var participacionFranquicia in participacionFranquicias)
            {
                participacionFranquicia.Localidad.id = localidad.id;
                participacionFranquicia.Localidad.nombre = localidad.nombre;
            }


            return participacionFranquicias;



        }
        public List<Entidades.ParticipacionFranquicia> ListarPartFranquicias()
        {
            return tabla.ParticipacionFranquicias.Include("Localidad").ToList();
        }

        /// <summary>
        /// Metodo que elimina una participacion franquicia con el detalle
        /// </summary>
        /// <param name="idPartFranquicia"></param>
        /// <returns>true o false</returns>
        public bool EliminarPartFranquicia(int idPartFranquicia, string Username)
        {
            //BORRAMOS LOS REGISTROS HIJOS
            List<DetallePartFranquicia> detparticiFranq = (from dpf in tabla.DetallePartFranquicias
                                                           where dpf.part_franquicia_id == idPartFranquicia
                                                           select dpf
                                                           ).ToList();
            foreach (DetallePartFranquicia participacionFranquicia in detparticiFranq)
            {
                tabla.DetallePartFranquicias.DeleteObject(participacionFranquicia);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.DetallePartFranquicia,
    SegmentosInsercion.Personas_Y_Pymes, participacionFranquicia, null);
            }

            //BORRAMOS EL REGISTRO PADRE
            ParticipacionFranquicia particiFranq = tabla.ParticipacionFranquicias.Where(e => e.id == idPartFranquicia).First();
            tabla.ParticipacionFranquicias.DeleteObject(particiFranq);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.DetallePartFranquicia,
    SegmentosInsercion.Personas_Y_Pymes, particiFranq, null);
            tabla.SaveChanges();
            return true;
        }

        public bool EliminarDetallePartFranquicia(int idPartFranquicia, string Username)
        {
            List<DetallePartFranquicia> detparticiFranq = tabla.DetallePartFranquicias.Where(e => e.id == idPartFranquicia).ToList();
            foreach (DetallePartFranquicia participacionFranquicia in detparticiFranq)
            {
                tabla.DetallePartFranquicias.DeleteObject(participacionFranquicia);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.DetallePartFranquicia,
    SegmentosInsercion.Personas_Y_Pymes, participacionFranquicia, null);
            }
            tabla.SaveChanges();
            return true;
        }

        public ParticipacionFranquicia InsertarPartFranquicia(ParticipacionFranquicia participacionFranquicia, string Username)
        {
            tabla.ParticipacionFranquicias.AddObject(participacionFranquicia);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ParticipacionFranquicia,
    SegmentosInsercion.Personas_Y_Pymes, null, participacionFranquicia);
            tabla.SaveChanges();
            return participacionFranquicia;
        }

        public bool InsertarDetallePartFranquicia(DetallePartFranquicia detalleNuevo, string Username)
        {
            bool resultado = false;
            DetallePartFranquicia detalle = new DetallePartFranquicia()
            {
                part_franquicia_id = detalleNuevo.part_franquicia_id,
                compania_id = detalleNuevo.compania_id,
                ramo_id = detalleNuevo.ramo_id,
                producto_id = detalleNuevo.producto_id,
                plan_id = detalleNuevo.plan_id,
                lineaNegocio_id = detalleNuevo.lineaNegocio_id,
                tipoVehiculo_id = detalleNuevo.tipoVehiculo_id,
                porcentaje = detalleNuevo.porcentaje,
                rangoinferior = detalleNuevo.rangoinferior,
                rangosuperior = detalleNuevo.rangosuperior,
                amparo_id = detalleNuevo.amparo_id
            };

            var existeRegistro = 0;
            if (detalle.rangoinferior == null)
            {
                existeRegistro = tabla.DetallePartFranquicias.Where(x => x.part_franquicia_id == detalle.part_franquicia_id && x.compania_id == detalle.compania_id && x.ramo_id == detalle.ramo_id && x.producto_id == detalle.producto_id && x.plan_id == detalle.plan_id && x.lineaNegocio_id == detalle.lineaNegocio_id && x.tipoVehiculo_id == detalle.tipoVehiculo_id && x.amparo_id == detalle.amparo_id && x.rangoinferior == null && x.rangosuperior == null).ToList().Count();
            }
            else
            {
                existeRegistro = tabla.DetallePartFranquicias.Where(x => x.part_franquicia_id == detalle.part_franquicia_id && x.compania_id == detalle.compania_id && x.ramo_id == detalle.ramo_id && x.producto_id == detalle.producto_id && x.plan_id == detalle.plan_id && x.lineaNegocio_id == detalle.lineaNegocio_id && x.tipoVehiculo_id == detalle.tipoVehiculo_id && x.amparo_id == detalle.amparo_id && x.rangoinferior == detalle.rangoinferior && x.rangosuperior == detalle.rangosuperior).ToList().Count();
            }

            if (existeRegistro == 0)
            {
                tabla.DetallePartFranquicias.AddObject(detalle);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.DetallePartFranquicia,
    SegmentosInsercion.Personas_Y_Pymes, null, detalle);
                resultado = true;
            }

            try { tabla.SaveChanges(); }
            catch { resultado = false; }
            return resultado;
        }

        public int ActualizarParticipacionFranquicia(ParticipacionFranquicia nuevaPartFranq, string Username)
        {
            var partFranqActual = tabla.ParticipacionFranquicias.Single(p => p.id == nuevaPartFranq.id);
            var pValorAntiguo = partFranqActual;
            partFranqActual.fecha_ini = nuevaPartFranq.fecha_ini;
            partFranqActual.fecha_fin = nuevaPartFranq.fecha_fin;
            partFranqActual.fecha_actualizacion = nuevaPartFranq.fecha_actualizacion;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ModeloxMeta,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, partFranqActual);
            return tabla.SaveChanges();
        }


        public int ActualizarPartFranquicia(ParticipacionFranquicia nuevaPartFranq, string Username)
        {
            var partFranqActual = tabla.ParticipacionFranquicias.Include("DetallePartFranquicias").Single(p => p.id == nuevaPartFranq.id);
            var pValorAntiguo = partFranqActual;
            partFranqActual.fecha_ini = nuevaPartFranq.fecha_ini;
            partFranqActual.fecha_fin = nuevaPartFranq.fecha_fin;
            partFranqActual.fecha_actualizacion = DateTime.Now;

            var nuevoDetallePartFranq = nuevaPartFranq.DetallePartFranquicias.First();

            if (partFranqActual.DetallePartFranquicias[0].id != 0)
            {
                partFranqActual.DetallePartFranquicias.First().compania_id = nuevoDetallePartFranq.compania_id;
                partFranqActual.DetallePartFranquicias.First().ramo_id = nuevoDetallePartFranq.ramo_id;
                partFranqActual.DetallePartFranquicias.First().producto_id = nuevoDetallePartFranq.producto_id;
                partFranqActual.DetallePartFranquicias.First().porcentaje = nuevoDetallePartFranq.porcentaje;
                partFranqActual.DetallePartFranquicias.First().rangoinferior = nuevoDetallePartFranq.rangoinferior;
                partFranqActual.DetallePartFranquicias.First().rangosuperior = nuevoDetallePartFranq.rangosuperior;
                partFranqActual.DetallePartFranquicias.First().plan_id = nuevoDetallePartFranq.plan_id;
                partFranqActual.DetallePartFranquicias.First().lineaNegocio_id = nuevoDetallePartFranq.lineaNegocio_id;
                partFranqActual.DetallePartFranquicias.First().tipoVehiculo_id = nuevoDetallePartFranq.tipoVehiculo_id;
                partFranqActual.DetallePartFranquicias.First().amparo_id = nuevoDetallePartFranq.amparo_id;
            }
            else
            {
                InsertarDetallePartFranquicia(nuevoDetallePartFranq, Username);
            }

            var existeRegistro = 0;
            if (nuevaPartFranq.DetallePartFranquicias.First().rangoinferior == null)
            {
                existeRegistro = tabla.DetallePartFranquicias.Where(x => x.part_franquicia_id != nuevaPartFranq.DetallePartFranquicias.First().part_franquicia_id && x.compania_id == nuevaPartFranq.DetallePartFranquicias.First().compania_id && x.ramo_id == nuevaPartFranq.DetallePartFranquicias.First().ramo_id && x.producto_id == nuevaPartFranq.DetallePartFranquicias.First().producto_id && x.plan_id == nuevaPartFranq.DetallePartFranquicias.First().plan_id && x.lineaNegocio_id == nuevaPartFranq.DetallePartFranquicias.First().lineaNegocio_id && x.tipoVehiculo_id == nuevaPartFranq.DetallePartFranquicias.First().tipoVehiculo_id && x.amparo_id == nuevaPartFranq.DetallePartFranquicias.First().amparo_id && x.rangoinferior == null && x.rangosuperior == null).ToList().Count();
            }
            else
            {
                existeRegistro = tabla.DetallePartFranquicias.Where(x => x.part_franquicia_id != nuevaPartFranq.DetallePartFranquicias.First().part_franquicia_id && x.compania_id == nuevaPartFranq.DetallePartFranquicias.First().compania_id && x.ramo_id == nuevaPartFranq.DetallePartFranquicias.First().ramo_id && x.producto_id == nuevaPartFranq.DetallePartFranquicias.First().producto_id && x.plan_id == nuevaPartFranq.DetallePartFranquicias.First().plan_id && x.lineaNegocio_id == nuevaPartFranq.DetallePartFranquicias.First().lineaNegocio_id && x.tipoVehiculo_id == nuevaPartFranq.DetallePartFranquicias.First().tipoVehiculo_id && x.amparo_id == nuevaPartFranq.DetallePartFranquicias.First().amparo_id && x.rangoinferior == nuevaPartFranq.DetallePartFranquicias.First().rangoinferior && x.rangosuperior == nuevaPartFranq.DetallePartFranquicias.First().rangosuperior).ToList().Count();
            }

            if (existeRegistro == 0)
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ParticipacionFranquicia,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, partFranqActual);
                return tabla.SaveChanges();
            }

            return 0;
        }

        public int CopiarParticipacionFranquicia(int origen, int destino, string Username)
        {
            int resultado = 0;
            try
            {
                List<DetallePartFranquicia> detalleOrigen =
                    tabla.DetallePartFranquicias.Include("ParticipacionFranquicia").Where(p => p.ParticipacionFranquicia.id == origen).ToList();

                ParticipacionFranquicia nuevaPartFranq = new ParticipacionFranquicia()
                {
                    fecha_actualizacion = DateTime.Now,
                    fecha_ini = detalleOrigen[0].ParticipacionFranquicia.fecha_ini,
                    fecha_fin = detalleOrigen[0].ParticipacionFranquicia.fecha_fin,
                    Localidad_id = destino
                };

                //DETERMINAMOS SI HAY PARAMETRIZACION EN LAS MISMAS FECHAS PARA EL DESTINO
                var listPartDestino = from pf in tabla.ParticipacionFranquicias
                                      where pf.Localidad_id == destino && pf.fecha_ini >= nuevaPartFranq.fecha_ini && pf.fecha_fin <= nuevaPartFranq.fecha_fin
                                      select pf;

                if (listPartDestino != null && listPartDestino.Count() == 0)
                {
                    foreach (var item in detalleOrigen)
                    {
                        DetallePartFranquicia detalleDestino = new DetallePartFranquicia()
                        {
                            compania_id = item.compania_id,
                            ramo_id = item.ramo_id,
                            producto_id = item.producto_id,
                            plan_id = item.plan_id,
                            porcentaje = item.porcentaje,
                            lineaNegocio_id = item.lineaNegocio_id,
                            rangoinferior = item.rangoinferior,
                            rangosuperior = item.rangosuperior,
                            tipoVehiculo_id = item.tipoVehiculo_id,
                            amparo_id = item.amparo_id
                        };
                        nuevaPartFranq.DetallePartFranquicias.Add(detalleDestino);
                    }
                    InsertarPartFranquicia(nuevaPartFranq, Username); resultado = 1;
                }
                else
                    resultado = 2;
            }
            catch { }
            return resultado;
        }

        public string obtenerSalarioMinimo()
        {
            return tabla.ParametrosApps.Single(p => p.id == 1).valor;
        }

        public List<DetallePartFranquicia> ListDetalleFranquiciaPorId(int idDetPartFranquicia)
        {
            return tabla.DetallePartFranquicias.Include("ParticipacionFranquicia").Where(e => e.id == idDetPartFranquicia).ToList();
        }

        public int ActualizarDetallePartFranquicia(DetallePartFranquicia detallepartfranquicia, string Username)
        {
            var existeRegistro = 0;
            if (detallepartfranquicia.rangoinferior == null)
            {
                existeRegistro = tabla.DetallePartFranquicias.Where(x => x.id != detallepartfranquicia.id && x.part_franquicia_id == detallepartfranquicia.part_franquicia_id && x.compania_id == detallepartfranquicia.compania_id && x.ramo_id == detallepartfranquicia.ramo_id && x.producto_id == detallepartfranquicia.producto_id && x.plan_id == detallepartfranquicia.plan_id && x.lineaNegocio_id == detallepartfranquicia.lineaNegocio_id && x.tipoVehiculo_id == detallepartfranquicia.tipoVehiculo_id && x.amparo_id == detallepartfranquicia.amparo_id && x.rangoinferior == null && x.rangosuperior == null).ToList().Count();
            }
            else
            {
                existeRegistro = tabla.DetallePartFranquicias.Where(x => x.id != detallepartfranquicia.id && x.part_franquicia_id == detallepartfranquicia.part_franquicia_id && x.compania_id == detallepartfranquicia.compania_id && x.ramo_id == detallepartfranquicia.ramo_id && x.producto_id == detallepartfranquicia.producto_id && x.plan_id == detallepartfranquicia.plan_id && x.lineaNegocio_id == detallepartfranquicia.lineaNegocio_id && x.tipoVehiculo_id == detallepartfranquicia.tipoVehiculo_id && x.amparo_id == detallepartfranquicia.amparo_id && x.rangoinferior == detallepartfranquicia.rangoinferior && x.rangosuperior == detallepartfranquicia.rangosuperior).ToList().Count();
            }

            if (existeRegistro == 0)
            {

                var detallepartfranquiciaActualizar = tabla.DetallePartFranquicias.Where(e => e.id == detallepartfranquicia.id).FirstOrDefault();
                var pValorAntiguo = detallepartfranquiciaActualizar;
                detallepartfranquiciaActualizar.compania_id = detallepartfranquicia.compania_id;
                detallepartfranquiciaActualizar.ramo_id = detallepartfranquicia.ramo_id;
                detallepartfranquiciaActualizar.producto_id = detallepartfranquicia.producto_id;
                detallepartfranquiciaActualizar.plan_id = detallepartfranquicia.plan_id;
                detallepartfranquiciaActualizar.lineaNegocio_id = detallepartfranquicia.lineaNegocio_id;
                detallepartfranquiciaActualizar.tipoVehiculo_id = detallepartfranquicia.tipoVehiculo_id;
                detallepartfranquiciaActualizar.amparo_id = detallepartfranquicia.amparo_id;
                detallepartfranquiciaActualizar.porcentaje = detallepartfranquicia.porcentaje;
                detallepartfranquiciaActualizar.rangoinferior = detallepartfranquicia.rangoinferior;
                detallepartfranquiciaActualizar.rangosuperior = detallepartfranquicia.rangosuperior;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ModeloxMeta,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, detallepartfranquiciaActualizar);
                tabla.SaveChanges();
                return 1;
            }

            return 0;
        }

        public DetallePartFranquicia DetalleFranquiciaporId(int id)
        {
            DetallePartFranquicia detallepartfranquicia = tabla.DetallePartFranquicias.Single(d => d.id == id);
            return detallepartfranquicia;
        }

        public int EliminarDetallePartFranquiciaPorId(int id, string Username)
        {
            DetallePartFranquicia detallepartfranquicia = tabla.DetallePartFranquicias.Single(d => d.id == id);
            tabla.DetallePartFranquicias.DeleteObject(detallepartfranquicia);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.DetallePartFranquicia,
    SegmentosInsercion.Personas_Y_Pymes, detallepartfranquicia, null);
            return tabla.SaveChanges();
        }

        /// <summary>
        /// Metodo que devuelve una lista de detalle participacion franquicias.
        /// se filtra el resultado a traves de los parametros enviados(ParticipacionFranquicia,franquicias).
        /// En partfranquicia contiene la informacion que se envia desde el  formulario.
        /// EN franquicias contiene el nombre  o los nombres de las franquicias  separados por una coma(",")
        /// </summary>
        /// <param name="partfranquicia"></param>
        /// <param name="franquicias"></param>
        /// <returns></returns>
        public List<DetallePartFranquicia> GetDetallePartFranquiciasActualizar(ParticipacionFranquicia partfranquicia, string franquicias)
        {

            List<DetallePartFranquicia> detpartfra = new List<DetallePartFranquicia>();
            List<DetallePartFranquicia> detpartfraResult = new List<DetallePartFranquicia>();
            List<Localidad> localidads = new List<Localidad>();
            Localidad localidad = new Localidad();
            char[] delimiterChars = { ',' };
            string[] franquiciass = franquicias.Split(delimiterChars);

            // se obtiene una lista de localidades que concurde con el string franquicias
            foreach (string franq in franquiciass)
            {

                if (franq != "")
                {
                    localidad = tabla.Localidads.FirstOrDefault(e => e.nombre == franq);
                    if (localidad != null)
                    {
                        localidads.Add(localidad);
                    }
                }

            }

            // se obtiene el detalle de participacion franquicias inclida la informacion de participacion franquicias, compañias,ramos,productos
            List<DetallePartFranquicia> detallePartFranquicias =
                tabla.DetallePartFranquicias.Include("ParticipacionFranquicia").Include("Compania").Include("Ramo").Include("Producto").Where(
                    e =>
                   e.ParticipacionFranquicia.fecha_fin <= partfranquicia.fecha_fin &&
                    e.ParticipacionFranquicia.fecha_ini >= partfranquicia.fecha_ini).ToList();

            // si no contiene rangos
            if (partfranquicia.DetallePartFranquicias[0].rangoinferior == null && partfranquicia.DetallePartFranquicias[0].rangosuperior == null)
            {

                // se filtra la informacion que viene del formulario y las lcalidades 
                foreach (var local in localidads)
                {


                    detpartfra = detallePartFranquicias.Where(
                        e => e.compania_id == partfranquicia.DetallePartFranquicias[0].compania_id &&
                             e.lineaNegocio_id == partfranquicia.DetallePartFranquicias[0].lineaNegocio_id &&
                             e.producto_id == partfranquicia.DetallePartFranquicias[0].producto_id &&
                             e.ramo_id == partfranquicia.DetallePartFranquicias[0].ramo_id &&
                             e.tipoVehiculo_id == partfranquicia.DetallePartFranquicias[0].tipoVehiculo_id &&
                             e.ParticipacionFranquicia.Localidad_id == local.id
                        ).ToList();
                    foreach (var detallePartFranquicia in detpartfra)
                    {
                        detallePartFranquicia.porcentaje = partfranquicia.DetallePartFranquicias[0].porcentaje;
                        detpartfraResult.Add(detallePartFranquicia);

                    }
                }
            }
            // si  contiene rangos
            else
            {
                // se filtra la informacion que viene del formulario y las lcalidades 
                foreach (var local in localidads)
                {
                    detpartfra =
                        detallePartFranquicias.Where(
                            e => e.compania_id == partfranquicia.DetallePartFranquicias[0].compania_id &&
                                 e.lineaNegocio_id ==
                                 partfranquicia.DetallePartFranquicias[0].lineaNegocio_id &&
                                 e.producto_id == partfranquicia.DetallePartFranquicias[0].producto_id &&
                                 e.ramo_id == partfranquicia.DetallePartFranquicias[0].ramo_id &&
                                 e.tipoVehiculo_id == partfranquicia.DetallePartFranquicias[0].tipoVehiculo_id &&
                                 e.rangoinferior == partfranquicia.DetallePartFranquicias[0].rangoinferior &&
                                 e.rangoinferior == partfranquicia.DetallePartFranquicias[0].rangosuperior &&
                             e.ParticipacionFranquicia.Localidad_id == local.id
                            ).ToList();
                    foreach (var detallePartFranquicia in detpartfra)
                    {
                        detallePartFranquicia.porcentaje = partfranquicia.DetallePartFranquicias[0].porcentaje;
                        detpartfraResult.Add(detallePartFranquicia);

                    }

                }
            }

            return detpartfraResult;
        }

        /// <summary>
        /// Metodo que actualiza el detalle participacion franquicias.
        /// se filtra la información a actualizar  a traves de los parametros enviados(ParticipacionFranquicia,franquicias).
        /// En partfranquicia contiene la informacion que se envia desde el  formulario.
        /// EN franquicias contiene el nombre  o los nombres de las franquicias  separados por una coma(",")
        /// </summary>
        /// <param name="partfranquicia"></param>
        /// <param name="franquicias"></param>
        /// <returns></returns>
        public int ActualizarDetallePartFranquicias(DetallePartFranquicia DetallePartFranquicia, DateTime fechaIni, DateTime fechaFin, string franquicias, string Username)
        {
            int regactaulizados = 0;
            List<ParticipacionFranquicia> partfraList = new List<ParticipacionFranquicia>();
            List<ParticipacionFranquicia> partfraListTemp = new List<ParticipacionFranquicia>();
            ParticipacionFranquicia partfra = new ParticipacionFranquicia();
            List<DetallePartFranquicia> detpartfra = new List<DetallePartFranquicia>();
            char[] delimiterChars = { ',' };
            string[] franquiciass = franquicias.Split(delimiterChars);

            //TRAEMOS LAS PARAMETRIZACIONES CREADAS PARA LAS FECHAS Y FRANQUICIAS SELECCIONADAS
            foreach (string idfranq in franquiciass)
            {
                if (!string.IsNullOrEmpty(idfranq))
                {
                    int idFranquicia = Convert.ToInt32(idfranq);
                    partfraListTemp = (from pf in tabla.ParticipacionFranquicias
                                       where pf.fecha_ini == fechaIni && pf.fecha_fin == fechaFin && pf.Localidad_id == idFranquicia
                                       select pf).ToList();

                    if (partfraListTemp != null && partfraListTemp.Count() > 0)
                    {
                        foreach (ParticipacionFranquicia partfranquicia in partfraListTemp)
                        {
                            partfraList.Add(partfranquicia);
                        }
                    }
                }
            }

            //RECORREMOS LAS PARTICIPACIONES QUE COINCIDIERON CON LAS FECHAS
            List<DetallePartFranquicia> detpartfraactualizarList = new List<DetallePartFranquicia>();
            foreach (ParticipacionFranquicia partfranquicia in partfraList)
            {

                //BUSCAMOS LAS PARTICIPACIONES DONDE EL RANGO INFERIOR Y SUPERIOR SEAN NULOS
                if (DetallePartFranquicia.rangoinferior == null && DetallePartFranquicia.rangosuperior == null)
                {
                    detpartfraactualizarList = (from dpf in tabla.DetallePartFranquicias
                                                where dpf.part_franquicia_id == partfranquicia.id &&
                                                dpf.compania_id == DetallePartFranquicia.compania_id &&
                                                dpf.ramo_id == DetallePartFranquicia.ramo_id &&
                                                dpf.producto_id == DetallePartFranquicia.producto_id &&
                                                dpf.tipoVehiculo_id == DetallePartFranquicia.tipoVehiculo_id &&
                                                dpf.plan_id == DetallePartFranquicia.plan_id &&
                                                dpf.lineaNegocio_id == DetallePartFranquicia.lineaNegocio_id &&
                                                dpf.amparo_id == DetallePartFranquicia.amparo_id &&
                                                dpf.rangoinferior == null &&
                                                dpf.rangosuperior == null
                                                select dpf).ToList();
                }
                else
                {
                    detpartfraactualizarList = (from dpf in tabla.DetallePartFranquicias
                                                where dpf.part_franquicia_id == partfranquicia.id &&
                                                dpf.compania_id == DetallePartFranquicia.compania_id &&
                                                dpf.ramo_id == DetallePartFranquicia.ramo_id &&
                                                dpf.producto_id == DetallePartFranquicia.producto_id &&
                                                dpf.tipoVehiculo_id == DetallePartFranquicia.tipoVehiculo_id &&
                                                dpf.plan_id == DetallePartFranquicia.plan_id &&
                                                dpf.lineaNegocio_id == DetallePartFranquicia.lineaNegocio_id &&
                                                dpf.amparo_id == DetallePartFranquicia.amparo_id &&
                                                dpf.rangoinferior == DetallePartFranquicia.rangoinferior &&
                                                dpf.rangosuperior == DetallePartFranquicia.rangosuperior
                                                select dpf).ToList();
                }


                foreach (DetallePartFranquicia detpartfraActualizar in detpartfraactualizarList)
                {
                    var pValorAntiguo = detpartfraActualizar;
                    detpartfraActualizar.porcentaje = DetallePartFranquicia.porcentaje;
                    detpartfraActualizar.rangoinferior = DetallePartFranquicia.rangoinferior;
                    detpartfraActualizar.rangosuperior = DetallePartFranquicia.rangosuperior;
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ModeloxMeta,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, detpartfraActualizar);
                    tabla.SaveChanges();
                    regactaulizados++;
                }
            }

            // se devuelven el numero de registros actualizados
            return regactaulizados;
        }


        public void reportePagosFranquicia(int idLiquidacion)
        {
            ///Se registra que el proceso de liquidación está en proceso
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
            {
                string script_1 = "INSERT INTO ProcesoLiquidacion VALUES (9, " + idLiquidacion.ToString() + ", GETDATE(), 20)";

                conn.Open();

                SqlCommand command = new SqlCommand(script_1, conn);
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 1000;

                command.ExecuteNonQuery();

                command.Connection.Close();
            }

            List<Localidad> ListaFranquicias = tabla.Localidads.Where(x => x.tipo_localidad_id == 2).ToList();

            string Query = string.Empty;


            string usuarioftp = ConfigurationManager.AppSettings["usuarioftp"];
            string passwordftp = ConfigurationManager.AppSettings["contrasenaftp"];
            string rutaFTP = ConfigurationManager.AppSettings["FTPReportes"] + "ReportePagoFranquicias_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm") + "/";
            string rutaarchivo = ConfigurationManager.AppSettings["RutaArchivoPagos"];

            try
            {
                WebRequest request = WebRequest.Create(rutaFTP);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(usuarioftp, passwordftp);
                var resp = (FtpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {

            }

            foreach (Localidad item in ListaFranquicias)
            {
                DataTable tablaDetalle = new DataTable();
                try
                {
                    Query = "SELECT	l.nombre AS 'Franquicia' " +
                            ", s.nombre AS 'Segmento' " +
                            ", CASE Ram.nombre " +
                                "WHEN 'EMERMEDICA' THEN 'EMERMEDICA' " +
                                "ELSE c.nombre " +
                            "END AS 'Compañía' " +
                            ", LiquidacionFranquicia.fechaLiquidacion AS 'Fecha Liquidacion' " +
                            ", Participante.clave AS 'Clave' " +
                            ", LTRIM(RTRIM(Participante.nombre)) + ' ' + LTRIM(RTRIM(Participante.apellidos)) as 'Participante' " +
                            ", Ram.nombre AS 'Ramo Agrupado' " +
                            ", r.nombre AS 'Ramo Detallado' " +
                            ", Prod.nombre AS 'Producto Agrupado' " +
                            ", p.nombre AS 'Producto Detallado' " +
                            ", Lin.nombre AS 'Línea Negocio' " +
                            ", TipoVehiculo AS 'Tipo de vehículo' " +
                            ", dlf.codigo_agrupador AS 'Código Agrupador' " +
                            ", dlf.numeroNegocio AS 'Número de Negocio' " +
                            ", Amparo.nombre AS 'Amparo' " +
                            ", dlf.concepto as 'Concepto' " +
                            ", dlf.altura AS 'Altura' " +
                            ", dlf.fechaRecaudo AS 'Fecha recaudo' " +
                            ", dlf.fechaContabl AS 'Fecha contable' " +
                            ", dlf.valorRecaudo AS 'Valor del Recaudo' " +
                            ", dlf.porcentajeParticipacion AS 'Porcentaje Participacion' " +
                            ", dlf.totalParticipacion AS 'Total Participación' " +
                            ", CASE dlf.liquidadoPor " +
                                "WHEN 1 THEN 'Excepciones' " +
                                "WHEN 2 THEN 'Participacion' " +
                                "WHEN 3 THEN 'Rangos' " +
                                "WHEN 4 THEN 'Especiales' " +
                                "ELSE 'Sin Liquidar' " +
                            "END as 'Liquidado Por' " +
                            "FROM LiquidacionFranquicia " +
                            "INNER JOIN DetalleLiquidacionFranquicia AS dlf ON dlf.liquidacionFranquicia_id = LiquidacionFranquicia.id  " +
                            "INNER JOIN Compania AS c ON dlf.compania_id = c.id  " +
                            "INNER JOIN RamoDetalle AS r ON dlf.ramo_id = r.id  " +
                            "INNER JOIN Ramo Ram on dlf.ramo_id_agrupado = ram.id  " +
                            "INNER JOIN Producto Prod on dlf.producto_id_agrupado =  prod.id " +
                            "INNER JOIN ProductoDetalle AS p ON dlf.producto_id = p.id  " +
                            "LEFT JOIN Amparo ON dlf.amparo_Id = Amparo.id  " +
                            "LEFT JOIN Localidad AS l ON dlf.localidad_id = l.id  " +
                            "LEFT JOIN Participante ON Participante.clave = dlf.claveParticipante " +
                            "LEFT JOIN TipoVehiculo ON TipoVehiculo.id = dlf.tipoVehiculo " +
                            "LEFT JOIN Segmento as s ON s.id = dlf.segmento_id " +
                            "LEFT JOIN LineaNegocio  Lin on dlf.lineaNegocio_id = Lin.id " +
                            "WHERE " +
                            "LiquidacionFranquicia.id =" + idLiquidacion.ToString() +
                            "and l.id =" + item.id.ToString();


                    using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
                    {
                        conn.Open();

                        SqlCommand command = new SqlCommand(Query, conn);
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = 1000;

                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = command;
                        adapter.Fill(tablaDetalle);
                    }

                    if (tablaDetalle.Rows.Count == 0)
                    {
                        continue;
                    }

                    StringWriter sw = new StringWriter();
                    HtmlTextWriter hw = new HtmlTextWriter(sw);
                    GridView GridViewResultados = new GridView();
                    GridViewResultados.DataSource = tablaDetalle;
                    GridViewResultados.AllowPaging = false;
                    GridViewResultados.DataBind();

                    foreach (TableCell cell in GridViewResultados.HeaderRow.Cells)
                    {
                        cell.Style.Add("background-color", "#0f2e86");
                        cell.ForeColor = System.Drawing.Color.White;
                        cell.Font.Bold = true;
                    }

                    GridViewResultados.RenderControl(hw);

                    string renderedGridView = sw.ToString();

                    string nombreArchivo = item.nombre + ".xls";
                    System.IO.File.WriteAllText(rutaarchivo + nombreArchivo, renderedGridView);

                    FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(rutaFTP + nombreArchivo);
                    ftpWebRequest.Credentials = new NetworkCredential(usuarioftp, passwordftp);
                    ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;


                    Byte[] BufferArchivo = File.ReadAllBytes(rutaarchivo + nombreArchivo);
                    Stream stream = ftpWebRequest.GetRequestStream();
                    stream.Write(BufferArchivo, 0, BufferArchivo.Length);
                    stream.Close();
                    stream.Dispose();

                    System.IO.File.Delete(rutaarchivo + nombreArchivo);

                }
                catch (Exception ex)
                {
                    using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
                    {
                        string script_1 = "DELETE FROM ProcesoLiquidacion WHERE liquidacion_id = " + idLiquidacion.ToString();

                        conn.Open();

                        SqlCommand command = new SqlCommand(script_1, conn);
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = 1000;

                        command.ExecuteNonQuery();

                        command.Connection.Close();
                    }
                    throw;
                }
            }

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings[0].ConnectionString))
            {
                string script_1 = "DELETE FROM ProcesoLiquidacion WHERE liquidacion_id = " + idLiquidacion.ToString();

                conn.Open();

                SqlCommand command = new SqlCommand(script_1, conn);
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 1000;

                command.ExecuteNonQuery();

                command.Connection.Close();
            }
        }
    }

}

