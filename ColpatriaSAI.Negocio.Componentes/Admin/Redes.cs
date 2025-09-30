using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Redes
    {
        private SAI_Entities tabla = new SAI_Entities();

        #region RED
        /// <summary>
        /// Obtiene listado de las redes
        /// </summary>
        /// <returns>Lista de redes</returns>
        public List<Red> ListarRedes()
        {
            return tabla.Reds.Where(Red => Red.id > 0).ToList();
        }

        /// <summary>
        /// Obtiene listado de las redes por id
        /// </summary>
        /// <param name="idRed">Id de la red a consultar</param>
        /// <returns>Lista de objectos Red</returns>
        public List<Red> ListarRedesPorId(int idRed)
        {
            return tabla.Reds.Where(Red => Red.id == idRed && Red.id > 0).ToList();
        }

        /// <summary>
        /// Guarda un registro de red
        /// </summary>
        /// <param name="red">Objecto Red a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarRed(Red red, string Username)
        {
            int resultado = 0;

            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.Reds.Where(Red => Red.nombre == red.nombre).ToList().Count() == 0)
            {
                tabla.Reds.AddObject(red);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Red,
     SegmentosInsercion.Personas_Y_Pymes, null, red);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Actualiza un registro de Red
        /// </summary>
        /// <param name="id">Id de la Red a modificar</param>
        /// <param name="red">Objeto Red utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarRed(int id, Red red, string Username)
        {
            int resultado = 0;
            if (tabla.Reds.Where(r => r.nombre == red.nombre && r.id != red.id).ToList().Count() == 0)
            {
                var redActual = this.tabla.Reds.Where(r => r.id == id).First();
                var pValorAntiguo = redActual;
                redActual.nombre = red.nombre;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Red,
     SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, redActual);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Elimina un registro de Red
        /// </summary>
        /// <param name="id">Id de la Red a eliminar</param>
        /// <param name="red">Objecto Red utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarRed(int id, Red red, string Username)
        {
            var redActual = this.tabla.Reds.Where(Red => Red.id == id).First();
            tabla.DeleteObject(redActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Red,
    SegmentosInsercion.Personas_Y_Pymes, redActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
        #endregion

        /// <summary>
        /// Obtiene listado de las redes del detalle para ser agrupados
        /// </summary>
        /// <returns>Lista de objectos REdes Detalle para agruparlos</returns>
        public List<RedDetalle> ListarRedesDetalle()
        {
            return tabla.RedDetalles.Include("Compania").Where(b => b.id > 0).ToList();
        }

        /// <summary>
        /// Agrupa las redes detalle 
        /// </summary>
        /// <returns>True or False</returns>
        public int AgruparRedDetalle(RedDetalle redDetalle)
        {
            var redActual = this.tabla.RedDetalles.Where(b => b.id == redDetalle.id).First();
            redActual.red_id = redDetalle.red_id;
            tabla.SaveChanges();
            return 1;
        }

        /// <summary>
        /// Elimina la agrupacion las redes detalle 
        /// </summary>
        /// <returns>True or False</returns>
        public int EliminarAgrupacionRedDetalle(int idRed, string Username)
        {

            List<RedDetalle> redDetalleList = this.tabla.RedDetalles.Where(x => x.red_id == idRed).ToList();

            foreach (RedDetalle redDetalle in redDetalleList)
            {
                var redActual = this.tabla.RedDetalles.Where(b => b.id == redDetalle.id).First();
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.RedDetalle,
    SegmentosInsercion.Personas_Y_Pymes, redActual, null);
                redActual.red_id = 0;
                tabla.SaveChanges();
            }

            return 1;
        }

        #region BANCO
        /// <summary>
        /// Obtiene listado de los bancos agrupados
        /// </summary>
        /// <returns>Lista de bancos agrupados</returns>
        public List<Banco> ListarBancos()
        {
            return tabla.Bancoes.Where(b => b.id > 0).ToList();
        }

        /// <summary>
        /// Obtiene listado de los bancos agrupados por id
        /// </summary>
        /// <param name="idBanco">Id del banco agrupado a consultar</param>
        /// <returns>Lista de objectos Banco agrupado</returns>
        public List<Banco> ListarBancosPorId(int idBanco)
        {
            return tabla.Bancoes.Where(b => b.id == idBanco && b.id > 0).ToList();
        }

        /// <summary>
        /// Guarda un registro de banco agrupado
        /// </summary>
        /// <param name="banco">Objecto Banco agrupado a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarBanco(Banco banco, string Username)
        {
            int resultado = 0;

            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.Bancoes.Where(b => b.nombre == banco.nombre).ToList().Count() == 0)
            {
                tabla.Bancoes.AddObject(banco);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Banco,
    SegmentosInsercion.Personas_Y_Pymes, null, banco);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Actualiza un registro de Banco agrupado
        /// </summary>
        /// <param name="id">Id del banco agrupado a modificar</param>
        /// <param name="banco">Objeto Banco utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarBanco(int id, Banco banco, string Username)
        {
            int resultado = 0;
            if (tabla.Bancoes.Where(b => b.nombre == banco.nombre && b.id != banco.id).ToList().Count() == 0)
            {
                var bancoActual = this.tabla.Bancoes.Where(b => b.id == id).First();
                var bancoAntiguo = bancoActual;
                bancoActual.nombre = banco.nombre;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Banco, SegmentosInsercion.Personas_Y_Pymes, bancoAntiguo, bancoActual);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Elimina un registro de banco agrupado
        /// </summary>
        /// <param name="id">Id del banco agrupado a eliminar</param>
        /// <param name="banco">Objecto banco agrupado utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarBanco(int id, Banco banco, string Username)
        {
            var bancoActual = this.tabla.Bancoes.Where(b => b.id == id).First();
            tabla.DeleteObject(bancoActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Red,
    SegmentosInsercion.Personas_Y_Pymes, bancoActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        /// <summary>
        /// Obtiene listado de los bancos del detalle para ser agrupados
        /// </summary>
        /// <returns>Lista de objectos Banco Detalle para agruparlos</returns>
        public List<BancoDetalle> ListarBancosDetalle()
        {
            return tabla.BancoDetalles.Include("Compania").Where(b => b.id > 0).ToList();
        }

        /// <summary>
        /// Agrupa los bancos detalle 
        /// </summary>
        /// <returns>True or False</returns>
        public int AgruparBancoDetalle(BancoDetalle bancoDetalle)
        {
            var bancoActual = this.tabla.BancoDetalles.Where(b => b.id == bancoDetalle.id).First();
            bancoActual.banco_id = bancoDetalle.banco_id;
            tabla.SaveChanges();
            return 1;
        }

        /// <summary>
        /// Elimina la agrupacion los bancos detalle 
        /// </summary>
        /// <returns>True or False</returns>
        public int EliminarAgrupacionBancoDetalle(int idBanco, string Username)
        {

            List<BancoDetalle> bancoDetalleList = this.tabla.BancoDetalles.Where(x => x.banco_id == idBanco).ToList();

            foreach (BancoDetalle bancoDetalle in bancoDetalleList)
            {
                var bancoActual = this.tabla.BancoDetalles.Where(b => b.id == bancoDetalle.id).First();
                bancoActual.banco_id = 0;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.BancoDetalle,
   SegmentosInsercion.Personas_Y_Pymes, bancoActual, null);
                tabla.SaveChanges();
            }

            return 1;
        }

        #endregion
    }
}

