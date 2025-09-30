using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Componentes.LiqFranquicias;
using ColpatriaSAI.Negocio.Entidades;
using ColpatriaSAI.Negocio.Componentes.Admin;
using ColpatriaSAI.Negocio.Componentes.Concursos;
using ColpatriaSAI.Negocio.Componentes.Produccion;
using ColpatriaSAI.Negocio.Componentes.Productos;
using ColpatriaSAI.Negocio.Componentes.Contratacion;
using ColpatriaSAI.Negocio.Componentes.Seguridad;
using System.Web.Mvc;
using SiteMap = ColpatriaSAI.Negocio.Componentes.Admin.SiteMapes;
using ColpatriaSAI.Negocio.Entidades.Informacion;
using TipoVehiculo = ColpatriaSAI.Negocio.Entidades.TipoVehiculo;
using ColpatriaSAI.Negocio.Componentes.Ajustes;
using System.Threading;
using ColpatriaSAI.Negocio.Componentes.Utilidades;
using System.Data;



namespace ColpatriaSAI.Negocio.Componentes
{
    public class Administracion : IAdministracion
    {
        private Zonas _zonas;
        private Localidades _localidades;
        private Redes _redes;
        private Monedas _monedas;
        private Companias _compañias;
        private Concursoes _concursoes;
        private Tipoconcursoes _tipoconcursoes;
        private CategoriasxRegla _categoriasxRegla;
        private ParticipanteConcursos _participanteconcursoes;
        private ProductoConcursos _productoconcursoes;
        private EtapaConcursos _etapaconcursoes;
        private CompaniaxEtapas _companiaxetapas;
        private Reglas _reglas;
        private Nivels _nivels;
        private Segmentos _segmentoes;
        private Beneficiarios _beneficiarios;
        private AntiguedadxNiveles _antiguedades;
        private BaseMonedas _baseMonedas;
        private BasexParticipantes _basexParticipantes;
        private Categorias _categorias;
        private EscalaNotas _escalaNotas;
        private FactorVariables _factorVariables;
        private FactorxNotas _factorxNotas;
        private GrupoEndosos _grupoEndosos;
        private SiteMapes _sitemapes;
        private Negocio.Componentes.Admin.Integracion _integracions;
        private Participantes _participantes;
        private TipoEscalas _tipoescalas;
        private Canales _canals;
        private Clientes _clientes;
        private LineaNegocios _lineanegocios;
        private Ramos _ramoes;
        private Amparos _amparoes;
        private ActividadEconomicas _actividadeconomicas;
        private Variables _variables;
        private Tipovariables _tipovariables;
        private Coberturas _coberturas;
        private Metas _metas;
        private ColpatriaSAI.Negocio.Componentes.Productos.Productos _productos;
        private TipoMetas _tipometas;
        private ModalidadPagos _modalidadpagos;
        private Planes _plans;
        private Plazos _plazos;
        private Modelos _modelos;
        private TipoEndosos _tipoEndosos;
        private GrupoTipoEndosos _grupoTipoEndosos;
        private Franquicias _franquicias;
        private Franquicias _detFranquicias;
        private Negocio.Componentes.Produccion.Negocio _negocio;
        private Presupuestos _presupuestos;
        private MonedaxNegocios _monedaxnegocio;
        private LiquidacionRegla _liquidacion;
        private LiquidacionConcurso _liquidacionConcurso;
        private TipoLocalidades _tipolocalidads;
        private TipoVehiculo _tipvehiculo;
        private Participaciones _participacion;
        private AnticiposFranquicias _antFranquicias;
        private LiquidacionContrats _liquiContrat;
        private ExcepcionesJerarquia _excepcionJerarquia;
        private Parametros _parametros;
        private SalariosMinimos _salarios;
        private IngresoLocalidades _ingresos;
        private AjustesConcursos _ajustesConcursos;
        private AjustesContratos _ajustesContratos;
        private AjustesFranquicias _ajustesFranquicias;
        private PremioAnterior _premioanterior;
        private Combos _combos;
        private PersistenciaCAPI _persistenciacapi;
        private ParametrizacionEficienciaARLS _eficienciaARL;
        private ParametrizacionCambioClaveS _clave;
        private AuditoriaModulos _auditoriaModulos;
        private Usuario _usuario;
        private ProcesoAutomatico.AUT aut;
        private Estadisticas.EstadisticasRecaudos estr;
        private Estadisticas.EstadisticasPrimas estp;
        private TiposContratos _tiposContratos;
        

        //AUDITORIA//******************************************

