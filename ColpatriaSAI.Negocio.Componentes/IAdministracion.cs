using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using ColpatriaSAI.Negocio.Entidades;
using ColpatriaSAI.Negocio.Entidades.Informacion;
using ColpatriaSAI.Negocio.Componentes.Utilidades;
//using System.Data;

namespace ColpatriaSAI.Negocio.Componentes
{
    [ServiceContract]
    public interface IAdministracion
    {
               
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
        [OperationContract]
        void InsertarAuditoria(int tablaModuloProceso, int tipoModificacion, DateTime fechaInicio, DateTime fechaFinal, string observacion, string primeraVersion,
            string versionFinal, string user);

        /// <summary>
        /// Funcion q se encarga de traer todos los datos de la tabla auditoria
        /// </summary>
        /// <returns>Listado con objtos de auditoria</returns>
        [OperationContract] List<Entidades.Auditoria> ListarAuditoria(int idTabla, DateTime fechaInicio, DateTime fechaFin, int idEvento, List<int> segmentos);

        [OperationContract] List<TablaAuditada> ListarTablasAuditadas();

        [OperationContract] List<EventoTabla> ListarEventosTabla();

        #endregion

        //INTEGRACION//***************************************

        #region Metodos INTEGRACION

        [OperationContract] int Linq2XmlCrearArchivosXml();

        [OperationContract] int CantidadPaquetesenEjecución();

        [OperationContract] int EnviarFTP();

        [OperationContract] int EjecutarSPETL(string nombreETL);

        [OperationContract] int EjecutarSP(string nombreSP);

        [OperationContract] string ValoresWebConfigServicio();

        [OperationContract] int RetornarHorasIntegracion(int id);

        [OperationContract] List<ParametrosPersistenciaVIDA> ListarPPVIDA();

        [OperationContract] List<ParametrosPersistenciaVIDA> ListarPPVIDAPorId(int idParametro);

        [OperationContract]
        int ActualizarPPV(int id, ParametrosPersistenciaVIDA ppv, string Username);

        [OperationContract] List<ParametrosApp> ListarParametrosApp();

        [OperationContract] List<ParametrosApp> ListarParametrosAppPorId(int id);

        [OperationContract]
        int ActualizarParametrosApp(List<ParametrosApp> parametrosapp, string Username);

        #endregion

        //VARIOS//********************************************

        #region Metodos ZONAS
        /// <summary>
        /// Guarda un registro de zona
        /// </summary>
        /// <param name="zona">Objecto Zona a insertar</param>
        /// <returns>Numero de registros guardados</returns>
        [OperationContract] int InsertarZona(Zona zona, string Username);

        /// <summary>
        /// Obtiene listado de zonas
        /// </summary>
        /// <returns>Lista de zonas</returns>
        [OperationContract] List<Zona> ListarZonas();

        /// <summary>
        /// Obtiene listado de zonas por id
        /// </summary>
        /// <returns>Lista de zonas</returns>
        [OperationContract] List<Zona> ListarZonasPorId(int idZona);

        /// <summary>
        /// Actualiza un registro de zona
        /// </summary>
        /// <param name="id">Id de la zona a modificar</param>
        /// <param name="zona">Objeto Zona utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarZona(int id, Zona zona,string Username);

        /// <summary>
        /// Elimina un registro de zona
        /// </summary>
        /// <param name="id">Id de la zona a eliminar</param>
        /// <param name="zona">Objecto Zona utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] string EliminarZona(int id, Zona zona, string Username);

        #endregion

        #region Metodos LOCALIDADES

        [OperationContract] List<Localidad> ListarLocalidades();

        [OperationContract] List<Localidad> ListarLocalidadesPorId(int idLocalidad);

        [OperationContract] List<Localidad> ListarLocalidadesPorZona(int idZona);

        [OperationContract] int InsertarLocalidad(Localidad localidad, string Username);

        [OperationContract] int ActualizarLocalidad(int id, Localidad localidad, string Username);

        [OperationContract] string EliminarLocalidad(int id, Localidad localidad, string Username);

        #endregion

        #region Metodos TIPOLOCALIDADES

        [OperationContract] List<TipoLocalidad> ListarTipoLocalidad();

        #endregion

        #region Metodos COMPAÑIAS

        [OperationContract] int InsertarCompania(Compania compañia, string Username);

        /// <summary>
        /// Obtiene listado de compañias
        /// </summary>
        /// <returns>Lista de compañias</returns>
        [OperationContract] List<Compania> ListarCompanias();

        /// <summary>
        /// Obtiene listado de compañias por id
        /// </summary>
        /// <returns>Lista de compañias</returns>
        [OperationContract] List<Compania> ListarCompaniasPorId(int idCompañia);

        [OperationContract] int ActualizarCompania(int id, Compania compania, string Username);

        [OperationContract] string EliminarCompania(int id, Compania compania, string Username);

        #endregion

        #region Metodos SEGMENTOS

        [OperationContract] List<Segmento> ListarSegmentoes();

        [OperationContract] List<Segmento> ListarSegmentoesPorId(int idSegmento);

        [OperationContract] int InsertarSegmento(Segmento segmento, string Username);

        [OperationContract] int ActualizarSegmento(int id, Segmento segmento, string Username);

        [OperationContract] string EliminarSegmento(int id, Segmento segmento, string Username);

        #endregion

        #region Metodos COBERTURAS

        [OperationContract] List<Cobertura> ListarCoberturas();

        [OperationContract] List<Cobertura> ListarCoberturasPorId(int idCobertura);

        #endregion

        #region Metodos ACTIVIDADECONOMICA

        /// <summary>
        /// Funcion que se encarga de cargar el listado de las actividades Economicas
        /// </summary>
        /// <returns>Listado de Actividades Economicas</returns>
        [OperationContract] List<ActividadEconomica> ListarActividadEconomicas();

        /// <summary>
        /// Funcion que se encarga de traer el listado de las actividades Economicas por Actividad
        /// </summary>
        /// <param name="idActividadEconomica"></param>
        /// <returns></returns>
        [OperationContract] List<ActividadEconomica> ListarActividadEconomicasPorId(int idActividadEconomica);

        /// <summary>
        /// Funcion que se encarga de traer el listado de las actividades por compañia
        /// </summary>
        /// <param name="companiaID">Compañia con la que se esta trabajando</param>
        /// <returns>Listado de Actividades economicas por compañia</returns>
        [OperationContract] List<ActividadEconomica> ListarActividadEconomicasPorCompania(int companiaID);
        #endregion

        #region Metodos REGLAXCONCEPTODESCUENTO

        [OperationContract] List<ReglaxConceptoDescuento> ListarReglaxConceptoDescuento();

        #endregion

        #region Metodos CANALES

        [OperationContract] List<Canal> ListarCanals();

        [OperationContract] List<Canal> ListarCanalsPorId(int idCanal);

        [OperationContract] int InsertarCanal(Canal canal, string Username);

        [OperationContract] int ActualizarCanal(int id, Canal canal, string Username);

        [OperationContract] string EliminarCanal(int id, Canal canal, string Username);

        [OperationContract]
        List<CanalDetalle> ListarCanalDetalles();

        #endregion

        #region Metodos PREMIOSANTERIORES

        [OperationContract] List<PremiosAnteriore> ListarPremioAnterior();

        [OperationContract] int ActualizarPremioConsolidadoMes(string clave, int anio, int tipo, string Username);

        [OperationContract] string RetornarClavePremio(int id);

        [OperationContract] int RetornarAnioPremio(int id);

        [OperationContract] List<PremiosAnteriore> ListarPremioAnteriorPorId(int idPAnterior);

        [OperationContract] int InsertarPremioAnterior(PremiosAnteriore premioanterior, string Username);

        [OperationContract] int ActualizarPremioAnterior(int id, PremiosAnteriore premioanterior, string Username);

        [OperationContract] string EliminarPremioAnterior(int id, PremiosAnteriore premioanterior, string Username);

        #endregion

        #region Metodos VARIABLES

        [OperationContract] List<Variable> ListarVariables();

        [OperationContract] List<Variable> ListarVariablesPremio();

        [OperationContract] List<Variable> ListarVariablesPorId(int idVariable);

        [OperationContract] List<TempList> ListarTablas(int idtabla);

        #endregion

        #region Metodos TIPOVARIABLES

        [OperationContract] List<TipoVariable> ListarTipovariables();
        #endregion

        #region Metodos SALARIOSMINIMOS
        /// <summary>
        /// Obtiene listado de los Salarios Minimos
        /// </summary>
        /// <returns>Lista de objectos SalarioMinimo</returns>
        [OperationContract] List<SalarioMinimo> ListarSalariosMinimos();

        /// <summary>
        /// Obtiene listado de los Salarios Minimos por id
        /// </summary>
        /// <param name="idSalario">Id del Salario Minimo a consultar</param>
        /// <returns>Lista de objectos SalarioMinimo</returns>
        [OperationContract] List<SalarioMinimo> ListarSalariosMinimosPorId(int idSalario);

        /// <summary>
        /// Guarda un registro de Salario Minimo
        /// </summary>
        /// <param name="salario">Objecto SalarioMinimo a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarSalarioMinimo(SalarioMinimo salario, string Username);

        /// <summary>
        /// Actualiza un registro de Salario Minimo
        /// </summary>
        /// <param name="salario">Objeto SalarioMinimo utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarSalarioMinimo(SalarioMinimo salario, string Username);

        /// <summary>
        /// Elimina un registro de Salario Minimo
        /// </summary>
        /// <param name="salario">Objecto SalarioMinimo utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo string</returns>
        [OperationContract] string EliminarSalarioMinimo(SalarioMinimo salario, string Username);
        #endregion

        #region Metodos INGRESOSLOCALIDADES
        /// <summary>
        /// Obtiene listado de los Ingresos por Localidad
        /// </summary>
        /// <returns>Lista de objectos IngresoLocalidad</returns>
        [OperationContract] List<IngresoLocalidad> ListarIngresoLocalidades();

        /// <summary>
        /// Obtiene listado de los Ingresos por Localidad por id
        /// </summary>
        /// <param name="idIngreso">Id del Ingreso por Localidad a consultar</param>
        /// <returns>Lista de objectos IngresoLocalidad</returns>
        [OperationContract] List<IngresoLocalidad> ListarIngresoLocalidadesPorId(int idIngreso);

        /// <summary>
        /// Guarda un registro de Ingreso por Localidad
        /// </summary>
        /// <param name="ingreso">Objecto IngresoLocalidad a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarIngresoLocalidades(IngresoLocalidad ingreso, string Username);

