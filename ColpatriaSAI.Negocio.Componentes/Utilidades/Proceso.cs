using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Negocio.Entidades;
using ColpatriaSAI.Datos;
using System.Net.Mail;

namespace ColpatriaSAI.Negocio.Componentes.Utilidades
{
    public class Proceso
    {
        public enum CorreoModulo {
            Jerarquia = 2
        }
        private static SAI_Entities tabla = new SAI_Entities();

        public static int registrarProceso(ProcesoLiquidacion proceso)
        {
            tabla.ProcesoLiquidacions.AddObject(proceso);
            tabla.SaveChanges();

            return proceso.id;
        }

        public static void eliminarProceso(int proceso_id)
        {
            ProcesoLiquidacion proceso = tabla.ProcesoLiquidacions.Where(p => p.id == proceso_id).Single();
            tabla.ProcesoLiquidacions.DeleteObject(proceso);
            tabla.SaveChanges();
        }

        public static string enviarCorreo(CorreoModulo correoPara, string asunto, string mensaje)
        {
            string destinatario = "";
            int idParametro = (int)correoPara; string estadoEnvio = "";
            destinatario = tabla.ParametrosApps.Where(p => p.id == idParametro).First().valor;

            var correo = new MailMessage();
            correo.To.Add(new MailAddress(destinatario));
            correo.Subject = asunto;
            correo.IsBodyHtml = true;
            correo.Body = "<span style='font:12px Arial'>***ATENCIÓN: Este remitente solo es utilizado para enviar notificaciones. Por favor no responder a dicho correo***</span><br /><br />"
                + mensaje;

            var servidorCorreo = new SmtpClient();
            try {
                servidorCorreo.Send(correo);
                estadoEnvio = "";
            }
            catch (Exception ex) {
                var tipo = ex.GetType();
                if (tipo == typeof(SmtpException))
                    estadoEnvio = "Error al enviar la notificación de actualización de la jeraraquía.";

                if (tipo == typeof(SmtpFailedRecipientException))
                    estadoEnvio = "Error del destinatario al recibir la notificación de actualización de la jeraraquía.";
            }
            finally { servidorCorreo.Dispose(); }
            return estadoEnvio;
        }
    }
}
            /*servidorCorreo.SendCompleted += delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
                if (e.Cancelled) {
                    string cancelado = string.Format("[{0}] Envio Cancelado.", e.UserState);
                }
                if (e.Error != null) {
                    string error = String.Format("[{0}] {1}", e.UserState, e.Error.ToString());
                }
                else { }
            };*/