        #region Metodos AUDITORIA

        
        /// <summary>
        /// Funcion que se encarga de guardar los datos en la tabla glControl para su respectiva auditoira
        /// </summary>
        /// <param name="tablaModuloProceso">Donde se hace la modificacion</param>
        /// <param name="tipoModificacion">Tipo de Modificacion Hecha</param>
        /// <param name="fechaInicio">Fecha que se inicio la modificacion</param>
        /// <param name="fechaFinal">Fecha que finalizo la modificacion</param>
        /// <param name="observacion">Observacion de modificacion</param>
        /// <param name="primeraVersion">Primera version (antes de modificar)</param>
        /// <param name="versionFinal">Ultima version (Despues de modificar)</param>
        /// <param name="user">Usuario que esta realizando la tarea</param>
        public void InsertarAuditoria(int tablaModuloProceso, int tipoModificacion, DateTime fechaInicio, DateTime fechaFinal, string observacion, string primeraVersion,
            string versionFinal, string user)
        {
            try
            {
                this._auditoriaModulos = new AuditoriaModulos();
                this._auditoriaModulos.InsertarAuditoria(tablaModuloProceso, tipoModificacion, fechaInicio, fechaFinal, observacion, primeraVersion, versionFinal, user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Funcion q se encarga de traer todos los datos de la tabla auditoria
        /// </summary>
        /// <returns>Listado con objtos de auditoria</returns>
        public List<Entidades.Auditoria> ListarAuditoria(int idTabla, DateTime fechaInicio, DateTime fechaFin, int idEvento, List<int> segmentos)
        {
            this._auditoriaModulos = new AuditoriaModulos();
            return _auditoriaModulos.ListarAuditoria(idTabla, fechaInicio, fechaFin, idEvento, segmentos);
        }

        public List<TablaAuditada> ListarTablasAuditadas()
        {
            _auditoriaModulos = new AuditoriaModulos();
            return _auditoriaModulos.ListarTablasAuditadas();
        }

        public List<EventoTabla> ListarEventosTabla()
        {
            _auditoriaModulos = new AuditoriaModulos();
            return _auditoriaModulos.ListarEventosTabla();
        }

        #endregion

        //INTEGRACION//***************************************

        #region Metodos INTEGRACION

        public int Linq2XmlCrearArchivosXml()
        {
            _integracions = new Integracion();
            return _integracions.Linq2XmlCrearArchivosXml();
        }

        public int CantidadPaquetesenEjecución()
        {
            _integracions = new Integracion();
            return _integracions.CantidadPaquetesenEjecución();
        }

        public int EnviarFTP()
        {
            _integracions = new Integracion();
            return _integracions.EnviarFTP();
        }

        public int EjecutarSPETL(string nombreETL)
        {
            _integracions = new Integracion();
            return _integracions.EjecutarSPETL(nombreETL);
        }

        public int EjecutarSP(string nombreSP)
        {
            _integracions = new Integracion();
            return _integracions.EjecutarSP(nombreSP);
        }

        public string ValoresWebConfigServicio()
        {
            _integracions = new Integracion();
            return _integracions.ValoresWebConfigServicio();
        }

        public int RetornarHorasIntegracion(int id)
        {
            _integracions = new Integracion();
            return _integracions.RetornarHorasIntegracion(id);
        }

        public List<ParametrosPersistenciaVIDA> ListarPPVIDA()
        {
            _parametros = new Parametros();
            return _parametros.ListarPPVIDA();
        }

        public List<ParametrosPersistenciaVIDA> ListarPPVIDAPorId(int idParametro)
        {
            _parametros = new Parametros();
            return _parametros.ListarPPVIDAPorId(idParametro);
        }

        public int ActualizarPPV(int id, ParametrosPersistenciaVIDA ppv, string Username)
        {
            _parametros = new Parametros();
            return _parametros.ActualizarPPV(id, ppv, Username);
        }

        public List<ParametrosApp> ListarParametrosApp()
        {
            _parametros = new Parametros();
            return _parametros.ListarParametrosApp();
        }

        public List<ParametrosApp> ListarParametrosAppPorId(int id)
        {
            _parametros = new Parametros();
            return _parametros.ListarParametrosAppPorId(id);
        }

        public int ActualizarParametrosApp(List<ParametrosApp> parametrosapp, string Username)
        {
            _parametros = new Parametros();
            return _parametros.ActualizarParametrosApp(parametrosapp, Username);
        }

        #endregion

        //VARIOS//********************************************

        #region Metodos ZONAS
        /// <summary>
        /// Guarda un registro de zona
        /// </summary>
        /// <param name="zona">Objecto Zona a insertar</param>
        /// <returns>Numero de registros guardados</returns>
        public int InsertarZona(Zona zona, string Username)
        {
            _zonas = new Zonas();
            return _zonas.InsertarZona(zona, Username);
        }

        /// <summary>
        /// Obtiene listado de zonas
        /// </summary>
        /// <returns>Lista de zonas</returns>
        public List<Zona> ListarZonas()
        {
            _zonas = new Zonas();
            return _zonas.ListarZonas();
        }

        /// <summary>
        /// Obtiene listado de zonas por id
        /// </summary>
        /// <returns>Lista de zonas</returns>
        public List<Zona> ListarZonasPorId(int idZona)
        {
            _zonas = new Zonas();
            return _zonas.ListarZonasPorId(idZona);
        }

        /// <summary>
        /// Actualiza un registro de zona
        /// </summary>
        /// <param name="id">Id de la zona a modificar</param>
        /// <param name="zona">Objeto Zona utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarZona(int id, Zona zona,string Username)
        {
            _zonas = new Zonas();
            return _zonas.ActualizarZona(id, zona, Username);
        }

        /// <summary>
        /// Elimina un registro de zona
        /// </summary>
        /// <param name="id">Id de la zona a eliminar</param>
        /// <param name="zona">Objecto Zona utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarZona(int id, Zona zona, string Username)
        {
            _zonas = new Zonas();
            return _zonas.EliminarZona(id, zona, Username);
        }

        #endregion

        #region Metodos LOCALIDADES

        public List<Localidad> ListarLocalidades()
        {
            _localidades = new Localidades();
            return _localidades.ListarLocalidades();
        }

        public List<Localidad> ListarLocalidadesPorId(int idLocalidad)
        {
            _localidades = new Localidades();
            return _localidades.ListarLocalidadesPorId(idLocalidad);
        }

        public List<Localidad> ListarLocalidadesPorZona(int idZona)
        {
            _localidades = new Localidades();
            return _localidades.ListarLocalidadesPorZona(idZona);
        }

        public int InsertarLocalidad(Localidad localidad, string Username)
        {
            _localidades = new Localidades();
            return _localidades.InsertarLocalidad(localidad, Username);
        }

        public int ActualizarLocalidad(int id, Localidad localidad, string Username)
        {
            _localidades = new Localidades();
            return _localidades.ActualizarLocalidad(id, localidad, Username);
        }

        public string EliminarLocalidad(int id, Localidad localidad, string Username)
        {
            _localidades = new Localidades();
            return _localidades.EliminarLocalidad(id, localidad, Username);
        }

        #endregion

        #region Metodos TIPOLOCALIDADES

        public List<TipoLocalidad> ListarTipoLocalidad()
        {
            _tipolocalidads = new TipoLocalidades();
            return _tipolocalidads.ListarTipoLocalidad();
        }

        #endregion

        #region Metodos COMPAÑIAS

        public int InsertarCompania(Compania compañia, string Username)
        {
            _compañias = new Companias();
            return _compañias.InsertarCompania(compañia, Username);
        }

        /// <summary>
        /// Obtiene listado de compañias
        /// </summary>
        /// <returns>Lista de compañias</returns>
        public List<Compania> ListarCompanias()
        {
            _compañias = new Companias();
            return _compañias.ListarCompanias();
        }

        /// <summary>
        /// Obtiene listado de compañias por id
        /// </summary>
        /// <returns>Lista de compañias</returns>
        public List<Compania> ListarCompaniasPorId(int idCompañia)
        {
            _compañias = new Companias();
            return _compañias.ListarCompaniasPorId(idCompañia);
        }

        public int ActualizarCompania(int id, Compania compania, string Username)
        {
            _compañias = new Companias();
            return _compañias.ActualizarCompania(id, compania, Username);
        }


        public string EliminarCompania(int id, Compania compania, string Username)
        {
            _compañias = new Companias();
            return _compañias.EliminarCompania(id, compania, Username);
        }

        #endregion

        #region Metodos SEGMENTOS

        public List<Segmento> ListarSegmentoes()
        {
            _segmentoes = new Segmentos();
            return _segmentoes.ListarSegmento();
        }

        public List<Segmento> ListarSegmentoesPorId(int idSegmento)
        {
            _segmentoes = new Segmentos();
            return _segmentoes.ListarSegmentoesPorId(idSegmento);
        }

        public int InsertarSegmento(Segmento segmento, string Username)
        {
            _segmentoes = new Segmentos();
            return _segmentoes.InsertarSegmento(segmento, Username);
        }

        public int ActualizarSegmento(int id, Segmento segmento, string Username)
        {
            _segmentoes = new Segmentos();
            return _segmentoes.ActualizarSegmento(id, segmento, Username);
        }


        public string EliminarSegmento(int id, Segmento segmento, string Username)
        {
            _segmentoes = new Segmentos();
            return _segmentoes.EliminarSegmento(id, segmento, Username);
        }

        #endregion

        #region Metodos COBERTURAS

        public List<Cobertura> ListarCoberturas()
        {
            _coberturas = new Coberturas();
            return _coberturas.ListarCobertura();
        }

        public List<Cobertura> ListarCoberturasPorId(int idCobertura)
        {
            _coberturas = new Coberturas();
            return _coberturas.ListarCoberturasPorId(idCobertura);
        }

        #endregion

        #region Metodos ACTIVIDADECONOMICA

        /// <summary>
        /// Funcion que se encarga de cargar el listado de las actividades Economicas
        /// </summary>
        /// <returns>Listado de Actividades Economicas</returns>
        public List<ActividadEconomica> ListarActividadEconomicas()
        {
            _actividadeconomicas = new ActividadEconomicas();
            return _actividadeconomicas.ListarActividadEconomica();
        }

        /// <summary>
        /// Funcion que se encarga de traer el listado de las actividades Economicas por Actividad
        /// </summary>
        /// <param name="idActividadEconomica"></param>
        /// <returns></returns>
        public List<ActividadEconomica> ListarActividadEconomicasPorId(int idActividadEconomica)
        {
            _actividadeconomicas = new ActividadEconomicas();
            return _actividadeconomicas.ListarActividadEconomicasPorId(idActividadEconomica);
        }

        /// <summary>
        /// Funcion que se encarga de traer el listado de las actividades por compañia
        /// </summary>
        /// <param name="companiaID">Compañia con la que se esta trabajando</param>
        /// <returns>Listado de Actividades economicas por compañia</returns>
        public List<ActividadEconomica> ListarActividadEconomicasPorCompania(int companiaID)
        {
            _actividadeconomicas = new ActividadEconomicas();
            return _actividadeconomicas.ListarActividadEconomicasPorCompania(companiaID);
        }
        #endregion

        #region Metodos REGLAXCONCEPTODESCUENTO

        public List<ReglaxConceptoDescuento> ListarReglaxConceptoDescuento()
        {
            _reglas = new Reglas();
            return _reglas.ListarReglaxConceptoDescuento();
        }

        #endregion

        #region Metodos CANALES

        public List<Canal> ListarCanals()
        {
            _canals = new Canales();
            return _canals.ListarCanals();
        }

        public List<Canal> ListarCanalsPorId(int idCanal)
        {
            _canals = new Canales();
            return _canals.ListarCanalsPorId(idCanal);
        }

        public int InsertarCanal(Canal canal, string Username)
        {
            _canals = new Canales();
            return _canals.InsertarCanal(canal, Username);
        }

        public int ActualizarCanal(int id, Canal canal, string Username)
        {
            _canals = new Canales();
            return _canals.ActualizarCanal(id, canal, Username);
        }

        public string EliminarCanal(int id, Canal canal, string Username)
        {
            _canals = new Canales();
            return _canals.EliminarCanal(id, canal, Username);
        }

        public List<CanalDetalle> ListarCanalDetalles()
        {
            _canals = new Canales();
            return _canals.ListarCanalDetalles();
        }

        #endregion

        #region Metodos PREMIOSANTERIORES

        public List<PremiosAnteriore> ListarPremioAnterior()
        {
            _premioanterior = new PremioAnterior();
            return _premioanterior.ListarPremioAnterior();
        }

        public int ActualizarPremioConsolidadoMes(string clave, int anio, int tipo, string Username)
        {
            _premioanterior = new PremioAnterior();
            return _premioanterior.ActualizarPremioConsolidadoMes(clave, anio, tipo, Username);
        }

        public string RetornarClavePremio(int id)
        {
            _premioanterior = new PremioAnterior();
            return _premioanterior.RetornarClavePremio(id);
        }

        public int RetornarAnioPremio(int id)
        {
            _premioanterior = new PremioAnterior();
            return _premioanterior.RetornarAnioPremio(id);
        }

        public List<PremiosAnteriore> ListarPremioAnteriorPorId(int idPAnterior)
        {
            _premioanterior = new PremioAnterior();
            return _premioanterior.ListarPremioAnteriorPorId(idPAnterior);
        }

        public int InsertarPremioAnterior(PremiosAnteriore premioanterior, string Username)
        {
            _premioanterior = new PremioAnterior();
            return _premioanterior.InsertarPremioAnterior(premioanterior, Username);
        }

        public int ActualizarPremioAnterior(int id, PremiosAnteriore premioanterior, string Username)
        {
            _premioanterior = new PremioAnterior();
            return _premioanterior.ActualizarPremioAnterior(id, premioanterior, Username);
        }

        public string EliminarPremioAnterior(int id, PremiosAnteriore premioanterior, string Username)
        {
            _premioanterior = new PremioAnterior();
            return _premioanterior.EliminarPremioAnterior(id, premioanterior, Username);
        }

        #endregion

        #region Metodos VARIABLES

        public List<Variable> ListarVariables()
        {
            _variables = new Variables();
            return _variables.ListarVariables();
        }

        public List<Variable> ListarVariablesPremio()
        {
            _variables = new Variables();
            return _variables.ListarVariablesPremio();
        }

        public List<Variable> ListarVariablesPorId(int idVariable)
        {
            _variables = new Variables();
            return _variables.ListarVariablesPorId(idVariable);
        }

        public List<TempList> ListarTablas(int idtabla)
        {
            _reglas = new Reglas();
            return _reglas.ListarTablas(idtabla);
        }

        #endregion

        #region Metodos TIPOVARIABLES

        public List<TipoVariable> ListarTipovariables()
        {
            _tipovariables = new Tipovariables();
            return _tipovariables.ListarTipovariables();
        }
        #endregion

        #region Metodos SALARIOSMINIMOS
        /// <summary>
        /// Obtiene listado de los Salarios Minimos
        /// </summary>
        /// <returns>Lista de objectos SalarioMinimo</returns>
        public List<SalarioMinimo> ListarSalariosMinimos()
        {
            _salarios = new SalariosMinimos();
            return _salarios.ListarSalariosMinimos();
        }

        /// <summary>
        /// Obtiene listado de los Salarios Minimos por id
        /// </summary>
        /// <param name="idSalario">Id del Salario Minimo a consultar</param>
        /// <returns>Lista de objectos SalarioMinimo</returns>
        public List<SalarioMinimo> ListarSalariosMinimosPorId(int idSalario)
        {
            _salarios = new SalariosMinimos();
            return _salarios.ListarSalariosMinimosPorId(idSalario);
        }

        /// <summary>
        /// Guarda un registro de Salario Minimo
        /// </summary>
        /// <param name="salario">Objecto SalarioMinimo a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarSalarioMinimo(SalarioMinimo salario, string Username)
        {
            _salarios = new SalariosMinimos();
            return _salarios.InsertarSalarioMinimo(salario, Username);
        }

        /// <summary>
        /// Actualiza un registro de Salario Minimo
        /// </summary>
        /// <param name="salario">Objeto SalarioMinimo utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarSalarioMinimo(SalarioMinimo salario, string Username)
        {
            _salarios = new SalariosMinimos();
            return _salarios.ActualizarSalarioMinimo(salario, Username);
        }

        /// <summary>
        /// Elimina un registro de Salario Minimo
        /// </summary>
        /// <param name="salario">Objecto SalarioMinimo utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo string</returns>
        public string EliminarSalarioMinimo(SalarioMinimo salario, string Username)
        {
            _salarios = new SalariosMinimos();
            return _salarios.EliminarSalarioMinimo(salario, Username);
        }
        #endregion

        #region Metodos INGRESOSLOCALIDADES
        /// <summary>
        /// Obtiene listado de los Ingresos por Localidad
        /// </summary>
        /// <returns>Lista de objectos IngresoLocalidad</returns>
        public List<IngresoLocalidad> ListarIngresoLocalidades()
        {
            _ingresos = new IngresoLocalidades();
            return _ingresos.ListarIngresoLocalidades();
        }

        /// <summary>
        /// Obtiene listado de los Ingresos por Localidad por id
        /// </summary>
        /// <param name="idIngreso">Id del Ingreso por Localidad a consultar</param>
        /// <returns>Lista de objectos IngresoLocalidad</returns>
        public List<IngresoLocalidad> ListarIngresoLocalidadesPorId(int idIngreso)
        {
            _ingresos = new IngresoLocalidades();
            return _ingresos.ListarIngresoLocalidadesPorId(idIngreso);
        }

        /// <summary>
        /// Guarda un registro de Ingreso por Localidad
        /// </summary>
        /// <param name="ingreso">Objecto IngresoLocalidad a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarIngresoLocalidades(IngresoLocalidad ingreso, string Username)
        {
            _ingresos = new IngresoLocalidades();
            return _ingresos.InsertarIngresoLocalidades(ingreso, Username);
        }

        /// <summary>
        /// Actualiza un registro de Ingreso por Localidad
        /// </summary>
        /// <param name="ingreso">Objeto IngresoLocalidad utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarIngresoLocalidades(IngresoLocalidad ingreso, string Username)
        {
            _ingresos = new IngresoLocalidades();
            return _ingresos.ActualizarIngresoLocalidades(ingreso, Username);
        }

        /// <summary>
        /// Elimina un registro de Ingreso por Localidad
        /// </summary>
        /// <param name="ingreso">Objecto IngresoLocalidad utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo string</returns>
        public string EliminarIngresoLocalidades(IngresoLocalidad ingreso, string Username)
        {
            _ingresos = new IngresoLocalidades();
            return _ingresos.EliminarIngresoLocalidades(ingreso, Username);
        }
        #endregion

        #region Metodos PERSISTENCIACAPI
        /// <summary>
        /// Obtiene listado de la tabla peristencia de CAPI detalle
        /// </summary>
        /// <returns>Lista de objectos PersistenciadeCAPIDetalle</returns>
        public List<PersistenciadeCAPIDetalle> ListarPersistenciaCAPIDetalle(string numeroNegocio, string clave)
        {
            _persistenciacapi = new PersistenciaCAPI();
            return _persistenciacapi.ListarPersistenciaCAPIDetalle(numeroNegocio, clave);
        }

        /// <summary>
        /// Obtiene listado de la tabla peristencia de CAPI detalle por id
        /// </summary>
        /// <param name="idPersistenciaCAPI">Id de la Persistencia CAPI Detalle a consultar</param>
        /// <returns>Lista de objectos PersistenciadeCAPIDetalle</returns>
        public List<PersistenciadeCAPIDetalle> ListarPersistenciaCAPIDetallePorId(int idPersistenciaCAPI)
        {
            _persistenciacapi = new PersistenciaCAPI();
            return _persistenciacapi.ListarPersistenciaCAPIDetallePorId(idPersistenciaCAPI);
        }

        /// <summary>
        /// Actualiza un registro de la tabla peristencia de CAPI detalle
        /// </summary>
        /// <param name="persistenciacapidetalle">Objeto PersistenciadeCAPIDetalle utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarPersistenciaCAPIDetalle(int id, PersistenciadeCAPIDetalle persistenciacapidetalle, string Username)
        {
            _persistenciacapi = new PersistenciaCAPI();
            return _persistenciacapi.ActualizarPersistenciaCAPIDetalle(id, persistenciacapidetalle, Username);
        }

        #endregion

        #region Metodos PARAMETRIZACIONEFICIENCIAARL

        /// <summary>
        /// Funcion que se encarga de traer la parametrizacion de eficiencia de ARL
        /// </summary>
        /// <returns>Listado de parametrizacion</returns>
        //public List<ParametrosEficienciaARL> ListarParametrizacionEficienciaARL()
        //{
        //    _eficienciaARL = new ParametrizacionEficienciaARLS();
        //    return _eficienciaARL.ListarParametrizacionEficienciaARL();
        //}

        /// <summary>
        /// Funcion que se encarga de traer la parametrizacion de la eficiencia ARL
        /// </summary>
        /// <param name="id">Identificador de la eficiencia a mostrar</param>
        /// <returns></returns>
        //public ParametrosEficienciaARL ListarParametrizacionEficienciaARLByID(int id)
        //{
        //    _eficienciaARL = new ParametrizacionEficienciaARLS();
        //    return _eficienciaARL.ListarParametrizacionEficienciaARLByID(id);
        //}

        /// <summary>
        /// Funcion que se encarga de guardar los registros en la base de datos
        /// </summary>
        /// <param name="parametrizacionEficienciaARL">Datos para insercion</param>
        /// <param name="user">Usuario q esta logueado (Necesario para la audoria)</param>
        /// <returns></returns>
        //public Result.result InsertParametrizacionEficienciaARL(ParametrosEficienciaARL parametrizacionEficienciaARL, string user)
        //{
        //    try
        //    {
        //        Result.result result = new Result.result();
        //        this._eficienciaARL = new ParametrizacionEficienciaARLS();
        //        result = this._eficienciaARL.InsertParametrizacionEficienciaARL(parametrizacionEficienciaARL);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Result.result error = new Result.result();
        //        error.message = string.Format("Ocurrio un erro con la insercion debido a " + ex);
        //        return error;
        //    }

        //}

        /// <summary>
        /// Funcion que se encarga de actualizar los registros en la tabla Parametrizacion Eficiencia ARL
        /// </summary>
        /// <param name="id">Identificador del registro a actualizar</param>
        /// <param name="parametrizacionEficiencia">Conjunto de datos para actualizar</param>
        /// <returns></returns>
        //public Result.result UpdateParametrizacionEficienciaARL(int id, ParametrosEficienciaARL parametrizacionEficiencia)
        //{
        //    this._eficienciaARL = new ParametrizacionEficienciaARLS();
        //    return this._eficienciaARL.UpdateParametrizacionEficienciaARL(id, parametrizacionEficiencia);
        //}

        /// <summary>
        /// Funcion que se encarga de borrar el registro que se identifica
        /// </summary>
        /// <param name="id">Identificador del registro a elimar</param>
        /// <param name="parametrizacionEficiencia">Datos a eliminar</param>
        /// <returns></returns>
        //public
        //Result.result DeleteParametrizacionEficienciaARL(int id, ParametrosEficienciaARL parametrizacionEficiencia)
        //{
        //    this._eficienciaARL = new ParametrizacionEficienciaARLS();
        //    return this._eficienciaARL.DeleteParametrizacionEficienciaARL(id, parametrizacionEficiencia);
        //}

        #endregion

        #region Metodos PARAMETRIZACIONCAMBIOCLAVE

        /// <summary>
        /// Funcion que se encarga de traer el listadode clave
        /// </summary>
        /// <returns>Listado de claves</returns>
        public List<clave_historico> GetHistricoCambioClave()
        {
            _clave = new ParametrizacionCambioClaveS();
            return _clave.GetHistricoCambioClave();
        }

        /// <summary>
        /// Funcion que se encarga de guardar los registros en la base de datos
        /// </summary>
        /// <param name="parametrizacionEficienciaARL">Datos para insercion</param>
        /// <returns></returns>
        public Result.result InsertParametrizacionCambioClave(clave_historico parametrizacionClave, string Username)
        {
            this._clave = new ParametrizacionCambioClaveS();
            return this._clave.InsertParametrizacionCambiolave(parametrizacionClave, Username);
        }

        /// <summary>
        /// Funcion que se encarga de actualizar los registros en la tabla Parametrizacion clave
        /// </summary>
        /// <param name="id">Identificador del registro a actualizar</param>
        /// <param name="parametrizacionEficiencia">Conjunto de datos para actualizar</param>
        /// <returns></returns>
        public Result.result UpdateParametrizacionCambioClave(int id, clave_historico parametrizacionClave, string Username)
        {
            this._clave = new ParametrizacionCambioClaveS();
            return this._clave.UpdateParametrizacionCambiolave(id, parametrizacionClave, Username);
        }

        /// <summary>
        /// Funcion que se encarga de borrar el registro
        /// </summary>
        /// <param name="id">Identificador del registro a elimar</param>
        /// <param name="parametrizacionEficiencia">Datos a eliminar</param>
        /// <returns></returns>
        //public Result.result DeleteParametrizacionCambioClave(int id, clave_historico parametrizacionClave)
        //{
        //    //this._clave = new ParametrizacionCambioClaveS();
        //    //return this._clave.DeleteParametrizacionCambiolave(id, parametrizacionClave);

        //    return();
        //}

        #endregion



        //PRODUCTOS//******************************************

        #region Metodos LINEANEGOCIOS

        public List<LineaNegocio> ListarLineaNegocios()
        {
            _lineanegocios = new LineaNegocios();
            return _lineanegocios.ListarLineaNegocios();
        }

        public List<LineaNegocio> ListarLineaNegociosPorId(int idLineaNegocio)
        {
            _lineanegocios = new LineaNegocios();
            return _lineanegocios.ListarLineanegociosPorId(idLineaNegocio);
        }

        #endregion

        #region Metodos RAMOS

        public List<Ramo> ListarRamos()
        {
            _ramoes = new Ramos();
            return _ramoes.ListarRamos();
        }

        public List<Ramo> ListarRamosPorId(int idRamo)
        {
            _ramoes = new Ramos();
            return _ramoes.ListarRamosPorId(idRamo);
        }

        public List<Ramo> ListarRamosPorCompania(int idCompania)
        {
            _ramoes = new Ramos();
            return _ramoes.ListarRamosPorCompania(idCompania);
        }

        public int InsertarRamo(Ramo ramo, string Username)
        {
            _ramoes = new Ramos();
            return _ramoes.InsertarRamo(ramo, Username);
        }

        public int ActualizarRamo(int id, Ramo ramo, string Username)
        {
            _ramoes = new Ramos();
            return _ramoes.ActualizarRamo(id, ramo, Username);
        }

        public string EliminarRamo(int id, Ramo ramo, string Username)
        {
            _ramoes = new Ramos();
            return _ramoes.EliminarRamo(id, ramo, Username);
        }

        #endregion

        #region Metodos RAMODETALLE

        public List<RamoDetalle> ListarRamoDetalle(int id)
        {
            return new Ramos().ListarRamoDetalle(id);
        }

        public int AgruparRamoDetalle(int ramo_id, string ramosTrue, string ramosFalse)
        {
            return new Ramos().AgruparRamoDetalle(ramo_id, ramosTrue, ramosFalse);
        }

        public List<RamoDetalle> ListarRamoDetalleXCompania(int companiaId)
        {
            return new Ramos().ListarRamoDetalleXCompania(companiaId);
        }

        #endregion

        #region Metodos AMPAROS

        public List<Amparo> ListarAmparoes()
        {
            _amparoes = new Amparos();
            return _amparoes.ListarAmparo();
        }

        public List<Amparo> ListarAmparoesPorId(int idAmparo)
        {
            _amparoes = new Amparos();
            return _amparoes.ListarAmparoesPorId(idAmparo);
        }

        public int InsertarAmparo(Amparo amparo, ref string mensajeDeError, string Username)
        {
            _amparoes = new Amparos();
            return _amparoes.Insertar(amparo, ref mensajeDeError, Username);
        }

        public void EliminarAmparo(int id, ref string mensajeDeError, string Username)
        {
            _amparoes = new Amparos();

            _amparoes.Eliminar(id, ref mensajeDeError, Username);
        }


        public List<AmparoDetalle> ListarAmparoDetalle()
        {
            _amparoes = new Amparos();

            return _amparoes.ListarAmparoDetalle();
        }

        public List<AmparoDetalle> ListarAmparoDetallePorId(int idamparo)
        {
            _amparoes = new Amparos();
            return _amparoes.ListarAmparoDetallePorId(idamparo);
        }

        public int InsertarAmparoDetalle(AmparoDetalle amparoDetalle, string Username)
        {
            _amparoes = new Amparos();
            return _amparoes.InsertarAmparoDetalle(amparoDetalle, Username);
        }

        public int EliminarAmparoDetalle(int amparoid, ref string mensajeDeError, string Username)
        {
            _amparoes = new Amparos();
            return _amparoes.EliminarAmparoDetalle(amparoid, ref mensajeDeError, Username);
        }

        #endregion

        

        //PRODUCCION//*****************************************

        #region Metodos BENEFICIARIOS

        public List<Beneficiario> ListarBeneficiarios()
        {
            _beneficiarios = new Beneficiarios();
            return _beneficiarios.ListarBeneficiarios();
        }

        public List<Beneficiario> ListarBeneficiariosPorId(int idBeneficiario)
        {
            _beneficiarios = new Beneficiarios();
            return _beneficiarios.ListarBeneficiariosPorId(idBeneficiario);
        }

        #endregion

        #region Metodos CLIENTES

        public List<Cliente> ListarClientes()
        {
            _clientes = new Clientes();
            return _clientes.ListarClientes();
        }

        #endregion

        #region REDES
        /// <summary>
        /// Obtiene listado de las redes
        /// </summary>
        /// <returns>Lista de redes</returns>
        public List<Red> ListarRedes()
        {
            _redes = new Redes();
            return _redes.ListarRedes();
        }

        /// <summary>
        /// Obtiene listado de las redes por id
        /// </summary>
        /// <param name="idRed">Id de la red a consultar</param>
        /// <returns>Lista de objectos Red</returns>
        public List<Red> ListarRedesPorId(int idRed)
        {
            _redes = new Redes();
            return _redes.ListarRedesPorId(idRed);
        }

        /// <summary>
        /// Guarda un registro de red
        /// </summary>
        /// <param name="zona">Objecto Red a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarRed(Red red, string Username)
        {
            _redes = new Redes();
            return _redes.InsertarRed(red, Username);
        }

        /// <summary>
        /// Actualiza un registro de Red
        /// </summary>
        /// <param name="id">Id de la Red a modificar</param>
        /// <param name="zona">Objeto Red utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarRed(int id, Red red, string Username)
        {
            _redes = new Redes();
            return _redes.ActualizarRed(id, red, Username);
        }

        /// <summary>
        /// Elimina un registro de Red
        /// </summary>
        /// <param name="id">Id de la Red a eliminar</param>
        /// <param name="zona">Objecto Red utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarRed(int id, Red red, string Username)
        {
            _redes = new Redes();
            return _redes.EliminarRed(id, red, Username);
        }

        /// <summary>
        /// Obtiene listado de las rede del detalle para ser agrupados
        /// </summary>
        /// <returns>Lista de objectos Redes Detalle para agruparlos</returns>
        public List<RedDetalle> ListarRedesDetalle()
        {
            _redes = new Redes();
            return _redes.ListarRedesDetalle();
        }

        /// <summary>
        /// Agrupa las redes detalle 
        /// </summary>
        /// <returns>True or False</returns>
        public int AgruparRedDetalle(RedDetalle redDetalle)
        {
            _redes = new Redes();
            return _redes.AgruparRedDetalle(redDetalle);
        }

        /// <summary>
        /// Elimina la agrupacion las redes detalle 
        /// </summary>
        /// <returns>True or False</returns>
        public int EliminarAgrupacionRedDetalle(int idRed, string Username)
        {
            _redes = new Redes();
            return _redes.EliminarAgrupacionRedDetalle(idRed, Username);
        }

        #endregion

        #region BANCO
        /// <summary>
        /// Obtiene listado de los bancos agrupados
        /// </summary>
        /// <returns>Lista de bancos agrupados</returns>
        public List<Banco> ListarBancos()
        {
            _redes = new Redes();
            return _redes.ListarBancos();
        }

        /// <summary>
        /// Obtiene listado de los bancos agrupados por id
        /// </summary>
        /// <param name="idBanco">Id del banco agrupado a consultar</param>
        /// <returns>Lista de objectos bancos</returns>
        public List<Banco> ListarBancosPorId(int idBanco)
        {
            _redes = new Redes();
            return _redes.ListarBancosPorId(idBanco);
        }

        /// <summary>
        /// Guarda un registro de banco agrupado
        /// </summary>
        /// <param name="banco">Objecto Banco agrupado a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarBanco(Banco banco, string Username)
        {
            _redes = new Redes();
            return _redes.InsertarBanco(banco, Username);
        }

        /// <summary>
        /// Actualiza un registro de banco agrupado
        /// </summary>
        /// <param name="id">Id de la Banco agrupado a modificar</param>
        /// <param name="banco">Objeto Banco utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarBanco(int id, Banco banco, string Username)
        {
            _redes = new Redes();
            return _redes.ActualizarBanco(id, banco, Username);
        }

        /// <summary>
        /// Elimina un registro de Red
        /// </summary>
        /// <param name="id">Id de la Red a eliminar</param>
        /// <param name="banco">Objecto Red utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarBanco(int id, Banco banco, string Username)
        {
            _redes = new Redes();
            return _redes.EliminarBanco(id, banco, Username);
        }

        /// <summary>
        /// Obtiene listado de los bancos del detalle para ser agrupados
        /// </summary>
        /// <returns>Lista de objectos Banco Detalle para agruparlos</returns>
        public List<BancoDetalle> ListarBancosDetalle()
        {
            _redes = new Redes();
            return _redes.ListarBancosDetalle();
        }

        /// <summary>
        /// Agrupa los bancos detalle 
        /// </summary>
        /// <returns>True or False</returns>
        public int AgruparBancoDetalle(BancoDetalle bancoDetalle)
        {
            _redes = new Redes();
            return _redes.AgruparBancoDetalle(bancoDetalle);
        }

        /// <summary>
        /// Elimina la agrupacion los bancos detalle 
        /// </summary>
        /// <returns>True or False</returns>
        public int EliminarAgrupacionBancoDetalle(int idBanco, string Username)
        {
            _redes = new Redes();
            return _redes.EliminarAgrupacionBancoDetalle(idBanco, Username);
        }



        #endregion

        //CONCURSOS//******************************************

        #region Metodos TIPOCONCURSOS

        public List<TipoConcurso> ListarTipoConcursoes()
        {
            _tipoconcursoes = new Tipoconcursoes();
            return _tipoconcursoes.ListarTipoconcursoes();
        }
        #endregion

        #region Metodos CONCURSOS

        public int InsertarConcurso(Concurso concurso, string Username)
        {
            _concursoes = new Concursoes();
            return _concursoes.InsertarConcurso(concurso, Username);
        }

        /// <summary>
        /// Obtiene listado de concursos
        /// </summary>
        /// <returns>Lista de concursos</returns>
        public List<Concurso> ListarConcursoes()
        {
            _concursoes = new Concursoes();
            return _concursoes.ListarConcursoes();
        }

        /// <summary>
        /// Obtiene listado de concursos por id
        /// </summary>
        /// <returns>Lista de concursos</returns>
        public List<Concurso> ListarConcursoesPorId(int idConcurso)
        {
            _concursoes = new Concursoes();
            return _concursoes.ListarConcursoesPorId(idConcurso);
        }

        public int ActualizarConcurso(int id, Concurso concurso, string Username)
        {
            _concursoes = new Concursoes();
            return _concursoes.ActualizarConcurso(id, concurso, Username);
        }

        public int DuplicarConcurso(int id, Concurso concurso, string Username)
        {
            _concursoes = new Concursoes();
            return _concursoes.DuplicarConcurso(id, concurso, Username);
        }

        public int DuplicarParticipanteConcurso(int id, int idNuevo, string Username)
        {
            _concursoes = new Concursoes();
            return _concursoes.DuplicarParticipanteConcurso(id, idNuevo, Username);
        }

        public string EliminarConcursos(int id, Concurso concurso, string Username)
        {
            _concursoes = new Concursoes();
            return _concursoes.EliminarConcursos(id, concurso, Username);
        }

        public int EliminarLiquidacionConcurso(int liqregla, string Username)
        {
            _liquidacionConcurso = new LiquidacionConcurso();
            return _liquidacionConcurso.EliminarLiquidacionConcurso(liqregla, Username);
        }

        public int validarEstadoLiquidacion(int regla_id, int concurso_id)
        {
            _liquidacionConcurso = new LiquidacionConcurso();
            return _liquidacionConcurso.validarEstadoLiquidacion(regla_id, concurso_id);
        }

        public List<DetalleLiquidacionEncabezado> TipoConcursoxRegla(int liquidacionRegla_id, string valorCOnsulta)
        {
            _liquidacionConcurso = new LiquidacionConcurso();
            return _liquidacionConcurso.TipoConcursoxRegla(liquidacionRegla_id, valorCOnsulta);
        }

        public int validarParticipantesxConcurso(int concurso_id)
        {
            _liquidacionConcurso = new LiquidacionConcurso();
            return _liquidacionConcurso.validarParticipantesxConcurso(concurso_id);
        }

        public int RetornarTipoConcurso(int concurso_id)
        {
            _liquidacionConcurso = new LiquidacionConcurso();
            return _liquidacionConcurso.RetornarTipoConcurso(concurso_id);
        }

        public int ValidarConcursoPrincipal(int tipoConcurso_id, int añofIni, int añofFin)
        {
            _concursoes = new Concursoes();
            return _concursoes.ValidarConcursoPrincipal(tipoConcurso_id, añofIni, añofFin);
        }

        public string RetornarNombreSegmentoUsuario(int segmento_id)
        {
            _concursoes = new Concursoes();
            return _concursoes.RetornarNombreSegmentoUsuario(segmento_id);
        }
        #endregion

        #region Metodos CATEGORIASXREGLA

        public List<CategoriaxRegla> ListarCategoriasxRegla(int regla_id)
        {
            _categoriasxRegla = new CategoriasxRegla();
            return _categoriasxRegla.ListarCategoriasxRegla(regla_id);
        }

        public int ActualizarCategoriaxRegla(List<CategoriaxRegla> categoriasxRegla, string Username)
        {
            _categoriasxRegla = new CategoriasxRegla();
            return _categoriasxRegla.ActualizarCategoriaxRegla(categoriasxRegla, Username);
        }

        #endregion

        #region Metodos MONEDAXNEGOCIO

        public int InsertarMonedaxNegocio(MonedaxNegocio monedaxnegocio, string Username)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.InsertarMonedaxNegocio(monedaxnegocio, Username);
        }

        public List<MonedaxNegocio> ListarMonedaxNegocio()
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.ListarMonedaxNegocio();
        }

        public List<MonedaxNegocio> ListarMonedaxNegocioPorId(int idMonedaxNegocio)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.ListarMonedaxNegocioPorId(idMonedaxNegocio);
        }