        /// <summary>
        /// Actualiza un registro de Ingreso por Localidad
        /// </summary>
        /// <param name="ingreso">Objeto IngresoLocalidad utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarIngresoLocalidades(IngresoLocalidad ingreso, string Username);

        /// <summary>
        /// Elimina un registro de Ingreso por Localidad
        /// </summary>
        /// <param name="ingreso">Objecto IngresoLocalidad utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo string</returns>
        [OperationContract] string EliminarIngresoLocalidades(IngresoLocalidad ingreso, string Username);
        #endregion

        #region Metodos PERSISTENCIACAPI
        /// <summary>
        /// Obtiene listado de la tabla peristencia de CAPI detalle
        /// </summary>
        /// <returns>Lista de objectos PersistenciadeCAPIDetalle</returns>
        [OperationContract] List<PersistenciadeCAPIDetalle> ListarPersistenciaCAPIDetalle(string numeroNegocio, string clave);

        /// <summary>
        /// Obtiene listado de la tabla peristencia de CAPI detalle por id
        /// </summary>
        /// <param name="idPersistenciaCAPI">Id de la Persistencia CAPI Detalle a consultar</param>
        /// <returns>Lista de objectos PersistenciadeCAPIDetalle</returns>
        [OperationContract] List<PersistenciadeCAPIDetalle> ListarPersistenciaCAPIDetallePorId(int idPersistenciaCAPI);

        /// <summary>
        /// Actualiza un registro de la tabla peristencia de CAPI detalle
        /// </summary>
        /// <param name="persistenciacapidetalle">Objeto PersistenciadeCAPIDetalle utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarPersistenciaCAPIDetalle(int id, PersistenciadeCAPIDetalle persistenciacapidetalle, string Username);

        #endregion

        #region Metodos PARAMETRIZACIONEFICIENCIAARL

        /// <summary>
        /// Funcion que se encarga de traer la parametrizacion de eficiencia de ARL
        /// </summary>
        /// <returns>Listado de parametrizacion</returns>
        //[OperationContract] List<ParametrosEficienciaARL> ListarParametrizacionEficienciaARL()
        //{
        //    _eficienciaARL = new ParametrizacionEficienciaARLS();
        //    return _eficienciaARL.ListarParametrizacionEficienciaARL();
        //}

        /// <summary>
        /// Funcion que se encarga de traer la parametrizacion de la eficiencia ARL
        /// </summary>
        /// <param name="id">Identificador de la eficiencia a mostrar</param>
        /// <returns></returns>
        //[OperationContract] ParametrosEficienciaARL ListarParametrizacionEficienciaARLByID(int id)
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
        //[OperationContract] Result.result InsertParametrizacionEficienciaARL(ParametrosEficienciaARL parametrizacionEficienciaARL, string user)
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
        //[OperationContract] Result.result UpdateParametrizacionEficienciaARL(int id, ParametrosEficienciaARL parametrizacionEficiencia)
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
        //[OperationContract]
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
        [OperationContract] List<clave_historico> GetHistricoCambioClave();

        /// <summary>
        /// Funcion que se encarga de guardar los registros en la base de datos
        /// </summary>
        /// <param name="parametrizacionEficienciaARL">Datos para insercion</param>
        /// <returns></returns>
        [OperationContract] Result.result InsertParametrizacionCambioClave(clave_historico parametrizacionClave, string Username);

        /// <summary>
        /// Funcion que se encarga de actualizar los registros en la tabla Parametrizacion clave
        /// </summary>
        /// <param name="id">Identificador del registro a actualizar</param>
        /// <param name="parametrizacionEficiencia">Conjunto de datos para actualizar</param>
        /// <returns></returns>
        [OperationContract] Result.result UpdateParametrizacionCambioClave(int id, clave_historico parametrizacionClave, string Username);

        /// <summary>
        /// Funcion que se encarga de borrar el registro
        /// </summary>
        /// <param name="id">Identificador del registro a elimar</param>
        /// <param name="parametrizacionEficiencia">Datos a eliminar</param>
        /// <returns></returns>
        //[OperationContract] Result.result DeleteParametrizacionCambioClave(int id, clave_historico parametrizacionClave)
        //{
        //    //this._clave = new ParametrizacionCambioClaveS();
        //    //return this._clave.DeleteParametrizacionCambiolave(id, parametrizacionClave);

        //    return();
        //}

        #endregion



        //PRODUCTOS//******************************************

        #region Metodos LINEANEGOCIOS

        [OperationContract] List<LineaNegocio> ListarLineaNegocios();

        [OperationContract] List<LineaNegocio> ListarLineaNegociosPorId(int idLineaNegocio);

        #endregion

        #region Metodos RAMOS

        [OperationContract] List<Ramo> ListarRamos();

        [OperationContract] List<Ramo> ListarRamosPorId(int idRamo);

        [OperationContract] List<Ramo> ListarRamosPorCompania(int idCompania);

        [OperationContract] int InsertarRamo(Ramo ramo, string Username);

        [OperationContract] int ActualizarRamo(int id, Ramo ramo, string Username);

        [OperationContract] string EliminarRamo(int id, Ramo ramo, string Username);

        #endregion

        #region Metodos RAMODETALLE

        [OperationContract] List<RamoDetalle> ListarRamoDetalle(int id);

        [OperationContract] int AgruparRamoDetalle(int ramo_id, string ramosTrue, string ramosFalse);

        [OperationContract]
        List<RamoDetalle> ListarRamoDetalleXCompania(int companiaId);
        
        #endregion

        #region Metodos AMPAROS

        [OperationContract] List<Amparo> ListarAmparoes();

        [OperationContract] List<Amparo> ListarAmparoesPorId(int idAmparo);

        [OperationContract] int InsertarAmparo(Amparo amparo, ref string mensajeDeError, string Username);

        [OperationContract] void EliminarAmparo(int id, ref string mensajeDeError, string Username);

        [OperationContract] List<AmparoDetalle> ListarAmparoDetalle();

        [OperationContract] List<AmparoDetalle> ListarAmparoDetallePorId(int idamparo);

        [OperationContract] int InsertarAmparoDetalle(AmparoDetalle amparoDetalle, string Username);

        [OperationContract] int EliminarAmparoDetalle(int amparoid, ref string mensajeDeError, string Username);

        #endregion

        //PRODUCCION//*****************************************

        #region Metodos BENEFICIARIOS

        [OperationContract] List<Beneficiario> ListarBeneficiarios();

        [OperationContract] List<Beneficiario> ListarBeneficiariosPorId(int idBeneficiario);

        #endregion

        #region Metodos CLIENTES

        [OperationContract] List<Cliente> ListarClientes();

        #endregion

        #region REDES
        /// <summary>
        /// Obtiene listado de las redes
        /// </summary>
        /// <returns>Lista de redes</returns>
        [OperationContract] List<Red> ListarRedes();

        /// <summary>
        /// Obtiene listado de las redes por id
        /// </summary>
        /// <param name="idRed">Id de la red a consultar</param>
        /// <returns>Lista de objectos Red</returns>
        [OperationContract] List<Red> ListarRedesPorId(int idRed);

        /// <summary>
        /// Guarda un registro de red
        /// </summary>
        /// <param name="zona">Objecto Red a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarRed(Red red, string Username);

        /// <summary>
        /// Actualiza un registro de Red
        /// </summary>
        /// <param name="id">Id de la Red a modificar</param>
        /// <param name="zona">Objeto Red utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarRed(int id, Red red, string Username);

        /// <summary>
        /// Elimina un registro de Red
        /// </summary>
        /// <param name="id">Id de la Red a eliminar</param>
        /// <param name="zona">Objecto Red utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] string EliminarRed(int id, Red red, string Username);

        /// <summary>
        /// Obtiene listado de las rede del detalle para ser agrupados
        /// </summary>
        /// <returns>Lista de objectos Redes Detalle para agruparlos</returns>
        [OperationContract] List<RedDetalle> ListarRedesDetalle();

        /// <summary>
        /// Agrupa las redes detalle 
        /// </summary>
        /// <returns>True or False</returns>
        [OperationContract] int AgruparRedDetalle(RedDetalle redDetalle);

        /// <summary>
        /// Elimina la agrupacion las redes detalle 
        /// </summary>
        /// <returns>True or False</returns>
        [OperationContract] int EliminarAgrupacionRedDetalle(int idRed, string Username);

        #endregion

        #region BANCO
        /// <summary>
        /// Obtiene listado de los bancos agrupados
        /// </summary>
        /// <returns>Lista de bancos agrupados</returns>
        [OperationContract] List<Banco> ListarBancos();

        /// <summary>
        /// Obtiene listado de los bancos agrupados por id
        /// </summary>
        /// <param name="idBanco">Id del banco agrupado a consultar</param>
        /// <returns>Lista de objectos bancos</returns>
        [OperationContract] List<Banco> ListarBancosPorId(int idBanco);

        /// <summary>
        /// Guarda un registro de banco agrupado
        /// </summary>
        /// <param name="banco">Objecto Banco agrupado a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarBanco(Banco banco, string Username);

        /// <summary>
        /// Actualiza un registro de banco agrupado
        /// </summary>
        /// <param name="id">Id de la Banco agrupado a modificar</param>
        /// <param name="banco">Objeto Banco utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarBanco(int id, Banco banco, string Username);

        /// <summary>
        /// Elimina un registro de Red
        /// </summary>
        /// <param name="id">Id de la Red a eliminar</param>
        /// <param name="banco">Objecto Red utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] string EliminarBanco(int id, Banco banco, string Username);

        /// <summary>
        /// Obtiene listado de los bancos del detalle para ser agrupados
        /// </summary>
        /// <returns>Lista de objectos Banco Detalle para agruparlos</returns>
        [OperationContract] List<BancoDetalle> ListarBancosDetalle();

        /// <summary>
        /// Agrupa los bancos detalle 
        /// </summary>
        /// <returns>True or False</returns>
        [OperationContract] int AgruparBancoDetalle(BancoDetalle bancoDetalle);

        /// <summary>
        /// Elimina la agrupacion los bancos detalle 
        /// </summary>
        /// <returns>True or False</returns>
        [OperationContract] int EliminarAgrupacionBancoDetalle(int idBanco, string Username);

        #endregion

        //CONCURSOS//******************************************

        #region Metodos TIPOCONCURSOS

        [OperationContract] List<TipoConcurso> ListarTipoConcursoes();
        #endregion

        #region Metodos CONCURSOS

        [OperationContract] int InsertarConcurso(Concurso concurso, string Username);

