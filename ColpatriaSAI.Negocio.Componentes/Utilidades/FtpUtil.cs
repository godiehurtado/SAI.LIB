using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace ColpatriaSAI.Negocio.Componentes.Utilidades
{
    public static class FtpUtil
    {
        public const int BUFFER_SIZE = 100000;
        public static string rutaFTPCargue = System.Configuration.ConfigurationManager.AppSettings["RutaFTPCargue"];
        public static string ipFtp = System.Configuration.ConfigurationManager.AppSettings["servidorftp"];
        public static string user = System.Configuration.ConfigurationManager.AppSettings["usuarioftp"];
        public static string pass = System.Configuration.ConfigurationManager.AppSettings["contrasenaftp"];

        public static bool descargarArchivo(string rutaArchivo, string nombreArchivo)
        {
            var rutaLocal = Path.Combine(rutaArchivo, nombreArchivo);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + ipFtp + rutaFTPCargue + nombreArchivo);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(user, pass);

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (FileStream destination = File.Create(rutaLocal))
                {
                    byte[] buffer = new byte[BUFFER_SIZE];
                    while (true)
                    {
                        int read = responseStream.Read(buffer, 0, BUFFER_SIZE);
                        if (read <= 0)
                            break;
                        destination.Write(buffer, 0, read);
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public static bool eliminarArchivo(string rutaArchivo, string nombreArchivo)
        {
            var rutaLocal = Path.Combine(rutaArchivo, nombreArchivo);

            FtpWebRequest requestDelete = (FtpWebRequest)WebRequest.Create("ftp://" + ipFtp + rutaFTPCargue + nombreArchivo);
            requestDelete.Method = WebRequestMethods.Ftp.DeleteFile;
            requestDelete.Credentials = new NetworkCredential(user, pass);

            try
            {
                FtpWebResponse responseFileDelete = (FtpWebResponse)requestDelete.GetResponse();
                if (responseFileDelete.StatusDescription.Contains("250")) return true;
            }
            catch { return false; }
            return false;
        }
    }
}