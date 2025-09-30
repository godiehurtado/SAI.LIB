using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Xml.Linq;
using System.IO;
using System.Net;
using System.Configuration;
using System.Timers;
using Microsoft.SqlServer.Dts.Runtime; //Se agrega en la ruta: C:\Windows\assembly\GAC_32\Microsoft.SqlServer.DTSRuntimeWrap\10.0.0.0__89845dcd8080cc91\Microsoft.SqlServer.DTSRuntimeWrap.dll
using Microsoft.SqlServer.Dts.Runtime.Wrapper; //Se agrega en la ruta: C:\Program Files\Microsoft SQL Server\100\SDK\Assemblies\Microsoft.SQLServer.DTSRuntimeWrap.dll
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Integracion
    {
        private SAI_Entities tabla = new SAI_Entities();

        #region LOGINTEGRACION

        public List<LogIntegracionwsIntegrador> ListarLogIntWsIns()
        {  
            return tabla.LogIntegracionwsIntegradors.Where(liwsis => liwsis.id > 0).ToList();
        }

        public List<LogIntegracionwsIntegrador> ListarLogIntWsInsPorId(int id)
        {
            return tabla.LogIntegracionwsIntegradors.Where(liwsis => liwsis.id == id).ToList();
        }

        public int ActualizarLogIntWsIns(int id, LogIntegracionwsIntegrador logintegracionwsintegrador, string Username)
        {
            var logintegracionwsActual = tabla.LogIntegracionwsIntegradors.Where(liwsis => liwsis.id == id).First();
            var pValorAntiguo = logintegracionwsActual;
            logintegracionwsActual.fechaInicial = logintegracionwsintegrador.fechaInicial;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.LogIntegracionwsIntegrador,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo,logintegracionwsActual );
            return tabla.SaveChanges();
        }

        public List<LogIntegracion> ListarLogIntegracion()
        {
            return tabla.LogIntegracions.Where(l => l.fechaInicio.Value.Year == DateTime.Now.Year && l.fechaInicio.Value.Month == DateTime.Now.Month && l.fechaInicio.Value.Day == DateTime.Now.Day).ToList();
        }

        #endregion    

        #region LOGICAINTEGRACION

        /// <summary>
        /// COnsulta la dirección IP del servidor aplicación.
        /// </summary>
        /// <returns>IP del servidor de aplicación.</returns>
        public string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {   
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        /// <summary>
        /// Toma los valores de parametros configurados en el web.config de servicios.
        /// </summary>
        /// <returns>Un string en donde se concatenan los valores traidos desde el web.config</returns>
        public string ValoresWebConfigServicio()
        {
            System.Configuration.AppSettingsReader reader = new AppSettingsReader();

            string rutaArchivo     = reader.GetValue("RutaArchivos", String.Empty.GetType()).ToString();
            string ftpServerIP     = reader.GetValue("servidorftp", String.Empty.GetType()).ToString();
            string ftpUserID       = reader.GetValue("usuarioftp", String.Empty.GetType()).ToString();
            string ftpPassword     = reader.GetValue("contrasenaftp", String.Empty.GetType()).ToString();
            string serverETL       = reader.GetValue("servidoretl", String.Empty.GetType()).ToString();
            string passwordPackage = reader.GetValue("passwordPackage", String.Empty.GetType()).ToString();
            string rutaFTPCargue   = reader.GetValue("RutaFTPCargue", String.Empty.GetType()).ToString();
            string horaSISE        = reader.GetValue("HoraFinRecaudosSISE", String.Empty.GetType()).ToString();

            return rutaArchivo + "," + ftpServerIP + "," + ftpUserID + "," + ftpPassword + "," + serverETL + "," + passwordPackage + "," + rutaFTPCargue + "," + horaSISE;
        }

        public List<PeriodoCierre> periodosAbiertos()
        {
            try
            {
                /// Retorna los Periodos Abiertos; solo es 1 por compania.
                /// 
                return tabla.PeriodoCierres.Where(pc => pc.estado == 1).ToList();
            }

            catch 
            {
                return null;
            }            
        }

        /// <summary>
        /// Consulta la cantidad de paquetes que se estan ejecutando, al momento del inicio de 
        /// la ejecución de los precalculos y entrega de información.
        /// </summary>
        /// <returns>Cantidad de paquetes ejecutandose</returns>
        public int CantidadPaquetesenEjecución()
        {
            var cantidadPaquetes = (from pl in tabla.PackageLogs
                                    where (pl.Status.Trim() == "R"
                                    && pl.StartDateTime.Year == DateTime.Now.Year
                                    && pl.StartDateTime.Month == DateTime.Now.Month
                                    && pl.StartDateTime.Day == DateTime.Now.Day)
                                    select pl).Count();

            return cantidadPaquetes;
        }

        /// <summary>
        /// Consulta el valor de la hora correspondiente al id de la tabla que llega por parametro.
        /// </summary>
        /// <param name="id">id de la tabla ParametrosApp</param>
        /// <returns>valor de la hora.</returns>
        public int RetornarHorasIntegracion(int id)
        {
            var hora = (from pa in tabla.ParametrosApps
                        where (pa.id == id)                                   
                        select pa.valor).FirstOrDefault();

            int valorHora = Convert.ToInt16(hora);            
            
            return valorHora;
        }

        /// <summary>
        /// Método que crea los XML por compañía.
        /// </summary>
        /// <param name="compania">id de la compañía: 1 GENERALES, 2 VIDA, 3 CAPI, 5 BH</param>
        /// <param name="fechaInicial">fecha inicial de la extracción que se envia en el XML que se genera.</param>
        /// <param name="fechaFinal">fecha final de la extracción que se envia en el XML que se genera.</param>
        public void CrearXml(int compania, DateTime fechaInicial, DateTime fechaFinal)
        {   
            System.Configuration.AppSettingsReader reader = new AppSettingsReader();

            string rutaLocal                              = reader.GetValue("RutaArchivos", String.Empty.GetType()).ToString(); // Ruta Local (Servidor de Apliacciones) donde se descargan los archivos XML cuando se crean desde .Net.
            string clienteIp                              = LocalIPAddress(); // Direccion Ip Servidor donde se ejecuta el proceso.
            string usuario                                = Dns.GetHostName(); // Nombre del Equipo.
            string formatoFecha                           = "yyyyMMdd";

            XNamespace ns0          = "http://tempuri.org/"; // Namespace de CAPI y SISE (Integrador).
            XNamespace beyondhealth = "http://BeyondHealth/WebServices"; // Namespace de BH.

            var DBWSSISE = (from p in tabla.ParametrosApps
                            where (p.id == 10)                            
                            select p.valor).First(); // Parametro DataBase de los xml de SISE

            var DBLibraryCAPI = (from p in tabla.ParametrosApps
                                where (p.id == 13)
                                select p.valor).First(); // Parametro Library de los xml de CAPI

            if (compania == 1) // SISE GENERALES
            {
                string fechaSistemaTotal = "'" + Convert.ToDateTime(fechaInicial).ToString(formatoFecha) + "," + Convert.ToDateTime(fechaFinal).ToString(formatoFecha) + "'";

                XElement xmlSISEGENERALESProductos = new XElement(ns0 + "SQL",
                 new XAttribute(XNamespace.Xmlns + "ns0", ns0),
                 new XElement(ns0 + "DataSource", "SQL"),
                 new XElement(ns0 + "Company", "SegurosGnrl"),
                 new XElement(ns0 + "StoredProcedure", "SPINS_ExecETL"),
                 new XElement(ns0 + "DataBase", DBWSSISE),
                 new XElement(ns0 + "Parameters", "'SAI_Productos_Generales',' '"),
                 new XElement(ns0 + "IpRequest", clienteIp),
                 new XElement(ns0 + "User", usuario)
             );

                XDocument archivoSISEGENERALESProductosXml = new XDocument(xmlSISEGENERALESProductos);
                archivoSISEGENERALESProductosXml.Save(rutaLocal + "SISEGENERALESProductos.xml");

                XElement xmlSISEGENERALESNegocios = new XElement(ns0 + "SQL",
                      new XAttribute(XNamespace.Xmlns + "ns0", ns0),
                      new XElement(ns0 + "DataSource", "SQL"),
                      new XElement(ns0 + "Company", "SegurosGnrl"),
                      new XElement(ns0 + "StoredProcedure", "SPINS_ExecETL"),
                      new XElement(ns0 + "DataBase", DBWSSISE),
                      new XElement(ns0 + "Parameters", "'SAI_Negocios_Generales'," + fechaSistemaTotal),
                      new XElement(ns0 + "IpRequest", clienteIp),
                      new XElement(ns0 + "User", usuario)
                  );

                XDocument archivoSISEGENERALESNegociosXml = new XDocument(xmlSISEGENERALESNegocios);
                archivoSISEGENERALESNegociosXml.Save(rutaLocal + "SISEGENERALESNegocios.xml");                
              
            }

            else if (compania == 2) // SISE VIDA
            {
                string fechaSistemaTotal = "'" + Convert.ToDateTime(fechaInicial).ToString(formatoFecha) + "," + Convert.ToDateTime(fechaFinal).ToString(formatoFecha) + "'";

                XElement xmlSISEVIDAProductos = new XElement(ns0 + "SQL",
                  new XAttribute(XNamespace.Xmlns + "ns0", ns0),
                  new XElement(ns0 + "DataSource", "SQL"),
                  new XElement(ns0 + "Company", "SegurosVida"),
                  new XElement(ns0 + "StoredProcedure", "SPINS_ExecETL"),
                  new XElement(ns0 + "DataBase", DBWSSISE),
                  new XElement(ns0 + "Parameters", "'SAI_Productos_Vida',' '"),
                  new XElement(ns0 + "IpRequest", clienteIp),
                  new XElement(ns0 + "User", usuario)
              );

                XDocument archivoSISEVIDAProductosXml = new XDocument(xmlSISEVIDAProductos);
                archivoSISEVIDAProductosXml.Save(rutaLocal + "SISEVIDAProductos.xml");

                XElement xmlSISEVIDANegocios = new XElement(ns0 + "SQL",
                      new XAttribute(XNamespace.Xmlns + "ns0", ns0),
                      new XElement(ns0 + "DataSource", "SQL"),
                      new XElement(ns0 + "Company", "SegurosVida"),
                      new XElement(ns0 + "StoredProcedure", "SPINS_ExecETL"),
                      new XElement(ns0 + "DataBase", DBWSSISE),
                      new XElement(ns0 + "Parameters", "'SAI_Negocios_Vida', " + fechaSistemaTotal),
                      new XElement(ns0 + "IpRequest", clienteIp),
                      new XElement(ns0 + "User", usuario)
                  );

                XDocument archivoSISEVIDANegociosXml = new XDocument(xmlSISEVIDANegocios);
                archivoSISEVIDANegociosXml.Save(rutaLocal + "SISEVIDANegocios.xml");                                
               
            }

            else if (compania == 3) // CAPI
            {
                string fechaSistemaTotal = Convert.ToDateTime(fechaInicial).ToString(formatoFecha) + Convert.ToDateTime(fechaFinal).ToString(formatoFecha);

                XElement xmlCAPIProductos = new XElement(ns0 + "AS4",
                      new XAttribute(XNamespace.Xmlns + "ns0", ns0),
                      new XElement(ns0 + "DataSource", "AS4"),
                      new XElement(ns0 + "Company", "CAPI"),
                      new XElement(ns0 + "Program", "CAPCISAP"),
                      new XElement(ns0 + "Library", DBLibraryCAPI),
                      new XElement(ns0 + "Parameters", "20110101"),
                      new XElement(ns0 + "IpRequest", clienteIp),
                      new XElement(ns0 + "User", usuario)
                  );

                XDocument archivoCAPIProductosXml = new XDocument(xmlCAPIProductos);
                archivoCAPIProductosXml.Save(rutaLocal + "CAPIProductos.xml");

                XElement xmlCAPIRecaudos = new XElement(ns0 + "AS4",
                      new XAttribute(XNamespace.Xmlns + "ns0", ns0),
                      new XElement(ns0 + "DataSource", "AS4"),
                      new XElement(ns0 + "Company", "CAPI"),
                      new XElement(ns0 + "Program", "CAPCISAR"),
                      new XElement(ns0 + "Library", DBLibraryCAPI),
                      new XElement(ns0 + "Parameters", fechaSistemaTotal),
                      new XElement(ns0 + "IpRequest", clienteIp),
                      new XElement(ns0 + "User", usuario)
                  );

                XDocument archivoCAPIRecaudosXml = new XDocument(xmlCAPIRecaudos);
                archivoCAPIRecaudosXml.Save(rutaLocal + "CAPIRecaudos.xml");
            }

            else if (compania == 5) // SALUD
            {
                XElement xmlBH = new XElement(beyondhealth + "WSIncentivesAdministrationSystem",
                      new XAttribute(XNamespace.Xmlns + "ns0", beyondhealth),
                      new XElement(beyondhealth + "datInitialDate", Convert.ToDateTime(fechaInicial).ToString("yyyy-MM-dd") + "T00:00:00.000-05:00"),
                      new XElement(beyondhealth + "datEndingDate", Convert.ToDateTime(fechaFinal).ToString("yyyy-MM-dd") + "T00:00:00.000-05:00")
                    );

                XDocument archivoBHXml = new XDocument(xmlBH);
                archivoBHXml.Save(rutaLocal + "BH.xml");
            }

            else if (compania == 0) // Recaudos SISE
            {
                string fechaSistemaRecaudosVIDA = "'" + Convert.ToDateTime(fechaInicial).ToString("yyyyMM") + "01" + "," + DateTime.Now.ToString(formatoFecha) + "'";

                XElement xmlSISEVIDARecaudos = new XElement(ns0 + "SQL",
                     new XAttribute(XNamespace.Xmlns + "ns0", ns0),
                     new XElement(ns0 + "DataSource", "SQL"),
                     new XElement(ns0 + "Company", "SegurosVida"),
                     new XElement(ns0 + "StoredProcedure", "SPINS_ExecETL"),
                     new XElement(ns0 + "DataBase", DBWSSISE),
                     new XElement(ns0 + "Parameters", "'SAI_Recaudos_Vida', " + fechaSistemaRecaudosVIDA),
                     new XElement(ns0 + "IpRequest", clienteIp),
                     new XElement(ns0 + "User", usuario)
                 );

                XDocument archivoSISEVIDARecaudosXml = new XDocument(xmlSISEVIDARecaudos);
                archivoSISEVIDARecaudosXml.Save(rutaLocal + "SISEVIDARecaudos.xml");

                string fechaSistemaRecaudosGEN = "'" + Convert.ToDateTime(fechaInicial).ToString("yyyyMM") + "01" + "," + DateTime.Now.ToString(formatoFecha) + "'";

                XElement xmlSISEGENERALESRecaudos = new XElement(ns0 + "SQL",
                    new XAttribute(XNamespace.Xmlns + "ns0", ns0),
                    new XElement(ns0 + "DataSource", "SQL"),
                    new XElement(ns0 + "Company", "SegurosGnrl"),
                    new XElement(ns0 + "StoredProcedure", "SPINS_ExecETL"),
                    new XElement(ns0 + "DataBase", DBWSSISE),
                    new XElement(ns0 + "Parameters", "'SAI_Recaudos_Generales', " + fechaSistemaRecaudosGEN),
                    new XElement(ns0 + "IpRequest", clienteIp),
                    new XElement(ns0 + "User", usuario)
                );

                XDocument archivoSISEGENERALESRecaudosXml = new XDocument(xmlSISEGENERALESRecaudos);
                archivoSISEGENERALESRecaudosXml.Save(rutaLocal + "SISEGENERALESRecaudos.xml"); 
            }
        }

        /// <summary>
        /// Método que consulta las fechas de parametrización de usuario para la generación
        /// de los archivos XML por compañía.
        /// </summary>
        /// <returns>1 si es exitoso, 0 si hay un proceso en curso.</returns>
        public int Linq2XmlCrearArchivosXml()
        {
            System.Configuration.AppSettingsReader reader = new AppSettingsReader();

            string rutaLocal = reader.GetValue("RutaArchivos", String.Empty.GetType()).ToString(); // Ruta Local (Servidor de Apliacciones) donde se descargan los archivos XML cuando se crean desde .Net.
            string clienteIp                              = LocalIPAddress(); // Direccion Ip Servidor donde se ejecuta el proceso.
            string usuario                                = Dns.GetHostName();  // Nombre del Equipo.      
            XNamespace ns0                                = "http://tempuri.org/"; // Namespace del llamado a Integrador.

            /// Estados Proceso Integración: 1 --> En Proceso, 2 --> Terminó, 3 --> ErrorEjecución.
            /// Si se consulta el estado del últmio registro insertado en la B.D (tabla LogIntegracion) y el estado es '1', se interrumpe el proceso,
            /// si es dos se inserta un nuevo registro.
            /// 
            string fechaAyer = DateTime.Now.AddDays(-1).ToShortDateString();

            var integracionActiva = (from pa in tabla.ParametrosApps
                                     where (pa.id == 16)
                                     select pa.valor).First().ToString().Trim();

            var validacionSegmentacion = (from pa in tabla.ParametrosApps
                                     where (pa.id == 20)
                                     select pa.valor).First().ToString().Trim();

            var registrosCOMJSN = (from cj in tabla.COMJSNs select cj).Count();

            try
            {
                var fechaIncialIntegracion = (from li in tabla.LogIntegracions
                                              where (li.proceso == "Integracion SAI" && li.estado == 1)
                                              orderby li.id descending
                                              select li.fechaInicio).First();

                string fechaCambioestado = Convert.ToDateTime(fechaIncialIntegracion).ToShortDateString();

                if (Convert.ToDateTime(fechaCambioestado) <= Convert.ToDateTime(fechaAyer))
                {                    
                    var logInActual = tabla.LogIntegracions.Where(LogIntegracion => LogIntegracion.estado == 1);
                    foreach (var item in logInActual)
                    {
                        item.estado = 3; // Estado Error.
                    }
                    tabla.SaveChanges();
                }
            }

            catch { }

            try
            {
                var estadoIntegracion = (from li in tabla.LogIntegracions
                                         where (li.proceso == "Integracion SAI")
                                         orderby li.id descending
                                         select li.estado).First();

                /// Si existe en la tabla LogIntegracion un registro del día actual con estado 1,
                /// el proceso de integración termina. Significa que hay otra tarea de integración
                /// en curso o que el proceso de integración del día falló y se debe reintentar al
                /// día siguiente. La integración termina si fue desactivada en parametrisApp (NO),
                /// si la tabla SIG.COMJSN está vacia o si la cantidad de registros en la COMJSN es
                /// menor a 6000.
                /// 
                if (estadoIntegracion == 1 || integracionActiva != "SI"/* || registrosCOMJSN == 0 || registrosCOMJSN < Convert.ToDouble(validacionSegmentacion)*/)
                {
                    string localIp = LocalIPAddress();

                    LogIntegracion log = new LogIntegracion();
                    {                        
                        log.fechaInicio   = DateTime.Now;
                        log.estado        = 2; // Estado terminado de la integración.
                        log.proceso       = "Local Ip: " + localIp + " | " + "Estado Integración: " + Convert.ToString(estadoIntegracion) + " | " + "Integración activa: " + integracionActiva + " | " + "Registros COMJSN: " + Convert.ToString(registrosCOMJSN);
                        tabla.LogIntegracions.AddObject(log);
                        tabla.SaveChanges();
                    }
                    return 0;
                }
            }

            catch { }

            LogIntegracion logintegracion = new LogIntegracion();
            {
                DateTime fechaHoy          = Convert.ToDateTime(Convert.ToString(DateTime.Now.Year) + "-" + Convert.ToString(DateTime.Now.Month) + "-" + Convert.ToString(DateTime.Now.Day));
                logintegracion.fechaInicio = DateTime.Now;
                logintegracion.estado      = 1; // Estado en proceso de la integración.
                logintegracion.proceso     = "Integracion SAI";
                tabla.LogIntegracions.AddObject(logintegracion);
                tabla.SaveChanges();
            }
            
            List<PeriodoCierre> periodos = periodosAbiertos();            

            int compania = 0;
            DateTime fechaInicial;
            DateTime fechaFinal;
            DateTime fechaActual = DateTime.Now;            
            while(compania <= 5) // Ciclo para las companias 0: Recaudos SISE, 1: SISE GENERALES, 2: SISE VIDA, 3: CAPI, 4: ARP, 5: SALUD.
            {
                try
                {
                    PeriodoCierre periodo = periodos.Where(p => p.compania_id == compania).First();

                    fechaInicial = Convert.ToDateTime(periodo.fechaInicio);
                    fechaFinal   = Convert.ToDateTime(periodo.fechaFin);

                    CrearXml(compania, fechaInicial, fechaFinal);
                }

                catch { }

                compania++;
            }

            var DBWSSISE = (from p in tabla.ParametrosApps
                            where (p.id == 10)
                            select p.valor).First(); // Parametro DataBase de los xml de SISE

            var DBLibraryCAPI = (from p in tabla.ParametrosApps
                                 where (p.id == 13)
                                 select p.valor).First(); // Parametro Library de los xml de CAPI

            //GP                       
            string fechaSistemaTotalGPComisiones = DateTime.Now.AddMonths(-1).ToString("yyyyMM") + DateTime.Now.ToString("yyyyMM");

            string fechaSistemaTotalGPNOM231 = DateTime.Now.AddMonths(-1).ToString("yyyyMM"); 

            /// XML para llamar el integrador y generar el archivo que se carga a la tabla
            /// dbo.Participante. (NOM301O).
            /// 
            XElement xmlGPParticipante = new XElement(ns0 + "AS4",
                  new XAttribute(XNamespace.Xmlns + "ns0", ns0),
                  new XElement(ns0 + "Library", DBLibraryCAPI),
                  new XElement(ns0 + "Program", "NOM301C1"),
                  new XElement(ns0 + "Parameters", ""),
                  new XElement(ns0 + "IpRequest", clienteIp),
                  new XElement(ns0 + "Company", "GP"),
                  new XElement(ns0 + "DataSource", "AS4"),
                  new XElement(ns0 + "User", usuario)
              );

            XDocument archivoGPParticipanteXml = new XDocument(xmlGPParticipante);
            archivoGPParticipanteXml.Save(rutaLocal + "GPParticipante.xml");

            /// XML para llamar el integrador y generar el archivo que se carga a la tabla
            /// dbo.jerarquiaDetalle. (COMJJEO).
            /// 
            XElement xmlGPMultijerarquia = new XElement(ns0 + "AS4",
                  new XAttribute(XNamespace.Xmlns + "ns0", ns0),
                  new XElement(ns0 + "Library", DBLibraryCAPI),
                  new XElement(ns0 + "Program", "NOM303C1"),
                  new XElement(ns0 + "Parameters", ""),
                  new XElement(ns0 + "IpRequest", clienteIp),
                  new XElement(ns0 + "Company", "GP"),
                  new XElement(ns0 + "DataSource", "AS4"),
                  new XElement(ns0 + "User", usuario)
              );

            XDocument archivoGPMultijerarquiaXml = new XDocument(xmlGPMultijerarquia);
            archivoGPMultijerarquiaXml.Save(rutaLocal + "GPMultijerarquia.xml");

            /// XML para llamar el integrador y generar el archivo que se carga a la tabla
            /// dbo.Comision. (CODC01).
            /// 
            XElement xmlGPComisiones = new XElement(ns0 + "AS4",
                  new XAttribute(XNamespace.Xmlns + "ns0", ns0),
                  new XElement(ns0 + "Library", DBLibraryCAPI),
                  new XElement(ns0 + "Program", "NOM305C1"),
                  new XElement(ns0 + "Parameters", fechaSistemaTotalGPComisiones),
                  new XElement(ns0 + "IpRequest", clienteIp),
                  new XElement(ns0 + "Company", "GP"),
                  new XElement(ns0 + "DataSource", "AS4"),
                  new XElement(ns0 + "User", usuario)
              );

            XDocument archivoGPComisionesXml = new XDocument(xmlGPComisiones);
            archivoGPComisionesXml.Save(rutaLocal + "GPComisiones.xml");

            /// XML para llamar el integrador y generar el archivo que se carga a la tabla
            /// dbo.Ingreso. (NOM231).
            /// 
            XElement xmlGPNOM231 = new XElement(ns0 + "AS4",
                  new XAttribute(XNamespace.Xmlns + "ns0", ns0),
                  new XElement(ns0 + "Library", DBLibraryCAPI),
                  new XElement(ns0 + "Program", "NOM304C1"),
                  new XElement(ns0 + "Parameters", fechaSistemaTotalGPNOM231),
                  new XElement(ns0 + "IpRequest", clienteIp),
                  new XElement(ns0 + "Company", "GP"),
                  new XElement(ns0 + "DataSource", "AS4"),
                  new XElement(ns0 + "User", usuario)
              );

            XDocument archivoGPNOM231Xml = new XDocument(xmlGPNOM231);
            archivoGPNOM231Xml.Save(rutaLocal + "GPNOM231.xml");

            EnviarFTP();

            return 1;
        }

        /// <summary>
        /// Envia los XML de GENERALES, VIDA y CAPI al servidor FTP.
        /// </summary>
        /// <returns>0</returns>
        public int EnviarFTP()
        {
            System.Configuration.AppSettingsReader reader = new AppSettingsReader();

            string rutaLocal                              = reader.GetValue("RutaArchivos", String.Empty.GetType()).ToString(); // Ruta Local (Servidor de Apliacciones) donde se descargan los archivos XML cuando se crean desde .Net.
            string ftpServerIP                            = reader.GetValue("servidorftp", String.Empty.GetType()).ToString(); // Dirección del servidor FTP.
            string ftpUserID                              = reader.GetValue("usuarioftp", String.Empty.GetType()).ToString(); // Nombre del usuario del servidor FTP.
            string ftpPassword                            = reader.GetValue("contrasenaftp", String.Empty.GetType()).ToString(); // Contraseña del servidor FTP.

            string[] filesCAPI = { "CAPIProductos.xml", "CAPIRecaudos.xml", "GPParticipante.xml", "GPMultijerarquia.xml", "GPComisiones.xml", "GPNOM231.xml" };
            string[] filesSISE = { "SISEVIDAProductos.xml", "SISEVIDANegocios.xml", "SISEVIDARecaudos.xml", "SISEGENERALESProductos.xml", "SISEGENERALESNegocios.xml", "SISEGENERALESRecaudos.xml" };

            for (int i = 0; i <= filesCAPI.Length - 1; i++)
            {
                FileInfo info    = new FileInfo(rutaLocal + filesCAPI[i]);
                string FilePath  = rutaLocal + filesCAPI[i];
                FileInfo fileInf = new FileInfo(FilePath);               
                string uri       = "ftp://" + ftpServerIP + "/" + "IntegracionBizTalk/BiztalkIn/AS4_CAPI/" + fileInf.Name;
                FtpWebRequest reqFTP;

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + "IntegracionBizTalk/BiztalkIn/AS4_CAPI/" + fileInf.Name));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.UseBinary = true;
                reqFTP.ContentLength = fileInf.Length;

                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;

                FileStream fs = fileInf.OpenRead();

                try
                {
                    Stream strm = reqFTP.GetRequestStream();
                    contentLen  = fs.Read(buff, 0, buffLength);

                    while (contentLen != 0)
                    {
                        strm.Write(buff, 0, contentLen);
                        contentLen = fs.Read(buff, 0, buffLength);
                    }

                    strm.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    string msg = ex.ToString();
                }
            }

            for (int i = 0; i <= filesSISE.Length - 1; i++)
            {
                FileInfo info    = new FileInfo(rutaLocal + filesSISE[i]);
                string FilePath  = rutaLocal + filesSISE[i];
                FileInfo fileInf = new FileInfo(FilePath);                
                string uri       = "ftp://" + ftpServerIP + "/" + "IntegracionBizTalk/BiztalkIn/SQL_SISE/" + fileInf.Name;
                FtpWebRequest reqFTP;

                reqFTP               = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + "IntegracionBizTalk/BiztalkIn/SQL_SISE/" + fileInf.Name));
                reqFTP.Credentials   = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.KeepAlive     = false;
                reqFTP.Method        = WebRequestMethods.Ftp.UploadFile;
                reqFTP.UseBinary     = true;
                reqFTP.ContentLength = fileInf.Length;

                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;

                FileStream fs = fileInf.OpenRead();

                try
                {
                    Stream strm = reqFTP.GetRequestStream();
                    contentLen = fs.Read(buff, 0, buffLength);

                    while (contentLen != 0)
                    {
                        strm.Write(buff, 0, contentLen);
                        contentLen = fs.Read(buff, 0, buffLength);
                    }

                    strm.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    string msg = ex.ToString();
                }                
            }
            EnviarFTPBH();
            return 0;
        }

        /// <summary>
        /// Envia el archivo XML de BH al servidor FTP.
        /// </summary>
        /// <returns>0</returns>
        public int EnviarFTPBH()
        {
            System.Configuration.AppSettingsReader reader = new AppSettingsReader();

            string rutaLocal   = reader.GetValue("RutaArchivos", String.Empty.GetType()).ToString(); // Ruta Local (Servidor de Apliacciones) donde se descargan los archivos XML cuando se crean desde .Net.
            string ftpServerIP = reader.GetValue("servidorftp", String.Empty.GetType()).ToString(); // Dirección del servidor FTP.
            string ftpUserID   = reader.GetValue("usuarioftp", String.Empty.GetType()).ToString(); // Nombre del usuario del servidor FTP.
            string ftpPassword = reader.GetValue("contrasenaftp", String.Empty.GetType()).ToString();  // Contraseña del servidor FTP.

            FileInfo info    = new FileInfo(rutaLocal + "BH.xml");
            string FilePath  = rutaLocal + "BH.xml";
            FileInfo fileInf = new FileInfo(FilePath);           
            string uri       = "ftp://" + ftpServerIP + "/" + "IntegracionBizTalk/BiztalkIn/BH/" + fileInf.Name;
            FtpWebRequest reqFTP;

            reqFTP               = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + "IntegracionBizTalk/BiztalkIn/BH/" + fileInf.Name));
            reqFTP.Credentials   = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.KeepAlive     = false;
            reqFTP.Method        = WebRequestMethods.Ftp.UploadFile;
            reqFTP.UseBinary     = true;
            reqFTP.ContentLength = fileInf.Length;

            int buffLength = 2048;
            byte[] buff    = new byte[buffLength];
            int contentLen;

            FileStream fs = fileInf.OpenRead();

            try
            {
                Stream strm = reqFTP.GetRequestStream();
                contentLen  = fs.Read(buff, 0, buffLength);

                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }

                strm.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
            }

            return 0;
        }   
          
        /// <summary>
        /// Método que ejecuta un procedimiento almacenado desde .Net
        /// </summary>
        /// <param name="nombreSP">Nombre del Procedimiento Almacenado a ejecutar</param>
        /// <returns>0</returns>
        public int EjecutarSP(string nombreSP)
        {
            int result = 0;
            {
                EntityConnection entityConnection = (EntityConnection)tabla.Connection;
                DbConnection storeConnection      = entityConnection.StoreConnection;
                DbCommand command                 = storeConnection.CreateCommand();
                command.CommandText               = nombreSP;
                command.CommandType               = CommandType.StoredProcedure;                

                bool openingConnection = command.Connection.State == ConnectionState.Closed;

                if (openingConnection) { command.Connection.Open(); command.CommandTimeout = 1200; }
                try
                {
                    result = command.ExecuteNonQuery();
                }
                finally
                {
                    if (openingConnection && command.Connection.State == ConnectionState.Open) { command.Connection.Close(); }
                }
            }
            return 0;
        } 
        
        /// <summary>
        /// Método que llama a un procedimiento almacenado que ejecuta ETL desde la Base de Datos.
        /// </summary>
        /// <param name="nombreETL">Nombre del paquete o ETL a ejecutar desde la Base de Datos.</param>
        /// <returns>0</returns>
        public int EjecutarSPETL(string nombreETL)
        {
            int result = 0;
                {
                    EntityConnection entityConnection = (EntityConnection)tabla.Connection;
                    DbConnection storeConnection      = entityConnection.StoreConnection;
                    DbCommand command                 = storeConnection.CreateCommand();
                    command.CommandText               = "SAI_EjecutarETL";
                    command.CommandType               = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("nombreETL", nombreETL));

                    bool openingConnection = command.Connection.State == ConnectionState.Closed;

                    if (openingConnection) { command.Connection.Open(); command.CommandTimeout = 1200; }
                    try
                    {
                        result = command.ExecuteNonQuery();
                    }
                    finally
                    {
                        if (openingConnection && command.Connection.State == ConnectionState.Open) { command.Connection.Close(); }
                    }

                }
            return 0;
        }

        #endregion
    }
}