        /// <summary>
        /// Obtiene listado de concursos
        /// </summary>
        /// <returns>Lista de concursos</returns>
        [OperationContract] List<Concurso> ListarConcursoes();

        /// <summary>
        /// Obtiene listado de concursos por id
        /// </summary>
        /// <returns>Lista de concursos</returns>
        [OperationContract] List<Concurso> ListarConcursoesPorId(int idConcurso);

        [OperationContract] int ActualizarConcurso(int id, Concurso concurso, string Username);

        [OperationContract] int DuplicarConcurso(int id, Concurso concurso, string Username);

        [OperationContract] int DuplicarParticipanteConcurso(int id, int idNuevo, string Username);

        [OperationContract] string EliminarConcursos(int id, Concurso concurso, string Username);

        [OperationContract] int EliminarLiquidacionConcurso(int liqregla, string Username);

        [OperationContract] int validarEstadoLiquidacion(int regla_id, int concurso_id);

        [OperationContract] List<DetalleLiquidacionEncabezado> TipoConcursoxRegla(int liquidacionRegla_id, string valorCOnsulta);

        [OperationContract] int validarParticipantesxConcurso(int concurso_id);

        [OperationContract] int RetornarTipoConcurso(int concurso_id);

        [OperationContract] int ValidarConcursoPrincipal(int tipoConcurso_id, int añofIni, int añofFin);

        [OperationContract] string RetornarNombreSegmentoUsuario(int segmento_id);
        #endregion

        #region Metodos CATEGORIASXREGLA

        [OperationContract] List<CategoriaxRegla> ListarCategoriasxRegla(int regla_id);

        [OperationContract] int ActualizarCategoriaxRegla(List<CategoriaxRegla> categoriasxRegla, string Username);

        #endregion

        #region Metodos MONEDAXNEGOCIO

        [OperationContract] int InsertarMonedaxNegocio(MonedaxNegocio monedaxnegocio, string Username);

        [OperationContract] List<MonedaxNegocio> ListarMonedaxNegocio();

        [OperationContract] List<MonedaxNegocio> ListarMonedaxNegocioPorId(int idMonedaxNegocio);

        [OperationContract] int ActualizarMonedaxNegocio(int id, MonedaxNegocio monedaxnegocio, string Username);

        [OperationContract] string EliminarMonedaxNegocio(int id, MonedaxNegocio monedaxnegocio, string Username);
        #endregion

        #region Metodos MAESTROMONEDAXNEGOCIO

        [OperationContract] int InsertarMaestroMonedaxNegocio(MaestroMonedaxNegocio maestromonedaxnegocio, string Username);

        [OperationContract] List<MaestroMonedaxNegocio> ListarMaestroMonedaxNegocio();

        [OperationContract] List<MaestroMonedaxNegocio> ListarMaestroMonedaxNegocioPorId(int idMaestroMonedaxNegocio);

        [OperationContract] int ActualizarMaestroMonedaxNegocio(int id, MaestroMonedaxNegocio maestromonedaxnegocio, string Username);

        [OperationContract] string EliminarMaestroMonedaxNegocio(int id, MaestroMonedaxNegocio maestromonedaxnegocio, string Username);

        [OperationContract] Dictionary<int, string> ListarSegmentoxUsuario(string userName);

        [OperationContract] List<UsuarioxSegmento> ListarSegmentodelUsuario();
        #endregion

        #region Metodos TOPEMONEDA

        [OperationContract] int InsertarTopeMoneda(TopeMoneda topemoneda, string Username);

        [OperationContract] List<TopeMoneda> ListarTopeMoneda();

        [OperationContract] List<TopeMoneda> ListarTopeMonedaPorId(int idTopeMoneda);

        [OperationContract] int ActualizarTopeMoneda(int id, TopeMoneda topemoneda, string Username);

        [OperationContract] string EliminarTopeMoneda(int id, TopeMoneda topemoneda, string Username);
        #endregion

        #region Metodos TOPEXEDAD

        [OperationContract] int InsertarTopexEdad(TopexEdad topexedad, string Username);

        [OperationContract] List<TopexEdad> ListarTopexEdad();

        [OperationContract] List<TopexEdad> ListarTopexEdadPorId(int idTopexEdad);

        [OperationContract] int ActualizarTopexEdad(int id, TopexEdad topexedad, string Username);

        [OperationContract] string EliminarTopexEdad(int id, TopexEdad topexedad, string Username);
        #endregion

        #region Metodos COMPANIAXETAPA

        [OperationContract] int InsertarCompaniaxEtapa(CompaniaxEtapa companiaxetapa, string Username);

        [OperationContract] List<CompaniaxEtapa> ListarCompaniaxEtapa();

        [OperationContract] List<CompaniaxEtapa> ListarCompaniaxEtapaPorId(int idCompaniaxEtapa);

        [OperationContract] int ActualizarCompaniaxEtapa(int id, CompaniaxEtapa companiaxetapa, string Username);

        [OperationContract] string EliminarCompaniaxEtapa(int id, CompaniaxEtapa companiaxetapa, string Username);
        #endregion

        #region Metodos REGLA

        [OperationContract] int InsertarRegla(Regla regla, string Username);

        [OperationContract] List<Regla> ListarRegla();

        [OperationContract] List<Regla> ListarReglaPorId(int idRegla);

        [OperationContract] List<Regla> ListarReglaPorConcursoId(int idConcurso);

        [OperationContract] int ActualizarRegla(int id, Regla regla, string Username);

        [OperationContract] int DuplicarRegla(int id, Regla regla, string Username);

        [OperationContract] List<Regla> ListarPremiosParaAsociar(int concurso_id, int regla_id);

        [OperationContract] string EliminarRegla(int id, Regla regla, string Username);
        #endregion

        #region Metodos PERIODOREGLA

        [OperationContract] List<PeriodoRegla> ListarPeriodoRegla();

        #endregion

        #region Metodos TIPOREGLA

        [OperationContract] List<TipoRegla> ListarTipoRegla();

        #endregion

        #region Metodos OPERADOR

        [OperationContract] List<Operador> ListarOperadorLogico();

        [OperationContract] List<Operador> ListarOperadorMatematico();

        #endregion

        #region Metodos tabla

        [OperationContract] List<tabla> Listartabla();

        #endregion

        #region Metodos ESTRATEGIAREGLA

        [OperationContract] List<EstrategiaRegla> ListarEstrategiaRegla();

        #endregion

        #region Metodos SUBREGLA

        [OperationContract] int InsertarSubRegla(SubRegla subregla, string Username);

        [OperationContract] List<SubRegla> ListarSubRegla();

        [OperationContract] List<SubRegla> ListarSubReglaPorId(int idSubRegla);

        [OperationContract] int ActualizarSubRegla(int id, SubRegla subregla, string Username);

        [OperationContract] string EliminarSubRegla(int id, SubRegla subregla, string Username);
        #endregion

        #region Metodos CONDICION

        [OperationContract] int InsertarCondicion(Condicion condicion, string Username);

        [OperationContract] int ContarVariablexLiquidacion(int concurso_id, int regla_id, int subregla_id);

        [OperationContract] int RetornarTipoTablaxVariable(int subregla_id);

        [OperationContract] int RetornarTipoSubRegla(int subregla_id);

        [OperationContract] string RetornarTipoDato(int variable_id);

        [OperationContract] List<Variable> ListarVariablesxTabla(int subregla_id);

        [OperationContract] List<Condicion> ListarCondicion();

        [OperationContract] List<Condicion> ListarCondicionPorId(int idCondicion);

        [OperationContract] int MostrarTipodeVariable(Condicion condicion);

        [OperationContract] int ActualizarCondicion(int id, Condicion condicion, string Username);

        [OperationContract] string EliminarCondicion(int id, Condicion condicion, string Username);
        #endregion

        #region Metodos CONDICIONXPREMIOSUBREGLA

        [OperationContract] List<CondicionxPremioSubregla> ListarCondicionxPremioSubRegla();

        [OperationContract] CondicionxPremioSubregla ListarCondicionxPremioSubReglaPorId(int id);

        [OperationContract] int InsertarCondicionxPremioSubRegla(CondicionxPremioSubregla condicionPS, string Username);

        [OperationContract] int ActualizarCondicionxPremioSubRegla(int id, CondicionxPremioSubregla condicionPS, string Username);

        [OperationContract] string EliminarCondicionxPremioSubRegla(int id, CondicionxPremioSubregla condicionPS, string Username);

        #endregion

        #region Metodos CONDICIONAGRUPADA

        [OperationContract] List<CondicionAgrupada> ListarCondicionAgrupada();

        [OperationContract] List<CondicionAgrupada> ListarCondicionAgrupadaPorId(int idCondicionAgrupada);

        [OperationContract] int InsertarCondicionAgrupada(CondicionAgrupada condicionagrupada, string Username);

        [OperationContract] int ActualizarCondicionAgrupada(int id, CondicionAgrupada condicionagrupada, string Username);

        [OperationContract] List<Condicion> ListarCondicionPorSubRegla(int idSubRegla);

        [OperationContract] string EliminarCondicionAgrupada(int id, CondicionAgrupada condicionagrupada, string Username);

        [OperationContract] List<Operador> ListarOperadorAgrupado();
        #endregion

        #region Metodos PREMIOS

        [OperationContract] int InsertarPremio(Premio premio, string Username);

        [OperationContract] List<Premio> ListarPremio();

        [OperationContract] List<Premio> ListarPremioPorId(int idPremio);

        [OperationContract] int ActualizarPremio(int id, Premio premio, string Username);

        [OperationContract] string EliminarPremio(int id, Premio premio, string Username);
        #endregion

        #region Metodos CONCEPTODESCUENTO

        [OperationContract] List<ConceptoDescuento> ListarConceptoDescuento();

        [OperationContract] int InsertarReglaxConceptoDescuento(string conceptoDescuento, int idRegla, InfoAplicacion info, string Username);

        #endregion

        #region Metodos TIPOPREMIOS

        [OperationContract] int InsertarTipoPremio(TipoPremio tipopremio, string Username);

        [OperationContract] List<TipoPremio> ListarTipoPremio();

        [OperationContract] List<TipoPremio> ListarTipoPremioPorId(int idTipoPremio);

        [OperationContract] int ActualizarTipoPremio(int id, TipoPremio tipopremio, string Username);

        [OperationContract] string EliminarTipoPremio(int id, TipoPremio tipopremio, string Username);
        #endregion

        #region Metodos UNIDADMEDIDA

        [OperationContract] int InsertarUnidadMedida(UnidadMedida unidadmedida, string Username);