        public int ActualizarMonedaxNegocio(int id, MonedaxNegocio monedaxnegocio, string Username)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.ActualizarMonedaxNegocio(id, monedaxnegocio, Username);
        }

        public string EliminarMonedaxNegocio(int id, MonedaxNegocio monedaxnegocio, string Username)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.EliminarMonedaxNegocio(id, monedaxnegocio, Username);
        }
        #endregion

        #region Metodos MAESTROMONEDAXNEGOCIO

        public int InsertarMaestroMonedaxNegocio(MaestroMonedaxNegocio maestromonedaxnegocio, string Username)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.InsertarMaestroMonedaxNegocio(maestromonedaxnegocio, Username);
        }

        public List<MaestroMonedaxNegocio> ListarMaestroMonedaxNegocio()
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.ListarMaestroMonedaxNegocio();
        }

        public List<MaestroMonedaxNegocio> ListarMaestroMonedaxNegocioPorId(int idMaestroMonedaxNegocio)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.ListarMaestroMonedaxNegocioPorId(idMaestroMonedaxNegocio);
        }

        public int ActualizarMaestroMonedaxNegocio(int id, MaestroMonedaxNegocio maestromonedaxnegocio, string Username)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.ActualizarMaestroMonedaxNegocio(id, maestromonedaxnegocio, Username);
        }

        public string EliminarMaestroMonedaxNegocio(int id, MaestroMonedaxNegocio maestromonedaxnegocio, string Username)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.EliminarMaestroMonedaxNegocio(id, maestromonedaxnegocio, Username);
        }

        public Dictionary<int, string> ListarSegmentoxUsuario(string userName)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.ListarSegmentoxUsuario(userName);
        }

        public List<UsuarioxSegmento> ListarSegmentodelUsuario()
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.ListarSegmentodelUsuario();
        }
        #endregion

        #region Metodos TOPEMONEDA

        public int InsertarTopeMoneda(TopeMoneda topemoneda, string Username)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.InsertarTopeMoneda(topemoneda, Username);
        }

        public List<TopeMoneda> ListarTopeMoneda()
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.ListarTopeMoneda();
        }

        public List<TopeMoneda> ListarTopeMonedaPorId(int idTopeMoneda)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.ListarTopeMonedaPorId(idTopeMoneda);
        }

        public int ActualizarTopeMoneda(int id, TopeMoneda topemoneda, string Username)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.ActualizarTopeMoneda(id, topemoneda, Username);
        }

        public string EliminarTopeMoneda(int id, TopeMoneda topemoneda, string Username)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.EliminarTopeMoneda(id, topemoneda, Username);
        }
        #endregion

        #region Metodos TOPEXEDAD

        public int InsertarTopexEdad(TopexEdad topexedad, string Username)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.InsertarTopexEdad(topexedad, Username);
        }

        public List<TopexEdad> ListarTopexEdad()
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.ListarTopexEdad();
        }

        public List<TopexEdad> ListarTopexEdadPorId(int idTopexEdad)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.ListarTopexEdadPorId(idTopexEdad);
        }

        public int ActualizarTopexEdad(int id, TopexEdad topexedad, string Username)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.ActualizarTopexEdad(id, topexedad, Username);
        }

        public string EliminarTopexEdad(int id, TopexEdad topexedad, string Username)
        {
            _monedaxnegocio = new MonedaxNegocios();
            return _monedaxnegocio.EliminarTopexEdad(id, topexedad, Username);
        }
        #endregion

        #region Metodos COMPANIAXETAPA

        public int InsertarCompaniaxEtapa(CompaniaxEtapa companiaxetapa, string Username)
        {
            _companiaxetapas = new CompaniaxEtapas();
            return _companiaxetapas.InsertarCompaniaxEtapa(companiaxetapa, Username);
        }

        public List<CompaniaxEtapa> ListarCompaniaxEtapa()
        {
            _companiaxetapas = new CompaniaxEtapas();
            return _companiaxetapas.ListarCompaniaxEtapa();
        }

        public List<CompaniaxEtapa> ListarCompaniaxEtapaPorId(int idCompaniaxEtapa)
        {
            _companiaxetapas = new CompaniaxEtapas();
            return _companiaxetapas.ListarCompaniaxEtapaPorId(idCompaniaxEtapa);
        }

        public int ActualizarCompaniaxEtapa(int id, CompaniaxEtapa companiaxetapa, string Username)
        {
            _companiaxetapas = new CompaniaxEtapas();
            return _companiaxetapas.ActualizarCompaniaxEtapa(id, companiaxetapa, Username);
        }

        public string EliminarCompaniaxEtapa(int id, CompaniaxEtapa companiaxetapa, string Username)
        {
            _companiaxetapas = new CompaniaxEtapas();
            return _companiaxetapas.EliminarCompaniaxEtapa(id, companiaxetapa, Username);
        }
        #endregion

        #region Metodos REGLA

        public int InsertarRegla(Regla regla, string Username)
        {
            _reglas = new Reglas();
            return _reglas.InsertarRegla(regla, Username);
        }

        public List<Regla> ListarRegla()
        {
            _reglas = new Reglas();
            return _reglas.ListarRegla();
        }

        public List<Regla> ListarReglaPorId(int idRegla)
        {
            _reglas = new Reglas();
            return _reglas.ListarReglaPorId(idRegla);
        }

        public List<Regla> ListarReglaPorConcursoId(int idConcurso)
        {
            _reglas = new Reglas();
            return _reglas.ListarReglaPorConcursoId(idConcurso);
        }

        public int ActualizarRegla(int id, Regla regla, string Username)
        {
            _reglas = new Reglas();
            return _reglas.ActualizarRegla(id, regla, Username);
        }

        public int DuplicarRegla(int id, Regla regla, string Username)
        {
            _reglas = new Reglas();
            return _reglas.DuplicarRegla(id, regla, Username);
        }

        public List<Regla> ListarPremiosParaAsociar(int concurso_id, int regla_id)
        {
            return new Reglas().ListarPremiosParaAsociar(concurso_id, regla_id);
        }

        public string EliminarRegla(int id, Regla regla, string Username)
        {
            _reglas = new Reglas();
            return _reglas.EliminarRegla(id, regla, Username);
        }
        #endregion

        #region Metodos PERIODOREGLA

        public List<PeriodoRegla> ListarPeriodoRegla()
        {
            _reglas = new Reglas();
            return _reglas.ListarPeriodoRegla();
        }

        #endregion

        #region Metodos TIPOREGLA

        public List<TipoRegla> ListarTipoRegla()
        {
            _reglas = new Reglas();
            return _reglas.ListarTipoRegla();
        }

        #endregion

        #region Metodos OPERADOR

        public List<Operador> ListarOperadorLogico()
        {
            _reglas = new Reglas();
            return _reglas.ListarOperadorLogico();
        }

        public List<Operador> ListarOperadorMatematico()
        {
            _reglas = new Reglas();
            return _reglas.ListarOperadorMatematico();
        }

        #endregion

        #region Metodos tabla

        public List<tabla> Listartabla()
        {
            _reglas = new Reglas();
            return _reglas.Listartabla();
        }

        #endregion

        #region Metodos ESTRATEGIAREGLA

        public List<EstrategiaRegla> ListarEstrategiaRegla()
        {
            _reglas = new Reglas();
            return _reglas.ListarEstrategiaRegla();
        }

        #endregion

        #region Metodos SUBREGLA

        public int InsertarSubRegla(SubRegla subregla, string Username)
        {
            _reglas = new Reglas();
            return _reglas.InsertarSubRegla(subregla, Username);
        }

        public List<SubRegla> ListarSubRegla()
        {
            _reglas = new Reglas();
            return _reglas.ListarSubRegla();
        }

        public List<SubRegla> ListarSubReglaPorId(int idSubRegla)
        {
            _reglas = new Reglas();
            return _reglas.ListarSubReglaPorId(idSubRegla);
        }

        public int ActualizarSubRegla(int id, SubRegla subregla, string Username)
        {
            _reglas = new Reglas();
            return _reglas.ActualizarSubRegla(id, subregla, Username);
        }

        public string EliminarSubRegla(int id, SubRegla subregla, string Username)
        {
            _reglas = new Reglas();
            return _reglas.EliminarSubRegla(id, subregla, Username);
        }
        #endregion

        #region Metodos CONDICION

        public int InsertarCondicion(Condicion condicion, string Username)
        {
            _reglas = new Reglas();
            return _reglas.InsertarCondicion(condicion, Username);
        }

        public int ContarVariablexLiquidacion(int concurso_id, int regla_id, int subregla_id)
        {
            _reglas = new Reglas();
            return _reglas.ContarVariablexLiquidacion(concurso_id, regla_id, subregla_id);
        }

        public int RetornarTipoTablaxVariable(int subregla_id)
        {
            _reglas = new Reglas();
            return _reglas.RetornarTipoTablaxVariable(subregla_id);
        }

        public int RetornarTipoSubRegla(int subregla_id)
        {
            _reglas = new Reglas();
            return _reglas.RetornarTipoSubRegla(subregla_id);
        }

        public string RetornarTipoDato(int variable_id)
        {
            _reglas = new Reglas();
            return _reglas.RetornarTipoDato(variable_id);
        }

        public List<Variable> ListarVariablesxTabla(int subregla_id)
        {
            _reglas = new Reglas();
            return _reglas.ListarVariablesxTabla(subregla_id);
        }

        public List<Condicion> ListarCondicion()
        {
            _reglas = new Reglas();
            return _reglas.ListarCondicion();
        }

        public List<Condicion> ListarCondicionPorId(int idCondicion)
        {
            _reglas = new Reglas();
            return _reglas.ListarCondicionPorId(idCondicion);
        }

        public int MostrarTipodeVariable(Condicion condicion)
        {
            _reglas = new Reglas();
            return _reglas.MostrarTipodeVariable(condicion);
        }

        public int ActualizarCondicion(int id, Condicion condicion, string Username)
        {
            _reglas = new Reglas();
            return _reglas.ActualizarCondicion(id, condicion, Username);
        }

        public string EliminarCondicion(int id, Condicion condicion, string Username)
        {
            _reglas = new Reglas();
            return _reglas.EliminarCondicion(id, condicion, Username);
        }
        #endregion

        #region Metodos CONDICIONXPREMIOSUBREGLA

        public List<CondicionxPremioSubregla> ListarCondicionxPremioSubRegla()
        {
            _reglas = new Reglas();
            return _reglas.ListarCondicionxPremioSubRegla();
        }

        public CondicionxPremioSubregla ListarCondicionxPremioSubReglaPorId(int id)
        {
            _reglas = new Reglas();
            return _reglas.ListarCondicionxPremioSubReglaPorId(id);
        }

        public int InsertarCondicionxPremioSubRegla(CondicionxPremioSubregla condicionPS, string Username)
        {
            _reglas = new Reglas();
            return _reglas.InsertarCondicionxPremioSubRegla(condicionPS, Username);
        }

        public int ActualizarCondicionxPremioSubRegla(int id, CondicionxPremioSubregla condicionPS, string Username)
        {
            _reglas = new Reglas();
            return _reglas.ActualizarCondicionxPremioSubRegla(id, condicionPS, Username);
        }

        public string EliminarCondicionxPremioSubRegla(int id, CondicionxPremioSubregla condicionPS, string Username)
        {
            _reglas = new Reglas();
            return _reglas.EliminarCondicionxPremioSubRegla(id, condicionPS, Username);
        }

        #endregion

        #region Metodos CONDICIONAGRUPADA

        public List<CondicionAgrupada> ListarCondicionAgrupada()
        {
            _reglas = new Reglas();
            return _reglas.ListarCondicionAgrupada();
        }

        public List<CondicionAgrupada> ListarCondicionAgrupadaPorId(int idCondicionAgrupada)
        {
            _reglas = new Reglas();
            return _reglas.ListarCondicionAgrupadaPorId(idCondicionAgrupada);
        }

        public int InsertarCondicionAgrupada(CondicionAgrupada condicionagrupada, string Username)
        {
            _reglas = new Reglas();
            return _reglas.InsertarCondicionAgrupada(condicionagrupada, Username);
        }

        public int ActualizarCondicionAgrupada(int id, CondicionAgrupada condicionagrupada, string Username)
        {
            _reglas = new Reglas();
            return _reglas.ActualizarCondicionAgrupada(id, condicionagrupada, Username);
        }

        public List<Condicion> ListarCondicionPorSubRegla(int idSubRegla)
        {
            _reglas = new Reglas();
            return _reglas.ListarCondicionPorSubRegla(idSubRegla);
        }

        public string EliminarCondicionAgrupada(int id, CondicionAgrupada condicionagrupada, string Username)
        {
            _reglas = new Reglas();
            return _reglas.EliminarCondicionAgrupada(id, condicionagrupada, Username);
        }

        public List<Operador> ListarOperadorAgrupado()
        {
            _reglas = new Reglas();
            return _reglas.ListarOperadorAgrupado();
        }
        #endregion

        #region Metodos PREMIOS

        public int InsertarPremio(Premio premio, string Username)
        {
            _reglas = new Reglas();
            return _reglas.InsertarPremio(premio, Username);
        }

        public List<Premio> ListarPremio()
        {
            _reglas = new Reglas();
            return _reglas.ListarPremio();
        }

        public List<Premio> ListarPremioPorId(int idPremio)
        {
            _reglas = new Reglas();
            return _reglas.ListarPremioPorId(idPremio);
        }

        public int ActualizarPremio(int id, Premio premio, string Username)
        {
            _reglas = new Reglas();
            return _reglas.ActualizarPremio(id, premio, Username);
        }

        public string EliminarPremio(int id, Premio premio, string Username)
        {
            _reglas = new Reglas();
            return _reglas.EliminarPremio(id, premio, Username);
        }
        #endregion

        #region Metodos CONCEPTODESCUENTO

        public List<ConceptoDescuento> ListarConceptoDescuento()
        {
            _reglas = new Reglas();
            return _reglas.ListarConceptoDescuento();
        }

        public int InsertarReglaxConceptoDescuento(string conceptoDescuento, int idRegla, InfoAplicacion info, string Username)
        {
            _reglas = new Reglas();
            return _reglas.InsertarReglaxConceptoDescuento(conceptoDescuento, idRegla, info, Username);
        }

        #endregion

        #region Metodos TIPOPREMIOS

        public int InsertarTipoPremio(TipoPremio tipopremio, string Username)
        {
            _reglas = new Reglas();
            return _reglas.InsertarTipoPremio(tipopremio, Username);
        }

        public List<TipoPremio> ListarTipoPremio()
        {
            _reglas = new Reglas();
            return _reglas.ListarTipoPremio();
        }

        public List<TipoPremio> ListarTipoPremioPorId(int idTipoPremio)
        {
            _reglas = new Reglas();
            return _reglas.ListarTipoPremioPorId(idTipoPremio);
        }

        public int ActualizarTipoPremio(int id, TipoPremio tipopremio, string Username)
        {
            _reglas = new Reglas();
            return _reglas.ActualizarTipoPremio(id, tipopremio, Username);
        }

        public string EliminarTipoPremio(int id, TipoPremio tipopremio, string Username)
        {
            _reglas = new Reglas();
            return _reglas.EliminarTipoPremio(id, tipopremio, Username);
        }
        #endregion

        #region Metodos UNIDADMEDIDA

        public int InsertarUnidadMedida(UnidadMedida unidadmedida, string Username)
        {
            _reglas = new Reglas();
            return _reglas.InsertarUnidadMedida(unidadmedida, Username);
        }

        public List<UnidadMedida> ListarUnidadMedida()
        {
            _reglas = new Reglas();
            return _reglas.ListarUnidadMedida();
        }

        public List<UnidadMedida> ListaUnidadMedidaPorId(int idUnidadMedida)
        {
            _reglas = new Reglas();
            return _reglas.ListaUnidadMedidaPorId(idUnidadMedida);
        }

        public int ActualizarUnidadMedida(int id, UnidadMedida unidadmedida, string Username)
        {
            _reglas = new Reglas();
            return _reglas.ActualizarUnidadMedida(id, unidadmedida, Username);
        }

        public string EliminarUnidadMedida(int id, UnidadMedida unidadmedida, string Username)
        {
            _reglas = new Reglas();
            return _reglas.EliminarUnidadMedida(id, unidadmedida, Username);
        }
        #endregion

        #region Metodos TIPOUNIDADMEDIDA

        public int InsertarTipoUnidadMedida(TipoUnidadMedida tipounidadmedida, string Username)
        {
            _reglas = new Reglas();
            return _reglas.InsertarTipoUnidadMedida(tipounidadmedida, Username);
        }

        public List<TipoUnidadMedida> ListarTipoUnidadMedida()
        {
            _reglas = new Reglas();
            return _reglas.ListarTipoUnidadMedida();
        }

        public List<TipoUnidadMedida> ListaTipoUnidadMedidaPorId(int idTipoUnidadMedida)
        {
            _reglas = new Reglas();
            return _reglas.ListaTipoUnidadMedidaPorId(idTipoUnidadMedida);
        }

        public int ActualizarTipoUnidadMedida(int id, TipoUnidadMedida tipounidadmedida, string Username)
        {
            _reglas = new Reglas();
            return _reglas.ActualizarTipoUnidadMedida(id, tipounidadmedida, Username);
        }

        public string EliminarTipoUnidadMedida(int id, TipoUnidadMedida tipounidadmedida, string Username)
        {
            _reglas = new Reglas();
            return _reglas.EliminarTipoUnidadMedida(id, tipounidadmedida, Username);
        }
        #endregion

        #region Metodos PREMIOXSUBREGLA

        public int InsertarPremioxSubregla(PremioxSubregla premioxsubregla, string Username)
        {
            _reglas = new Reglas();
            return _reglas.InsertarPremioxSubregla(premioxsubregla, Username);
        }

        public List<PremioxSubregla> ListarPremioxSubregla()
        {
            _reglas = new Reglas();
            return _reglas.ListarPremioxSubregla();
        }

        public List<PremioxSubregla> ListaPremioxSubreglaPorId(int idPremioxSubRegla)
        {
            _reglas = new Reglas();
            return _reglas.ListaPremioxSubreglaPorId(idPremioxSubRegla);
        }

        public int ActualizarPremioxSubregla(int id, PremioxSubregla premioxsubregla, string Username)
        {
            _reglas = new Reglas();
            return _reglas.ActualizarPremioxSubregla(id, premioxsubregla, Username);
        }

        public string EliminarPremioxSubregla(int id, PremioxSubregla premioxsubregla, string Username)
        {
            _reglas = new Reglas();
            return _reglas.EliminarPremioxSubregla(id, premioxsubregla, Username);
        }
        #endregion

        #region Metodos PARTICIPANTECONCURSOS

        public int InsertarParticipanteConcurso(ParticipanteConcurso participanteconcurso, string Username)
        {
            _participanteconcursoes = new ParticipanteConcursos();
            return _participanteconcursoes.InsertarParticipanteConcurso(participanteconcurso, Username);
        }

        public List<Participante> ListarParticipantesTotales()
        {
            _participanteconcursoes = new ParticipanteConcursos();
            return _participanteconcursoes.ListarParticipantes();
        }

        public List<ParticipanteConcurso> ListarParticipanteConcursoes()
        {
            _participanteconcursoes = new ParticipanteConcursos();
            return _participanteconcursoes.ListarParticipanteConcursoes();
        }

        public List<ParticipanteConcurso> ListarParticipanteConcursoesPorId(int idParticipanteConcurso)
        {
            _participanteconcursoes = new ParticipanteConcursos();
            return _participanteconcursoes.ListarParticipanteConcursoesPorId(idParticipanteConcurso);
        }

        public int ActualizarParticipanteConcurso(int id, ParticipanteConcurso participanteconcurso, string Username)
        {
            _participanteconcursoes = new ParticipanteConcursos();
            return _participanteconcursoes.ActualizarParticipanteConcurso(id, participanteconcurso, Username);
        }

        public string EliminarParticipanteConcurso(int id, ParticipanteConcurso participanteconcurso, string Username)
        {
            _participanteconcursoes = new ParticipanteConcursos();
            return _participanteconcursoes.EliminarParticipanteConcurso(id, participanteconcurso, Username);
        }
        #endregion

        #region Metodos PRODUCTOCONCURSOS

        public int InsertarProductoConcurso(ProductoConcurso productoconcurso, string Username)
        {
            _productoconcursoes = new ProductoConcursos();
            return _productoconcursoes.InsertarProductoConcurso(productoconcurso, Username);
        }

        public List<ProductoConcurso> ListarProductoConcursoes()
        {
            _productoconcursoes = new ProductoConcursos();
            return _productoconcursoes.ListarProductoConcursoes();
        }

        public List<ProductoConcurso> ListarProductoConcursoesPorId(int idProductoConcurso)
        {
            _productoconcursoes = new ProductoConcursos();
            return _productoconcursoes.ListarProductoConcursoesPorId(idProductoConcurso);
        }

        public int ActualizarProductoConcurso(int id, ProductoConcurso productoconcurso, string Username)
        {
            _productoconcursoes = new ProductoConcursos();
            return _productoconcursoes.ActualizarProductoConcurso(id, productoconcurso, Username);
        }

        public string EliminarProductoConcurso(int id, ProductoConcurso productoconcurso, string Username)
        {
            _productoconcursoes = new ProductoConcursos();
            return _productoconcursoes.EliminarProductoConcurso(id, productoconcurso, Username);
        }
        #endregion

        #region Metodos ETAPAPRODUCTO

        public int InsertarEtapaProducto(EtapaProducto etapaproducto, string Username)
        {
            _etapaconcursoes = new EtapaConcursos();
            return _etapaconcursoes.InsertarEtapaProducto(etapaproducto, Username);
        }

        public List<EtapaProducto> ListarEtapaProductoes()
        {
            _etapaconcursoes = new EtapaConcursos();
            return _etapaconcursoes.ListarEtapaProductoes();
        }

        public List<EtapaProducto> ListarEtapaProductoesPorId(int idEtapaProducto)
        {
            _etapaconcursoes = new EtapaConcursos();
            return _etapaconcursoes.ListarEtapaProductoesPorId(idEtapaProducto);
        }

        public int ActualizarEtapaProducto(int id, EtapaProducto etapaproducto, string Username)
        {
            _etapaconcursoes = new EtapaConcursos();
            return _etapaconcursoes.ActualizarEtapaProducto(id, etapaproducto, Username);
        }

        public string EliminarEtapaProducto(int id, EtapaProducto etapaproducto, string Username)
        {
            _etapaconcursoes = new EtapaConcursos();
            return _etapaconcursoes.EliminarEtapaProducto(id, etapaproducto, Username);
        }


        #endregion

        #region MONEDA



        public List<Moneda> ListarMonedas()
        {
            _monedas = new Monedas();
            return _monedas.ListarMonedas();
        }

        /// <summary>
        /// Obtiene listado de las redes por id
        /// </summary>
        /// <param name="idMoneda">Id de la red a consultar</param>
        /// <returns>Lista de objecto Red</returns>
        public List<Moneda> ListarMonedasPorId(int idMoneda)
        {
            _monedas = new Monedas();
            return _monedas.ListarMonedasPorId(idMoneda);
        }

        /// <summary>
        /// Guarda un registro de red
        /// </summary>
        /// <param name="moneda">Objecto Red a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarMoneda(Moneda moneda, string Username)
        {
            _monedas = new Monedas();
            return _monedas.InsertarMoneda(moneda, Username);
        }

        /// <summary>
        /// Actualiza un registro de Red
        /// </summary>
        /// <param name="id">Id de la Red a modificar</param>
        /// <param name="red">Objeto Red utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarMoneda(int id, Moneda moneda, string Username)
        {
            _monedas = new Monedas();
            return _monedas.ActualizarMoneda(id, moneda, Username);
        }

        /// <summary>
        /// Elimina un registro de Red
        /// </summary>
        /// <param name="id">Id de la Red a eliminar</param>
        /// <param name="red">Objecto Red utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarMoneda(int id, Moneda moneda, string Username)
        {
            _monedas = new Monedas();
            return _monedas.EliminarMoneda(id, moneda, Username);
        }

        /// <summary>
        /// Obtiene listado de las unidades de medida
        /// </summary>
        /// <returns>Lista de Unidades de medida</returns>
        public List<UnidadMedida> ListarUnidadesMedida()
        {
            _monedas = new Monedas();
            return _monedas.ListarUnidadesMedida();
        }
        #endregion

        #region NIVEL

        public List<Nivel> ListarNivels()
        {
            _nivels = new Nivels();
            return _nivels.ListarNivels();
        }

        public List<Nivel> ListarNivelsPorId(int idNivel)
        {
            _nivels = new Nivels();
            return _nivels.ListarNivelsPorId(idNivel);
        }

        public int InsertarNivel(Nivel nivel, string Username)
        {
            _nivels = new Nivels();
            return _nivels.InsertarNivel(nivel, Username);
        }

        public int ActualizarNivel(int id, Nivel nivel, string Username)
        {
            _nivels = new Nivels();
            return _nivels.ActualizarNivel(id, nivel, Username);
        }


        public string EliminarNivel(int id, Nivel nivel, string Username)
        {
            _nivels = new Nivels();
            return _nivels.EliminarNivel(id, nivel, Username);
        }

        #endregion

        #region ANTIGUEDAD
        public List<AntiguedadxNivel> ListarAntiguedades()
        {
            _antiguedades = new AntiguedadxNiveles();
            return _antiguedades.ListarAntiguedades();
        }

        public List<AntiguedadxNivel> ListarAntiguedadesPorId(int id)
        {
            _antiguedades = new AntiguedadxNiveles();
            return _antiguedades.ListarAntiguedadesPorId(id);
        }

        public int InsertarAntiguedadxNivel(AntiguedadxNivel antiguedad, string Username)
        {
            _antiguedades = new AntiguedadxNiveles();
            return _antiguedades.InsertarAntiguedadxNivel(antiguedad, Username);
        }

        public int ActualizarAntiguedadxNivel(int id, AntiguedadxNivel antiguedad, string Username)
        {
            _antiguedades = new AntiguedadxNiveles();
            return _antiguedades.ActualizarAntiguedadxNivel(id, antiguedad, Username);
        }

        public string EliminarAntiguedadxNivel(int id, AntiguedadxNivel antiguedad, string Username)
        {
            _antiguedades = new AntiguedadxNiveles();
            return _antiguedades.EliminarAntiguedadxNivel(id, antiguedad, Username);
        }


        #endregion

        #region BASE MONEDA

        /// <summary>
        /// Obtiene listado de las Monedas
        /// </summary>
        /// <returns>Lista de Monedas</returns>
        public List<BaseMoneda> ListarBaseMoneda()
        {
            _baseMonedas = new BaseMonedas();
            return _baseMonedas.ListarBaseMoneda();
        }

        /// <summary>
        /// Obtiene listado de las BaseMonedas por id
        /// </summary>
        /// <param name="id">Id de la BaseMoneda a consultar</param>
        /// <returns>Lista de objectos BaseMoneda</returns>
        public List<BaseMoneda> ListarBaseMonedasPorId(int id)
        {
            _baseMonedas = new BaseMonedas();
            return _baseMonedas.ListarBaseMonedasPorId(id);
        }

        /// <summary>
        /// Guarda un registro de BaseMoneda
        /// </summary>
        /// <param name="baseMoneda">Objecto BaseMoneda a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarBaseMoneda(BaseMoneda baseMoneda, string Username)
        {
            _baseMonedas = new BaseMonedas();
            return _baseMonedas.InsertarBaseMoneda(baseMoneda, Username);
        }

        /// <summary>
        /// Actualiza un registro de BaseMoneda
        /// </summary>
        /// <param name="id">Id de la BaseMoneda a modificar</param>
        /// <param name="baseMoneda">Objeto BaseMoneda utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarBaseMoneda(int id, BaseMoneda baseMoneda, string Username)
        {
            _baseMonedas = new BaseMonedas();
            return _baseMonedas.ActualizarBaseMoneda(id, baseMoneda, Username);
        }

        /// <summary>
        /// Elimina un registro de BaseMoneda
        /// </summary>
        /// <param name="id">Id de la BaseMoneda a eliminar</param>
        /// <param name="baseMoneda">Objecto BaseMoneda utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarBaseMoneda(int id, BaseMoneda baseMoneda, string Username)
        {
            _baseMonedas = new BaseMonedas();
            return _baseMonedas.EliminarBaseMoneda(id, baseMoneda, Username);
        }

        #endregion

        #region BASE POR PARTICIPANTE
        /// <summary>
        /// Obtiene listado de BasexParticipante
        /// </summary>
        /// <returns>Lista de BasexParticipante</returns>
        public List<BasexParticipante> ListarBasexParticipantes()
        {
            _basexParticipantes = new BasexParticipantes();
            return _basexParticipantes.ListarBasexParticipantes();
        }

        /// <summary>
        /// Obtiene listado de los Participantes
        /// </summary>
        /// <returns>Lista de Participantes</returns>
        public List<Participante> ListarBaseYParticipantes()
        {
            _basexParticipantes = new BasexParticipantes();
            return _basexParticipantes.ListarBaseYParticipantes();
        }

        /// <summary>
        /// Obtiene listado de BasexParticipante por id
        /// </summary>
        /// <param name="id">Id de la BasexParticipante a consultar</param>
        /// <returns>Lista de objectos BasexParticipante</returns>
        public List<BasexParticipante> ListarBasexParticipantesPorId(int id)
        {
            _basexParticipantes = new BasexParticipantes();
            return _basexParticipantes.ListarBasexParticipantesPorId(id);
        }

        /// <summary>
        /// Guarda un registro de BasexParticipante
        /// </summary>
        /// <param name="basexParticipante">Objecto BasexParticipante a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarBasexParticipante(BasexParticipante basexParticipante, string Username)
        {
            _basexParticipantes = new BasexParticipantes();
            return _basexParticipantes.InsertarBasexParticipante(basexParticipante, Username);
        }

        /// <summary>
        /// Actualiza un registro de BasexParticipante
        /// </summary>
        /// <param name="id">Id de BasexParticipante a modificar</param>
        /// <param name="basexParticipante">Objeto BasexParticipante utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarBasexParticipante(int id, BasexParticipante basexParticipante, string Username)
        {
            _basexParticipantes = new BasexParticipantes();
            return _basexParticipantes.ActualizarBasexParticipante(id, basexParticipante, Username);
        }

        /// <summary>
        /// Elimina un registro de BasexParticipante
        /// </summary>
        /// <param name="id">Id de BasexParticipante a eliminar</param>
        /// <param name="basexParticipante">Objecto BasexParticipante utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarBasexParticipante(int id, BasexParticipante basexParticipante, string Username)
        {
            _basexParticipantes = new BasexParticipantes();
            return _basexParticipantes.EliminarBasexParticipante(id, basexParticipante, Username);
        }
        #endregion

        #region CATEGORIA
        /// <summary>
        /// Obtiene listado de las Categorias
        /// </summary>
        /// <returns>Lista de Categorias</returns>
        public List<Categoria> ListarCategorias()
        {
            _categorias = new Categorias();
            return _categorias.ListarCategorias();
        }

        /// <summary>
        /// Obtiene listado de Categorias por id
        /// </summary>
        /// <param name="id">Id de la Categoria a consultar</param>
        /// <returns>Lista de objectos Categoria</returns>
        public List<Categoria> ListarCategoriasPorId(int id)
        {
            _categorias = new Categorias();
            return _categorias.ListarCategoriasPorId(id);
        }

        /// <summary>
        /// Guarda un registro de Categoria
        /// </summary>
        /// <param name="categoria">Objecto Categoria a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarCategoria(Categoria categoria, string Username)
        {
            _categorias = new Categorias();
            return _categorias.InsertarCategoria(categoria, Username);
        }

        /// <summary>
        /// Actualiza un registro de Categoria
        /// </summary>
        /// <param name="id">Id de la Categoria a modificar</param>
        /// <param name="categoria">Objeto Categoria utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarCategoria(int id, Categoria categoria, string Username)
        {
            _categorias = new Categorias();
            return _categorias.ActualizarCategoria(id, categoria, Username);
        }

        /// <summary>
        /// Elimina un registro de Categoria
        /// </summary>
        /// <param name="id">Id de la Categoria a eliminar</param>
        /// <param name="categoria">Objecto Categoria utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarCategoria(int id, Categoria categoria, string Username)
        {
            _categorias = new Categorias();
            return _categorias.EliminarCategoria(id, categoria, Username);
        }
        #endregion

        #region ESCALA NOTA
        /// <summary>
        /// Obtiene listado de EscalaNota
        /// </summary>
        /// <returns>Lista de EscalaNotas</returns>
        public List<EscalaNota> ListarEscalaNotas()
        {
            _escalaNotas = new EscalaNotas();
            return _escalaNotas.ListarEscalaNotas();
        }

        /// <summary>
        /// Obtiene listado de EscalaNota por id
        /// </summary>
        /// <param name="id">Id de la EscalaNota a consultar</param>
        /// <returns>Lista de objectos EscalaNota</returns>
        public List<EscalaNota> ListarEscalaNotasPorId(int id)
        {
            _escalaNotas = new EscalaNotas();
            return _escalaNotas.ListarEscalaNotasPorId(id);
        }

        /// <summary>
        /// Guarda un registro de EscalaNota
        /// </summary>
        /// <param name="escalaNota">Objecto EscalaNota a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarEscalaNota(EscalaNota escalaNota, string Username)
        {
            _escalaNotas = new EscalaNotas();
            return _escalaNotas.InsertarEscalaNota(escalaNota, Username);
        }

        /// <summary>
        /// Actualiza un registro de EscalaNota
        /// </summary>
        /// <param name="id">Id de la EscalaNota a modificar</param>
        /// <param name="escalaNota">Objeto EscalaNota utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarEscalaNota(int id, EscalaNota escalaNota, string Username)
        {
            _escalaNotas = new EscalaNotas();
            return _escalaNotas.ActualizarEscalaNota(id, escalaNota, Username);
        }

        /// <summary>
        /// Elimina un registro de EscalaNota
        /// </summary>
        /// <param name="id">Id de la EscalaNota a eliminar</param>
        /// <param name="escalaNota">Objecto EscalaNota utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarEscalaNota(int id, EscalaNota escalaNota, string Username)
        {
            _escalaNotas = new EscalaNotas();
            return _escalaNotas.EliminarEscalaNota(id, escalaNota, Username);
        }
        #endregion

        #region FACTOR VARIABLE
        /// <summary>
        /// Obtiene listado de las FactorVariable
        /// </summary>
        /// <returns>Lista de FactorVariable</returns>
        public List<FactorVariable> ListarFactorVariables()
        {
            _factorVariables = new FactorVariables();
            return _factorVariables.ListarFactorVariables();
        }

        /// <summary>
        /// Obtiene listado de FactorVariable por id
        /// </summary>
        /// <param name="id">Id del FactorVariable a consultar</param>
        /// <returns>Lista de objectos FactorVariable</returns>
        public List<FactorVariable> ListarFactorVariablesPorId(int id)
        {
            _factorVariables = new FactorVariables();
            return _factorVariables.ListarFactorVariablesPorId(id);
        }

        /// <summary>
        /// Guarda un registro del FactorVariable
        /// </summary>
        /// <param name="factorVariable">Objecto FactorVariable a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarFactorVariable(FactorVariable factorVariable, string Username)
        {
            _factorVariables = new FactorVariables();
            return _factorVariables.InsertarFactorVariable(factorVariable, Username);
        }

        /// <summary>
        /// Actualiza un registro del FactorVariable
        /// </summary>
        /// <param name="id">Id del FactorVariable a modificar</param>
        /// <param name="factorVariable">Objeto FactorVariable utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarFactorVariable(int id, FactorVariable factorVariable, string Username)
        {
            _factorVariables = new FactorVariables();
            return _factorVariables.ActualizarFactorVariable(id, factorVariable, Username);
        }

        /// <summary>
        /// Elimina un registro de FactorVariable
        /// </summary>
        /// <param name="id">Id del FactorVariable a eliminar</param>
        /// <param name="factorVariable">Objecto FactorVariable utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarFactorVariable(int id, FactorVariable factorVariable, string Username)
        {
            _factorVariables = new FactorVariables();
            return _factorVariables.EliminarFactorVariable(id, factorVariable, Username);
        }

        //public List<Variable> ListarVariablesPortabla(int idtabla)
        //{
        //    _reglas = new Reglas();
        //    return _reglas.ListarVariablesPortabla(idtabla);
        //}

        #endregion

        #region FACTOR POR NOTAS
        /// <summary>
        /// Obtiene listado de FactorxNotas
        /// </summary>
        /// <returns>Lista de Categorias</returns>
        public List<FactorxNota> ListarFactorxNotas()
        {
            _factorxNotas = new FactorxNotas();
            return _factorxNotas.ListarFactorxNotas();
        }

        /// <summary>
        /// Obtiene listado de FactorxNotas por id
        /// </summary>
        /// <param name="id">Id de la FactorxNota a consultar</param>
        /// <returns>Lista de objectos FactorxNota</returns>
        public List<FactorxNota> ListarFactorxNotasPorId(int id)
        {
            _factorxNotas = new FactorxNotas();
            return _factorxNotas.ListarFactorxNotasPorId(id);
        }

        public List<PeriodoFactorxNota> ListarPeriodoFactorxNotasPorFactor(int id)
        {
            _factorxNotas = new FactorxNotas();
            return _factorxNotas.ListarPeriodoFactorxNotasPorFactor(id);
        }

        public List<FactorxNotaDetalle> ListarFactorxNotaDetallesPorPeriodo(int id)
        {
            _factorxNotas = new FactorxNotas();
            return _factorxNotas.ListarFactorxNotaDetallesPorPeriodo(id);
        }

        /// <summary>
        /// Guarda un registro de FactorxNota
        /// </summary>
        /// <param name="factorxNota">Objecto FactorxNota a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarFactorxNota(FactorxNota factorxNota, string Username)
        {
            _factorxNotas = new FactorxNotas();
            return _factorxNotas.InsertarFactorxNota(factorxNota, Username);
        }

        public int InsertarPeriodoFactorxNota(PeriodoFactorxNota periodo, string Username)
        {
            _factorxNotas = new FactorxNotas();
            return _factorxNotas.InsertarPeriodoFactorxNota(periodo, Username);
        }

        public int InsertarFactorxNotaDetalle(FactorxNotaDetalle detalle, string Username)
        {
            _factorxNotas = new FactorxNotas();
            return _factorxNotas.InsertarFactorxNotaDetalle(detalle, Username);
        }

        /// <summary>
        /// Actualiza un registro de FactorxNota
        /// </summary>
        /// <param name="id">Id del FactorxNota a modificar</param>
        /// <param name="factorxNota">Objeto FactorxNota utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarFactorxNota(int id, FactorxNota factorxNota, string Username)
        {
            _factorxNotas = new FactorxNotas();
            return _factorxNotas.ActualizarFactorxNota(id, factorxNota, Username);
        }

        /// <summary>
        /// Elimina un registro de FactorxNota
        /// </summary>
        /// <param name="id">Id del FactorxNota a eliminar</param>
        /// <param name="factorxNota">Objecto FactorxNota utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarFactorxNota(int id, FactorxNota factorxNota, string Username)
        {
            _factorxNotas = new FactorxNotas();
            return _factorxNotas.EliminarFactorxNota(id, factorxNota, Username);
        }

        public string EliminarPeriodoFactorxNota(int id, PeriodoFactorxNota periodo, string Username)
        {
            _factorxNotas = new FactorxNotas();
            return _factorxNotas.EliminarPeriodoFactorxNota(id, periodo, Username);
        }

        public string EliminarFactorxNotaDetalle(int id, FactorxNotaDetalle detalle, string Username)
        {
            _factorxNotas = new FactorxNotas();
            return _factorxNotas.EliminarFactorxNotaDetalle(id, detalle, Username);
        }
        #endregion

        #region GRUPO ENDOSO
        /// <summary>
        /// Obtiene listado de GrupoEndoso
        /// </summary>
        /// <returns>Lista de GrupoEndoso</returns>
        public List<GrupoEndoso> ListarGrupoEndosos()
        {
            _grupoEndosos = new GrupoEndosos();
            return _grupoEndosos.ListarGrupoEndosos();
        }

        /// <summary>
        /// Obtiene listado de GrupoEndoso por id
        /// </summary>
        /// <param name="id">Id del GrupoEndoso a consultar</param>
        /// <returns>Lista de objectos GrupoEndoso</returns>
        public List<GrupoEndoso> ListarGrupoEndososPorId(int id)
        {
            _grupoEndosos = new GrupoEndosos();
            return _grupoEndosos.ListarGrupoEndososPorId(id);
        }

        /// <summary>
        /// Guarda un registro de GrupoEndoso
        /// </summary>
        /// <param name="grupoEndoso">Objecto GrupoEndoso a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarGrupoEndoso(GrupoEndoso grupoEndoso, string Username)
        {
            _grupoEndosos = new GrupoEndosos();
            return _grupoEndosos.InsertarGrupoEndoso(grupoEndoso, Username);
        }

        /// <summary>
        /// Actualiza un registro de GrupoEndoso
        /// </summary>
        /// <param name="id">Id del GrupoEndoso a modificar</param>
        /// <param name="grupoEndoso">Objeto GrupoEndoso utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarGrupoEndoso(int id, GrupoEndoso grupoEndoso, string Username)
        {
            _grupoEndosos = new GrupoEndosos();
            return _grupoEndosos.ActualizarGrupoEndoso(id, grupoEndoso, Username);
        }

        /// <summary>
        /// Elimina un registro de GrupoEndoso
        /// </summary>
        /// <param name="id">Id del GrupoEndoso a eliminar</param>
        /// <param name="grupoEndoso">Objecto GrupoEndoso utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarGrupoEndoso(int id, GrupoEndoso grupoEndoso, string Username)
        {
            _grupoEndosos = new GrupoEndosos();
            return _grupoEndosos.EliminarGrupoEndoso(id, grupoEndoso, Username);
        }
        #endregion

        #region Metodos SiteMap
        public List<Entidades.SiteMap> ListarSiteMap()
        {
            SAI_Entities tabla = new SAI_Entities();

            return tabla.SiteMaps.ToList();


            _sitemapes = new SiteMapes();
            return _sitemapes.ListarSiteMap();
        }

        public int InsertarSiteMap(Entidades.SiteMap sitemap, string Username)
        {
            _sitemapes = new SiteMapes();
            return _sitemapes.InsertarSiteMap(sitemap, Username);
        }

        public List<Entidades.SiteMap> ListarSiteMapPorId(string idSiteMap)
        {
            _sitemapes = new SiteMapes();
            return _sitemapes.ListarSiteMapPorId(idSiteMap);
        }

        public int ActualizarsiteMap(string id, Entidades.SiteMap sitemap, string Username)
        {
            _sitemapes = new SiteMapes();
            return _sitemapes.ActualizarsiteMap(id, sitemap, Username);
        }

        public int Eliminarsitemap(string id, Entidades.SiteMap sitemap, string Username)
        {
            _sitemapes = new SiteMapes();
            return _sitemapes.Eliminarsitemap(id, sitemap, Username);
        }

        public List<aspnet_Roles> ListarRoles()
        {
            _sitemapes = new SiteMapes();
            return _sitemapes.ListarRoles();
        }

        public aspnet_Roles GetRolById(System.Guid id)
        {
            _sitemapes = new SiteMapes();
            return _sitemapes.GetRolById(id);
        }

        #endregion

        #region PARTICIPANTE
        /// <summary>
        /// Obtiene listado de los Participantes
        /// </summary>
        /// <returns>Lista de Participantes</returns>
        public List<Participante> ListarParticipantes(int? nivelGP)
        {
            _participantes = new Participantes();
            return _participantes.ListarParticipantes(nivelGP);
        }

        public List<TipoDocumento> ListarTipodocumentoes()
        {
            _participantes = new Participantes();
            return _participantes.ListarTipodocumentoes();
        }

        public List<TipoParticipante> ListarTipoparticipantes()
        {
            _participantes = new Participantes();
            return _participantes.ListarTipoparticipantes();
        }

        public List<EstadoParticipante> ListarEstadoparticipantes()
        {
            _participantes = new Participantes();
            return _participantes.ListarEstadoparticipantes();
        }
        /// <summary>
        /// Obtiene listado de Participantes por id
        /// </summary>
        /// <param name="idRed">Id des Participante a consultar</param>
        /// <returns>Lista de objetos Participante</returns>
        /// 
        public List<Participante> ListarParticipantesIndex(int inicio, int cantidad, int zona_id)
        {
            _participantes = new Participantes();
            return _participantes.ListarParticipantesIndex(inicio, cantidad, zona_id);
        }

        public List<Participante> ListarParticipantesBuscador(string texto, int inicio, int cantidad, int nivel, int zona_id)
        {
            _participantes = new Participantes();
            return _participantes.ListarParticipantesBuscador(texto, inicio, cantidad, nivel, zona_id);
        }

        public List<JerarquiaDetalle> ListarJerarquiaIndex(int inicio, int cantidad, int zona_id)
        {
            _participantes = new Participantes();
            return _participantes.ListarJerarquiaIndex(inicio, cantidad, zona_id);
        }

        public List<JerarquiaDetalle> ListarJerarquiaBuscador(string texto, int inicio, int cantidad, int nivel, int zona_id)
        {
            _participantes = new Participantes();
            return _participantes.ListarJerarquiaBuscador(texto, inicio, cantidad, nivel, zona_id);
        }

        public List<Participante> ListarParticipantesPorId(int id)
        {
            _participantes = new Participantes();
            return _participantes.ListarParticipantesPorId(id);
        }

        public List<Participante> ListarParticipanteXCedula(string cedula)
        {
            return new Participantes().ListarParticipanteXCedula(cedula);
        }
        /// <summary>
        /// Guarda un registro de Participante
        /// </summary>
        /// <param name="zona">Objeto Participante a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarParticipante(Participante participante, string userName)
        {
            _participantes = new Participantes();
            return _participantes.InsertarParticipante(participante, userName);
        }

        /// <summary>
        /// Actualiza un registro de Participante
        /// </summary>
        /// <param name="id">Id del Participante a modificar</param>
        /// <param name="zona">Objeto Participante utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarParticipante(int id, Participante participante, string userName)
        {
            _participantes = new Participantes();
            return _participantes.ActualizarParticipante(id, participante, userName);
        }

        /// <summary>
        /// Elimina un registro de Participante
        /// </summary>
        /// <param name="id">Id del Participante a eliminar</param>
        /// <param name="zona">Objecto Participante utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarParticipante(int id, Participante participante, string userName)
        {
            _participantes = new Participantes();
            return _participantes.EliminarParticipante(id, participante, userName);
        }
        #endregion

        #region TIPO DE ESCALAS
        /// <summary>
        /// Obtiene listado de los tipos de escalas
        /// </summary>
        /// <returns>Lista de TipoEscala</returns>
        public List<TipoEscala> ListarTipoEscalas()
        {
            _tipoescalas = new TipoEscalas();
            return _tipoescalas.ListarTipoEscalas();
        }

        /// <summary>
        /// Obtiene listado de los tipos de escalas por id
        /// </summary>
        /// <param name="id">Id del TipoEscala a consultar</param>
        /// <returns>Lista de objetos TipoEscala</returns>
        public List<TipoEscala> ListarTipoEscalasPorId(int id)
        {
            _tipoescalas = new TipoEscalas();
            return _tipoescalas.ListarTipoEscalasPorId(id);
        }

        /// <summary>
        /// Guarda un registro de TipoEscala
        /// </summary>
        /// <param name="tipoEscala">Objeto TipoEscala a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarTipoEscala(TipoEscala tipoEscala, string Username)
        {
            _tipoescalas = new TipoEscalas();
            return _tipoescalas.InsertarTipoEscala(tipoEscala, Username);
        }

        /// <summary>
        /// Actualiza un registro de TipoEscala
        /// </summary>
        /// <param name="id">Id del TipoEscala a modificar</param>
        /// <param name="tipoEscala">Objeto TipoEscala utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarTipoEscala(int id, TipoEscala tipoEscala, string Username)
        {
            _tipoescalas = new TipoEscalas();
            return _tipoescalas.ActualizarTipoEscala(id, tipoEscala, Username);
        }

        /// <summary>
        /// Elimina un registro de TipoEscala
        /// </summary>
        /// <param name="id">Id del TipoEscala a eliminar</param>
        /// <param name="tipoEscala">Objeto TipoEscala utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarTipoEscala(int id, TipoEscala tipoEscala, string Username)
        {
            _tipoescalas = new TipoEscalas();
            return _tipoescalas.EliminarTipoEscala(id, tipoEscala, Username);
        }
        #endregion

        #region Metodos METAS

        public List<Meta> ListarMetas()
        {
            _metas = new Metas();
            return _metas.ListarMetas();
        }

        public List<Meta> ListarMetasPorId(int idMeta)
        {
            _metas = new Metas();
            return _metas.ListarMetasPorId(idMeta);
        }

        public List<Meta> ListarMetasMensuales(int idMeta)
        {
            _metas = new Metas();
            return _metas.ListarMetasMensuales(idMeta);
        }

        public int InsertarMeta(Meta meta, string Username)
        {
            _metas = new Metas();
            return _metas.InsertarMeta(meta, Username);
        }

        public int ActualizarMeta(int id, Meta meta, string Username)
        {
            _metas = new Metas();
            return _metas.ActualizarMeta(id, meta, Username);
        }

        public int ActualizarMetaAcumulada(int id, Meta meta, string Username)
        {
            _metas = new Metas();
            return _metas.ActualizarMetaAcumulada(id, meta, Username);
        }

        public int EliminarMeta(int idMeta, string Username)
        {
            _metas = new Metas();
            return _metas.EliminarMeta(idMeta, Username);
        }

        public List<ProductosMeta> ListarProductosMetaPorId(int id)
        {
            _metas = new Metas();
            return _metas.ListarProductosMetaPorId(id);
        }

        public int InsertarProductoMeta(ProductosMeta productometa, string Username)
        {
            _metas = new Metas();
            return _metas.InsertarProductoMeta(productometa, Username);
        }

        public int ActualizarProductoMeta(int id, ProductosMeta productometa, string Username)
        {
            _metas = new Metas();
            return _metas.ActualizarProductoMeta(id, productometa, Username);
        }

        public int EliminarProductoMeta(int idMeta, string Username)
        {
            _metas = new Metas();
            return _metas.EliminarProductoMeta(idMeta, Username);
        }

        public List<MetaCompuesta> ListarMetaCompuestaPorId(int id)
        {
            _metas = new Metas();
            return _metas.ListarMetaCompuestaPorId(id);
        }

        public int InsertarMetaCompuesta(int idMetaDestino, int idMetaOrigen, string Username)
        {
            _metas = new Metas();
            return _metas.InsertarMetaCompuesta(idMetaDestino, idMetaOrigen, Username);
        }

        public int EliminarMetaCompuesta(int idMeta, string Username)
        {
            _metas = new Metas();
            return _metas.EliminarMetaCompuesta(idMeta, Username);
        }

        //EL REQUERIMIENTO SE DIJO QUE NO SE IBA HACER. 
        //SE DEJA COMENTADO POR SI HAY LA POSIBILIDAD QUE EL NEGOCIO LO APRUEBE NUEVAMENTE
        /*
        public List<MetaValidacionCumplimiento> ListarMetaValidacionCumplimientoPorId(int id)
        {
            _metas = new Metas();
            return _metas.ListarMetaValidacionCumplimientoPorId(id);
        }

        public int InsertarMetaValidacionCumplimiento(int idMetaValidacion, int idMetaReponderacion)
        {
            _metas = new Metas();
            return _metas.InsertarMetaValidacionCumplimiento(idMetaValidacion, idMetaReponderacion);
        }

        public int EliminarMetaValidacionCumplimiento(int idMetaValidacion)
        {
            _metas = new Metas();
            return _metas.EliminarMetaValidacionCumplimiento(idMetaValidacion);
        } 
        */

        #endregion

        #region Metodos PRODUCTOS

        public List<Producto> ListarProductoes()
        {
            _productos = new ColpatriaSAI.Negocio.Componentes.Productos.Productos();
            return _productos.ListarProductoes();
        }

        public List<Producto> ListarProductoesPorId(int idProducto)
        {
            return new Productos.Productos().ListarProductoesPorId(idProducto);
        }

        public List<Producto> ListarProductosporRamo(int idRamo)
        {
            _productos = new ColpatriaSAI.Negocio.Componentes.Productos.Productos();
            return _productos.ListarProductosporRamo(idRamo);
        }

        public List<ProductoDetalle> ListarProductoDetalles(int id)
        {
            return new Productos.Productos().ListarProductoDetalles(id);
        }

        public int AgruparProductoDetalle(int producto_id, string productosTrue, string productosFalse)
        {
            return new Productos.Productos().AgruparProductoDetalle(producto_id, productosTrue, productosFalse);
        }

        public int InsertarProducto(Producto producto, string Username)
        {
            return new Productos.Productos().InsertarProducto(producto, Username);
        }

        public string EliminarProducto(int id, string Username)
        {
            return new Productos.Productos().EliminarProducto(id, Username);
        }

        public List<ProductoDetalle> ListarProductoDetalleXRamoDetalle(int ramoDetalleId)
        {
            return new Productos.Productos().ListarProductoDetalleXRamoDetalle(ramoDetalleId);
        }
        #endregion

        #region Metodos TIPOMETA

        public List<TipoMeta> ListarTipometas()
        {
            _tipometas = new TipoMetas();
            return _tipometas.ListarTipometas();
        }
        #endregion

        #region Metodos PLANES

        public List<Plan> ListarPlans()
        {
            _plans = new Planes();
            return _plans.ListarPlans();
        }

        public List<Plan> ListarPlansPorId(int idPlan)
        {
            _plans = new Planes();
            return _plans.ListarPlansPorId(idPlan);
        }

        public List<Plan> ListarPlansPorProducto(int productoid)
        {
            _plans = new Planes();
            return _plans.ListarPlansPorProducto(productoid);
        }

        public List<Plan> ListarPlanPorProducto(int idProducto)
        {
            _plans = new Planes();
            return _plans.ListarPlanPorProducto(idProducto);
        }

        public int InsertarPlan(Plan plan, string Username)
        {
            return new Planes().InsertarPlan(plan, Username);
        }

        public string EliminarPlan(int id, string Username)
        {
            return new Planes().EliminarPlan(id, Username);
        }

        public List<PlanDetalle> ListarPlanDetalles(int id)
        {
            return new Planes().ListarPlanDetalles(id);
        }

        public int AgruparPlanDetalle(int plan_id, string planesTrue, string planesFalse)
        {
            return new Planes().AgruparPlanDetalle(plan_id, planesTrue, planesFalse);
        }

        public List<PlanDetalle> ListarPlanDetalleXProductoDetalle(int productoDetalleId)
        {
            return new Planes().ListarPlanDetalleXProductoDetalle(productoDetalleId);
        }

        public List<PlanDetalle> ListarPlanDetalleActivosXProductoDetalle(int productoDetalleId)
        {
            return new Planes().ListarPlanDetalleActivosXProductoDetalle(productoDetalleId);
        }
        #endregion

        #region Metodos PLAZO

        public List<Plazo> ListarPlazoes()
        {
            _plazos = new Plazos();
            return _plazos.ListarPlazoes();
        }
        #endregion

        #region Metodos MODALIDADPAGO

        public List<ModalidadPago> ListarModalidadPagoes()
        {
            _modalidadpagos = new ModalidadPagos();
            return _modalidadpagos.ListarModalidadPagoes();
        }
        #endregion

        #region MODELOS
        /// <summary>
        /// Obtiene listado de los Modelos
        /// </summary>
        /// <returns>Lista de Modelos</returns>
        public List<Modelo> ListarModelos()
        {
            _modelos = new Modelos();
            return _modelos.ListarModelos();
        }

        /// <summary>
        /// Obtiene listado de los Modelos por id
        /// </summary>
        /// <param name="id">Id del Nivel a consultar</param>
        /// <returns>Lista de objectos Modelos</returns>
        public List<Modelo> ListarModelosPorId(int id)
        {
            _modelos = new Modelos();
            return _modelos.ListarModelosPorId(id);
        }

        public List<ModeloxMeta> ListarModelosxMetaPorIdModelo(int id)
        {
            _modelos = new Modelos();
            return _modelos.ListarModelosxMetaPorIdModelo(id);
        }

        /// <summary>
        /// Obtiene listado de los participantes que pertenecen un determinado modelo
        /// </summary>
        /// <param name="id">Id del modelo</param>
        /// <returns>Lista de objectos ModeloxParticipante</returns>
        public List<ModeloxNodo> ListarModeloxParticipantesPorId(int id)
        {
            _modelos = new Modelos();
            return _modelos.ListarModeloxParticipantesPorId(id);
        }

        /// <summary>
        /// Guarda un registro de Modelo
        /// </summary>
        /// <param name="modelo">Objecto Modelo a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarModelo(Modelo modelo, string Username)
        {
            _modelos = new Modelos();
            return _modelos.InsertarModelo(modelo, Username);
        }

        public string AsociarFactorToModelo(int modelo, int factor)
        {
            _modelos = new Modelos();
            return _modelos.AsociarFactorToModelo(modelo, factor);
        }

        /// <summary>
        /// Guarda un registro de ModeloxMeta
        /// </summary>
        /// <param name="modeloxMeta">Objecto ModeloxMeta a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarModeloxMeta(ModeloxMeta modeloxMeta, string Username)
        {
            _modelos = new Modelos();
            return _modelos.InsertarModeloxMeta(modeloxMeta, Username);
        }

        public int CargarModeloxMeta(List<ModelosContratacion> modeloxMeta, int esUltimaHoja)
        {
            return new Modelos().CargarModeloxMeta(modeloxMeta, esUltimaHoja);
        }

        public int InsertarModeloxParticipante(ModeloxNodo modeloxPart, string Username)
        {
            _modelos = new Modelos();
            return _modelos.InsertarModeloxParticipante(modeloxPart, Username);
        }

        /// <summary>
        /// Actualiza un registro de Modelo
        /// </summary>
        /// <param name="id">Id del Modelo a modificar</param>
        /// <param name="modelo">Objeto Modelo utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarModelo(int id, Modelo modelo, string Username)
        {
            _modelos = new Modelos();
            return _modelos.ActualizarModelo(id, modelo, Username);
        }

        public int ActualizarModeloxMeta(int id, ModeloxMeta modeloxMeta, string Username)
        {
            _modelos = new Modelos();
            return _modelos.ActualizarModeloxMeta(id, modeloxMeta, Username);
        }

        /// <summary>
        /// Elimina un registro de Modelo
        /// </summary>
        /// <param name="id">Id del Modelo a eliminar</param>
        /// <param name="nivel">Objecto Modelo utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarModelo(int id, Modelo modelo, string Username)
        {
            _modelos = new Modelos();
            return _modelos.EliminarModelo(id, modelo, Username);
        }

        public int EliminarModeloxMetaPorModelos(List<int> codigosModelo, string Username)
        {
            return new Modelos().EliminarModeloxMetaPorModelos(codigosModelo, Username);
        }

        public string EliminarModeloxMeta(int id, ModeloxMeta modeloxMeta, string Username)
        {
            _modelos = new Modelos();
            return _modelos.EliminarModeloxMeta(id, modeloxMeta, Username);
        }

        public string EliminarModeloxParticipante(int id, ModeloxNodo modeloxPart, string Username)
        {
            _modelos = new Modelos();
            return _modelos.EliminarModeloxParticipante(id, modeloxPart, Username);
        }

        #endregion

        #region TIPO DE ENDOSOS
        /// <summary>
        /// Obtiene listado de las redes
        /// </summary>
        /// <returns>Lista de redes</returns>
        public List<TipoEndoso> ListarTipoEndoso()
        {
            _tipoEndosos = new TipoEndosos();
            return _tipoEndosos.ListarTipoEndoso();
        }

        /// <summary>
        /// Obtiene listado de los tipos de endoso por id
        /// </summary>
        /// <param name="id">Id de la red a consultar</param>
        /// <returns>Lista de objectos TipoEndoso</returns>
        public List<TipoEndoso> ListarTipoEndososPorId(int id)
        {
            _tipoEndosos = new TipoEndosos();
            return _tipoEndosos.ListarTipoEndososPorId(id);
        }

        /// <summary>
        /// Guarda un registro de TipoEndoso
        /// </summary>
        /// <param name="tipoEndoso">Objecto TipoEndoso a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarTipoEndoso(TipoEndoso tipoEndoso, string Username)
        {
            _tipoEndosos = new TipoEndosos();
            return _tipoEndosos.InsertarTipoEndoso(tipoEndoso, Username);
        }

        /// <summary>
        /// Actualiza un registro de TipoEndoso
        /// </summary>
        /// <param name="id">Id del TipoEndoso a modificar</param>
        /// <param name="tipoEndoso">Objeto TipoEndoso utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarTipoEndoso(int id, TipoEndoso tipoEndoso, string Username)
        {
            _tipoEndosos = new TipoEndosos();
            return _tipoEndosos.ActualizarTipoEndoso(id, tipoEndoso, Username);
        }

        /// <summary>
        /// Elimina un registro de TipoEndoso
        /// </summary>
        /// <param name="id">Id del TipoEndoso a eliminar</param>
        /// <param name="tipoEndoso">Objecto TipoEndoso utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarTipoEndoso(int id, TipoEndoso tipoEndoso, string Username)
        {
            _tipoEndosos = new TipoEndosos();
            return _tipoEndosos.EliminarTipoEndoso(id, tipoEndoso, Username);
        }
        #endregion

        #region LOGINTEGRACION

        public List<LogIntegracionwsIntegrador> ListarLogIntWsIns()
        {
            _integracions = new Negocio.Componentes.Admin.Integracion();
            return _integracions.ListarLogIntWsIns();
        }

        public List<LogIntegracionwsIntegrador> ListarLogIntWsInsPorId(int id)
        {
            _integracions = new Negocio.Componentes.Admin.Integracion();
            return _integracions.ListarLogIntWsInsPorId(id);
        }

        public int ActualizarLogIntWsIns(int id, LogIntegracionwsIntegrador logintegracionwsintegrador, string Username)
        {
            _integracions = new Negocio.Componentes.Admin.Integracion();
            return _integracions.ActualizarLogIntWsIns(id, logintegracionwsintegrador, Username);
        }

        public List<LogIntegracion> ListarLogIntegracion()
        {
            _integracions = new Negocio.Componentes.Admin.Integracion();
            return _integracions.ListarLogIntegracion();
        }
        #endregion

        #region  GRUPO DE ENDOSO Y TIPO DE ENDOSOS (ESTADOS)

        public List<ExcepcionesxGrupoTipoEndoso> ListarExcepcionesxGrupoTipoEndoso()
        {
            _grupoTipoEndosos = new GrupoTipoEndosos();
            return _grupoTipoEndosos.ListarExcepcionesxGrupoTipoEndoso();
        }

        public List<ExcepcionesxGrupoTipoEndoso> ListarExcepcionesxGrupoTipoEndosoPorId(int id)
        {
            _grupoTipoEndosos = new GrupoTipoEndosos();
            return _grupoTipoEndosos.ListarExcepcionesxGrupoTipoEndosoPorId(id);
        }

        public int InsertarExcepcionesxGrupoTipoEndoso(ExcepcionesxGrupoTipoEndoso excepcionesporGrupoTipoEndoso, string Username)
        {
            _grupoTipoEndosos = new GrupoTipoEndosos();
            return _grupoTipoEndosos.InsertarExcepcionesxGrupoTipoEndoso(excepcionesporGrupoTipoEndoso, Username);
        }

        public int ActualizarExcepcionesxGrupoTipoEndoso(int id, ExcepcionesxGrupoTipoEndoso excepcionesporGrupoTipoEndoso, string Username)
        {
            _grupoTipoEndosos = new GrupoTipoEndosos();
            return _grupoTipoEndosos.ActualizarExcepcionesxGrupoTipoEndoso(id, excepcionesporGrupoTipoEndoso, Username);
        }

        public string EliminarExcepcionesxGrupoTipoEndoso(int id, ExcepcionesxGrupoTipoEndoso excepcionesporGrupoTipoEndoso, string Username)
        {
            _grupoTipoEndosos = new GrupoTipoEndosos();
            return _grupoTipoEndosos.EliminarExcepcionesxGrupoTipoEndoso(id, excepcionesporGrupoTipoEndoso, Username);
        }
        #endregion

        #region Franquicias

        public List<Localidad> ListarFranquicias()
        {
            _franquicias = new Franquicias();
            return _franquicias.ListarFranquicias();
        }

        public int InsertarFranquicia(Localidad franquicia)
        {
            throw new NotImplementedException();
        }

        public List<Localidad> ListarFranquiciaPorId(int idFranquicia)
        {
            _franquicias = new Franquicias();
            return _franquicias.ListarFranquiciaPorId(idFranquicia);
        }

        public int ActualizarFranquicia(int id, Localidad franquicia)
        {
            throw new NotImplementedException();
        }

        public int EliminarFranquicia(int id, Localidad franquicia)
        {
            throw new NotImplementedException();
        }
        public ColpatriaSAI.Negocio.Entidades.ParticipacionFranquicia DetalleFranquicia(int idFranquicia)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.DetalleFranquicia(idFranquicia);

        }
        public List<ColpatriaSAI.Negocio.Entidades.DetallePartFranquicia> DetalleFranquicias(int idFranquicia)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.DetalleFranquicias(idFranquicia);
        }
        public List<ColpatriaSAI.Negocio.Entidades.DetallePartFranquicia> DetalleFranquiciaPorPartFranqId(int idPartFranquicia)
        {

            _detFranquicias = new Franquicias();
            return _detFranquicias.DetalleFranquiciaPorPartFranqId(idPartFranquicia);
        }

        public List<Entidades.ParticipacionFranquicia> ListarPartFranquiciasPorlocalidad(int idlocalidad)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.ListarPartFranquiciasPorlocalidad(idlocalidad);
        }
        public List<Entidades.ParticipacionFranquicia> ListarPartFranquicias()
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.ListarPartFranquicias();
        }

        public bool EliminarPartFranquicia(int idPartFranquicia, string Username)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.EliminarPartFranquicia(idPartFranquicia, Username);
        }

        public bool EliminarDetallePartFranquicia(int idPartFranquicia, string Username)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.EliminarDetallePartFranquicia(idPartFranquicia, Username);
        }

        public ParticipacionFranquicia InsertarPartFranquicia(Entidades.ParticipacionFranquicia partfranquicia, string Username)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.InsertarPartFranquicia(partfranquicia, Username);
        }

        public bool InsertarDetallePartFranquicia(DetallePartFranquicia detalleParticipacion, string Username)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.InsertarDetallePartFranquicia(detalleParticipacion, Username);
        }

        public int ActualizarDetallePartFranquicia(DetallePartFranquicia participacionFranquicia, string Username)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.ActualizarDetallePartFranquicia(participacionFranquicia, Username);
        }

        public int ActualizarParticipacionFranquicia(ParticipacionFranquicia participacionFranquicia, string Username)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.ActualizarParticipacionFranquicia(participacionFranquicia, Username);
        }

        public int ActualizarPartFranquicia(ParticipacionFranquicia participacionFranquicia, string Username)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.ActualizarPartFranquicia(participacionFranquicia, Username);
        }

        public List<Entidades.DetallePartFranquicia> ListDetalleFranquiciaPorId(int idDetPartFranquicia)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.ListDetalleFranquiciaPorId(idDetPartFranquicia);
        }

        public DetallePartFranquicia DetalleFranquiciaporId(int id)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.DetalleFranquiciaporId(id);
        }

        public int EliminarDetallePartFranquiciaPorId(int id, string Username)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.EliminarDetallePartFranquiciaPorId(id, Username);
        }

        public Entidades.DetallePartFranquicia DetalleFranquiciaPorIdFryIdDetFr(int idFranquicia, int iddetpartfra)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.DetalleFranquiciaPorIdFryIdDetFr(idFranquicia, iddetpartfra);
        }

        public int ActualizarDetallePartFranquicias(DetallePartFranquicia DetallePartFranquicia, DateTime fechaIni, DateTime fechaFin, string franquicias, string Username)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.ActualizarDetallePartFranquicias(DetallePartFranquicia, fechaIni, fechaFin, franquicias, Username);
        }

        public List<DetallePartFranquicia> GetDetallePartFranquiciasActualizar(ParticipacionFranquicia partfranquicia, string franquicias)
        {
            _detFranquicias = new Franquicias();
            return _detFranquicias.GetDetallePartFranquiciasActualizar(partfranquicia, franquicias);
        }

        public int CopiarParticipacionFranquicia(int origen, int destino, string Username)
        {
            return new Franquicias().CopiarParticipacionFranquicia(origen, destino, Username);
        }

        public string obtenerSalarioMinimo()
        {
            return new Franquicias().obtenerSalarioMinimo();
        }


        public void reportePagosFranquicia(int idLiquidacion)
        {
            new Franquicias().reportePagosFranquicia(idLiquidacion); 
        }

        #endregion

        #region  Excepciones
        public List<Excepcion> ListarExcepciones(int idfranquicia)
        {
            var _excepciones = new Excepciones();

            return _excepciones.ListarExcepciones(idfranquicia);

        }

        public List<Excepcion> ListarExcepcionesEspeciales()
        {
            var _excepciones = new Excepciones();

            return _excepciones.ListarExcepcionesEspeciales();

        }

        public int InsertarException(Entidades.Excepcion excepcion, string Username)
        {
            var _excepciones = new Excepciones();
            return _excepciones.InsertarException(excepcion, Username);
        }


        public int EliminarException(int idexcepcion, string Username)
        {
            var _excepciones = new Excepciones();
            return _excepciones.EliminarException(idexcepcion, Username);
        }
        public List<Entidades.Excepcion> ListarExcepcionesporId(int idException)
        {
            var _excepciones = new Excepciones();

            return _excepciones.ListarExcepcionesporId(idException);
        }

        public int ActualizarExcepcion(Entidades.Excepcion exception, string Username)
        {
            var _excepciones = new Excepciones();
            return _excepciones.ActualizarExcepcion(exception, Username);
        }

        #endregion

        #region  Negocio
        public List<Entidades.Negocio> ListarNegocios()
        {
            var _negocio = new Negocio.Componentes.Produccion.Negocio();
            return _negocio.ListarNegociosConCompanias();
        }

        #endregion

        #region liq franquicia
        public List<ColpatriaSAI.Negocio.Entidades.LiquidacionFranquicia> ListarLiquidacionFranquicias()
        {
            var _liquidacionfranq = new Negocio.Componentes.LiqFranquicias.LiquidacionFranquicias();


            return _liquidacionfranq.ListarLiquidacionFranquicias();
        }

        public ColpatriaSAI.Negocio.Entidades.LiquidacionFranquicia TraerLiquidacionFranquicia(int idLiquidacionFranquicia)
        {
            var _liquidacionfranq = new Negocio.Componentes.LiqFranquicias.LiquidacionFranquicias();
            return _liquidacionfranq.TraerLiquidacionFranquicia(idLiquidacionFranquicia);
        }

        //NUEVOS METODOS

        public int InsertarLiquidacionFranquicia(ColpatriaSAI.Negocio.Entidades.LiquidacionFranquicia liquidacionFranquicia, string Username)
        {
            var _liquidacionfranq = new Negocio.Componentes.LiqFranquicias.LiquidacionFranquicias();
            return _liquidacionfranq.InsertarLiquidacionFranquicia(liquidacionFranquicia, Username);
        }

        public int LiquidarFranquiciasDetalleTotal(ColpatriaSAI.Negocio.Entidades.LiquidacionFranquicia liquidacionFranquicia)
        {
            var _liquidacionfranq = new Negocio.Componentes.LiqFranquicias.LiquidacionFranquicias();
            return _liquidacionfranq.LiquidarFranquiciasDetalleTotal(liquidacionFranquicia);
        }

        public int LiquidarFranquiciasExcepciones(ColpatriaSAI.Negocio.Entidades.LiquidacionFranquicia liquidacionFranquicia)
        {
            var _liquidacionfranq = new Negocio.Componentes.LiqFranquicias.LiquidacionFranquicias();
            return _liquidacionfranq.LiquidarFranquiciasExcepciones(liquidacionFranquicia);
        }

        public int LiquidarFranquiciasParticipaciones(ColpatriaSAI.Negocio.Entidades.LiquidacionFranquicia liquidacionFranquicia)
        {
            var _liquidacionfranq = new Negocio.Componentes.LiqFranquicias.LiquidacionFranquicias();
            return _liquidacionfranq.LiquidarFranquiciasParticipaciones(liquidacionFranquicia);
        }

        public int LiquidarFranquiciasPorRangos(ColpatriaSAI.Negocio.Entidades.LiquidacionFranquicia liquidacionFranquicia)
        {
            var _liquidacionfranq = new Negocio.Componentes.LiqFranquicias.LiquidacionFranquicias();
            return _liquidacionfranq.LiquidarFranquiciasPorRangos(liquidacionFranquicia);
        }

        //FIN NUEVOS METODOS     

        public List<ColpatriaSAI.Negocio.Entidades.AnticipoFranquicia> ListarAnticipoFranquicias()
        {
            _antFranquicias = new AnticiposFranquicias();
            return _antFranquicias.ListarAnticipoFranquicias();
        }

        public ColpatriaSAI.Negocio.Entidades.AnticipoFranquicia AnticipoFranquiciaPorId(int id)
        {
            _antFranquicias = new AnticiposFranquicias();
            return _antFranquicias.AnticipoFranquiciaPorId(id);
        }

        public int AnularAnticipo(int idAnticipo)
        {
            _antFranquicias = new AnticiposFranquicias();
            return _antFranquicias.AnularAnticipo(idAnticipo);
        }

        public int ActualizarAnticipoFranquicia(int idAnticipo, AnticipoFranquicia anticipo, string Username)
        {
            _antFranquicias = new AnticiposFranquicias();
            return _antFranquicias.ActualizarAnticipoFranquicia(idAnticipo, anticipo, Username);
        }

        public int ActualizarAnticipoFranquicias(int id, ColpatriaSAI.Negocio.Entidades.AnticipoFranquicia anticipoFranquicia, string usuario)
        {
            _antFranquicias = new AnticiposFranquicias();
            return _antFranquicias.ActualizarAnticipoFranquicias(id, anticipoFranquicia, usuario);
        }

        public int InsertarAnticipoFranquicias(ColpatriaSAI.Negocio.Entidades.AnticipoFranquicia anticipoFranquicia, string Username)
        {
            _antFranquicias = new AnticiposFranquicias();
            return _antFranquicias.InsertarAnticipoFranquicias(anticipoFranquicia, Username);
        }

        public List<ColpatriaSAI.Negocio.Entidades.EstadoLiquidacion> ListarEstadosLiquidacionFranquicias()
        {
            var _liquidacionfranq = new Negocio.Componentes.LiqFranquicias.LiquidacionFranquicias();
            return _liquidacionfranq.ListarEstadosLiquidacionFranquicias().ToList();
        }

        public int ActualizarLiquidacionFranquiciaEstado(int idLiquidacion, int idEstado, string Username)
        {
            var _liquidacionfranq = new Negocio.Componentes.LiqFranquicias.LiquidacionFranquicias();
            return _liquidacionfranq.ActualizarLiquidacionFranquiciaEstado(idLiquidacion, idEstado, Username);
        }

        public int ActualizarLiquidacionFranquiciaReliquidacion(int idLiquidacion, int permiteReliquidar, string Username)
        {
            var _liquidacionfranq = new Negocio.Componentes.LiqFranquicias.LiquidacionFranquicias();
            return _liquidacionfranq.ActualizarLiquidacionFranquiciaReliquidacion(idLiquidacion, permiteReliquidar, Username);
        }

        public int GenerarPagosLiquidacionFranquicia(int idLiqFranquica, string usuario)
        {
            var _liquidacionfranq = new Negocio.Componentes.LiqFranquicias.LiquidacionFranquicias();
            return _liquidacionfranq.GenerarPagosLiquidacionFranquicia(idLiqFranquica, usuario);
        }

        public int ObtenerProcesoLiquidacion()
        {
            var _liquidacionfranq = new Negocio.Componentes.LiqFranquicias.LiquidacionFranquicias();
            return _liquidacionfranq.ObtenerProcesoLiquidacion();
        }

        public int EliminarLiquidacionProceso(int idLiquidacion, string Username)
        {
            var _liquidacionfranq = new Negocio.Componentes.LiqFranquicias.LiquidacionFranquicias();
            return _liquidacionfranq.EliminarLiquidacionProceso(idLiquidacion, Username);
        }

        #endregion

        #region PRESUPUESTO

        public List<Presupuesto> ListarPresupuestos()
        {
            _presupuestos = new Presupuestos();
            return _presupuestos.ListarPresupuestos();
        }

        public int InsertarPresupuesto(Presupuesto presupuesto, string Username)
        {
            _presupuestos = new Presupuestos();
            return _presupuestos.InsertarPresupuesto(presupuesto, Username);
        }

        public int InsertarDetallePresupuesto(List<PresupuestoDetalles> detalle, int idPresupuesto, int anio, string hojaActual, int esUltimaHoja, int fila, string Username)
        {
            _presupuestos = new Presupuestos();
            return _presupuestos.InsertarDetallePresupuesto(detalle, idPresupuesto, anio, hojaActual, esUltimaHoja, fila, Username);
        }

        public int CalcularMetasCompuestas(int presupuesto_id)
        {
            return new Presupuestos().CalcularMetasCompuestas(presupuesto_id);
        }

        public int CalcularMetasAcumuladas(int presupuesto_id)
        {
            return new Presupuestos().CalcularMetasAcumuladas(presupuesto_id);
        }

        public int CalcularEjecucionPresupuesto(int presupuesto_id)
        {
            return new Presupuestos().CalcularEjecucionPresupuesto(presupuesto_id);
        }

        public List<DetallePresupuesto> ListarDetallePresupuestoPorId(int id)
        {
            return new Presupuestos().ListarDetallePresupuestoPorId(id);
        }

        public string EliminarPresupuesto(int id, string Username)
        {
            _presupuestos = new Presupuestos();
            return _presupuestos.EliminarPresupuesto(id, Username);
        }

        public int BorrarPresupuestoACargar(int anio, int segmento_id, string Username)
        {
            return new Presupuestos().BorrarPresupuestoACargar(anio, segmento_id, Username);
        }

        public List<PresupuestoDetalle> ListarPresupuestoDetalle(int id)
        {
            _presupuestos = new Presupuestos();
            return _presupuestos.ListarPresupuestoDetalle(id);
        }

        public void CarguePresupuesto(string nombreArchivo, string anio, int segmento_id, string Username)
        {
            _presupuestos = new Presupuestos();
            _presupuestos.CargarPresupuesto(nombreArchivo, anio, segmento_id, Username);
        }

        #endregion

        #region LIQUIDACIÓN DE CONCURSOS

        #region NUEVOSMETODOS
        /// <summary>
        /// Realiza la liquidación y generación del reporte de la liquidación de Reglas (premios)
        /// </summary>
        /// <param name="fechaInicio">Determina la fecha inicio del periodo a liquidar (Solo si la regla es periódica)</param>
        /// <param name="fechaFin">Determina la fecha final del periodo a liquidar (Solo si la regla es periódica)</param>
        /// <param name="idRegla">Es el id de la regla a liquidar</param>
        /// <param name="idConcurso">Es el id del concurso relacionado</param>
        /// <returns>Retorna 1 cuando la liquidación fue correcta o 0 en caso contrario</returns>
        public void GenerarLiquidacionRegla_Iniciar(DateTime fechaInicio, DateTime fechaFin, int idRegla, int idConcurso)
        {
            _liquidacionConcurso = new LiquidacionConcurso();
            _liquidacionConcurso.GenerarLiquidacionRegla_Iniciar(fechaInicio,fechaFin, idRegla, idConcurso);
        }

        public object GenerarReporteLiquidacionAsesor(int idLiquidacionRegla)
        {
            _liquidacionConcurso = new LiquidacionConcurso();
            return _liquidacionConcurso.GenerarReporteLiquidacionAsesor(idLiquidacionRegla);
        }

        #endregion


        /// <summary>
        /// Permite liquidar una regla de un concurso
        /// </summary>
        /// <returns></returns>
        public int InsertarLiquidacionRegla(LiquidacionRegla liquidacion, string Username)
        {
            _liquidacionConcurso = new LiquidacionConcurso();
            return _liquidacionConcurso.InsertarLiquidacionRegla(liquidacion, Username);
        }

        public int GenerarLiquidacionRegla(int idLiquidacionRegla, DateTime fechaInicio, DateTime fechaFin, int idRegla, int idConcurso)
        {
            _liquidacionConcurso = new LiquidacionConcurso();
            return _liquidacionConcurso.GenerarLiquidacionRegla(idLiquidacionRegla, fechaInicio, fechaFin, idRegla, idConcurso);
        }

        public List<string> ListarLiquidaciones(int regla_id)
        {
            return new LiquidacionConcurso().ListarLiquidaciones(regla_id);
        }

        public List<LiquidacionRegla> ListarLiquidacionesRegla(int idRegla)
        {
            _liquidacionConcurso = new LiquidacionConcurso();
            return _liquidacionConcurso.ListarLiquidacionesRegla(idRegla);
        }
        public List<Participante> ListarParticipantesLiquidacion(int idLiquidacionRegla, int inicio, int cantidad)
        {
            _liquidacionConcurso = new LiquidacionConcurso();
            return _liquidacionConcurso.ListarParticipantesConcurso(idLiquidacionRegla, inicio, cantidad);
        }

        public List<VistaDetalleLiquidacionReglaParticipante> ListarDetalleLiquidacionReglaParticipante(int idLiquidacionRegla, int participante_id)
        {
            _liquidacionConcurso = new LiquidacionConcurso();
            return _liquidacionConcurso.ListarDetalleLiquidacionReglaParticipante(idLiquidacionRegla, participante_id);
        }

        public int LiquidarPagosRegla(int idLiquidacionRegla)
        {
            _liquidacionConcurso = new LiquidacionConcurso();
            return _liquidacionConcurso.GenerarLiquidacionPagos(idLiquidacionRegla);
        }

        public int ActualizarLiquidacionReglaEstado(int idLiquidacion, int idEstado, string Username)
        {
            var _liquidacionfranq = new LiquidacionConcurso();
            return _liquidacionfranq.ActualizarLiquidacionReglaEstado(idLiquidacion, idEstado, Username);
        }

        #endregion

        #region TipoVehiculo


        public List<Entidades.TipoVehiculo> ListarTipoVehiculos()
        {
            return new Admin.TipoVehiculo().ListarTipoVehiculos();
        }

        public List<Entidades.TipoVehiculo> ListarTipoVehiculosporRamo(int ramo_id)
        {
            return new Admin.TipoVehiculo().ListarTipoVehiculosporRamo(ramo_id);
        }
        #endregion

        #region PARTICIPACIONES

        public List<Participacione> ListarParticipaciones()
        {
            _participacion = new Participaciones();
            return _participacion.ListarParticipaciones();
        }

        public List<Participacione> ListarParticipacionPorId(int id)
        {
            _participacion = new Participaciones();
            return _participacion.ListarParticipacionPorId(id);
        }

        public int InsertarParticipacion(Participacione part, string Username)
        {
            _participacion = new Participaciones();
            return _participacion.InsertarParticipacion(part, Username);
        }

        public int ActualizarParticipacion(int id, Participacione part, string Username)
        {
            _participacion = new Participaciones();
            return _participacion.ActualizarParticipacion(id, part, Username);
        }

        public string EliminarParticipacion(int id, Participacione part, string Username)
        {
            _participacion = new Participaciones();
            return _participacion.EliminarParticipacion(id, part, Username);
        }
        public List<ParticipacionDirector> ListarParticipacionesDirector()
        {
            _participacion = new Participaciones();
            return _participacion.ListarParticipacionesDirector();
        }

        public List<ParticipacionDirector> ListarParticipacionDirectorPorId(int id)
        {
            _participacion = new Participaciones();
            return _participacion.ListarParticipacionDirectorPorId(id);
        }

        public int InsertarParticipacionDirector(ParticipacionDirector part, string Username)
        {
            _participacion = new Participaciones();
            return _participacion.InsertarParticipacionDirector(part, Username);
        }

        public int ActualizarParticipacionDirector(int id, ParticipacionDirector part, string Username)
        {
            _participacion = new Participaciones();
            return _participacion.ActualizarParticipacionDirector(id, part, Username);
        }

        public string EliminarParticipacionDirector(int id, ParticipacionDirector part, string Username)
        {
            _participacion = new Participaciones();
            return _participacion.EliminarParticipacionDirector(id, part, Username);
        }

        public List<Participante> ListarParticipantesPorNivel(string texto, int inicio, int cantidad, int nivel)
        {
            _participacion = new Participaciones();
            return _participacion.ListarParticipantesPorNivel(texto, inicio, cantidad, nivel);
        }

        public List<Ramo> ListarRamosXCompania(int id)
        {
            _participacion = new Participaciones();
            return _participacion.ListarRamosXCompania(id);
        }

        #endregion

        #region LIQUIDACION DE CONTRATACION DE DESEMPEÑO

        public int InsertarLiquidacionContrat(LiquidacionContratacion liquiContrat, string Username)
        {
            _liquiContrat = new LiquidacionContrats();
            return _liquiContrat.InsertarLiquidacionContrat(liquiContrat, Username);
        }

        public int InsertarLiquidacionContratMeta(LiquiContratMeta liquiContrat, string Username)
        {
            _liquiContrat = new LiquidacionContrats();
            return _liquiContrat.InsertarLiquidacionContratMeta(liquiContrat, Username);
        }

        public int InsertarDetalleLiquidacionContratPP(DetalleLiquiContratPpacionPpante liquiContrat, string Username)
        {
            _liquiContrat = new LiquidacionContrats();
            return _liquiContrat.InsertarDetalleLiquidacionContratPP(liquiContrat, Username);
        }

        public int InsertarLiquidacionContratFP(LiquiContratFactorParticipante liquiContrat, string Username)
        {
            _liquiContrat = new LiquidacionContrats();
            return _liquiContrat.InsertarLiquidacionContratFP(liquiContrat, Username);
        }

        public int InsertarLiquidacionContratPP(LiquiContratPpacionPpante liquiContrat, string Username)
        {
            _liquiContrat = new LiquidacionContrats();
            return _liquiContrat.InsertarLiquidacionContratPP(liquiContrat, Username);
        }
        #endregion

        #region JERARQUIAS

        public List<Jerarquia> ListarJerarquias()
        {
            return new Jerarquias().ListarJerarquias();
        }

        public Jerarquia ListarJerarquiaPorId(int id)
        {
            return new Jerarquias().ListarJerarquiaPorId(id);
        }

        public int InsertarJerarquia(Jerarquia jerarquia, string userName)
        {
            return new Jerarquias().InsertarJerarquia(jerarquia, userName);
        }

        public int ActualizarJerarquia(int id, Jerarquia jerarquia, string userName)
        {
            return new Jerarquias().ActualizarJerarquia(id, jerarquia, userName);
        }

        public int EliminarJerarquia(int id, string Username)
        {
            return new Jerarquias().EliminarJerarquia(id, Username);
        }

        public List<TipoJerarquia> ListarTiposJerarquia()
        {
            return new Jerarquias().ListarTiposJerarquia();
        }

        public List<JerarquiaDetalle> ListarJerarquiaDetalle()
        {
            return new Jerarquias().ListarJerarquiaDetalle();
        }

        public JerarquiaDetalle InsertarJerarquiaDetalle(JerarquiaDetalle detalle, string userName)
        {
            return new Jerarquias().InsertarJerarquiaDetalle(detalle, userName);
        }

        public List<JerarquiaDetalle> ListarNodosBuscador(int tipo, string texto, int inicio, int cantidad, int nivel, int zona, int canal, string codNivel)
        {
            return new Jerarquias().ListarNodosBuscador(tipo, texto, inicio, cantidad, nivel, zona, canal, codNivel);
        }

        public List<NodoArbol> ListarArbol(int id, int padre_id)
        {
            return new Jerarquias().ListarArbol(id, padre_id);
        }

        public int EliminarJerarquiaDetalle(int id, string userName)
        {
            return new Jerarquias().EliminarJerarquiaDetalle(id,userName);
        }

        public JerarquiaDetalle ListarNodoArbol(int id)
        {
            return new Jerarquias().ListarNodoArbol(id);
        }

        public int ActualizarOrdenNodo(JerarquiaDetalle detalle, string userName)
        {
            return new Jerarquias().ActualizarOrdenNodo(detalle, userName);
        }

        #endregion

        #region Metodos PERSISTENCIAESPERADA

        public int InsertarPersistenciaEsperada(PersistenciaEsperada persistenciaesperada, string Username)
        {
            _companiaxetapas = new CompaniaxEtapas();
            return _companiaxetapas.InsertarPersistenciaEsperada(persistenciaesperada, Username);
        }

        public List<PersistenciaEsperada> ListarPersistenciaEsperada()
        {
            _companiaxetapas = new CompaniaxEtapas();
            return _companiaxetapas.ListarPersistenciaEsperada();
        }

        public List<PersistenciaEsperada> ListarPersistenciaEsperadaPorId(int idPersistenciaEsperada)
        {
            _companiaxetapas = new CompaniaxEtapas();
            return _companiaxetapas.ListarPersistenciaEsperadaPorId(idPersistenciaEsperada);
        }

        public int ActualizarPersistenciaEsperada(int id, PersistenciaEsperada persistenciaesperada, string Username)
        {
            _companiaxetapas = new CompaniaxEtapas();
            return _companiaxetapas.ActualizarPersistenciaEsperada(id, persistenciaesperada, Username);
        }

        public string EliminarPersistenciaEsperada(int id, PersistenciaEsperada persistenciaesperada, string Username)
        {
            _companiaxetapas = new CompaniaxEtapas();
            return _companiaxetapas.EliminarPersistenciaEsperada(id, persistenciaesperada, Username);
        }
        #endregion

        #region Metodos SINIESTRALIDADESPERADA

        public int InsertarSiniestralidadEsperada(SiniestralidadEsperada siniestralidadesperada, string Username)
        {
            _companiaxetapas = new CompaniaxEtapas();
            return _companiaxetapas.InsertarSiniestralidadEsperada(siniestralidadesperada, Username);
        }

        public List<SiniestralidadEsperada> ListarSiniestralidadEsperada()
        {
            _companiaxetapas = new CompaniaxEtapas();
            return _companiaxetapas.ListarSiniestralidadEsperada();
        }

        public List<SiniestralidadEsperada> ListarSiniestralidadEsperadaPorId(int idSiniestralidadEsperada)
        {
            _companiaxetapas = new CompaniaxEtapas();
            return _companiaxetapas.ListarSiniestralidadEsperadaPorId(idSiniestralidadEsperada);
        }

        public int ActualizarSiniestralidadEsperada(int id, SiniestralidadEsperada siniestralidadesperada, string Username)
        {
            _companiaxetapas = new CompaniaxEtapas();
            return _companiaxetapas.ActualizarSiniestralidadEsperada(id, siniestralidadesperada, Username);
        }

        public string EliminarSiniestralidadEsperada(int id, SiniestralidadEsperada siniestralidadesperada, string Username)
        {
            _companiaxetapas = new CompaniaxEtapas();
            return _companiaxetapas.EliminarSiniestralidadEsperada(id, siniestralidadesperada, Username);
        }
        #endregion

        #region  Excepciones Jerarquia
        public List<ExcepcionJerarquiaDetalle> ListarExcepcionesJerarquiaporId(int idJerarquia)
        {
            var _excepcion = new ExcepcionesJerarquia();
            return _excepcion.ListarExcepcionesJerarquiaporId(idJerarquia);
        }

        public int InsertarExceptionJerarquia(ExcepcionJerarquiaDetalle excepcion, string Username)
        {
            var _excepcion = new ExcepcionesJerarquia();
            return _excepcion.InsertarExceptionJerarquia(excepcion, Username);
        }

        public int EliminarExceptionJerarquia(int idexcepcion, string Username)
        {
            var _excepcion = new ExcepcionesJerarquia();
            return _excepcion.EliminarExceptionJerarquia(idexcepcion, Username);
        }

        public ExcepcionJerarquiaDetalle TraerExceptionJerarquiaporId(int idException)
        {
            var _excepcion = new ExcepcionesJerarquia();
            return _excepcion.TraerExceptionJerarquiaporId(idException);
        }

        public int ActualizarExcepcionJerarquia(ExcepcionJerarquiaDetalle exception, string Username)
        {
            var _excepcion = new ExcepcionesJerarquia();
            return _excepcion.ActualizarExcepcionJerarquia(exception, Username);
        }

        #endregion

        #region Liquidaciones Moneda

        public List<LiquidacionMoneda> ListarLiquidacionesMoneda(int tipo)
        {
            var _liquidacionMoneda = new LiquidacionMonedas();
            return _liquidacionMoneda.ListarLiquidacionesMoneda(tipo);
        }

        public int GuardarLiquidacionMoneda(LiquidacionMoneda liquidacionMoneda, string Username)
        {
            var _liquidacionMoneda = new LiquidacionMonedas();
            return _liquidacionMoneda.GuardarLiquidacionMoneda(liquidacionMoneda, Username);
        }

        public int BorrarColquinesManuales(DateTime fechaCargue, string Username)
        {
            var _liquidacionMoneda = new LiquidacionMonedas();
            return _liquidacionMoneda.BorrarColquinesManuales(fechaCargue, Username);
        }

        #endregion

        #region Tipo Medidas

        public List<TipoMedida> ListarTipoMedidas()
        {
            var _tipoMedidas = new TipoMedidas();
            return _tipoMedidas.ListarTipoMedidas();
        }

        #endregion

        #region Tipo Contratos

        public List<Entidades.TipoContrato> ListarTipoContratos()
        {
            _tiposContratos = new TiposContratos();
            return _tiposContratos.ListarTipoContratos();
        }

        #endregion 

        #region  Metas Jerarquia
        public List<MetaxNodo> ListarMetasxJerarquiaId(int idJerarquia)
        {
            var _metas = new MetasNodos();
            return _metas.ListarMetasxJerarquiaId(idJerarquia);
        }

        public int InsertarMetaNodo(MetaxNodo metaNodo, string Username)
        {
            var _metas = new MetasNodos();
            return _metas.InsertarMetaNodo(metaNodo, Username);
        }

        public int EliminarMetaNodo(int idMetaNodo, string Username)
        {
            var _metas = new MetasNodos();
            return _metas.EliminarMetaNodo(idMetaNodo, Username);
        }

        public List<MetaxNodo> ListarMetasNodos()
        {
            var _metas = new MetasNodos();
            return _metas.ListarMetasNodos();
        }


        #endregion

        #region Ejecucion

        public int GuardarEjecucionDetalle(EjecucionDetalle ejecucionDetalle)
        {
            var _ejecucion = new EjecucionPresupuesto();
            return _ejecucion.GuardarEjecucionDetalle(ejecucionDetalle);
        }

        public int GuardarEjecucionDetalleBatch(String strSQL)
        {
            var _ejecucion = new EjecucionPresupuesto();
            return _ejecucion.GuardarEjecucionDetalleBatch(strSQL);
        }

        public Ejecucion TraerEjecucionPorPresupuesto(int idPresupuesto)
        {
            var _ejecucion = new EjecucionPresupuesto();
            return _ejecucion.TraerEjecucionPorPresupuesto(idPresupuesto);
        }

        public EjecucionDetalle TraerEjecucionDetalle(int idEjecucion, int idMeta, int idNodo, int periodo, int idCanal)
        {
            var _ejecucion = new EjecucionPresupuesto();
            return _ejecucion.TraerEjecucionDetalle(idEjecucion, idMeta, idNodo, periodo, idCanal);
        }

        public int EliminarEjecucionDetallePorIdEjecucion(int idEjecucion)
        {
            var _ejecucion = new EjecucionPresupuesto();
            return _ejecucion.EliminarEjecucionDetallePorIdEjecucion(idEjecucion);
        }

        public EjecucionDetalle TraerEjecucionDetallePorId(int idEjecucionDetalle)
        {
            var _ejecucion = new EjecucionPresupuesto();
            return _ejecucion.TraerEjecucionDetallePorId(idEjecucionDetalle);
        }

        public int ActualizarEjecucionDetalle(EjecucionDetalle ejecucionDetalle)
        {
            var _ejecucion = new EjecucionPresupuesto();
            return _ejecucion.ActualizarEjecucionDetalle(ejecucionDetalle);
        }

        public void CargueManualEjecucionDetalle(string nombreArchivo, int idPresupuesto)
        {
            var _ejecucion = new EjecucionPresupuesto();
            _ejecucion.CargueManualEjecucionDetalle(nombreArchivo, idPresupuesto);
        }
        #endregion

        #region Liquidacion Contratacion
        public List<LiquidacionContratacion> ListarLiquidacionContratacion()
        {
            var _liquidacionContrat = new Negocio.Componentes.Contratacion.LiquidacionContrats();

            return _liquidacionContrat.ListarLiquidacionContratacion();
        }

        public LiquidacionContratacion TraerLiquidacionContratacion(int idLiquidacion)
        {
            var _liquidacionContrat = new Negocio.Componentes.Contratacion.LiquidacionContrats();

            return _liquidacionContrat.TraerLiquidacionContratacion(idLiquidacion);
        }

        public int LiquidarContratacion(LiquidacionContratacion liquidacionContratacion, int idSegmento)
        {
            var _liquidacionContrat = new Negocio.Componentes.Contratacion.LiquidacionContrats();

            return _liquidacionContrat.LiquidarContratacion(liquidacionContratacion, idSegmento);
        }

        public int ActualizarLiquidacionContratacionEstado(int idLiquidacion, int idEstado, string Username)
        {
            var _liquidacionContrat = new Negocio.Componentes.Contratacion.LiquidacionContrats();

            return _liquidacionContrat.ActualizarLiquidacionContratacionEstado(idLiquidacion, idEstado, Username);
        }

        public int EliminarLiquidacionContratacion(int idLiquidacion, string Username)
        {
            var _liquidacionContrat = new Negocio.Componentes.Contratacion.LiquidacionContrats();

            return _liquidacionContrat.EliminarLiquidacionContratacion(idLiquidacion, Username);
        }

        #endregion

        public List<NotificacionProceso> ObtenerProcesosEnCurso()
        {
            return Helper.ObtenerProcesosEnCurso();
        }

        public int EliminarProcesosEnCurso(string Username)
        {
            return Helper.EliminarProcesosEnCurso(Username);
        }

        public int CancelarProceso(int tipo, string Username)
        {
            return Helper.CancelarProceso(tipo, Username);
        }

        #region Dashboard

        public int ObtenerDatosDashboard()
        {
            var _Dashboard = new Negocio.Componentes.Admin.Dashboard();

            return _Dashboard.ObtenerDatosDashboard();
        }

        public List<ColpatriaSAI.Negocio.Entidades.Dashboard> TraerDashboard()
        {
            var _Dashboard = new Negocio.Componentes.Admin.Dashboard();

            return _Dashboard.TraerDashboard();
        }

        public List<ColpatriaSAI.Negocio.Entidades.TipoPanel> TraerDashboardxPanel()
        {
            var _Dashboard = new Negocio.Componentes.Admin.Dashboard();

            return _Dashboard.TraerDashboardxPanel();
        }

        #endregion

        #region  Periodos Cierre

        public List<PeriodoCierre> ListarPeriodos()
        {
            var _periodos = new PeriodosCierre();

            return _periodos.ListarPeriodos();
        }

        public List<PeriodoCierre> ListarPeriodosPorCompania(int idCompania)
        {
            var _periodos = new PeriodosCierre();

            return _periodos.ListarPeriodosPorCompania(idCompania);
        }

        public int InsertarPeriodoCierre(PeriodoCierre periodo, string Username)
        {
            var _periodos = new PeriodosCierre();

            return _periodos.InsertarPeriodoCierre(periodo, Username);
        }

        public int EliminarPeriodoCierre(int idPeriodo, string Username)
        {
            var _periodos = new PeriodosCierre();

            return _periodos.EliminarPeriodoCierre(idPeriodo, Username);
        }

        public List<PeriodoCierre> TraerPeriodoCierrePorId(int idPeriodo)
        {
            var _periodos = new PeriodosCierre();

            return _periodos.TraerPeriodoCierrePorId(idPeriodo);
        }

        public int ActualizarPeriodoCierre(PeriodoCierre periodo, string Username)
        {
            var _periodos = new PeriodosCierre();

            return _periodos.ActualizarPeriodoCierre(periodo, Username);
        }

        public int ActualizarEstadoPeriodoCierre(int idPeriodo, int idEstado, string Username)
        {
            var _periodos = new PeriodosCierre();

            return _periodos.ActualizarEstadoPeriodoCierre(idPeriodo, idEstado, Username);
        }

        public int SPPeriodoCierre(int companiaId, int mesCierre, int anioCierre)
        {
            var _periodos = new PeriodosCierre();

            return _periodos.SPPeriodoCierre(companiaId, mesCierre, anioCierre);
        }

        public int CerrarMesAbierto(int companiaId, int mesCierre, int anioCierre)
        {
            var _periodos = new PeriodosCierre();

            return _periodos.CerrarMesAbierto(companiaId, mesCierre, anioCierre);
        }

        public int DeleteReprocesos(int mesCierre, int añoCierre)
        {
            var _reprocesos = new PeriodosCierre();

            return _reprocesos.DeleteReprocesos(mesCierre, añoCierre);
        }

        #endregion

        #region "Ajustes"
        public List<DetallePagosRegla> ListarPagosConcurso(int liquidacionRegla_id)
        {
            _ajustesConcursos = new AjustesConcursos();
            return _ajustesConcursos.ListarPagosConcurso(liquidacionRegla_id);
        }

        public int ActualizarPagosConcurso(String usuario, List<DetallePagosRegla> listaPagosConcurso, string Username)
        {
            _ajustesConcursos = new AjustesConcursos();
            return _ajustesConcursos.ActualizarPagosConcurso(usuario, listaPagosConcurso, Username);
        }

        public List<LiquiContratFactorParticipante> ListarPagosContratos(int liquidacionContratos_id)
        {
            _ajustesContratos = new AjustesContratos();
            return _ajustesContratos.ListarPagosContratos(liquidacionContratos_id);
        }

        public int ActualizarPagosContratos(String usuario, List<LiquiContratFactorParticipante> listaPagosContratos, string Username)
        {
            _ajustesContratos = new AjustesContratos();
            return _ajustesContratos.ActualizarPagosContratos(usuario, listaPagosContratos, Username);
        }

        public List<DetallePagosFranquicia> ListarPagosFranquicia(int liquidacionFranquicia_id)
        {
            _ajustesFranquicias = new AjustesFranquicias();
            return _ajustesFranquicias.ListarPagosFranquicia(liquidacionFranquicia_id);
        }

        public int ActualizarPagosFranquicia(String usuario, List<DetallePagosFranquicia> listaPagosFranquicia, string Username)
        {
            _ajustesFranquicias = new AjustesFranquicias();
            return _ajustesFranquicias.ActualizarPagosFranquicia(usuario, listaPagosFranquicia, Username);
        }
        #endregion

        #region Combos

        public List<Combo> ListarCombos()
        {
            _combos = new Combos();
            return _combos.ListarCombos();
        }

        public List<Combo> ListarCombosPorId(int idCombo)
        {
            _combos = new Combos();
            return _combos.ListarCombosPorId(idCombo);
        }

        public int InsertarCombo(Combo combo, string Username)
        {
            _combos = new Combos();
            return _combos.InsertarCombo(combo, Username);
        }

        public int ActualizarCombo(int id, Combo combo, string Username)
        {
            _combos = new Combos();
            return _combos.ActualizarCombo(id, combo, Username);
        }

        public int ActualizarComboValidado(int id, int validado, string Username)
        {
            _combos = new Combos();
            return _combos.ActualizarComboValidado(id, validado, Username);
        }


        public int EliminarCombo(int idCombo, string Username)
        {
            _combos = new Combos();
            return _combos.EliminarCombo(idCombo, Username);
        }

        public List<ProductoCombo> ListarProductosComboPorId(int id)
        {
            _combos = new Combos();
            return _combos.ListarProductosCombosPorId(id);
        }

        public int InsertarProductoCombo(ProductoCombo productocombo, string Username)
        {
            _combos = new Combos();
            return _combos.InsertarProductoCombo(productocombo, Username);
        }

        public int ActualizarProductoCombo(int id, ProductoCombo productocombo, string Username)
        {
            _combos = new Combos();
            return _combos.ActualizarProductoCombo(id, productocombo, Username);
        }

        public int EliminarProductoCombo(int idCombo, string Username)
        {
            _combos = new Combos();
            return _combos.EliminarProductoCombo(idCombo, Username);
        }


        #endregion

        #region "Excepciones"

        public List<ExcepcionesGenerale> ListarExcepcionesGeneralesXcompanyXTipoMedida()
        {
            _concursoes = new Concursoes();
            return _concursoes.ListarExcepcionesGeneralesXcompanyXTipoMedida();
        }

        public List<ExcepcionesGenerale> ListarExcepcionesGeneralesXTipoMedida()
        {
            _concursoes = new Concursoes();
            return _concursoes.ListarExcepcionesGeneralesXTipoMedida();
        }

        public List<ExcepcionesGenerale> ListarExcepcionesGenerales()
        {
            _concursoes = new Concursoes();
            return _concursoes.ListarExcepcionesGenerales();
        }

        public bool validarExcepcionesGenerales(ExcepcionesGenerale excepcionG)
        {
            _concursoes = new Concursoes();
            return _concursoes.validarExcepcionesGenerales(excepcionG);
        }

        public int CrearExcepcionesGenerales(ExcepcionesGenerale excepcionG, string Username)
        {
            _concursoes = new Concursoes();
            return _concursoes.CrearExcepcionesGenerales(excepcionG, Username);
        }

        public int ActualizarExcepcionesGenerales(ExcepcionesGenerale excepcionG, string Username)
        {
            _concursoes = new Concursoes();
            return _concursoes.ActualizarExcepcionesGenerales(excepcionG, Username);
        }

        public int EliminarExcepcionGenerales(int id, string Username)
        {
            _concursoes = new Concursoes();
            return _concursoes.EliminarExcepcionGenerales(id, Username);
        }

        #endregion

        #region SEGURIDAD

        public int CrearUsuario(string nombreUsuario, string tipoDocumento, string numeroDocumento, string email, string rol, int segmento, string Username)
        {
            _usuario = new Usuario();
            return _usuario.CrearUsuario(nombreUsuario, tipoDocumento, numeroDocumento, email, rol, segmento, Username);
        }

        public int InsertarSegmentodeUsuario(UsuarioxSegmento usuarioxsegmento, string Username)
        {
            _usuario = new Usuario();
            return _usuario.InsertarSegmentodeUsuario(usuarioxsegmento, Username);
        }

        public int EliminarSegmentodeUsuario(UsuarioxSegmento usuarioxsegmento, string Username)
        {
            _usuario = new Usuario();
            return _usuario.EliminarSegmentodeUsuario(usuarioxsegmento, Username);
        }

        #endregion

        #region PROCESO AUTOMATICO
        public List<Ejecuciones> TraerUltimaEjecucion()
        {
            aut = new ProcesoAutomatico.AUT();
            return aut.TraerUltimaEjecucion();
        }

        public List<Ejecuciones> TraerEjecucion(DateTime fechaIni)
        {
            aut = new ProcesoAutomatico.AUT();
            return aut.TraerEjecucion(fechaIni);
        }

        public List<AUT_Programacion_Proceso> TraerUltimasFechasProgramacion()
        {
            aut = new ProcesoAutomatico.AUT();
            return aut.TraerUltimasFechasProgramacion();
        }

        public List<AUT_Programacion_Proceso> TraerFechasProgramacion()
        {
            aut = new ProcesoAutomatico.AUT();
            return aut.TraerFechasProgramacion();
        }

        public AUT_Programacion_Proceso InsertarProgramacion(AUT_Programacion_Proceso programacion)
        {
            aut = new ProcesoAutomatico.AUT();
            return aut.InsertarProgramacion(programacion);
        }

        public bool EliminarProgramacion(AUT_Programacion_Proceso programacion)
        {
            aut = new ProcesoAutomatico.AUT();
            return aut.EliminarProgramacion(programacion);
        }

        public List<AUT_Proceso> TraerProcesos()
        {
            aut = new ProcesoAutomatico.AUT();
            return aut.TraerProcesos();
        }

        public AUT_Proceso ActualizarProceso(AUT_Proceso procesoold)
        {
            aut = new ProcesoAutomatico.AUT();
            return aut.ActualizarProceso(procesoold);
        }

        public List<AUT_Proceso_Dependencia> TraerDependenciaPorProceso(int idProceso)
        {
            aut = new ProcesoAutomatico.AUT();
            return aut.TraerDependenciaPorProceso(idProceso);
        }

        public List<AUT_Tipo_Accion_En_Error> TraerAccionesEnError()
        {
            aut = new ProcesoAutomatico.AUT();
            return aut.TraerAccionesEnError();
        }

        public AUT_Proceso_Dependencia InsertarProcesoDependencia(AUT_Proceso_Dependencia procesoDependencia)
        {
            aut = new ProcesoAutomatico.AUT();
            return aut.InsertarProcesoDependencia(procesoDependencia);
        }

        public AUT_Proceso_Dependencia ActualizarProcesoDependencia(AUT_Proceso_Dependencia procesoold, AUT_Proceso_Dependencia procesonew)
        {
            aut = new ProcesoAutomatico.AUT();
            return aut.ActualizarProcesoDependencia(procesoold, procesonew);
        }

        public bool EliminarProcesoDependencia(AUT_Proceso_Dependencia procesoDependencia)
        {
            aut = new ProcesoAutomatico.AUT();
            return aut.EliminarProcesoDependencia(procesoDependencia);
        }
        #endregion

        #region ESTADISTICAS
        public List<ReporteValores> TraerValoresRecaudosMesPromedio(int anio, int mes)
        {
            estr = new Estadisticas.EstadisticasRecaudos();
            return estr.TraerValoresRecaudosMesPromedio(anio,mes);
        }

        public List<ReporteRegistros> TraerRegistrosRecaudosMesPromedio(int anio, int mes)
        {
            estr = new Estadisticas.EstadisticasRecaudos();
            return estr.TraerRegistrosRecaudosMesPromedio(anio, mes);
        }

        public List<ReporteValores> TraerValoresPrimasMesPromedio(int anio, int mes)
        {
            estp = new Estadisticas.EstadisticasPrimas();
            return estp.TraerValoresPrimasMesPromedio(anio, mes);
        }

        public List<ReporteRegistros> TraerRegistrosPrimasMesPromedio(int anio, int mes)
        {
            estp = new Estadisticas.EstadisticasPrimas();
            return estp.TraerRegistrosPrimasMesPromedio(anio, mes);
        }

        #endregion
    }
}
