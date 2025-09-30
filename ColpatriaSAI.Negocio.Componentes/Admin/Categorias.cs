using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{
    public class Categorias
    {
        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado de las Categorias
        /// </summary>
        /// <returns>Lista de Categorias</returns>
        public List<Categoria> ListarCategorias()
        {
            return tabla.Categorias.Include("Nivel").Where(Categoria => Categoria.id > 0).ToList();
        }

        /// <summary>
        /// Obtiene listado de Categorias por id
        /// </summary>
        /// <param name="idRed">Id de la Categoria a consultar</param>
        /// <returns>Lista de objectos Categoria</returns>
        public List<Categoria> ListarCategoriasPorId(int id)
        {
            return tabla.Categorias.Include("Nivel").Where(Categoria => Categoria.id == id && Categoria.id > 0).ToList();
        }

        /// <summary>
        /// Guarda un registro de Categoria
        /// </summary>
        /// <param name="zona">Objecto Categoria a insertar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int InsertarCategoria(Categoria categoria, string Username)
        {
            int resultado = 0;

            // Buscar si ya existe un registro igual, para evitar la creación de duplicados.
            if (tabla.Categorias.Where(Categoria => Categoria.nombre.Trim() == categoria.nombre.Trim()).ToList().Count() == 0)
            {
                tabla.Categorias.AddObject(categoria);
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Categoria,
    SegmentosInsercion.Personas_Y_Pymes, null, categoria);
                resultado = tabla.SaveChanges();
            }
            return resultado;

        }
        /// <summary>
        /// Actualiza un registro de Categoria
        /// </summary>
        /// <param name="id">Id de la Categoria a modificar</param>
        /// <param name="zona">Objeto Categoria utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarCategoria(int id, Categoria categoria, string Username)
        {
            int resultado = 0;
            if (tabla.Categorias.Where(Categoria => Categoria.nombre.Trim() == categoria.nombre.Trim()).ToList().Count() == 0)
            {
                var categoriaActual = this.tabla.Categorias.Where(Categoria => Categoria.id == id && Categoria.id > 0).First();
                var pValorAntiguo = categoriaActual;
                categoriaActual.nombre = categoria.nombre;
                //categoriaActual.nivel_id  = categoria.nivel_id;
                //categoriaActual.principal = categoria.principal;
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Categoria,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, categoriaActual);
                resultado = tabla.SaveChanges();
            }
            return resultado;
        }

        /// <summary>
        /// Elimina un registro de Categoria
        /// </summary>
        /// <param name="id">Id de la Categoria a eliminar</param>
        /// <param name="zona">Objecto Categoria utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public string EliminarCategoria(int id, Categoria categoria, string Username)
        {
            var categoriaActual = this.tabla.Categorias.Where(Categoria => Categoria.id == id && Categoria.id > 0).First();
            tabla.DeleteObject(categoriaActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.Categoria,
    SegmentosInsercion.Personas_Y_Pymes, categoriaActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
    }
}