        [OperationContract] List<UnidadMedida> ListarUnidadMedida();

        [OperationContract] List<UnidadMedida> ListaUnidadMedidaPorId(int idUnidadMedida);

        [OperationContract] int ActualizarUnidadMedida(int id, UnidadMedida unidadmedida, string Username);

        [OperationContract] string EliminarUnidadMedida(int id, UnidadMedida unidadmedida, string Username);
        #endregion

        #region Metodos TIPOUNIDADMEDIDA

        [OperationContract] int InsertarTipoUnidadMedida(TipoUnidadMedida tipounidadmedida, string Username);

        [OperationContract] List<TipoUnidadMedida> ListarTipoUnidadMedida();

        [OperationContract] List<TipoUnidadMedida> ListaTipoUnidadMedidaPorId(int idTipoUnidadMedida);

        [OperationContract]
        int ActualizarTipoUnidadMedida(int id, TipoUnidadMedida tipounidadmedida, string Username);

        [OperationContract] string EliminarTipoUnidadMedida(int id, TipoUnidadMedida tipounidadmedida, string Username);
        #endregion

        #region Metodos PREMIOXSUBREGLA

        [OperationContract] int InsertarPremioxSubregla(PremioxSubregla premioxsubregla, string Username);

        [OperationContract] List<PremioxSubregla> ListarPremioxSubregla();

        [OperationContract] List<PremioxSubregla> ListaPremioxSubreglaPorId(int idPremioxSubRegla);

        [OperationContract] int ActualizarPremioxSubregla(int id, PremioxSubregla premioxsubregla, string Username);

        [OperationContract] string EliminarPremioxSubregla(int id, PremioxSubregla premioxsubregla, string Username);
        #endregion

        #region Metodos PARTICIPANTECONCURSOS

        [OperationContract] int InsertarParticipanteConcurso(ParticipanteConcurso participanteconcurso, string Username);

        [OperationContract] List<Participante> ListarParticipantesTotales();

        [OperationContract] List<ParticipanteConcurso> ListarParticipanteConcursoes();

        [OperationContract] List<ParticipanteConcurso> ListarParticipanteConcursoesPorId(int idParticipanteConcurso);

        [OperationContract] int ActualizarParticipanteConcurso(int id, ParticipanteConcurso participanteconcurso, string Username);

        [OperationContract] string EliminarParticipanteConcurso(int id, ParticipanteConcurso participanteconcurso, string Username);
        #endregion

        #region Metodos PRODUCTOCONCURSOS

        [OperationContract] int InsertarProductoConcurso(ProductoConcurso productoconcurso, string Username);

        [OperationContract] List<ProductoConcurso> ListarProductoConcursoes();

        [OperationContract] List<ProductoConcurso> ListarProductoConcursoesPorId(int idProductoConcurso);

        [OperationContract] int ActualizarProductoConcurso(int id, ProductoConcurso productoconcurso, string Username);

        [OperationContract] string EliminarProductoConcurso(int id, ProductoConcurso productoconcurso, string Username);
        #endregion

        #region Metodos ETAPAPRODUCTO

        [OperationContract] int InsertarEtapaProducto(EtapaProducto etapaproducto, string Username);

        [OperationContract] List<EtapaProducto> ListarEtapaProductoes();

        [OperationContract] List<EtapaProducto> ListarEtapaProductoesPorId(int idEtapaProducto);

        [OperationContract] int ActualizarEtapaProducto(int id, EtapaProducto etapaproducto, string Username);

        [OperationContract] string EliminarEtapaProducto(int id, EtapaProducto etapaproducto, string Username);
        #endregion

        #region MONEDA
        [OperationContract] List<Moneda> ListarMonedas();

        /// <summary>
        /// Obtiene listado de las redes por id
        /// </summary>
        /// <param name="idMoneda">Id de la red a consultar</param>
        /// <returns>Lista de objecto Red</returns>
        [OperationContract] List<Moneda> ListarMonedasPorId(int idMoneda);

        /// <summary>
        /// Guarda un registro de red
        /// </summary>
        /// <param name="moneda">Objecto Red a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarMoneda(Moneda moneda, string Username);

        /// <summary>
        /// Actualiza un registro de Red
        /// </summary>
        /// <param name="id">Id de la Red a modificar</param>
        /// <param name="red">Objeto Red utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarMoneda(int id, Moneda moneda, string Username);

        /// <summary>
        /// Elimina un registro de Red
        /// </summary>
        /// <param name="id">Id de la Red a eliminar</param>
        /// <param name="red">Objecto Red utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] string EliminarMoneda(int id, Moneda moneda, string Username);

        /// <summary>
        /// Obtiene listado de las unidades de medida
        /// </summary>
        /// <returns>Lista de Unidades de medida</returns>
        [OperationContract] List<UnidadMedida> ListarUnidadesMedida();
        #endregion

        #region NIVEL

        [OperationContract] List<Nivel> ListarNivels();

        [OperationContract] List<Nivel> ListarNivelsPorId(int idNivel);

        [OperationContract] int InsertarNivel(Nivel nivel, string Username);

        [OperationContract] int ActualizarNivel(int id, Nivel nivel, string Username);

        [OperationContract] string EliminarNivel(int id, Nivel nivel, string Username);

        #endregion

        #region ANTIGUEDAD
        [OperationContract] List<AntiguedadxNivel> ListarAntiguedades();

        [OperationContract] List<AntiguedadxNivel> ListarAntiguedadesPorId(int id);

        [OperationContract] int InsertarAntiguedadxNivel(AntiguedadxNivel antiguedad, string Username);

        [OperationContract] int ActualizarAntiguedadxNivel(int id, AntiguedadxNivel antiguedad, string Username);

        [OperationContract] string EliminarAntiguedadxNivel(int id, AntiguedadxNivel antiguedad, string Username);

        #endregion

        #region BASE MONEDA

        /// <summary>
        /// Obtiene listado de las Monedas
        /// </summary>
        /// <returns>Lista de Monedas</returns>
        [OperationContract] List<BaseMoneda> ListarBaseMoneda();

        /// <summary>
        /// Obtiene listado de las BaseMonedas por id
        /// </summary>
        /// <param name="id">Id de la BaseMoneda a consultar</param>
        /// <returns>Lista de objectos BaseMoneda</returns>
        [OperationContract] List<BaseMoneda> ListarBaseMonedasPorId(int id);

        /// <summary>
        /// Guarda un registro de BaseMoneda
        /// </summary>
        /// <param name="baseMoneda">Objecto BaseMoneda a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarBaseMoneda(BaseMoneda baseMoneda, string Username);

        /// <summary>
        /// Actualiza un registro de BaseMoneda
        /// </summary>
        /// <param name="id">Id de la BaseMoneda a modificar</param>
        /// <param name="baseMoneda">Objeto BaseMoneda utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarBaseMoneda(int id, BaseMoneda baseMoneda, string Username);

        /// <summary>
        /// Elimina un registro de BaseMoneda
        /// </summary>
        /// <param name="id">Id de la BaseMoneda a eliminar</param>
        /// <param name="baseMoneda">Objecto BaseMoneda utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] string EliminarBaseMoneda(int id, BaseMoneda baseMoneda, string Username);

        #endregion

        #region BASE POR PARTICIPANTE
        /// <summary>
        /// Obtiene listado de BasexParticipante
        /// </summary>
        /// <returns>Lista de BasexParticipante</returns>
        [OperationContract] List<BasexParticipante> ListarBasexParticipantes();

        /// <summary>
        /// Obtiene listado de los Participantes
        /// </summary>
        /// <returns>Lista de Participantes</returns>
        [OperationContract] List<Participante> ListarBaseYParticipantes();

        /// <summary>
        /// Obtiene listado de BasexParticipante por id
        /// </summary>
        /// <param name="id">Id de la BasexParticipante a consultar</param>
        /// <returns>Lista de objectos BasexParticipante</returns>
        [OperationContract] List<BasexParticipante> ListarBasexParticipantesPorId(int id);

        /// <summary>
        /// Guarda un registro de BasexParticipante
        /// </summary>
        /// <param name="basexParticipante">Objecto BasexParticipante a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarBasexParticipante(BasexParticipante basexParticipante, string Username);

        /// <summary>
        /// Actualiza un registro de BasexParticipante
        /// </summary>
        /// <param name="id">Id de BasexParticipante a modificar</param>
        /// <param name="basexParticipante">Objeto BasexParticipante utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarBasexParticipante(int id, BasexParticipante basexParticipante, string Username);

        /// <summary>
        /// Elimina un registro de BasexParticipante
        /// </summary>
        /// <param name="id">Id de BasexParticipante a eliminar</param>
        /// <param name="basexParticipante">Objecto BasexParticipante utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] string EliminarBasexParticipante(int id, BasexParticipante basexParticipante, string Username);
        #endregion

        #region CATEGORIA
        /// <summary>
        /// Obtiene listado de las Categorias
        /// </summary>
        /// <returns>Lista de Categorias</returns>
        [OperationContract] List<Categoria> ListarCategorias();

        /// <summary>
        /// Obtiene listado de Categorias por id
        /// </summary>
        /// <param name="id">Id de la Categoria a consultar</param>
        /// <returns>Lista de objectos Categoria</returns>
        [OperationContract] List<Categoria> ListarCategoriasPorId(int id);

        /// <summary>
        /// Guarda un registro de Categoria
        /// </summary>
        /// <param name="categoria">Objecto Categoria a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarCategoria(Categoria categoria, string Username);

        /// <summary>
        /// Actualiza un registro de Categoria
        /// </summary>
        /// <param name="id">Id de la Categoria a modificar</param>
        /// <param name="categoria">Objeto Categoria utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarCategoria(int id, Categoria categoria, string Username);

        /// <summary>
        /// Elimina un registro de Categoria
        /// </summary>
        /// <param name="id">Id de la Categoria a eliminar</param>
        /// <param name="categoria">Objecto Categoria utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] string EliminarCategoria(int id, Categoria categoria, string Username);
        #endregion

        #region ESCALA NOTA
        /// <summary>
        /// Obtiene listado de EscalaNota
        /// </summary>
        /// <returns>Lista de EscalaNotas</returns>
        [OperationContract] List<EscalaNota> ListarEscalaNotas();

        /// <summary>
        /// Obtiene listado de EscalaNota por id
        /// </summary>
        /// <param name="id">Id de la EscalaNota a consultar</param>
        /// <returns>Lista de objectos EscalaNota</returns>
        [OperationContract] List<EscalaNota> ListarEscalaNotasPorId(int id);

