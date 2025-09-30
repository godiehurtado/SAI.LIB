using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;


namespace ColpatriaSAI.Seguridad.Proveedores
{
    public class MvcSQLSitemapProvider : StaticSiteMapProvider
    {
        private const string _errormsg1 = "Missing node ID";
        private const string _errormsg2 = "MDuplicate node ID";
        private const string _errormsg3 = "Missing parent ID";
        private const string _errormsg4 = "Invalid parent ID";
        private const string _errormsg5 = "Empty or missing connection string";
        private const string _errormsg6 = "Missing connection string";
        private const string _errormsg7 = "Empty connection string";

        private string _connect = String.Empty;
        private Dictionary<string, SiteMapNode> _nodes = new Dictionary<string,SiteMapNode>(16);
        private SiteMapNode _root;
         

        //public MvcSQLSitemapProvider(string name,NameValueCollection attributes)
        //{
        //    this.Initialize(name, attributes);
        //}

        public override void Initialize(string name, NameValueCollection attributes)
        {
            //Verifica se attributes é nulo
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            //Se não tiver um nome, atribui um
            if (string.IsNullOrEmpty(name))
                name = "MvcSitemapProvider";

            //Adicionar uma descrição por defeito caso não exista
            if (string.IsNullOrEmpty(attributes["description"]))
            {
                attributes.Remove("description");
                attributes.Add("description", "MVC site map provider");
            }

            //Chamar a inicialização da classe base
            base.Initialize(name, attributes);

            //Inicializar a conecção
            string connect = attributes["connectionStringName"];

            if (string.IsNullOrEmpty(connect))
                throw new ProviderException(_errormsg5);

            attributes.Remove("connectionStringName");

            if (WebConfigurationManager.ConnectionStrings[connect] == null)
                throw new ProviderException(_errormsg6);

            _connect = WebConfigurationManager.ConnectionStrings[connect].ConnectionString;

            if (string.IsNullOrEmpty(_connect))
                throw new ProviderException(_errormsg7);

            //Enviar erro se houver mais atributos
            if (attributes.Count > 0)
            {
                string attr = attributes.GetKey(0);
                if (!string.IsNullOrEmpty(attr))
                    throw new ProviderException(string.Format("Unrecognized attribute: {0}", attr));
            }
        }

        public override SiteMapNode BuildSiteMap()
        {
            lock (this)
            {
                //retornar se o método foi executado antes
                if (_root != null)
                    return _root;

                InvocarMetodoWS invocarws = new InvocarMetodoWS(System.Configuration.ConfigurationManager.AppSettings["WsMvcSQLSitemapProvider"]);
                // llmamos el metodo del webservice  que nos devuelve un resultado
                string resultado = invocarws.Invocarmetodo1("SiteMapProviderSoap", "BuildSiteMap");

               
                //resultado = resultado.Replace("~", HttpUtility.UrlEncode("~"));
                //resultado = resultado.Replace("/", HttpUtility.UrlEncode("/"));

                //var resultado2 = HttpUtility.UrlEncode(resultado);
               


                SiteMapContextDataContext db = new SiteMapContextDataContext(_connect);

                try
                {
                    MemoryStream stream = new MemoryStream();

                  StreamWriter swriter = new StreamWriter(stream, System.Text.Encoding.Unicode);
                   swriter.Write(resultado);
                    
                    swriter.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    //XmlDocument siteMapXml = LoadSiteMapXml(stream);
                    var siteMpaQuery2 = from s in XElement.Load(stream).Elements("siteMapNode")
                                        select s;
                    foreach (var item2 in siteMpaQuery2)
                    {

                        SITEMAP item = new SITEMAP();
                        item.ID = item2.Element("id").Value.ToString();
                        item.CONTROLLER = item2.Element("controller").Value.ToString();

                        if (item2.Element("url").Value.ToString() == "")
                        {
                            item.URL = null;
                        }
                        else
                        {
                           
                                item.URL = item2.Element("url").Value.ToString();
                          
                           
                        }
                       
                        item.DESCRIPTION = item2.Element("description").Value.ToString();
                        item.TITLE = item2.Element("title").Value.ToString();
                        item.ROLES = item2.Element("roles").Value.ToString();
                        item.PARENT_ID = item2.Element("parent_id").Value.ToString();
                       
                         if (item2.Equals(siteMpaQuery2.First()))
                         {
                             _root = CreateSiteMapFromRow(item);
                             AddNode(_root, null);
                         }
                         else
                         {
                             SiteMapNode node = CreateSiteMapFromRow(item);
                             AddNode(node, GetParentNodeFromNode(item));
                         }

                    }
                }
                
                catch (XmlException xmlEx)
                {
                    //throw xmlEx;  
                }
                   
                
                
                
                //XmlDocument siteMapXml = LoadSiteMapXml(resultado);




              
                //var siteMpaQuery = from s in db.SITEMAPs
                //                   orderby s.ID
                //                   select s;

                //foreach (var item in siteMpaQuery)
                //{
                //    if (item.Equals(siteMpaQuery.First()))
                //    {
                //        _root = CreateSiteMapFromRow(item);
                //        AddNode(_root, null);
                //    }
                //    else
                //    {
                //        SiteMapNode node = CreateSiteMapFromRow(item);
                //        AddNode(node, GetParentNodeFromNode(item));
                //    }
                //}

                return _root;
            }
        }

