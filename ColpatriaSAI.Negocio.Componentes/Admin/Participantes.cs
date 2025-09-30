using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Participantes
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Este método permite obtener el listado de participantes de acuerdo al parámetro enviado. 
        /// Si es null retorna solo ejecutivos creados en el SAI
        /// Si es 1 retorna todos
        /// Si es 2 retorna solo asesores
        /// </summary>
        /// <param name="nivelGP"></param>
        /// <returns>Retorna un listado de participantes</returns>
        public List<Participante> ListarParticipantes(int? nivelGP)
        {
            if (nivelGP == null)
                return tabla.Participantes.Include("Canal").Include("Categoria").Include("Compania").Include("Localidad").Include("Nivel")
                    .Include("TipoDocumento").Include("TipoParticipante").Include("Zona").Include("EstadoParticipante").Include("Segmento")
                    .Where(p => p.id > 0  && p.GP == false).ToList();
            //else
            //    if (nivelGP == 1)
            //        return tabla.Participantes.Include("Nivel").Include("TipoDocumento").Include("Segmento")
            //            .Where(p => p.id > 0).ToList();
            else
                return tabla.Participantes.Include("Nivel").Include("TipoDocumento").Include("Segmento").ToList();
            //.Where(p => p.id > 0 && p.GP == true)
        }

        #region Metodos POP PARTICIPANTES

        /// <summary>
        /// Permite obtener un listado de participantes con la opción de paginar los registros
        /// </summary>
        /// <param name="inicio">Registro inicial</param>
        /// <param name="cantidad">Cantidad de registros a retornar</param>
        /// <param name="localidad_id">Parametro opcional que permite filtrar solamente los participantes de una localidad determinada</param>
        /// <returns>Retorna una lista de participantes</returns>
        public List<Participante> ListarParticipantesIndex(int inicio, int cantidad, int? zona_id)
        {
            if (zona_id == 0)
            {
                if (cantidad > 0)
                    return tabla.Participantes.Where(p => p.id > 0).OrderBy(p => p.id).Skip(inicio).Take(cantidad).ToList();
                else
                    return tabla.Participantes.Where(p => p.id > 0).OrderBy(p => p.id).Skip(inicio).ToList();

            }
            else
            {
                if (cantidad > 0)
                    return tabla.Participantes.Where(p => p.id > 0 && p.zona_id == zona_id).OrderBy(p => p.id).Skip(inicio).Take(cantidad).ToList();
                else
                    return tabla.Participantes.Where(p => p.id > 0 && p.zona_id == zona_id).OrderBy(p => p.id).Skip(inicio).ToList();
            }
        }

        public List<JerarquiaDetalle> ListarJerarquiaIndex(int inicio, int cantidad, int? zona_id)
        {
            if (zona_id == 0)
                return tabla.JerarquiaDetalles.Where(jd => jd.id > 0).OrderBy(jd => jd.id).Skip(inicio).Take(cantidad).ToList();
            else
                return tabla.JerarquiaDetalles.Where(jd => jd.id > 0 && jd.zona_id == zona_id).OrderBy(jd => jd.id).Skip(inicio).Take(cantidad).ToList();
        }

        public List<Participante> ListarParticipantesBuscador(string texto, int inicio, int cantidad, int nivel, int zona)
        {
            List<Participante> participante = new List<Participante>();//.Where(p => p.estadoParticipante_id != 5)
            if (texto != "" && nivel != 0)
                if (zona != 0)
                    //participante = tabla.Participantes.Include("Nivel").Include("JerarquiaDetalle")
                    //.Where(p => (p.nombre.Contains(texto) || p.apellidos.Contains(texto) || p.clave.Contains(texto)) && p.nivel_id == nivel && p.zona_id == zona && p.id > 0)
                    //.OrderBy(p => p.id).Skip(inicio).Take(cantidad).ToList();
                    participante = (from pa in tabla.Participantes.Include("Nivel")
                                    join je in tabla.JerarquiaDetalles on pa.id equals je.participante_id
                                    where (pa.nombre.Contains(texto)
                                    || pa.apellidos.Contains(texto)
                                    || pa.clave.Contains(texto))
                                    && je.nivel_id == nivel
                                    && pa.zona_id == zona
                                    && pa.id > 0
                                    orderby pa.id
                                    select pa).Skip(inicio).Take(cantidad).Distinct().ToList();
                else
                    //participante = tabla.Participantes.Include("Nivel")
                    //.Where(p => (p.nombre.Contains(texto) || p.apellidos.Contains(texto) || p.clave.Contains(texto)) && p.nivel_id == nivel && p.id > 0)
                    //.OrderBy(p => p.id).Skip(inicio).Take(cantidad).ToList();
                    participante = (from pa in tabla.Participantes
                                    where (pa.nombre.Contains(texto)
                                    || pa.apellidos.Contains(texto)
                                    || pa.clave.Contains(texto))
                                    && pa.id > 0
                                    orderby pa.id
                                    select pa).Skip(inicio).Take(cantidad).Distinct().ToList();
            else
                if (texto == "" && nivel != 0)
                    if (zona != 0)
                        //participante = tabla.Participantes.Include("Nivel")
                        //.Where(p => p.nivel_id == nivel && p.zona_id == zona && p.id > 0)
                        //.OrderBy(p => p.id).Skip(inicio).Take(cantidad).ToList();
                        participante = (from pa in tabla.Participantes.Include("Nivel")
                                        join je in tabla.JerarquiaDetalles on pa.id equals je.participante_id
                                        where je.nivel_id == nivel
                                        && pa.zona_id == zona
                                        && pa.id > 0
                                        orderby pa.id
                                        select pa).Skip(inicio).Take(cantidad).Distinct().ToList();
                    else
                        //participante = tabla.Participantes.Include("Nivel")
                        //.Where(p => p.nivel_id == nivel && p.id > 0)
                        //.OrderBy(p => p.id).Skip(inicio).Take(cantidad).ToList();
                        participante = (from pa in tabla.Participantes.Include("Nivel")
                                        join je in tabla.JerarquiaDetalles on pa.id equals je.participante_id
                                        where je.nivel_id == nivel
                                        && pa.id > 0
                                        orderby pa.id
                                        select pa).Skip(inicio).Take(cantidad).Distinct().ToList();
                else
                    if (texto != "" && nivel == 0)
                        if (zona != 0)
                            participante = tabla.Participantes.Include("Nivel")
                            .Where(p => (p.nombre.Contains(texto) || p.apellidos.Contains(texto) || p.clave.Contains(texto)) && p.zona_id == zona && p.id > 0)
                            .OrderBy(p => p.id).Skip(inicio).Take(cantidad).ToList();
                        else
                            participante = tabla.Participantes.Include("Nivel")
                            .Where(p => (p.nombre.Contains(texto) || p.apellidos.Contains(texto) || p.clave.Contains(texto)) && p.id > 0)
                            .OrderBy(p => p.id).Skip(inicio).Take(cantidad).ToList();
                    else
                        if (zona != 0)
                            participante = tabla.Participantes.Include("Nivel")
                            .Where(p => p.zona_id == zona && p.id > 0)
                            .OrderBy(p => p.id).Skip(inicio).Take(cantidad).ToList();
                        else
                            participante = tabla.Participantes.Include("Nivel")
                            .Where(p => p.id > 0)
                            .OrderBy(p => p.id).Skip(inicio).Take(cantidad).ToList();

            return participante;
        }

        public List<Participante> ListarParticipantesBuscadorxCampo(string Campo, string Valor)
        {
            List<Participante> participante = new List<Participante>();
            switch (Campo)
            {
                case "Clave":
                    participante = tabla.Participantes.Where(x => x.clave == Valor).ToList();
                    break;
                default:
                    break;
            }

            return participante;
        }

        public List<JerarquiaDetalle> ListarJerarquiaBuscador(string texto, int inicio, int cantidad, int nivel, int zona)
        {
            List<JerarquiaDetalle> jerarquia = new List<JerarquiaDetalle>();
            if (texto != "Digite Código de Nivel" && nivel != 0) // El Texto viene por defecto del control Jerarquia de la clase ParticipanteConcurso
                if (zona != 0)
                    jerarquia = tabla.JerarquiaDetalles.Include("Nivel")
                    .Where(jd => jd.codigoNivel.Contains(texto) && jd.nivel_id == nivel && jd.zona_id == zona && jd.id > 0)
                    .OrderBy(jd => jd.id).Skip(inicio).Take(cantidad).ToList();
                else
                    jerarquia = tabla.JerarquiaDetalles.Include("Nivel")
                    .Where(jd => jd.codigoNivel.Contains(texto) && jd.nivel_id == nivel && jd.id > 0)
                    .OrderBy(jd => jd.id).Skip(inicio).Take(cantidad).ToList();
            else
                if (texto == "Digite Código de Nivel" && nivel != 0)
                    if (zona != 0)
                        jerarquia = tabla.JerarquiaDetalles.Include("Nivel")
                        .Where(jd => jd.nivel_id == nivel && jd.zona_id == zona)
                        .OrderBy(jd => jd.id).Skip(inicio).Take(cantidad).ToList();
                    else
                        jerarquia = tabla.JerarquiaDetalles.Include("Nivel")
                        .Where(jd => jd.nivel_id == nivel && jd.id > 0)
                        .OrderBy(jd => jd.id).Skip(inicio).Take(cantidad).ToList();
                else
                    if (texto != "Digite Código de Nivel" && nivel == 0)
                        if (zona == 0)
                            jerarquia = tabla.JerarquiaDetalles.Include("Nivel")
                            .Where(jd => jd.codigoNivel.Contains(texto) && jd.id > 0)
                            .OrderBy(jd => jd.id).Skip(inicio).Take(cantidad).ToList();
                        else
                            jerarquia = tabla.JerarquiaDetalles.Include("Nivel")
                            .Where(jd => jd.id > 0)
                            .OrderBy(jd => jd.id).Skip(inicio).Take(cantidad).ToList();
                    else
                        if (zona != 0)
                            jerarquia = tabla.JerarquiaDetalles.Include("Nivel")
                            .Where(jd => jd.zona_id == zona && jd.id > 0)
                            .OrderBy(jd => jd.id).Skip(inicio).Take(cantidad).ToList();
                        else
                            jerarquia = tabla.JerarquiaDetalles.Include("Nivel")
                            .Where(jd => jd.id > 0)
                            .OrderBy(jd => jd.id).Skip(inicio).Take(cantidad).ToList();

            return jerarquia;
        }

        public List<Participante> ListarParticipanteXCedula(string cedula)
        {
            return tabla.Participantes.Include("Canal").Include("Categoria").Include("Compania").Include("Localidad").Include("Nivel")
                    .Include("TipoDocumento").Include("TipoParticipante").Include("Zona").Include("EstadoParticipante").Include("Segmento")
                    .Where(p => p.documento == cedula).ToList();
        }
        #endregion

        public List<Participante> ListarParticipantesPorId(int id)
        {
            return tabla.Participantes.Where(Participante => Participante.id == id && Participante.id > 0).ToList();
        }

        public int InsertarParticipante(Participante participante, string userName)
        {
            int resultado = 0;
            List<Participante> participantes = tabla.Participantes.Where(Participante => Participante.documento == participante.documento && Participante.tipoDocumento_id == participante.tipoDocumento_id && Participante.GP == false).ToList();
            int cantidad = participantes.Count();
            if (cantidad == 0)
            {
                //AUDITORIA
                Entidades.Auditoria auditoria = new Entidades.Auditoria();
                auditoria.Fecha = DateTime.Now;
                auditoria.id_EventoTabla = (int)tipoEventoTabla.Creacion;
                auditoria.id_TablaAuditada = (int)tablasAuditadas.Participante;
                auditoria.Usuario = userName;
                auditoria.id_Segmento = (int)participante.segmento_id;
                auditoria.Version_Anterior = "";
                auditoria.Version_Nueva = Utilidades.Auditoria.CrearDescripcionAuditoria(
                    new
                    {
                        Nombre = participante.nombre,
                        Apellidos = participante.apellidos,
                        TipoDocumento = (participante.TipoDocumento != null ? participante.TipoDocumento.nombre : tabla.TipoDocumentoes.Where(x => x.id == participante.tipoDocumento_id).FirstOrDefault().nombre),
                        Documento = participante.documento,
                        Estado = (participante.EstadoParticipante != null ? participante.EstadoParticipante.nombre : tabla.EstadoParticipantes.Where(x => x.id == participante.estadoParticipante_id).FirstOrDefault().nombre),
                        FechaIngreso = ((DateTime)participante.fechaIngreso).ToShortDateString(),
                        FechaRetiro = ((DateTime)participante.fechaRetiro).ToShortDateString(),
                        FechaNacimiento = ((DateTime)participante.fechaNacimiento).ToShortDateString(),
                        CompañiaNomina = (participante.Compania != null ? participante.Compania.nombre : tabla.Companias.Where(x => x.id == participante.compania_id).FirstOrDefault().nombre),
                        Nivel = (participante.Nivel != null ? participante.Nivel.nombre : tabla.Nivels.Where(x => x.id == participante.nivel_id).FirstOrDefault().nombre),
                        Zona = (participante.Zona != null ? participante.Zona.nombre : tabla.Zonas.Where(x => x.id == participante.zona_id).FirstOrDefault().nombre),
                        Localidad = (participante.Localidad != null ? participante.Localidad.nombre : tabla.Localidads.Where(x => x.id == participante.localidad_id).FirstOrDefault().nombre),
                        Categoria = (participante.Categoria != null ? participante.Categoria.nombre : tabla.Categorias.Where(x => x.id == participante.categoria_id).FirstOrDefault().nombre),
                        Segmento = (participante.Segmento != null ? participante.Segmento.nombre : tabla.Segmentoes.Where(x => x.id == participante.segmento_id).FirstOrDefault().nombre),
                        Canal = (participante.Canal != null ? participante.Canal.nombre : tabla.Canals.Where(x => x.id == participante.canal_id).FirstOrDefault().nombre),
                        Email = participante.email,
                        Salario = participante.salario.ToString(),
                        Telefono = participante.telefono,
                        Direccion = participante.direccion,
                        PorcentajeParticipacion = participante.porcentajeParticipacion.ToString(),
                        PorcentajeSalario = participante.porcentajeSalario.ToString()
                    });

                tabla.Auditorias.AddObject(auditoria);
                tabla.Participantes.AddObject(participante);
                tabla.SaveChanges();
                resultado = participante.id;
            }
            return resultado;
        }

        public int ActualizarParticipante(int id, Participante participante, string userName)
        {
            int resultado = 0;
            if (tabla.Participantes.Where(Participante => Participante.documento == participante.documento && Participante.tipoDocumento_id == participante.tipoDocumento_id && Participante.id != id && Participante.GP == false).ToList().Count() == 0)
            {
                var participanteActual = this.tabla.Participantes.Where(Participante => Participante.id == id && Participante.id > 0).First();

                //AUDITORIA
                string estadoAnterior = Utilidades.Auditoria.CrearDescripcionAuditoria(
                    new
                    {
                        Nombre = participanteActual.nombre,
                        Apellidos = participanteActual.apellidos,
                        TipoDocumento = (participanteActual.TipoDocumento != null ? participanteActual.TipoDocumento.nombre : tabla.TipoDocumentoes.Where(x => x.id == participanteActual.tipoDocumento_id).FirstOrDefault().nombre),
                        Documento = participanteActual.documento,
                        Estado = (participanteActual.EstadoParticipante != null ? participanteActual.EstadoParticipante.nombre : tabla.EstadoParticipantes.Where(x => x.id == participanteActual.estadoParticipante_id).FirstOrDefault().nombre),
                        FechaIngreso = ((DateTime)participanteActual.fechaIngreso).ToShortDateString(),
                        FechaRetiro = ((DateTime)participanteActual.fechaRetiro).ToShortDateString(),
                        FechaNacimiento = ((DateTime)participanteActual.fechaNacimiento).ToShortDateString(),
                        CompañiaNomina = (participanteActual.Compania != null ? participanteActual.Compania.nombre : tabla.Companias.Where(x => x.id == participanteActual.compania_id).FirstOrDefault().nombre),
                        Nivel = (participanteActual.Nivel != null ? participanteActual.Nivel.nombre : tabla.Nivels.Where(x => x.id == participanteActual.nivel_id).FirstOrDefault().nombre),
                        Zona = (participanteActual.Zona != null ? participanteActual.Zona.nombre : tabla.Zonas.Where(x => x.id == participanteActual.zona_id).FirstOrDefault().nombre),
                        Localidad = (participanteActual.Localidad != null ? participanteActual.Localidad.nombre : tabla.Localidads.Where(x => x.id == participanteActual.localidad_id).FirstOrDefault().nombre),
                        Categoria = (participanteActual.Categoria != null ? participanteActual.Categoria.nombre : tabla.Categorias.Where(x => x.id == participanteActual.categoria_id).FirstOrDefault().nombre),
                        Segmento = (participanteActual.Segmento != null ? participanteActual.Segmento.nombre : tabla.Segmentoes.Where(x => x.id == participanteActual.segmento_id).FirstOrDefault().nombre),
                        Canal = (participanteActual.Canal != null ? participanteActual.Canal.nombre : tabla.Canals.Where(x => x.id == participanteActual.canal_id).FirstOrDefault().nombre),
                        Email = participanteActual.email,
                        Salario = participanteActual.salario.ToString(),
                        Telefono = participanteActual.telefono,
                        Direccion = participanteActual.direccion,
                        PorcentajeParticipacion = participanteActual.porcentajeParticipacion.ToString(),
                        PorcentajeSalario = participanteActual.porcentajeSalario.ToString()
                    });

                string estadoActual = Utilidades.Auditoria.CrearDescripcionAuditoria(
                    new
                    {
                        Nombre = participante.nombre,
                        Apellidos = participante.apellidos,
                        TipoDocumento = (participante.TipoDocumento != null ? participante.TipoDocumento.nombre : tabla.TipoDocumentoes.Where(x => x.id == participante.tipoDocumento_id).FirstOrDefault().nombre),
                        Documento = participante.documento,
                        Estado = (participante.EstadoParticipante != null ? participante.EstadoParticipante.nombre : tabla.EstadoParticipantes.Where(x => x.id == participante.estadoParticipante_id).FirstOrDefault().nombre),
                        FechaIngreso = ((DateTime)participante.fechaIngreso).ToShortDateString(),
                        FechaRetiro = ((DateTime)participante.fechaRetiro).ToShortDateString(),
                        FechaNacimiento = ((DateTime)participante.fechaNacimiento).ToShortDateString(),
                        CompañiaNomina = (participante.Compania != null ? participante.Compania.nombre : tabla.Companias.Where(x => x.id == participante.compania_id).FirstOrDefault().nombre),
                        Nivel = (participante.Nivel != null ? participante.Nivel.nombre : tabla.Nivels.Where(x => x.id == participante.nivel_id).FirstOrDefault().nombre),
                        Zona = (participante.Zona != null ? participante.Zona.nombre : tabla.Zonas.Where(x => x.id == participante.zona_id).FirstOrDefault().nombre),
                        Localidad = (participante.Localidad != null ? participante.Localidad.nombre : tabla.Localidads.Where(x => x.id == participante.localidad_id).FirstOrDefault().nombre),
                        Categoria = (participante.Categoria != null ? participante.Categoria.nombre : tabla.Categorias.Where(x => x.id == participante.categoria_id).FirstOrDefault().nombre),
                        Segmento = (participante.Segmento != null ? participante.Segmento.nombre : tabla.Segmentoes.Where(x => x.id == participante.segmento_id).FirstOrDefault().nombre),
                        Canal = (participante.Canal != null ? participante.Canal.nombre : tabla.Canals.Where(x => x.id == participante.canal_id).FirstOrDefault().nombre),
                        Email = participante.email,
                        Salario = participante.salario.ToString(),
                        Telefono = participante.telefono,
                        Direccion = participante.direccion,
                        PorcentajeParticipacion = participante.porcentajeParticipacion.ToString(),
                        PorcentajeSalario = participante.porcentajeSalario.ToString()
                    });

                participanteActual.clave = participante.clave;
                participanteActual.codProductor = participante.codProductor;
                participanteActual.nombre = participante.nombre;
                participanteActual.apellidos = participante.apellidos;
                participanteActual.documento = participante.documento;
                participanteActual.email = participante.email;
                participanteActual.estadoParticipante_id = participante.estadoParticipante_id;
                participanteActual.fechaIngreso = participante.fechaIngreso;
                participanteActual.fechaRetiro = participante.fechaRetiro;
                participanteActual.fechaNacimiento = participante.fechaNacimiento;
                participanteActual.nivel_id = participante.nivel_id;
                participanteActual.compania_id = participante.compania_id;
                participanteActual.zona_id = participante.zona_id;
                participanteActual.localidad_id = participante.localidad_id;
                participanteActual.canal_id = participante.canal_id;
                participanteActual.categoria_id = participante.categoria_id;
                participanteActual.ingresosMinimos = participante.ingresosMinimos;
                participanteActual.tipoParticipante_id = participante.tipoParticipante_id;
                participanteActual.salario = participante.salario;
                participanteActual.tipoDocumento_id = participante.tipoDocumento_id;
                participanteActual.telefono = participante.telefono;
                participanteActual.direccion = participante.direccion;
                participanteActual.segmento_id = participante.segmento_id;
                participanteActual.porcentajeParticipacion = participante.porcentajeParticipacion;
                participanteActual.porcentajeSalario = participante.porcentajeSalario;

                //AUDITORIA
                Entidades.Auditoria auditoria = new Entidades.Auditoria();
                auditoria.Fecha = DateTime.Now;
                auditoria.id_EventoTabla = (int)tipoEventoTabla.Modificacion;
                auditoria.id_TablaAuditada = (int)tablasAuditadas.Participante;
                auditoria.Usuario = userName;
                auditoria.id_Segmento = (int)participante.segmento_id;
                auditoria.Version_Anterior = estadoAnterior;
                auditoria.Version_Nueva = estadoActual;
                tabla.Auditorias.AddObject(auditoria);

                tabla.SaveChanges();
                resultado = 1;
            }
            return resultado;
        }

        public string EliminarParticipante(int id, Participante participante, string userName)
        {
            var participanteActual = this.tabla.Participantes.Where(Participante => Participante.id == id && Participante.id > 0).First();

            //AUDITORIA
            Entidades.Auditoria auditoria = new Entidades.Auditoria();
            auditoria.Fecha = DateTime.Now;
            auditoria.id_EventoTabla = (int)tipoEventoTabla.Eliminacion;
            auditoria.id_TablaAuditada = (int)tablasAuditadas.Participante;
            auditoria.Usuario = userName;
            auditoria.id_Segmento = (int)participanteActual.segmento_id;
            auditoria.Version_Anterior = Utilidades.Auditoria.CrearDescripcionAuditoria(
                new
                {
                    Nombre = participanteActual.nombre,
                    Apellidos = participanteActual.apellidos,
                    TipoDocumento = (participanteActual.TipoDocumento != null ? participanteActual.TipoDocumento.nombre : tabla.TipoDocumentoes.Where(x => x.id == participanteActual.tipoDocumento_id).FirstOrDefault().nombre),
                    Documento = participanteActual.documento,
                    Estado = (participanteActual.EstadoParticipante != null ? participanteActual.EstadoParticipante.nombre : tabla.EstadoParticipantes.Where(x => x.id == participanteActual.estadoParticipante_id).FirstOrDefault().nombre),
                    FechaIngreso = ((DateTime)participanteActual.fechaIngreso).ToShortDateString(),
                    FechaRetiro = ((DateTime)participanteActual.fechaRetiro).ToShortDateString(),
                    FechaNacimiento = ((DateTime)participanteActual.fechaNacimiento).ToShortDateString(),
                    CompañiaNomina = (participanteActual.Compania != null ? participanteActual.Compania.nombre : tabla.Companias.Where(x => x.id == participanteActual.compania_id).FirstOrDefault().nombre),
                    Nivel = (participanteActual.Nivel != null ? participanteActual.Nivel.nombre : tabla.Nivels.Where(x => x.id == participanteActual.nivel_id).FirstOrDefault().nombre),
                    Zona = (participanteActual.Zona != null ? participanteActual.Zona.nombre : tabla.Zonas.Where(x => x.id == participanteActual.zona_id).FirstOrDefault().nombre),
                    Localidad = (participanteActual.Localidad != null ? participanteActual.Localidad.nombre : tabla.Localidads.Where(x => x.id == participanteActual.localidad_id).FirstOrDefault().nombre),
                    Categoria = (participanteActual.Categoria != null ? participanteActual.Categoria.nombre : tabla.Categorias.Where(x => x.id == participanteActual.categoria_id).FirstOrDefault().nombre),
                    Segmento = (participanteActual.Segmento != null ? participanteActual.Segmento.nombre : tabla.Segmentoes.Where(x => x.id == participanteActual.segmento_id).FirstOrDefault().nombre),
                    Canal = (participanteActual.Canal != null ? participanteActual.Canal.nombre : tabla.Canals.Where(x => x.id == participanteActual.canal_id).FirstOrDefault().nombre),
                    Email = participanteActual.email,
                    Salario = participanteActual.salario.ToString(),
                    Telefono = participanteActual.telefono,
                    Direccion = participanteActual.direccion,
                    PorcentajeParticipacion = participanteActual.porcentajeParticipacion.ToString(),
                    PorcentajeSalario = participanteActual.porcentajeSalario.ToString()
                });
            auditoria.Version_Nueva = "";

            tabla.Auditorias.AddObject(auditoria);

            tabla.DeleteObject(participanteActual);
            try
            {
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        public List<TipoDocumento> ListarTipodocumentoes()
        {
            return tabla.TipoDocumentoes.Where(TipoDocumento => TipoDocumento.id > 0 && TipoDocumento.id > 0).ToList();
        }

        public List<TipoParticipante> ListarTipoparticipantes()
        {
            return tabla.TipoParticipantes.Where(TipoParticipante => TipoParticipante.id > 0 && TipoParticipante.id > 0).ToList();
        }

        public List<EstadoParticipante> ListarEstadoparticipantes()
        {
            return tabla.EstadoParticipantes.Where(EstadoParticipante => EstadoParticipante.id > 0 && EstadoParticipante.id > 0).ToList();
        }
    }
}