        /// <summary>
        /// Guarda un registro de EscalaNota
        /// </summary>
        /// <param name="escalaNota">Objecto EscalaNota a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarEscalaNota(EscalaNota escalaNota, string Username);

        /// <summary>
        /// Actualiza un registro de EscalaNota
        /// </summary>
        /// <param name="id">Id de la EscalaNota a modificar</param>
        /// <param name="escalaNota">Objeto EscalaNota utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarEscalaNota(int id, EscalaNota escalaNota, string Username);

        /// <summary>
        /// Elimina un registro de EscalaNota
        /// </summary>
        /// <param name="id">Id de la EscalaNota a eliminar</param>
        /// <param name="escalaNota">Objecto EscalaNota utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] string EliminarEscalaNota(int id, EscalaNota escalaNota, string Username);
        #endregion

        #region FACTOR VARIABLE
        /// <summary>
        /// Obtiene listado de las FactorVariable
        /// </summary>
        /// <returns>Lista de FactorVariable</returns>
        [OperationContract] List<FactorVariable> ListarFactorVariables();

        /// <summary>
        /// Obtiene listado de FactorVariable por id
        /// </summary>
        /// <param name="id">Id del FactorVariable a consultar</param>
        /// <returns>Lista de objectos FactorVariable</returns>
        [OperationContract] List<FactorVariable> ListarFactorVariablesPorId(int id);

        /// <summary>
        /// Guarda un registro del FactorVariable
        /// </summary>
        /// <param name="factorVariable">Objecto FactorVariable a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarFactorVariable(FactorVariable factorVariable, string Username);

        /// <summary>
        /// Actualiza un registro del FactorVariable
        /// </summary>
        /// <param name="id">Id del FactorVariable a modificar</param>
        /// <param name="factorVariable">Objeto FactorVariable utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarFactorVariable(int id, FactorVariable factorVariable, string Username);

        /// <summary>
        /// Elimina un registro de FactorVariable
        /// </summary>
        /// <param name="id">Id del FactorVariable a eliminar</param>
        /// <param name="factorVariable">Objecto FactorVariable utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] string EliminarFactorVariable(int id, FactorVariable factorVariable, string Username);

        //[OperationContract] List<Variable> ListarVariablesPortabla(int idtabla)
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
        [OperationContract] List<FactorxNota> ListarFactorxNotas();

        /// <summary>
        /// Obtiene listado de FactorxNotas por id
        /// </summary>
        /// <param name="id">Id de la FactorxNota a consultar</param>
        /// <returns>Lista de objectos FactorxNota</returns>
        [OperationContract] List<FactorxNota> ListarFactorxNotasPorId(int id);

        [OperationContract] List<PeriodoFactorxNota> ListarPeriodoFactorxNotasPorFactor(int id);

        [OperationContract] List<FactorxNotaDetalle> ListarFactorxNotaDetallesPorPeriodo(int id);

        /// <summary>
        /// Guarda un registro de FactorxNota
        /// </summary>
        /// <param name="factorxNota">Objecto FactorxNota a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarFactorxNota(FactorxNota factorxNota, string Username);

        [OperationContract] int InsertarPeriodoFactorxNota(PeriodoFactorxNota periodo, string Username);

        [OperationContract] int InsertarFactorxNotaDetalle(FactorxNotaDetalle detalle, string Username);

        /// <summary>
        /// Actualiza un registro de FactorxNota
        /// </summary>
        /// <param name="id">Id del FactorxNota a modificar</param>
        /// <param name="factorxNota">Objeto FactorxNota utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarFactorxNota(int id, FactorxNota factorxNota, string Username);

        /// <summary>
        /// Elimina un registro de FactorxNota
        /// </summary>
        /// <param name="id">Id del FactorxNota a eliminar</param>
        /// <param name="factorxNota">Objecto FactorxNota utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] string EliminarFactorxNota(int id, FactorxNota factorxNota, string Username);

        [OperationContract] string EliminarPeriodoFactorxNota(int id, PeriodoFactorxNota periodo, string Username);

        [OperationContract] string EliminarFactorxNotaDetalle(int id, FactorxNotaDetalle detalle, string Username);
        #endregion

        #region GRUPO ENDOSO
        /// <summary>
        /// Obtiene listado de GrupoEndoso
        /// </summary>
        /// <returns>Lista de GrupoEndoso</returns>
        [OperationContract] List<GrupoEndoso> ListarGrupoEndosos();

        /// <summary>
        /// Obtiene listado de GrupoEndoso por id
        /// </summary>
        /// <param name="id">Id del GrupoEndoso a consultar</param>
        /// <returns>Lista de objectos GrupoEndoso</returns>
        [OperationContract] List<GrupoEndoso> ListarGrupoEndososPorId(int id);

        /// <summary>
        /// Guarda un registro de GrupoEndoso
        /// </summary>
        /// <param name="grupoEndoso">Objecto GrupoEndoso a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarGrupoEndoso(GrupoEndoso grupoEndoso, string Username);

        /// <summary>
        /// Actualiza un registro de GrupoEndoso
        /// </summary>
        /// <param name="id">Id del GrupoEndoso a modificar</param>
        /// <param name="grupoEndoso">Objeto GrupoEndoso utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarGrupoEndoso(int id, GrupoEndoso grupoEndoso, string Username);

        /// <summary>
        /// Elimina un registro de GrupoEndoso
        /// </summary>
        /// <param name="id">Id del GrupoEndoso a eliminar</param>
        /// <param name="grupoEndoso">Objecto GrupoEndoso utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] string EliminarGrupoEndoso(int id, GrupoEndoso grupoEndoso, string Username);
        #endregion

        #region Metodos SiteMap
        [OperationContract] List<Entidades.SiteMap> ListarSiteMap();

        [OperationContract] int InsertarSiteMap(Entidades.SiteMap sitemap, string Username);

        [OperationContract] List<Entidades.SiteMap> ListarSiteMapPorId(string idSiteMap);

        [OperationContract] int ActualizarsiteMap(string id, Entidades.SiteMap sitemap, string Username);

        [OperationContract] int Eliminarsitemap(string id, Entidades.SiteMap sitemap, string Username);

        [OperationContract] List<aspnet_Roles> ListarRoles();

        [OperationContract] aspnet_Roles GetRolById(System.Guid id);

        #endregion

        #region PARTICIPANTE
        /// <summary>
        /// Obtiene listado de los Participantes
        /// </summary>
        /// <returns>Lista de Participantes</returns>
        [OperationContract] List<Participante> ListarParticipantes(int? nivelGP);

        [OperationContract] List<TipoDocumento> ListarTipodocumentoes();

        [OperationContract] List<TipoParticipante> ListarTipoparticipantes();

        [OperationContract] List<EstadoParticipante> ListarEstadoparticipantes();
        /// <summary>
        /// Obtiene listado de Participantes por id
        /// </summary>
        /// <param name="idRed">Id des Participante a consultar</param>
        /// <returns>Lista de objetos Participante</returns>
        /// 
        [OperationContract] List<Participante> ListarParticipantesIndex(int inicio, int cantidad, int zona_id);

        [OperationContract] List<Participante> ListarParticipantesBuscador(string texto, int inicio, int cantidad, int nivel, int zona_id);

        [OperationContract] List<JerarquiaDetalle> ListarJerarquiaIndex(int inicio, int cantidad, int zona_id);

        [OperationContract] List<JerarquiaDetalle> ListarJerarquiaBuscador(string texto, int inicio, int cantidad, int nivel, int zona_id);

        [OperationContract] List<Participante> ListarParticipantesPorId(int id);

        [OperationContract] List<Participante> ListarParticipanteXCedula(string cedula);
        /// <summary>
        /// Guarda un registro de Participante
        /// </summary>
        /// <param name="zona">Objeto Participante a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarParticipante(Participante participante, string userName);

        /// <summary>
        /// Actualiza un registro de Participante
        /// </summary>
        /// <param name="id">Id del Participante a modificar</param>
        /// <param name="zona">Objeto Participante utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarParticipante(int id, Participante participante, string userName);

        /// <summary>
        /// Elimina un registro de Participante
        /// </summary>
        /// <param name="id">Id del Participante a eliminar</param>
        /// <param name="zona">Objecto Participante utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] string EliminarParticipante(int id, Participante participante, string userName);
        #endregion

        #region TIPO DE ESCALAS
        /// <summary>
        /// Obtiene listado de los tipos de escalas
        /// </summary>
        /// <returns>Lista de TipoEscala</returns>
        [OperationContract] List<TipoEscala> ListarTipoEscalas();

        /// <summary>
        /// Obtiene listado de los tipos de escalas por id
        /// </summary>
        /// <param name="id">Id del TipoEscala a consultar</param>
        /// <returns>Lista de objetos TipoEscala</returns>
        [OperationContract] List<TipoEscala> ListarTipoEscalasPorId(int id);

        /// <summary>
        /// Guarda un registro de TipoEscala
        /// </summary>
        /// <param name="tipoEscala">Objeto TipoEscala a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarTipoEscala(TipoEscala tipoEscala, string Username);

        /// <summary>
        /// Actualiza un registro de TipoEscala
        /// </summary>
        /// <param name="id">Id del TipoEscala a modificar</param>
        /// <param name="tipoEscala">Objeto TipoEscala utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarTipoEscala(int id, TipoEscala tipoEscala, string Username);

        /// <summary>
        /// Elimina un registro de TipoEscala
        /// </summary>
        /// <param name="id">Id del TipoEscala a eliminar</param>
        /// <param name="tipoEscala">Objeto TipoEscala utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] string EliminarTipoEscala(int id, TipoEscala tipoEscala, string Username);
        #endregion

        #region Metodos METAS

        [OperationContract] List<Meta> ListarMetas();

        [OperationContract] List<Meta> ListarMetasPorId(int idMeta);

        [OperationContract] List<Meta> ListarMetasMensuales(int idMeta);

        [OperationContract] int InsertarMeta(Meta meta, string Username);

        [OperationContract] int ActualizarMeta(int id, Meta meta, string Username);

        [OperationContract] int ActualizarMetaAcumulada(int id, Meta meta, string Username);

        [OperationContract] int EliminarMeta(int idMeta, string Username);

        [OperationContract] List<ProductosMeta> ListarProductosMetaPorId(int id);

        [OperationContract] int InsertarProductoMeta(ProductosMeta productometa, string Username);

        [OperationContract] int ActualizarProductoMeta(int id, ProductosMeta productometa, string Username);

        [OperationContract] int EliminarProductoMeta(int idMeta, string Username);

