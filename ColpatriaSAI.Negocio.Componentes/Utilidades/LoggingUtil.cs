// <copyright file="Logging.cs" company="Avantis">
//     COPYRIGHT(C), 2011, Avantis S.A.
// </copyright>
// <author>Frank Payares</author>

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.Specialized;
using System.Threading;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
//using ColpatriaSAI.UI.MVC.Views.Shared;
using ColpatriaSAI.Negocio.Entidades.Informacion;
using System.Security.Principal;
using System.Net;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.ServiceLocation;

namespace ColpatriaSAI.Negocio.Componentes.Utilidades
{

    /// <summary>
    /// Nombre del módulo a registrar
    /// </summary>
    public static class Modulo {
        public static readonly string General = "General";
        public static readonly string Contratacion = "Contratación";
        public static readonly string Franquicias = "Franquicias";
        public static readonly string Concursos = "Concursos";
        public static readonly string Jerarquia = "Jerarquia";
    }
    /// <summary>
    /// Estructura para los tipos de eventos
    /// </summary>
    public struct TipoEvento {
        public const TraceEventType Error = TraceEventType.Error;
        public const TraceEventType Advertencia = TraceEventType.Warning;
        public const TraceEventType Critico = TraceEventType.Critical;
        public const TraceEventType Informacion = TraceEventType.Information;
        public const TraceEventType Inicio = TraceEventType.Start;
        public const TraceEventType Parada = TraceEventType.Stop;
    }

    /// <summary>
    /// Clase para registrar eventos de la aplicación en el Event Viewer de Windows
    /// </summary>
    public class LoggingUtil
    {
        //private LogWriter entradaLog = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
        /// <summary>
        /// Importancia del evento a registrar
        /// </summary>
        public enum Prioridad {
            MuyAlta = 4,
            Alta = 3,
            Media = 2,
            Baja = 1
        }

        /// <summary>
        /// Inserta un registro en el Event Viewer con categoria Información
        /// </summary>
        /// <param name="mensaje">Descripción del evento a registrar</param>
        /// <param name="prioridad">Importancia del evento</param>
        /// <param name="modulo">Origen del evento</param>
        /// <param name="Info">Usuario e IP de donde se originó el evento</param>
        public bool Auditoria(string mensaje, Prioridad prioridad, string modulo, InfoAplicacion Info) {
            LogWriter logEscritor = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
            bool valor = false;

            LogEntry entradaLog = new LogEntry();
            entradaLog = construirLog(mensaje, prioridad, modulo, TipoEvento.Informacion, Info);

            if (prioridad == Prioridad.MuyAlta)
                entradaLog.Categories = new List<string>() { "Auditoria", "Correo" };
            else
                entradaLog.Categories = new List<string>() { "Auditoria" };
            
            logEscritor.Write(entradaLog);
            valor = true;

            return valor;
        }

        /// <summary>
        /// Inserta un registro en el Event Viewer de Windows con categoria Error y Advertencia
        /// </summary>
        /// <param name="mensaje">Descripcion del evento a registrar</param>
        /// <param name="prioridad">Frecuencia con que ocurre el evento</param>
        /// <param name="modulo">Origen del evento</param>
        /// <param name="tipoEvento">Tipo de evento generado</param>
        /// <param name="Info">Usuario e IP de donde se originó el error</param>
        public bool Error(string mensaje, Prioridad prioridad, string modulo, TraceEventType tipoEvento, InfoAplicacion Info)
        {
            LogWriter logEscritor = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
            bool valor = false;

            //if (!EventLog.SourceExists("SAI")) EventLog.CreateEventSource("SAI", "Application");

            LogEntry entradaLog = new LogEntry();
            entradaLog = construirLog(mensaje, prioridad, modulo, tipoEvento, Info);

            if (prioridad == Prioridad.MuyAlta)
                entradaLog.Categories = new List<string>() { "Errores", "Correo" };
            else
                entradaLog.Categories = new List<string>() { "Errores" };

            logEscritor.Write(entradaLog);
            valor = true;

            return valor;
        }

        private LogEntry construirLog(string mensaje, Prioridad prioridad, string modulo, TraceEventType tipoEvento, InfoAplicacion Info)
        {
            LogEntry entradaLog = new LogEntry();

            String[] parametros = { "\n", mensaje, Info.NombreUsuario, Info.DireccionIP };
            String texto1 = String.Format("Mensaje:\t\t{1}{0}Usuario:\t{2}{0}IP:\t{3}{0}", parametros);

            entradaLog.Message = texto1;
            entradaLog.Severity = tipoEvento;
            entradaLog.Title = modulo;
            entradaLog.EventId = 0;
            entradaLog.Priority = (int)prioridad;
            return entradaLog;
        }
    }
}