        private XmlDocument LoadSiteMapXml(MemoryStream xml)
        {

            XmlDocument siteMapXml = new XmlDocument();

            siteMapXml.Load(xml);

            return siteMapXml;

        }





        private SiteMapNode CreateSiteMapFromRow(SITEMAP item)
        {
            if (_nodes.ContainsKey(item.ID))
                throw new ProviderException(_errormsg2);

            // Ciar um novo sitemapnode 
            SiteMapNode node = new SiteMapNode(this, item.ID);

            if (!string.IsNullOrEmpty(item.URL))
            {
                node.Title = string.IsNullOrEmpty(item.TITLE) ? null : item.TITLE;
                node.Description = string.IsNullOrEmpty(item.DESCRIPTION) ? null : item.DESCRIPTION;
                node.Url = string.IsNullOrEmpty(item.URL) ? null : item.URL;

            }
            else
            {
                node.Title = string.IsNullOrEmpty(item.TITLE) ? null : item.TITLE;
                node.Description = string.IsNullOrEmpty(item.DESCRIPTION) ? null : item.DESCRIPTION;

                // Crear o objecto routeValues para despues construir la URL
                IDictionary<string, object> routeValues = new Dictionary<string, object>();
 
                if (string.IsNullOrEmpty(item.CONTROLLER))
                    routeValues.Add("controller", "Home");
                else
                    routeValues.Add("controller", item.CONTROLLER);
                
                if (string.IsNullOrEmpty(item.CONTROLLER))
                    routeValues.Add("action", "Index");
                else
                    routeValues.Add("action", item.ACTION);

                if (!string.IsNullOrEmpty(item.PARAMID))
                    routeValues.Add("id", item.PARAMID);

                // crear una URL
                HttpContextWrapper httpContext = new HttpContextWrapper(HttpContext.Current);
                RouteData routeData = RouteTable.Routes.GetRouteData(httpContext);
                if (routeData != null)
                {

                   // RouteCollection rc = new RouteCollection();
                   //RouteData routeData2 = rc.GetRouteData(httpContext);
                   // VirtualPathData virtualPath3 = System.Web.Mvc.RouteCollectionExtensions.GetVirtualPathForArea(rc,
                   //                                                                                               new RequestContext
                   //                                                                                                   (httpContext,
                   //                                                                                                    routeData2),
                   //                                                                                               new RouteValueDictionary
                   //                                                                                                   (routeValues));


                    //VirtualPathData virtualPath2 = rc.GetVirtualPath(new RequestContext(httpContext, routeData), new RouteValueDictionary(routeValues));
               
                    
                    VirtualPathData virtualPath = routeData.Route.GetVirtualPath(new RequestContext(httpContext, routeData), new RouteValueDictionary(routeValues));

                   if (virtualPath != null)
                    {
                        node.Url = "~/" + virtualPath.VirtualPath;
                    }
                    //else
                    //{
                    //    canCache = false;
                    //}
                }
            }
            _nodes.Add(item.ID, node);

            return node;
        }

        private SiteMapNode GetParentNodeFromNode(SITEMAP item)
        {
            if (!_nodes.ContainsKey(item.PARENT_ID))
                throw new ProviderException(_errormsg4);

            return _nodes[item.PARENT_ID];
        }

        protected override SiteMapNode GetRootNodeCore()
        {
            return BuildSiteMap();
        }
    }
}