        [OperationContract] List<MetaCompuesta> ListarMetaCompuestaPorId(int id);

        [OperationContract] int InsertarMetaCompuesta(int idMetaDestino, int idMetaOrigen, string Username);

        [OperationContract] int EliminarMetaCompuesta(int idMeta, string Username);

        //EL REQUERIMIENTO SE DIJO QUE NO SE IBA HACER. 
        //SE DEJA COMENTADO POR SI HAY LA POSIBILIDAD QUE EL NEGOCIO LO APRUEBE NUEVAMENTE
        /*
        [OperationContract] List<MetaValidacionCumplimiento> ListarMetaValidacionCumplimientoPorId(int id)
        {
            _metas = new Metas();
            return _metas.ListarMetaValidacionCumplimientoPorId(id);
        }

        [OperationContract] int InsertarMetaValidacionCumplimiento(int idMetaValidacion, int idMetaReponderacion)
        {
            _metas = new Metas();
            return _metas.InsertarMetaValidacionCumplimiento(idMetaValidacion, idMetaReponderacion);
        }

        [OperationContract] int EliminarMetaValidacionCumplimiento(int idMetaValidacion)
        {
            _metas = new Metas();
            return _metas.EliminarMetaValidacionCumplimiento(idMetaValidacion);
        } 
        */

        #endregion

        #region Metodos PRODUCTOS

        [OperationContract] List<Producto> ListarProductoes();

        [OperationContract] List<Producto> ListarProductoesPorId(int idProducto);

        [OperationContract] List<Producto> ListarProductosporRamo(int idRamo);

        [OperationContract] List<ProductoDetalle> ListarProductoDetalles(int id);

        [OperationContract] int AgruparProductoDetalle(int producto_id, string productosTrue, string productosFalse);

        [OperationContract] int InsertarProducto(Producto producto, string Username);

        [OperationContract] string EliminarProducto(int id, string Username);

        [OperationContract]
        List<ProductoDetalle> ListarProductoDetalleXRamoDetalle(int ramoDetalleId);
        #endregion

        #region Metodos TIPOMETA

        [OperationContract] List<TipoMeta> ListarTipometas();
        #endregion

        #region Metodos PLANES

        [OperationContract] List<Plan> ListarPlans();

        [OperationContract] List<Plan> ListarPlansPorId(int idPlan);

        [OperationContract] List<Plan> ListarPlansPorProducto(int productoid);

        [OperationContract] List<Plan> ListarPlanPorProducto(int idProducto);

        [OperationContract] int InsertarPlan(Plan plan, string Username);

        [OperationContract] string EliminarPlan(int id, string Username);

        [OperationContract] List<PlanDetalle> ListarPlanDetalles(int id);

        [OperationContract] int AgruparPlanDetalle(int plan_id, string planesTrue, string planesFalse);

        [OperationContract]
        List<PlanDetalle> ListarPlanDetalleXProductoDetalle(int productoDetalleId);

        [OperationContract]
        List<PlanDetalle> ListarPlanDetalleActivosXProductoDetalle(int productoDetalleId);
        #endregion

        #region Metodos PLAZO

        [OperationContract] List<Plazo> ListarPlazoes();
        #endregion

        #region Metodos MODALIDADPAGO

        [OperationContract] List<ModalidadPago> ListarModalidadPagoes();
        #endregion

        #region MODELOS
        /// <summary>
        /// Obtiene listado de los Modelos
        /// </summary>
        /// <returns>Lista de Modelos</returns>
        [OperationContract] List<Modelo> ListarModelos();

        /// <summary>
        /// Obtiene listado de los Modelos por id
        /// </summary>
        /// <param name="id">Id del Nivel a consultar</param>
        /// <returns>Lista de objectos Modelos</returns>
        [OperationContract] List<Modelo> ListarModelosPorId(int id);

        [OperationContract] List<ModeloxMeta> ListarModelosxMetaPorIdModelo(int id);

        /// <summary>
        /// Obtiene listado de los participantes que pertenecen un determinado modelo
        /// </summary>
        /// <param name="id">Id del modelo</param>
        /// <returns>Lista de objectos ModeloxParticipante</returns>
        [OperationContract] List<ModeloxNodo> ListarModeloxParticipantesPorId(int id);

        /// <summary>
        /// Guarda un registro de Modelo
        /// </summary>
        /// <param name="modelo">Objecto Modelo a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarModelo(Modelo modelo, string Username);

        [OperationContract] string AsociarFactorToModelo(int modelo, int factor);

        /// <summary>
        /// Guarda un registro de ModeloxMeta
        /// </summary>
        /// <param name="modeloxMeta">Objecto ModeloxMeta a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarModeloxMeta(ModeloxMeta modeloxMeta, string Username);

        [OperationContract] int CargarModeloxMeta(List<ModelosContratacion> modeloxMeta, int esUltimaHoja);

        [OperationContract] int InsertarModeloxParticipante(ModeloxNodo modeloxPart, string Username);

        /// <summary>
        /// Actualiza un registro de Modelo
        /// </summary>
        /// <param name="id">Id del Modelo a modificar</param>
        /// <param name="modelo">Objeto Modelo utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarModelo(int id, Modelo modelo, string Username);

        [OperationContract] int ActualizarModeloxMeta(int id, ModeloxMeta modeloxMeta, string Username);

        /// <summary>
        /// Elimina un registro de Modelo
        /// </summary>
        /// <param name="id">Id del Modelo a eliminar</param>
        /// <param name="nivel">Objecto Modelo utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] string EliminarModelo(int id, Modelo modelo, string Username);

        [OperationContract] int EliminarModeloxMetaPorModelos(List<int> codigosModelo, string Username);

        [OperationContract] string EliminarModeloxMeta(int id, ModeloxMeta modeloxMeta, string Username);

        [OperationContract] string EliminarModeloxParticipante(int id, ModeloxNodo modeloxPart, string Username);

        #endregion

        #region TIPO DE ENDOSOS
        /// <summary>
        /// Obtiene listado de las redes
        /// </summary>
        /// <returns>Lista de redes</returns>
        [OperationContract] List<TipoEndoso> ListarTipoEndoso();

        /// <summary>
        /// Obtiene listado de los tipos de endoso por id
        /// </summary>
        /// <param name="id">Id de la red a consultar</param>
        /// <returns>Lista de objectos TipoEndoso</returns>
        [OperationContract] List<TipoEndoso> ListarTipoEndososPorId(int id);

        /// <summary>
        /// Guarda un registro de TipoEndoso
        /// </summary>
        /// <param name="tipoEndoso">Objecto TipoEndoso a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int InsertarTipoEndoso(TipoEndoso tipoEndoso, string Username);

        /// <summary>
        /// Actualiza un registro de TipoEndoso
        /// </summary>
        /// <param name="id">Id del TipoEndoso a modificar</param>
        /// <param name="tipoEndoso">Objeto TipoEndoso utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] int ActualizarTipoEndoso(int id, TipoEndoso tipoEndoso, string Username);

        /// <summary>
        /// Elimina un registro de TipoEndoso
        /// </summary>
        /// <param name="id">Id del TipoEndoso a eliminar</param>
        /// <param name="tipoEndoso">Objecto TipoEndoso utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        [OperationContract] string EliminarTipoEndoso(int id, TipoEndoso tipoEndoso, string Username);
        #endregion

        #region LOGINTEGRACION

        [OperationContract] List<LogIntegracionwsIntegrador> ListarLogIntWsIns();

        [OperationContract] List<LogIntegracionwsIntegrador> ListarLogIntWsInsPorId(int id);

        [OperationContract] int ActualizarLogIntWsIns(int id, LogIntegracionwsIntegrador logintegracionwsintegrador, string Username);

        [OperationContract] List<LogIntegracion> ListarLogIntegracion();
        #endregion

        #region  GRUPO DE ENDOSO Y TIPO DE ENDOSOS (ESTADOS)

        [OperationContract] List<ExcepcionesxGrupoTipoEndoso> ListarExcepcionesxGrupoTipoEndoso();

        [OperationContract] List<ExcepcionesxGrupoTipoEndoso> ListarExcepcionesxGrupoTipoEndosoPorId(int id);

        [OperationContract] int InsertarExcepcionesxGrupoTipoEndoso(ExcepcionesxGrupoTipoEndoso excepcionesporGrupoTipoEndoso, string Username);

        [OperationContract] int ActualizarExcepcionesxGrupoTipoEndoso(int id, ExcepcionesxGrupoTipoEndoso excepcionesporGrupoTipoEndoso, string Username);

        [OperationContract] string EliminarExcepcionesxGrupoTipoEndoso(int id, ExcepcionesxGrupoTipoEndoso excepcionesporGrupoTipoEndoso, string Username);
        #endregion

        #region Franquicias

        [OperationContract] List<Localidad> ListarFranquicias();

        [OperationContract] int InsertarFranquicia(Localidad franquicia);

        [OperationContract] List<Localidad> ListarFranquiciaPorId(int idFranquicia);

        [OperationContract] int ActualizarFranquicia(int id, Localidad franquicia);

        [OperationContract] int EliminarFranquicia(int id, Localidad franquicia);

        [OperationContract] ColpatriaSAI.Negocio.Entidades.ParticipacionFranquicia DetalleFranquicia(int idFranquicia);

        [OperationContract] List<ColpatriaSAI.Negocio.Entidades.DetallePartFranquicia> DetalleFranquicias(int idFranquicia);

        [OperationContract] List<ColpatriaSAI.Negocio.Entidades.DetallePartFranquicia> DetalleFranquiciaPorPartFranqId(int idPartFranquicia);

        [OperationContract] List<Entidades.ParticipacionFranquicia> ListarPartFranquiciasPorlocalidad(int idlocalidad);

        [OperationContract] List<Entidades.ParticipacionFranquicia> ListarPartFranquicias();

        [OperationContract] bool EliminarPartFranquicia(int idPartFranquicia, string Username);

        [OperationContract] bool EliminarDetallePartFranquicia(int idPartFranquicia, string Username);

        [OperationContract] ParticipacionFranquicia InsertarPartFranquicia(Entidades.ParticipacionFranquicia partfranquicia, string Username);

        [OperationContract] bool InsertarDetallePartFranquicia(DetallePartFranquicia detalleParticipacion, string Username);

        [OperationContract] int ActualizarDetallePartFranquicia(DetallePartFranquicia participacionFranquicia, string Username);

        [OperationContract] int ActualizarParticipacionFranquicia(ParticipacionFranquicia participacionFranquicia, string Username);

