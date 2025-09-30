using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Seguridad
{
    class Usuario
    {
        private SAI_Entities contexto = new SAI_Entities();

        public int CrearUsuario(string nombreUsuario, string tipoDocumento, string numeroDocumento, string email, string rol, int segmento, string Username)
        {
            EntityConnection entityConnection = (EntityConnection)contexto.Connection;
            DbConnection storeConnection = entityConnection.StoreConnection;
            DbCommand command = storeConnection.CreateCommand();
            command.CommandText = "SAI_Usuarios_CrearUsuario";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("UserName", nombreUsuario));
            command.Parameters.Add(new SqlParameter("TipoIdentificacion", tipoDocumento));
            command.Parameters.Add(new SqlParameter("NumeroIdentificacion", numeroDocumento));
            command.Parameters.Add(new SqlParameter("Email", email));
            command.Parameters.Add(new SqlParameter("RoleNames", rol));
            command.Parameters.Add(new SqlParameter("Segmento", segmento));

            bool openingConnection = command.Connection.State == ConnectionState.Closed;

            if (openingConnection) { command.Connection.Open(); command.CommandTimeout = 10000; }

            int result;

            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.UsuarioxSegmento,
    SegmentosInsercion.Personas_Y_Pymes, null, null);
                result = command.ExecuteNonQuery();
            }
            finally
            {
                if (openingConnection && command.Connection.State == ConnectionState.Open) { command.Connection.Close(); }
            }
            return result;
        }

        public int InsertarSegmentodeUsuario(UsuarioxSegmento usuarioxsegmento, string Username)
        {
            int result = 0;

            try
            {
                contexto.UsuarioxSegmentoes.AddObject(usuarioxsegmento);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.UsuarioxSegmento,
    SegmentosInsercion.Personas_Y_Pymes, null, usuarioxsegmento);
                contexto.SaveChanges();
                result = usuarioxsegmento.id;
            }
            catch (Exception ex)
            {
                result = 0;
            }
            return result;
        }

        public int EliminarSegmentodeUsuario(UsuarioxSegmento usuarioxsegmento, string Username)
        {
            int result = 0;
            UsuarioxSegmento usuariodelete = contexto.UsuarioxSegmentoes.Where(x => x.id == usuarioxsegmento.id).FirstOrDefault();
            try
            {
                contexto.UsuarioxSegmentoes.DeleteObject(usuariodelete);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.UsuarioxSegmento,
    SegmentosInsercion.Personas_Y_Pymes, usuariodelete, null);
                contexto.SaveChanges();
                result = 1;
            }
            catch (Exception ex)
            {
                result = 0;
            }
            return result;
        }
    }
}
