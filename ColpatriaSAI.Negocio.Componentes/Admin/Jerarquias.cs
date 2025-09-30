using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Collections;
using System.Data;
using ColpatriaSAI.Negocio.Componentes.Utilidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Jerarquias
    {
        private SAI_Entities tabla = new SAI_Entities();

        #region Jerarquia

        public List<Jerarquia> ListarJerarquias()
        {
            return tabla.Jerarquias.Include("TipoJerarquia").Include("Segmento").Where(j => j.id > 0).ToList();
        }

        public List<TipoJerarquia> ListarTiposJerarquia()
        {
            return tabla.TipoJerarquias.Where(t => t.id > 0).ToList();
        }

        public Jerarquia ListarJerarquiaPorId(int id)
        {
            return tabla.Jerarquias.Include("JerarquiaDetalles").Include("TipoJerarquia").Include("Segmento").Single(j => j.id == id);
        }

        public int InsertarJerarquia(Jerarquia nuevaJerarquia, string userName)
        {
            int cantidad = tabla.Jerarquias.Where(j => j.nombre == nuevaJerarquia.nombre &&
                                            j.segmento_id == nuevaJerarquia.segmento_id &&
                                            j.ano == nuevaJerarquia.ano).Count();
            if (cantidad == 0)
            {

                //AUDITORIA
                Entidades.Auditoria auditoria = new Entidades.Auditoria();
                auditoria.Fecha = DateTime.Now;
                auditoria.id_EventoTabla = (int)tipoEventoTabla.Creacion;
                auditoria.id_TablaAuditada = (int)tablasAuditadas.Jerarquia_Comercial;
                auditoria.Usuario = userName;
                auditoria.Version_Anterior = "";
                auditoria.id_Segmento = nuevaJerarquia.segmento_id;
                auditoria.Version_Nueva = Utilidades.Auditoria.CrearDescripcionAuditoria(
                    new
                    {
                        Nombre = nuevaJerarquia.nombre,
                        TipoJerarquia = (nuevaJerarquia.TipoJerarquia != null ? nuevaJerarquia.TipoJerarquia.nombre : tabla.TipoJerarquias.Where(x => x.id == nuevaJerarquia.tipoJerarquia_id).FirstOrDefault().nombre),
                        Segmento = (nuevaJerarquia.Segmento != null ? nuevaJerarquia.Segmento.nombre : tabla.Segmentoes.Where(x => x.id == nuevaJerarquia.segmento_id).FirstOrDefault().nombre),
                        Año = nuevaJerarquia.ano.ToString()
                    });

                tabla.Auditorias.AddObject(auditoria);

                tabla.Jerarquias.AddObject(nuevaJerarquia);
                cantidad = tabla.SaveChanges();

                tabla.JerarquiaDetalles.AddObject(new JerarquiaDetalle()
                {
                    jerarquia_id = nuevaJerarquia.id,
                    nombre = nuevaJerarquia.nombre,
                    zona_id = 0,
                    localidad_id = 0,
                    canal_id = 0,
                    participante_id = 0,
                    nivel_id = 0
                });
                tabla.SaveChanges();
                return cantidad;
            }
            return 0;
        }

        public int ActualizarJerarquia(int id, Jerarquia nuevaJerarquia, string userName)
        {
            Jerarquia jerarquiaActual = tabla.Jerarquias.Single(j => j.id == id);
            if (jerarquiaActual != null)
            {

                //AUDITORIA
                string estadoAnterior = Utilidades.Auditoria.CrearDescripcionAuditoria(
                    new
                    {
                        Nombre = jerarquiaActual.nombre,
                        TipoJerarquia = (jerarquiaActual.TipoJerarquia != null ? jerarquiaActual.TipoJerarquia.nombre : tabla.TipoJerarquias.Where(x => x.id == jerarquiaActual.tipoJerarquia_id).FirstOrDefault().nombre),
                        Segmento = (jerarquiaActual.Segmento != null ? jerarquiaActual.Segmento.nombre : tabla.Segmentoes.Where(x => x.id == jerarquiaActual.segmento_id).FirstOrDefault().nombre),
                        Año = jerarquiaActual.ano.ToString()
                    });

                string estadoActual = Utilidades.Auditoria.CrearDescripcionAuditoria(
                    new
                    {
                        Nombre = nuevaJerarquia.nombre,
                        TipoJerarquia = (nuevaJerarquia.TipoJerarquia != null ? nuevaJerarquia.TipoJerarquia.nombre : tabla.TipoJerarquias.Where(x => x.id == nuevaJerarquia.tipoJerarquia_id).FirstOrDefault().nombre),
                        Segmento = (nuevaJerarquia.Segmento != null ? nuevaJerarquia.Segmento.nombre : tabla.Segmentoes.Where(x => x.id == nuevaJerarquia.segmento_id).FirstOrDefault().nombre),
                        Año = nuevaJerarquia.ano.ToString()
                    });

                jerarquiaActual.nombre = nuevaJerarquia.nombre;
                jerarquiaActual.tipoJerarquia_id = nuevaJerarquia.tipoJerarquia_id;
                jerarquiaActual.segmento_id = nuevaJerarquia.segmento_id;
                jerarquiaActual.ano = nuevaJerarquia.ano;

                //AUDITORIA
                Entidades.Auditoria auditoria = new Entidades.Auditoria();
                auditoria.Fecha = DateTime.Now;
                auditoria.id_EventoTabla = (int)tipoEventoTabla.Modificacion;
                auditoria.id_TablaAuditada = (int)tablasAuditadas.Jerarquia_Comercial;
                auditoria.Usuario = userName;
                auditoria.id_Segmento = nuevaJerarquia.segmento_id;
                auditoria.Version_Anterior = estadoAnterior;
                auditoria.Version_Nueva = estadoActual;
                tabla.Auditorias.AddObject(auditoria);

                JerarquiaDetalle detalle = tabla.JerarquiaDetalles.Single(j => j.jerarquia_id == jerarquiaActual.id && j.padre_id == null);
                detalle.nombre = jerarquiaActual.nombre;
                return tabla.SaveChanges();
            }
            return 0;
        }

        public int EliminarJerarquia(int id, string Username)
        {
            var jerarquia = tabla.Jerarquias.Single(j => j.id == id);
            tabla.Jerarquias.DeleteObject(jerarquia);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Jerarquia,
    SegmentosInsercion.Personas_Y_Pymes, jerarquia, null);
            return tabla.SaveChanges();
        }
        #endregion
        /*******************************************************************************************/
        #region JerarquiaDetalle

        public List<JerarquiaDetalle> ListarNodosBuscador(int tipo, string texto, int inicio, int cantidad, int nivel, int zona, int canal, string codNivel)
        {
            List<JerarquiaDetalle> nodos = new List<JerarquiaDetalle>();
            if (tipo == 1)
            {
                if (texto != "")
                {
                    //Listado de nodos para la parametrización de Participaciones de Directores (Contrataciónn)
                    nodos = tabla.JerarquiaDetalles.Include("Participante").Include("Zona").Include("Nivel").Include("Canal")
                        .Where(j => j.nivel_id == nivel && (j.Participante.apellidos.Contains(texto) || j.Participante.nombre.Contains(texto))).OrderBy(j => j.id).ToList();
                }
                else if (texto == "" && nivel != 0)
                {
                    //Listado de nodos para la parametrización de Participaciones de Directores (Contrataciónn)
                    nodos = tabla.JerarquiaDetalles.Include("Zona").Include("Nivel").Include("Canal")
                        .Where(j => j.nivel_id == nivel).OrderBy(j => j.id).Skip(inicio).Take(cantidad).ToList();
                }
            }
            else
            {
                if (texto != "" && nivel != 0) //Listado de nodos para la parametrización de Participaciones de Directores (Contrataciónn)
                    nodos = tabla.JerarquiaDetalles.Include("Zona").Include("Nivel").Include("Canal")
                        .Where(j => j.nivel_id == nivel && j.nombre.Contains(texto)).OrderBy(j => j.id).Skip(inicio).Take(cantidad).ToList();

                else if (texto != "" && codNivel != "" && canal != 0)
                    nodos = tabla.JerarquiaDetalles.Include("Zona").Include("Nivel").Include("Canal").Where(j => j.zona_id == zona && j.nivel_id == nivel &&
                        j.nombre.Contains(texto) && j.codigoNivel.Contains(codNivel) && j.canal_id == canal).OrderBy(j => j.id).Skip(inicio).Take(cantidad).ToList();

                else if (texto != "" && codNivel != "" && canal == 0)
                    nodos = tabla.JerarquiaDetalles.Include("Zona").Include("Nivel").Where(j => j.zona_id == zona && j.nivel_id == nivel &&
                        j.nombre.Contains(texto) && j.codigoNivel.Contains(codNivel)).OrderBy(j => j.id).Skip(inicio).Take(cantidad).ToList();

                else if (texto != "" && codNivel == "" && canal == 0)
                    nodos = tabla.JerarquiaDetalles.Include("Zona").Include("Nivel").Where(j => j.zona_id == zona && j.nivel_id == nivel &&
                        j.nombre.Contains(texto)).OrderBy(j => j.id).Skip(inicio).Take(cantidad).ToList();

                else if (texto != "" && codNivel == "" && canal != 0)
                    nodos = tabla.JerarquiaDetalles.Include("Zona").Include("Nivel").Include("Canal").Where(j => j.zona_id == zona && j.nivel_id == nivel &&
                        j.nombre.Contains(texto) && j.canal_id == canal).OrderBy(j => j.id).Skip(inicio).Take(cantidad).ToList();

                else if (texto == "" && codNivel != "" && canal != 0)
                    nodos = tabla.JerarquiaDetalles.Include("Zona").Include("Nivel").Include("Canal").Where(j => j.zona_id == zona && j.nivel_id == nivel &&
                        j.codigoNivel.Contains(codNivel) && j.canal_id == canal).OrderBy(j => j.id).Skip(inicio).Take(cantidad).ToList();

                else if (texto == "" && codNivel == "" && canal != 0)
                    nodos = tabla.JerarquiaDetalles.Include("Zona").Include("Nivel").Include("Canal").Where(j => j.zona_id == zona && j.nivel_id == nivel &&
                        j.canal_id == canal).OrderBy(j => j.id).Skip(inicio).Take(cantidad).ToList();

                else nodos = tabla.JerarquiaDetalles.Include("Zona").Include("Nivel").Where(j => j.zona_id == zona && j.nivel_id == nivel)
                    .OrderBy(j => j.id).Skip(inicio).Take(cantidad).ToList();
            }
            return nodos;
        }

        public List<NodoArbol> ListarArbol(int id, int padre_id)
        {
            List<NodoArbol> Arbol = new List<NodoArbol>();
            List<JerarquiaDetalle> detalles = new List<JerarquiaDetalle>();
            string perfil = "";
            if (padre_id == 0)
            {
                detalles = tabla.JerarquiaDetalles.Where(j => j.jerarquia_id == id && j.padre_id == null).ToList(); perfil = "root";
            }
            else
            {
                detalles = tabla.JerarquiaDetalles.Include("Nivel").Where(j => j.jerarquia_id == id && j.padre_id == padre_id).ToList();
                perfil = "folder";
            }
            foreach (var item in detalles)
            {
                var estructura = armarEstructura(item, perfil);
                estructura.children = new List<NodoArbol>();//ConstruirArbol(id, item.id);//obtenerNodosHijos(id, padre_id).Select(n => new { nombre = n.nombre }).ToList();//
                Arbol.Add(estructura);
            }
            return Arbol; //return tabla.JerarquiaDetalles.Include("Jerarquia").Include("JerarquiaDetalle1").Where(j => j.jerarquia_id == id).ToList();
        }

        public NodoArbol armarEstructura(JerarquiaDetalle nodo, string perfil)
        {
            if (perfil != "root")
            {
                perfil = nodo.Nivel.nombre.Replace(" de ", " ").Replace(" ", "_").Trim().ToLower();
            }
            NodoArbol estructura = new NodoArbol();
            estructura.data = nodo.nombre;
            estructura.attr = new Atributo() { id = nodo.id, rel = perfil, cod = nodo.codJerarquia };
            estructura.state = nodo.JerarquiaDetalle1.Count == 0 ? "open" : "closed";
            return estructura;
        }

        public List<NodoArbol> ConstruirArbol(int jerarquia_id, int? padre_id)
        {
            List<NodoArbol> arbol = new List<NodoArbol>();
            /*
            var nodosHijos = tabla.JerarquiaDetalles.Where(j => j.jerarquia_id == jerarquia_id && j.padre_id == padre_id).ToList();
            foreach (var item in nodosHijos) {
                var estructura = armarEstructura(item, perfil);
                var Nodo            = ConstruirArbol(jerarquia_id, item.id, perfil);
                estructura.children = new List<NodoArbol>();
                arbol.Add(estructura);
            }*/
            return arbol;
        }

        public List<JerarquiaDetalle> obtenerNodosHijos(int jerarquia_id, int? id)
        {
            return tabla.JerarquiaDetalles.Where(j => j.jerarquia_id == jerarquia_id && j.padre_id == id).OrderBy(j => j.codJerarquia).ToList();
        }

        public List<JerarquiaDetalle> ListarJerarquiaDetalle()
        {
            return tabla.JerarquiaDetalles/*.Include("JerarquiaDetalle1")*/.ToList();
        }

        public JerarquiaDetalle ListarNodoArbol(int id)
        {
            List<JerarquiaDetalle> detalles = tabla.JerarquiaDetalles.Include("Participante").Include("Zona").Include("Localidad")
                .Include("Canal").Include("Nivel").Include("JerarquiaDetalle2").Include("Jerarquia").Where(d => d.id == id).ToList();
            if (detalles.Count() != 0)
            {
                //detalles[0].codJerarquia = obtenerCodJerarquia(detalles[0].jerarquia_id, detalles[0].padre_id);
                return detalles.First();
            }
            return new JerarquiaDetalle();
        }

        //public string obtenerCodJerarquia(int jerarquia_id, int? padre_id)
        //{
        //    if (padre_id != null) {

        //    }
        //    else return "";
        //}

        public string generarCodJerarquia(int jerarquia_id, int? padre_id)
        {
            var nodos = obtenerNodosHijos(jerarquia_id, padre_id);
            if (nodos.Count != 0)
                return (Int32.Parse(nodos.Last().codJerarquia) + 1).ToString().PadLeft(2, '0');
            else
                return "01";
        }

        /// <summary>
        /// Este método permite crear o actualizar un nodo de la jerarquía
        /// </summary>
        /// <param name="detalle">Ingresa el nodo con la información a actualizar</param>
        /// <returns></returns>
        public JerarquiaDetalle InsertarJerarquiaDetalle(JerarquiaDetalle detalle, string userName)
        {
            var detalleActual = tabla.JerarquiaDetalles.Where(j => j.codigoNivel == detalle.codigoNivel);
            var jerarquiaTemp = new List<JerarquiaDetalle>(); var nodo = new List<JerarquiaDetalle>();
            int resultado = 0;

            //VALIDAMOS QUE EL USUARIO NO ESTE CREADO PARA LA MISMA JERARQUIA EN EL MISMO CANAL Y SEA UN DIRECTOR(2)
            if (detalle.nivel_id == 2)
            {
                jerarquiaTemp = (from jd in tabla.JerarquiaDetalles
                                 where jd.jerarquia_id == detalle.jerarquia_id && jd.nivel_id == 2 && jd.canal_id == detalle.canal_id && jd.participante_id == detalle.participante_id
                                 && jd.id != detalle.id
                                 select jd).ToList();
            }
            if (jerarquiaTemp.Count() >= 1)
                return new JerarquiaDetalle();
            else
            {
                if (detalle.id == 0)
                {
                    if (detalleActual.Count() == 0)
                    {
                        detalle.codJerarquia = generarCodJerarquia(detalle.jerarquia_id, detalle.padre_id);
                        tabla.JerarquiaDetalles.AddObject(detalle);
                        resultado = tabla.SaveChanges();

                        //AUDITORIA
                        Entidades.Auditoria auditoria = new Entidades.Auditoria();
                        auditoria.Fecha = DateTime.Now;
                        auditoria.id_EventoTabla = (int)tipoEventoTabla.Creacion;
                        auditoria.id_TablaAuditada = (int)tablasAuditadas.Nodo_Jerarquia_Comercial;
                        auditoria.Usuario = userName;
                        auditoria.id_Segmento = (tabla.Jerarquias.Where(x => x.id == detalle.jerarquia_id).FirstOrDefault().segmento_id);
                        auditoria.Version_Anterior = "";
                        auditoria.Version_Nueva = Utilidades.Auditoria.CrearDescripcionAuditoria(
                            new
                            {
                                Canal = (detalle.Canal != null ? detalle.Canal.nombre : tabla.Canals.Where(x => x.id == detalle.canal_id).FirstOrDefault().nombre),
                                CodigoNivel = detalle.codigoNivel,
                                Descripcion = detalle.descripcion,
                                Localidad = (detalle.Localidad != null ? detalle.Localidad.nombre : tabla.Localidads.Where(x => x.id == detalle.localidad_id).FirstOrDefault().nombre),
                                Nivel = (detalle.Nivel != null ? detalle.Nivel.nombre : tabla.Nivels.Where(x => x.id == detalle.nivel_id).FirstOrDefault().nombre),
                                Nombre = detalle.nombre,
                                Padre = (detalle.padre_id != null ? tabla.JerarquiaDetalles.Where(x => x.id == detalle.padre_id).FirstOrDefault().nombre : ""),
                                Participante = (detalle.Participante != null ? detalle.Participante.nombre + " " + detalle.Participante.apellidos : tabla.Participantes.Where(x => x.id == detalle.participante_id).Select(x => x.nombre + " " + x.apellidos).FirstOrDefault().ToString()),
                                Zona = (detalle.Zona != null ? detalle.Zona.nombre : tabla.Zonas.Where(x => x.id == detalle.zona_id).FirstOrDefault().nombre)
                            });

                        tabla.Auditorias.AddObject(auditoria);
                        tabla.SaveChanges();

                        var tablaNodo = generarTablaNodo(ListarNodoArbol(detalle.id), true);
                        //if (resultado > 0)
                        //    Proceso.enviarCorreo(Proceso.CorreoModulo.Jerarquia, "Actualización de la jerarquía '" + detalle.Jerarquia.nombre + "'",
                        //        generarCuerpoCorreo(tablaNodo, null, 1));
                        return new JerarquiaDetalle() { id = detalle.id };
                    }
                    else return new JerarquiaDetalle();
                }
                else
                {
                    if (detalleActual.Where(j => j.id != detalle.id).Count() == 0)
                    {
                        JerarquiaDetalle nodoActual = tabla.JerarquiaDetalles.Where(j => j.jerarquia_id == detalle.jerarquia_id && j.id == detalle.id).First();
                        string tablaNodoAnterior = generarTablaNodo(ListarNodoArbol(nodoActual.id), false);

                        //AUDITORIA
                        string estadoAnterior = Utilidades.Auditoria.CrearDescripcionAuditoria(
                            new
                            {
                                Canal = (nodoActual.Canal != null ? nodoActual.Canal.nombre : tabla.Canals.Where(x => x.id == nodoActual.canal_id).FirstOrDefault().nombre),
                                CodigoNivel = nodoActual.codigoNivel,
                                Descripcion = nodoActual.descripcion,
                                Localidad = (nodoActual.Localidad != null ? nodoActual.Localidad.nombre : tabla.Localidads.Where(x => x.id == nodoActual.localidad_id).FirstOrDefault().nombre),
                                Nivel = (nodoActual.Nivel != null ? nodoActual.Nivel.nombre : tabla.Nivels.Where(x => x.id == nodoActual.nivel_id).FirstOrDefault().nombre),
                                Nombre = nodoActual.nombre,
                                Padre = (nodoActual.padre_id != null ? tabla.JerarquiaDetalles.Where(x => x.id == nodoActual.padre_id).FirstOrDefault().nombre : ""),
                                Participante = (nodoActual.Participante != null ? nodoActual.Participante.nombre + " " + nodoActual.Participante.apellidos : tabla.Participantes.Where(x => x.id == nodoActual.participante_id).Select(x => x.nombre + " " + x.apellidos).FirstOrDefault().ToString()),
                                Zona = (nodoActual.Zona != null ? nodoActual.Zona.nombre : tabla.Zonas.Where(x => x.id == nodoActual.zona_id).FirstOrDefault().nombre)
                            });

                        string estadoActual = Utilidades.Auditoria.CrearDescripcionAuditoria(
                            new
                            {
                                Canal = (detalle.Canal != null ? detalle.Canal.nombre : tabla.Canals.Where(x => x.id == detalle.canal_id).FirstOrDefault().nombre),
                                CodigoNivel = detalle.codigoNivel,
                                Descripcion = detalle.descripcion,
                                Localidad = (detalle.Localidad != null ? detalle.Localidad.nombre : tabla.Localidads.Where(x => x.id == detalle.localidad_id).FirstOrDefault().nombre),
                                Nivel = (detalle.Nivel != null ? detalle.Nivel.nombre : tabla.Nivels.Where(x => x.id == detalle.nivel_id).FirstOrDefault().nombre),
                                Nombre = detalle.nombre,
                                Padre = (detalle.padre_id != null ? tabla.JerarquiaDetalles.Where(x => x.id == detalle.padre_id).FirstOrDefault().nombre : ""),
                                Participante = (detalle.Participante != null ? detalle.Participante.nombre + " " + detalle.Participante.apellidos : tabla.Participantes.Where(x => x.id == detalle.participante_id).Select(x => x.nombre + " " + x.apellidos).FirstOrDefault().ToString()),
                                Zona = (detalle.Zona != null ? detalle.Zona.nombre : tabla.Zonas.Where(x => x.id == detalle.zona_id).FirstOrDefault().nombre)
                            });

                        detalle.codJerarquia = nodoActual.codJerarquia; //generarCodJerarquia(detalle.jerarquia_id, nodoActual.padre_id).ToString().PadLeft(2, '0');
                        nodoActual.nombre = detalle.nombre;
                        nodoActual.descripcion = detalle.descripcion;
                        nodoActual.zona_id = detalle.zona_id;
                        nodoActual.localidad_id = detalle.localidad_id;
                        nodoActual.canal_id = detalle.canal_id;
                        nodoActual.participante_id = detalle.participante_id;
                        nodoActual.codigoNivel = detalle.codigoNivel;
                        nodoActual.nivel_id = detalle.nivel_id;
                        nodoActual.codJerarquia = detalle.codJerarquia;

                        //AUDITORIA
                        Entidades.Auditoria auditoria = new Entidades.Auditoria();
                        auditoria.Fecha = DateTime.Now;
                        auditoria.id_EventoTabla = (int)tipoEventoTabla.Modificacion;
                        auditoria.id_TablaAuditada = (int)tablasAuditadas.Nodo_Jerarquia_Comercial;
                        auditoria.Usuario = userName;
                        auditoria.id_Segmento = (tabla.Jerarquias.Where(x => x.id == detalle.jerarquia_id).FirstOrDefault().segmento_id);
                        auditoria.Version_Anterior = estadoAnterior;
                        auditoria.Version_Nueva = estadoActual;
                        tabla.Auditorias.AddObject(auditoria);

                        resultado = tabla.SaveChanges();

                        var tablaNodoNuevo = generarTablaNodo(ListarNodoArbol(nodoActual.id), true);
                        //if (resultado > 0)
                        //    Proceso.enviarCorreo(Proceso.CorreoModulo.Jerarquia, "Actualización de la jerarquía '" + nodoActual.Jerarquia.nombre + "'",
                        //        generarCuerpoCorreo(tablaNodoAnterior, tablaNodoNuevo, 2));
                        return new JerarquiaDetalle() { id = nodoActual.id };
                    }
                    else return new JerarquiaDetalle();
                }
            }
        }

        public int ActualizarOrdenNodo(JerarquiaDetalle detalle, string userName)
        {
            int resultado = 0;
            var nodoActual = tabla.JerarquiaDetalles.Single(d => d.id == detalle.id);

            //AUDITORIA
            string estadoAnterior = Utilidades.Auditoria.CrearDescripcionAuditoria(
                new
                {
                    Observacion = "Cambio de Padre",
                    Canal = (nodoActual.Canal != null ? nodoActual.Canal.nombre : tabla.Canals.Where(x => x.id == nodoActual.canal_id).FirstOrDefault().nombre),
                    CodigoNivel = nodoActual.codigoNivel,
                    Descripcion = nodoActual.descripcion,
                    Localidad = (nodoActual.Localidad != null ? nodoActual.Localidad.nombre : tabla.Localidads.Where(x => x.id == nodoActual.localidad_id).FirstOrDefault().nombre),
                    Nivel = (nodoActual.Nivel != null ? nodoActual.Nivel.nombre : tabla.Nivels.Where(x => x.id == nodoActual.nivel_id).FirstOrDefault().nombre),
                    Nombre = nodoActual.nombre,
                    Padre = (nodoActual.padre_id != null ? tabla.JerarquiaDetalles.Where(x => x.id == nodoActual.padre_id).FirstOrDefault().nombre : ""),
                    Participante = (nodoActual.Participante != null ? nodoActual.Participante.nombre + " " + nodoActual.Participante.apellidos : tabla.Participantes.Where(x => x.id == nodoActual.participante_id).Select(x => x.nombre + " " + x.apellidos).FirstOrDefault().ToString()),
                    Zona = (nodoActual.Zona != null ? nodoActual.Zona.nombre : tabla.Zonas.Where(x => x.id == nodoActual.zona_id).FirstOrDefault().nombre)
                });

            JerarquiaDetalle nodoPadre = tabla.JerarquiaDetalles.Single(jd => jd.id == detalle.padre_id);
            nodoActual.padre_id = detalle.padre_id;
            nodoActual.zona_id = nodoPadre.zona_id;
            nodoActual.localidad_id = nodoPadre.localidad_id;
            nodoActual.canal_id = nodoPadre.canal_id;

            string estadoActual = Utilidades.Auditoria.CrearDescripcionAuditoria(
                new
                {
                    Observacion = "Cambio de Padre",
                    Canal = (nodoPadre.Canal != null ? nodoPadre.Canal.nombre : tabla.Canals.Where(x => x.id == nodoPadre.canal_id).FirstOrDefault().nombre),
                    CodigoNivel = nodoActual.codigoNivel,
                    Descripcion = nodoActual.descripcion,
                    Localidad = (nodoPadre.Localidad != null ? nodoPadre.Localidad.nombre : tabla.Localidads.Where(x => x.id == nodoPadre.localidad_id).FirstOrDefault().nombre),
                    Nivel = (nodoActual.Nivel != null ? nodoActual.Nivel.nombre : tabla.Nivels.Where(x => x.id == nodoActual.nivel_id).FirstOrDefault().nombre),
                    Nombre = nodoActual.nombre,
                    Padre = (detalle.padre_id != null ? tabla.JerarquiaDetalles.Where(x => x.id == nodoActual.padre_id).FirstOrDefault().nombre : ""),
                    Participante = (nodoActual.Participante != null ? nodoActual.Participante.nombre + " " + nodoActual.Participante.apellidos : tabla.Participantes.Where(x => x.id == nodoActual.participante_id).Select(x => x.nombre + " " + x.apellidos).FirstOrDefault().ToString()),
                    Zona = (nodoPadre.Zona != null ? nodoPadre.Zona.nombre : tabla.Zonas.Where(x => x.id == nodoPadre.zona_id).FirstOrDefault().nombre)
                });

            //AUDITORIA
            Entidades.Auditoria auditoria = new Entidades.Auditoria();
            auditoria.Fecha = DateTime.Now;
            auditoria.id_EventoTabla = (int)tipoEventoTabla.Modificacion;
            auditoria.id_TablaAuditada = (int)tablasAuditadas.Nodo_Jerarquia_Comercial;
            auditoria.Usuario = userName;
            auditoria.id_Segmento = (tabla.Jerarquias.Where(x => x.id == detalle.jerarquia_id).FirstOrDefault().segmento_id);
            auditoria.Version_Anterior = estadoAnterior;
            auditoria.Version_Nueva = estadoActual;
            tabla.Auditorias.AddObject(auditoria);

            resultado = tabla.SaveChanges();
            ActualizarInformacionHijos(nodoActual, userName);
            return resultado;
        }

        public void ActualizarInformacionHijos(JerarquiaDetalle nodoActual, string userName)
        {

            List<JerarquiaDetalle> hijos = obtenerNodosHijos(nodoActual.jerarquia_id, nodoActual.id);
            foreach (JerarquiaDetalle hijo in hijos)
            {
                //AUDITORIA
                string estadoAnterior = Utilidades.Auditoria.CrearDescripcionAuditoria(
                    new
                    {
                        Observacion = "Cambio de Zona, Localidad y Canal por causa de cambio de Jerarquía del Nodo Padre",
                        Canal = (hijo.Canal != null ? hijo.Canal.nombre : tabla.Canals.Where(x => x.id == hijo.canal_id).FirstOrDefault().nombre),
                        CodigoNivel = hijo.codigoNivel,
                        Descripcion = hijo.descripcion,
                        Localidad = (hijo.Localidad != null ? hijo.Localidad.nombre : tabla.Localidads.Where(x => x.id == hijo.localidad_id).FirstOrDefault().nombre),
                        Nivel = (hijo.Nivel != null ? hijo.Nivel.nombre : tabla.Nivels.Where(x => x.id == hijo.nivel_id).FirstOrDefault().nombre),
                        Nombre = hijo.nombre,
                        Padre = (hijo.padre_id != null ? tabla.JerarquiaDetalles.Where(x => x.id == hijo.padre_id).FirstOrDefault().nombre : ""),
                        Participante = (hijo.Participante != null ? hijo.Participante.nombre + " " + hijo.Participante.apellidos : tabla.Participantes.Where(x => x.id == hijo.participante_id).Select(x => x.nombre + " " + x.apellidos).FirstOrDefault().ToString()),
                        Zona = (hijo.Zona != null ? hijo.Zona.nombre : tabla.Zonas.Where(x => x.id == hijo.zona_id).FirstOrDefault().nombre)
                    });

                hijo.zona_id = nodoActual.zona_id;
                hijo.localidad_id = nodoActual.localidad_id;
                hijo.canal_id = nodoActual.canal_id;

                string estadoActual = Utilidades.Auditoria.CrearDescripcionAuditoria(
                new
                {
                    Observacion = "Cambio de Zona, Localidad y Canal por causa de cambio de Jerarquía del Nodo Padre",
                    Canal = (nodoActual.Canal != null ? nodoActual.Canal.nombre : tabla.Canals.Where(x => x.id == nodoActual.canal_id).FirstOrDefault().nombre),
                    CodigoNivel = hijo.codigoNivel,
                    Descripcion = hijo.descripcion,
                    Localidad = (nodoActual.Localidad != null ? nodoActual.Localidad.nombre : tabla.Localidads.Where(x => x.id == nodoActual.localidad_id).FirstOrDefault().nombre),
                    Nivel = (hijo.Nivel != null ? hijo.Nivel.nombre : tabla.Nivels.Where(x => x.id == hijo.nivel_id).FirstOrDefault().nombre),
                    Nombre = hijo.nombre,
                    Padre = (hijo.padre_id != null ? tabla.JerarquiaDetalles.Where(x => x.id == hijo.padre_id).FirstOrDefault().nombre : ""),
                    Participante = (hijo.Participante != null ? hijo.Participante.nombre + " " + hijo.Participante.apellidos : tabla.Participantes.Where(x => x.id == hijo.participante_id).Select(x => x.nombre + " " + x.apellidos).FirstOrDefault().ToString()),
                    Zona = (nodoActual.Zona != null ? nodoActual.Zona.nombre : tabla.Zonas.Where(x => x.id == nodoActual.zona_id).FirstOrDefault().nombre)
                });

                //AUDITORIA
                Entidades.Auditoria auditoria = new Entidades.Auditoria();
                auditoria.Fecha = DateTime.Now;
                auditoria.id_EventoTabla = (int)tipoEventoTabla.Modificacion;
                auditoria.id_TablaAuditada = (int)tablasAuditadas.Nodo_Jerarquia_Comercial;
                auditoria.Usuario = userName;
                auditoria.id_Segmento = (tabla.Jerarquias.Where(x => x.id == nodoActual.jerarquia_id).FirstOrDefault().segmento_id);
                auditoria.Version_Anterior = estadoAnterior;
                auditoria.Version_Nueva = estadoActual;
                tabla.Auditorias.AddObject(auditoria);

                tabla.SaveChanges();
                ActualizarInformacionHijos(hijo, userName);
            }
        }

        public int EliminarJerarquiaDetalle(int id, string userName)
        {
            var detalles = ListarNodoArbol(id);
            int resultado = 0;
            if (detalles.id != 0)
            {

                List<JerarquiaDetalle> hijos = obtenerNodosHijos(detalles.jerarquia_id, detalles.id).ToList();

                // Consultar si el nodo tiene excepciones como origen o como destino para evitar la eliminación.
                ExcepcionesJerarquia excepciones = new ExcepcionesJerarquia();
                List<ExcepcionJerarquiaDetalle> excepcionesOrigen = excepciones.ListarExcepcionesJerarquiaporId(id);
                List<ExcepcionJerarquiaDetalle> excepcionesDestino = excepciones.ListarExcepcionesJerarquiaporIdDestino(id);

                if ((hijos.Count == 0) && (excepcionesOrigen.Count == 0) && (excepcionesDestino.Count == 0))
                {
                    var nombreJerarquia = detalles.Jerarquia.nombre;
                    var cuerpoCorreo = generarCuerpoCorreo(generarTablaNodo(detalles, true), null, 0);

                    //AUDITORIA
                    Entidades.Auditoria auditoria = new Entidades.Auditoria();
                    auditoria.Fecha = DateTime.Now;
                    auditoria.id_EventoTabla = (int)tipoEventoTabla.Eliminacion;
                    auditoria.id_TablaAuditada = (int)tablasAuditadas.Nodo_Jerarquia_Comercial;
                    auditoria.Usuario = userName;
                    auditoria.id_Segmento = (tabla.Jerarquias.Where(x => x.id == detalles.jerarquia_id).FirstOrDefault().segmento_id);
                    auditoria.Version_Anterior = Utilidades.Auditoria.CrearDescripcionAuditoria(
                        new
                        {
                            Canal = (detalles.Canal != null ? detalles.Canal.nombre : tabla.Canals.Where(x => x.id == detalles.canal_id).FirstOrDefault().nombre),
                            CodigoNivel = detalles.codigoNivel,
                            Descripcion = detalles.descripcion,
                            Localidad = (detalles.Localidad != null ? detalles.Localidad.nombre : tabla.Localidads.Where(x => x.id == detalles.localidad_id).FirstOrDefault().nombre),
                            Nivel = (detalles.Nivel != null ? detalles.Nivel.nombre : tabla.Nivels.Where(x => x.id == detalles.nivel_id).FirstOrDefault().nombre),
                            Nombre = detalles.nombre,
                            Padre = (detalles.padre_id != null ? tabla.JerarquiaDetalles.Where(x => x.id == detalles.padre_id).FirstOrDefault().nombre : ""),
                            Participante = (detalles.Participante != null ? detalles.Participante.nombre + " " + detalles.Participante.apellidos : tabla.Participantes.Where(x => x.id == detalles.participante_id).Select(x => x.nombre + " " + x.apellidos).FirstOrDefault().ToString()),
                            Zona = (detalles.Zona != null ? detalles.Zona.nombre : tabla.Zonas.Where(x => x.id == detalles.zona_id).FirstOrDefault().nombre)
                        });
                    auditoria.Version_Nueva = "";

                    tabla.Auditorias.AddObject(auditoria);

                    tabla.JerarquiaDetalles.DeleteObject(detalles);
                    resultado = tabla.SaveChanges();
                    //if (resultado > 0)
                    //    Proceso.enviarCorreo(Proceso.CorreoModulo.Jerarquia, "Actualización de la jerarquía '" + nombreJerarquia + "'", cuerpoCorreo);
                }
                //foreach (JerarquiaDetalle item in hijos) {
                //    tabla.JerarquiaDetalles.Attach(item);
                //    tabla.ObjectStateManager.ChangeObjectState(item, EntityState.Deleted);
                //    EliminarJerarquiaDetalle(item.id);
                //    tabla.SaveChanges();
                //}
            }
            return resultado;
        }

        public string generarTablaNodo(JerarquiaDetalle nodo, bool principal)
        {
            var color = principal ? "#8EC780" : "#CCC";
            return "<table cellspacing='0' style='font:12px Arial; border-spacing:0px; border:solid 1px " + color + ";'>\n" +
                "<tr><td style='font-weight:bold; border:solid 1px " + color + "; padding:3px 5px 3px 5px'>Nombre del nodo</td><td style='border:solid 1px " + color + "; padding:3px 5px 3px 5px'>" + nodo.nombre + "</td></tr>\n" +
                "<tr><td style='font-weight:bold; border:solid 1px " + color + "; padding:3px 5px 3px 5px'>Descripción</td><td style='border:solid 1px " + color + "; padding:3px 5px 3px 5px'>" + nodo.descripcion + "</td></tr>\n" +
                "<tr><td style='font-weight:bold; border:solid 1px " + color + "; padding:3px 5px 3px 5px'>Nodo padre</td><td style='border:solid 1px " + color + "; padding:3px 5px 3px 5px'>" + nodo.JerarquiaDetalle2.nombre + "</td></tr>\n" +
                "<tr><td style='font-weight:bold; border:solid 1px " + color + "; padding:3px 5px 3px 5px'>Zona</td><td style='border:solid 1px " + color + "; padding:3px 5px 3px 5px'>" + nodo.Zona.nombre + "</td></tr>\n" +
                "<tr><td style='font-weight:bold; border:solid 1px " + color + "; padding:3px 5px 3px 5px'>Localidad</td><td style='border:solid 1px " + color + "; padding:3px 5px 3px 5px'>" + nodo.Localidad.nombre + "</td></tr>\n" +
                "<tr><td style='font-weight:bold; border:solid 1px " + color + "; padding:3px 5px 3px 5px'>Canal</td><td style='border:solid 1px " + color + "; padding:3px 5px 3px 5px'>" + nodo.Canal.nombre + "</td></tr>\n" +
                "<tr><td style='font-weight:bold; border:solid 1px " + color + "; padding:3px 5px 3px 5px'>Funcionario</td><td style='border:solid 1px " + color + "; padding:3px 5px 3px 5px'>" + nodo.Participante.nombre + " " + nodo.Participante.apellidos + "</td></tr>\n" +
                "<tr><td style='font-weight:bold; border:solid 1px " + color + "; padding:3px 5px 3px 5px'>Nivel</td><td style='border:solid 1px " + color + "; padding:3px 5px 3px 5px'>" + nodo.Nivel.nombre + "</td></tr>\n" +
                "<tr><td style='font-weight:bold; border:solid 1px " + color + "; padding:3px 5px 3px 5px'>Código de nivel:</td><td style='border:solid 1px " + color + "; padding:3px 5px 3px 5px'>" + nodo.codigoNivel + "</td></tr></table>";
        }

        public string generarCuerpoCorreo(string tablaNodo1, string tablaNodo2, int accion)
        {
            var texto = ""; var cuerpo = "";
            if (accion == 0) texto = "eliminado";
            else if (accion == 1) texto = "creado";
            else if (accion == 2)
            {
                texto = "actualizado";
                cuerpo = "<br /><br /><span style='font:12px Arial'>Y debe quedar con la siguiente información:</span><br /><br />\n";
            }
            return "<span style='font:12px Arial'>Se ha " + texto + " el siguiente nodo de la jerarquía:</span><br /><br />\n" + tablaNodo1 + cuerpo + tablaNodo2;
        }
        #endregion
    }
}