        [OperationContract] int ActualizarPartFranquicia(ParticipacionFranquicia participacionFranquicia, string Username);

        [OperationContract] List<Entidades.DetallePartFranquicia> ListDetalleFranquiciaPorId(int idDetPartFranquicia);

        [OperationContract] DetallePartFranquicia DetalleFranquiciaporId(int id);

        [OperationContract] int EliminarDetallePartFranquiciaPorId(int id, string Username);

        [OperationContract] Entidades.DetallePartFranquicia DetalleFranquiciaPorIdFryIdDetFr(int idFranquicia, int iddetpartfra);

        [OperationContract] int ActualizarDetallePartFranquicias(DetallePartFranquicia DetallePartFranquicia, DateTime fechaIni, DateTime fechaFin, string franquicias, string Username);

        [OperationContract] List<DetallePartFranquicia> GetDetallePartFranquiciasActualizar(ParticipacionFranquicia partfranquicia, string franquicias);

        [OperationContract] int CopiarParticipacionFranquicia(int origen, int destino, string Username);

        [OperationContract] string obtenerSalarioMinimo();
        
        [OperationContract] void reportePagosFranquicia(int idLiquidacion);

        #endregion

        #region  Excepciones
        [OperationContract] List<Excepcion> ListarExcepciones(int idfranquicia);

        [OperationContract] List<Excepcion> ListarExcepcionesEspeciales();

        [OperationContract] int InsertarException(Entidades.Excepcion excepcion, string Username);

        [OperationContract] int EliminarException(int idexcepcion, string Username);

        [OperationContract] List<Entidades.Excepcion> ListarExcepcionesporId(int idException);

        [OperationContract] int ActualizarExcepcion(Entidades.Excepcion exception, string Username);

        #endregion

        #region  Negocio
        [OperationContract] List<Entidades.Negocio> ListarNegocios();

        #endregion

        #region liq franquicia
        [OperationContract] List<ColpatriaSAI.Negocio.Entidades.LiquidacionFranquicia> ListarLiquidacionFranquicias();

        [OperationContract] ColpatriaSAI.Negocio.Entidades.LiquidacionFranquicia TraerLiquidacionFranquicia(int idLiquidacionFranquicia);

        //NUEVOS METODOS

        [OperationContract] int InsertarLiquidacionFranquicia(ColpatriaSAI.Negocio.Entidades.LiquidacionFranquicia liquidacionFranquicia, string Username);

        [OperationContract] int LiquidarFranquiciasDetalleTotal(ColpatriaSAI.Negocio.Entidades.LiquidacionFranquicia liquidacionFranquicia);

        [OperationContract] int LiquidarFranquiciasExcepciones(ColpatriaSAI.Negocio.Entidades.LiquidacionFranquicia liquidacionFranquicia);

        [OperationContract] int LiquidarFranquiciasParticipaciones(ColpatriaSAI.Negocio.Entidades.LiquidacionFranquicia liquidacionFranquicia);

        [OperationContract] int LiquidarFranquiciasPorRangos(ColpatriaSAI.Negocio.Entidades.LiquidacionFranquicia liquidacionFranquicia);

        //FIN NUEVOS METODOS     

        [OperationContract] List<ColpatriaSAI.Negocio.Entidades.AnticipoFranquicia> ListarAnticipoFranquicias();

        [OperationContract] ColpatriaSAI.Negocio.Entidades.AnticipoFranquicia AnticipoFranquiciaPorId(int id);

        [OperationContract] int AnularAnticipo(int idAnticipo);

        [OperationContract] int ActualizarAnticipoFranquicia(int idAnticipo, AnticipoFranquicia anticipo, string Username);

        [OperationContract] int ActualizarAnticipoFranquicias(int id, ColpatriaSAI.Negocio.Entidades.AnticipoFranquicia anticipoFranquicia, string usuario);

        [OperationContract] int InsertarAnticipoFranquicias(ColpatriaSAI.Negocio.Entidades.AnticipoFranquicia anticipoFranquicia, string Username);

        [OperationContract] List<ColpatriaSAI.Negocio.Entidades.EstadoLiquidacion> ListarEstadosLiquidacionFranquicias();

        [OperationContract] int ActualizarLiquidacionFranquiciaEstado(int idLiquidacion, int idEstado, string Username);

        [OperationContract] int ActualizarLiquidacionFranquiciaReliquidacion(int idLiquidacion, int permiteReliquidar, string Username);

        [OperationContract] int GenerarPagosLiquidacionFranquicia(int idLiqFranquica, string usuario);

        [OperationContract] int ObtenerProcesoLiquidacion();

        [OperationContract] int EliminarLiquidacionProceso(int idLiquidacion, string Username);

        #endregion

        #region PRESUPUESTO

        [OperationContract] List<Presupuesto> ListarPresupuestos();

        [OperationContract] int InsertarPresupuesto(Presupuesto presupuesto, string Username);

        [OperationContract] int InsertarDetallePresupuesto(List<PresupuestoDetalles> detalle, int idPresupuesto, int anio, string hojaActual, int esUltimaHoja, int fila, string Username);

        [OperationContract] int CalcularMetasCompuestas(int presupuesto_id);

        [OperationContract] int CalcularMetasAcumuladas(int presupuesto_id);

        [OperationContract] int CalcularEjecucionPresupuesto(int presupuesto_id);

        [OperationContract] List<DetallePresupuesto> ListarDetallePresupuestoPorId(int id);

        [OperationContract] string EliminarPresupuesto(int id, string Username);

        [OperationContract] int BorrarPresupuestoACargar(int anio, int segmento_id, string Username);

        [OperationContract] List<PresupuestoDetalle> ListarPresupuestoDetalle(int id);

        [OperationContract] void CarguePresupuesto(string nombreArchivo, string anio, int segmento_id, string Username);

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
        [OperationContract] void GenerarLiquidacionRegla_Iniciar(DateTime fechaInicio, DateTime fechaFin, int idRegla, int idConcurso);

        [OperationContract] object GenerarReporteLiquidacionAsesor(int idLiquidacionRegla);

        #endregion


        /// <summary>
        /// Permite liquidar una regla de un concurso
        /// </summary>
        /// <returns></returns>
        [OperationContract] int InsertarLiquidacionRegla(LiquidacionRegla liquidacion, string Username);

        [OperationContract] int GenerarLiquidacionRegla(int idLiquidacionRegla, DateTime fechaInicio, DateTime fechaFin, int idRegla, int idConcurso);

        [OperationContract] List<string> ListarLiquidaciones(int regla_id);

        [OperationContract] List<LiquidacionRegla> ListarLiquidacionesRegla(int idRegla);

        [OperationContract] List<Participante> ListarParticipantesLiquidacion(int idLiquidacionRegla, int inicio, int cantidad);

        [OperationContract] List<VistaDetalleLiquidacionReglaParticipante> ListarDetalleLiquidacionReglaParticipante(int idLiquidacionRegla, int participante_id);

        [OperationContract] int LiquidarPagosRegla(int idLiquidacionRegla);

        [OperationContract] int ActualizarLiquidacionReglaEstado(int idLiquidacion, int idEstado, string Username);

        #endregion

        #region TipoVehiculo
        
        [OperationContract] List<Entidades.TipoVehiculo> ListarTipoVehiculos();

        [OperationContract] List<Entidades.TipoVehiculo> ListarTipoVehiculosporRamo(int ramo_id);
        #endregion

        #region PARTICIPACIONES

        [OperationContract] List<Participacione> ListarParticipaciones();

        [OperationContract] List<Participacione> ListarParticipacionPorId(int id);

        [OperationContract] int InsertarParticipacion(Participacione part, string Username);

        [OperationContract] int ActualizarParticipacion(int id, Participacione part, string Username);

        [OperationContract] string EliminarParticipacion(int id, Participacione part, string Username);

        [OperationContract] List<ParticipacionDirector> ListarParticipacionesDirector();

        [OperationContract] List<ParticipacionDirector> ListarParticipacionDirectorPorId(int id);

        [OperationContract] int InsertarParticipacionDirector(ParticipacionDirector part, string Username);

        [OperationContract] int ActualizarParticipacionDirector(int id, ParticipacionDirector part, string Username);

        [OperationContract] string EliminarParticipacionDirector(int id, ParticipacionDirector part, string Username);

        [OperationContract] List<Participante> ListarParticipantesPorNivel(string texto, int inicio, int cantidad, int nivel);

        [OperationContract] List<Ramo> ListarRamosXCompania(int id);

        #endregion

        #region LIQUIDACION DE CONTRATACION DE DESEMPEÑO

        [OperationContract] int InsertarLiquidacionContrat(LiquidacionContratacion liquiContrat, string Username);

        [OperationContract] int InsertarLiquidacionContratMeta(LiquiContratMeta liquiContrat, string Username);

        [OperationContract] int InsertarDetalleLiquidacionContratPP(DetalleLiquiContratPpacionPpante liquiContrat, string Username);

        [OperationContract] int InsertarLiquidacionContratFP(LiquiContratFactorParticipante liquiContrat, string Username);

        [OperationContract] int InsertarLiquidacionContratPP(LiquiContratPpacionPpante liquiContrat, string Username);
        #endregion

        #region JERARQUIAS

        [OperationContract] List<Jerarquia> ListarJerarquias();

        [OperationContract] Jerarquia ListarJerarquiaPorId(int id);

        [OperationContract] int InsertarJerarquia(Jerarquia jerarquia, string userName);

        [OperationContract] int ActualizarJerarquia(int id, Jerarquia jerarquia, string userName);

        [OperationContract] int EliminarJerarquia(int id, string Username);

        [OperationContract] List<TipoJerarquia> ListarTiposJerarquia();

        [OperationContract] List<JerarquiaDetalle> ListarJerarquiaDetalle();

        [OperationContract] JerarquiaDetalle InsertarJerarquiaDetalle(JerarquiaDetalle detalle, string userName);

        [OperationContract] List<JerarquiaDetalle> ListarNodosBuscador(int tipo, string texto, int inicio, int cantidad, int nivel, int zona, int canal, string codNivel);

        [OperationContract] List<NodoArbol> ListarArbol(int id, int padre_id);

        [OperationContract] int EliminarJerarquiaDetalle(int id, string userName);

        [OperationContract] JerarquiaDetalle ListarNodoArbol(int id);

        [OperationContract] int ActualizarOrdenNodo(JerarquiaDetalle detalle, string userName);

        #endregion

        #region Metodos PERSISTENCIAESPERADA

        [OperationContract] int InsertarPersistenciaEsperada(PersistenciaEsperada persistenciaesperada, string Username);

