using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;

namespace ColpatriaSAI.Negocio.Componentes.Admin
{


    public class SiteMapes
    {

        private SAI_Entities tabla = new SAI_Entities();

        /// <summary>
        /// Obtiene listado del SiteMap
        /// </summary>
        /// <returns>Lista del SiteMap</returns>
        public List<Entidades.SiteMap> ListarSiteMap()
        {
            return tabla.SiteMaps.ToList();
        }

        /// <summary>
        /// Guarda un registro del SiteMap
        /// </summary>
        /// <param name="sitemap">Objecto SiteMap a insertar</param>
        /// <returns>Numero de registros guardados</returns>
        public int InsertarSiteMap(Entidades.SiteMap sitemap, string Username)
        {
            tabla.SiteMaps.AddObject(sitemap);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.SiteMap,
    SegmentosInsercion.Personas_Y_Pymes, null, sitemap);
            return tabla.SaveChanges();
        }

        /// <summary>
        /// Obtiene listado de SiteMap por id
        /// </summary>
        /// <returns>Lista SiteMap</returns>
        public List<Entidades.SiteMap> ListarSiteMapPorId(String idSiteMap)
        {
            return tabla.SiteMaps.Where(siteMap => siteMap.ID == idSiteMap).ToList();
        }

        /// <summary>
        /// Actualiza un registro de siteMap
        /// </summary>
        /// <param name="id">Id de la siteMap a modificar</param>
        /// <param name="sitemap">Objeto siteMap utilizado para modificar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int ActualizarsiteMap(string id, SiteMap sitemap, string Username)
        {
            var siteMapActual = this.tabla.SiteMaps.Where(siteMap => siteMap.ID == id).First();
            var pValorAntiguo = siteMapActual;
            siteMapActual.TITLE = sitemap.TITLE;
            siteMapActual.DESCRIPTION = sitemap.DESCRIPTION;
            siteMapActual.Roles = sitemap.Roles;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.SiteMap,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, siteMapActual);
            return tabla.SaveChanges();
        }


        /// <summary>
        /// Elimina un registro de siteMap
        /// </summary>
        /// <param name="id">Id de la siteMap a eliminar</param>
        /// <param name="sitemap">Objecto siteMap utilizado para eliminar</param>
        /// <returns>Resultado de la operacion tipo int</returns>
        public int Eliminarsitemap(string id, SiteMap sitemap, string Username)
        {
            var sitemapActual = this.tabla.SiteMaps.Where(siteMap => siteMap.ID == id).First();
            tabla.DeleteObject(sitemapActual);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.SiteMap,
                SegmentosInsercion.Personas_Y_Pymes, sitemapActual, null);
            return tabla.SaveChanges();
        }

        public List<Entidades.aspnet_Roles> ListarRoles()
        {
            return tabla.aspnet_Roles.ToList();
        }
        public Entidades.aspnet_Roles GetRolById(System.Guid id)
        {
            aspnet_Roles roles = new aspnet_Roles();
            return tabla.aspnet_Roles.Where(roleses => roleses.RoleId == id).First();

        }
    }
}