        [OperationContract] List<PersistenciaEsperada> ListarPersistenciaEsperada();

        [OperationContract] List<PersistenciaEsperada> ListarPersistenciaEsperadaPorId(int idPersistenciaEsperada);

        [OperationContract] int ActualizarPersistenciaEsperada(int id, PersistenciaEsperada persistenciaesperada, string Username);

        [OperationContract] string EliminarPersistenciaEsperada(int id, PersistenciaEsperada persistenciaesperada, string Username);
        #endregion

        #region Metodos SINIESTRALIDADESPERADA

        [OperationContract] int InsertarSiniestralidadEsperada(SiniestralidadEsperada siniestralidadesperada, string Username);

        [OperationContract] List<SiniestralidadEsperada> ListarSiniestralidadEsperada();

        [OperationContract] List<SiniestralidadEsperada> ListarSiniestralidadEsperadaPorId(int idSiniestralidadEsperada);

        [OperationContract] int ActualizarSiniestralidadEsperada(int id, SiniestralidadEsperada siniestralidadesperada, string Username);

        [OperationContract] string EliminarSiniestralidadEsperada(int id, SiniestralidadEsperada siniestralidadesperada, string Username);
        #endregion

        #region  Excepciones Jerarquia
        [OperationContract] List<ExcepcionJerarquiaDetalle> ListarExcepcionesJerarquiaporId(int idJerarquia);

        [OperationContract] int InsertarExceptionJerarquia(ExcepcionJerarquiaDetalle excepcion, string Username);

        [OperationContract] int EliminarExceptionJerarquia(int idexcepcion, string Username);

        [OperationContract] ExcepcionJerarquiaDetalle TraerExceptionJerarquiaporId(int idException);

        [OperationContract] int ActualizarExcepcionJerarquia(ExcepcionJerarquiaDetalle exception, string Username);

        #endregion

        #region Liquidaciones Moneda

        [OperationContract] List<LiquidacionMoneda> ListarLiquidacionesMoneda(int tipo);

        [OperationContract] int GuardarLiquidacionMoneda(LiquidacionMoneda liquidacionMoneda, string Username);

        [OperationContract] int BorrarColquinesManuales(DateTime fechaCargue, string Username);

        #endregion

        #region Tipo Medidas

        [OperationContract] List<TipoMedida> ListarTipoMedidas();

        #endregion

        #region Tipo Contratos

        [OperationContract]
        List<Entidades.TipoContrato> ListarTipoContratos();

        #endregion

        #region  Metas Jerarquia
        [OperationContract] List<MetaxNodo> ListarMetasxJerarquiaId(int idJerarquia);

        [OperationContract] int InsertarMetaNodo(MetaxNodo metaNodo, string Username);

        [OperationContract] int EliminarMetaNodo(int idMetaNodo, string Username);

        [OperationContract] List<MetaxNodo> ListarMetasNodos();

        #endregion

        #region Ejecucion

        [OperationContract] int GuardarEjecucionDetalle(EjecucionDetalle ejecucionDetalle);

        [OperationContract] int GuardarEjecucionDetalleBatch(String strSQL);

        [OperationContract] Ejecucion TraerEjecucionPorPresupuesto(int idPresupuesto);

        [OperationContract] EjecucionDetalle TraerEjecucionDetalle(int idEjecucion, int idMeta, int idNodo, int periodo, int idCanal);

        [OperationContract] int EliminarEjecucionDetallePorIdEjecucion(int idEjecucion);

        [OperationContract] EjecucionDetalle TraerEjecucionDetallePorId(int idEjecucionDetalle);

        [OperationContract] int ActualizarEjecucionDetalle(EjecucionDetalle ejecucionDetalle);

        [OperationContract] void CargueManualEjecucionDetalle(string nombreArchivo, int idPresupuesto);
        #endregion

        #region Liquidacion Contratacion
        [OperationContract] List<LiquidacionContratacion> ListarLiquidacionContratacion();

        [OperationContract] LiquidacionContratacion TraerLiquidacionContratacion(int idLiquidacion);

        [OperationContract] int LiquidarContratacion(LiquidacionContratacion liquidacionContratacion, int idSegmento);

        [OperationContract] int ActualizarLiquidacionContratacionEstado(int idLiquidacion, int idEstado, string Username);

        [OperationContract] int EliminarLiquidacionContratacion(int idLiquidacion, string Username);

        #endregion

        [OperationContract] List<NotificacionProceso> ObtenerProcesosEnCurso();

        [OperationContract] int EliminarProcesosEnCurso(string Username);

        [OperationContract] int CancelarProceso(int tipo, string Username);

        #region Dashboard

        [OperationContract] int ObtenerDatosDashboard();

        [OperationContract] List<ColpatriaSAI.Negocio.Entidades.Dashboard> TraerDashboard();

        [OperationContract] List<ColpatriaSAI.Negocio.Entidades.TipoPanel> TraerDashboardxPanel();

        #endregion

        #region  Periodos Cierre

        [OperationContract] List<PeriodoCierre> ListarPeriodos();

        [OperationContract] List<PeriodoCierre> ListarPeriodosPorCompania(int idCompania);

        [OperationContract]
        int InsertarPeriodoCierre(PeriodoCierre periodo, string Username);

        [OperationContract] int EliminarPeriodoCierre(int idPeriodo, string Username);

        [OperationContract] List<PeriodoCierre> TraerPeriodoCierrePorId(int idPeriodo);

        [OperationContract] int ActualizarPeriodoCierre(PeriodoCierre periodo, string Username);

        [OperationContract] int ActualizarEstadoPeriodoCierre(int idPeriodo, int idEstado, string Username);

        [OperationContract] int SPPeriodoCierre(int companiaId, int mesCierre, int anioCierre);

        [OperationContract] int DeleteReprocesos(int mesCierre, int añoCierre);

        #endregion

        #region "Ajustes"
        [OperationContract] List<DetallePagosRegla> ListarPagosConcurso(int liquidacionRegla_id);

        [OperationContract] int ActualizarPagosConcurso(String usuario, List<DetallePagosRegla> listaPagosConcurso, string Username);

        [OperationContract] List<LiquiContratFactorParticipante> ListarPagosContratos(int liquidacionContratos_id);

        [OperationContract] int ActualizarPagosContratos(String usuario, List<LiquiContratFactorParticipante> listaPagosContratos, string Username);

        [OperationContract] List<DetallePagosFranquicia> ListarPagosFranquicia(int liquidacionFranquicia_id);

        [OperationContract] int ActualizarPagosFranquicia(String usuario, List<DetallePagosFranquicia> listaPagosFranquicia, string Username);
        #endregion

        #region Combos

        [OperationContract] List<Combo> ListarCombos();

        [OperationContract] List<Combo> ListarCombosPorId(int idCombo);

        [OperationContract] int InsertarCombo(Combo combo, string Username);

        [OperationContract] int ActualizarCombo(int id, Combo combo, string Username);

        [OperationContract] int ActualizarComboValidado(int id, int validado, string Username);

        [OperationContract] int EliminarCombo(int idCombo, string Username);

        [OperationContract] List<ProductoCombo> ListarProductosComboPorId(int id);

        [OperationContract] int InsertarProductoCombo(ProductoCombo productocombo, string Username);

        [OperationContract] int ActualizarProductoCombo(int id, ProductoCombo productocombo, string Username);

        [OperationContract] int EliminarProductoCombo(int idCombo, string Username);

        #endregion

        #region "Excepciones"

        [OperationContract] List<ExcepcionesGenerale> ListarExcepcionesGeneralesXcompanyXTipoMedida();

        [OperationContract] List<ExcepcionesGenerale> ListarExcepcionesGeneralesXTipoMedida();

        [OperationContract] List<ExcepcionesGenerale> ListarExcepcionesGenerales();

        [OperationContract] bool validarExcepcionesGenerales(ExcepcionesGenerale excepcionG);

        [OperationContract] int CrearExcepcionesGenerales(ExcepcionesGenerale excepcionG, string Username);

        [OperationContract] int ActualizarExcepcionesGenerales(ExcepcionesGenerale excepcionG, string Username);

        [OperationContract] int EliminarExcepcionGenerales(int id, string Username);

        #endregion

        #region SEGURIDAD

        [OperationContract] int CrearUsuario(string nombreUsuario, string tipoDocumento, string numeroDocumento, string email, string rol, int segmento, string Username);

        [OperationContract] int InsertarSegmentodeUsuario(UsuarioxSegmento usuarioxsegmento, string Username);

        [OperationContract] int EliminarSegmentodeUsuario(UsuarioxSegmento usuarioxsegmento, string Username);

        #endregion

        #region PROCESO AUTOMATICO
        [OperationContract] List<Ejecuciones> TraerUltimaEjecucion();

        [OperationContract] List<Ejecuciones> TraerEjecucion(DateTime fechaIni);

        [OperationContract] List<AUT_Programacion_Proceso> TraerUltimasFechasProgramacion();

        [OperationContract] List<AUT_Programacion_Proceso> TraerFechasProgramacion();

        [OperationContract] AUT_Programacion_Proceso InsertarProgramacion(AUT_Programacion_Proceso programacion);

        [OperationContract] bool EliminarProgramacion(AUT_Programacion_Proceso programacion);

        [OperationContract] List<AUT_Proceso> TraerProcesos();

        [OperationContract] AUT_Proceso ActualizarProceso(AUT_Proceso procesoold);

        [OperationContract] List<AUT_Proceso_Dependencia> TraerDependenciaPorProceso(int idProceso);

        [OperationContract] List<AUT_Tipo_Accion_En_Error> TraerAccionesEnError();

        [OperationContract] AUT_Proceso_Dependencia InsertarProcesoDependencia(AUT_Proceso_Dependencia procesoDependencia);

        [OperationContract] AUT_Proceso_Dependencia ActualizarProcesoDependencia(AUT_Proceso_Dependencia procesoold, AUT_Proceso_Dependencia procesonew);

        [OperationContract] bool EliminarProcesoDependencia(AUT_Proceso_Dependencia procesoDependencia);
        #endregion

        #region ESTADISTICAS
        [OperationContract] List<ReporteValores> TraerValoresRecaudosMesPromedio(int anio, int mes);

        [OperationContract] List<ReporteRegistros> TraerRegistrosRecaudosMesPromedio(int anio, int mes);

        [OperationContract] List<ReporteValores> TraerValoresPrimasMesPromedio(int anio, int mes);

        [OperationContract] List<ReporteRegistros> TraerRegistrosPrimasMesPromedio(int anio, int mes);

        #endregion
    }
}
