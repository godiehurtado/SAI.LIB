using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using Entidades = ColpatriaSAI.Negocio.Entidades;
using ColpatriaSAI.Negocio.Componentes.Comision;
using ColpatriaSAI.Datos;

namespace ColpatriaSAI.Negocio.Componentes.Utilidades
{
    public class Auditoria
    {

        private SAI_Entities _dbcontext = new SAI_Entities();

        /// <summary>
        /// Objeto que tiene los datos a guardar
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string CrearDescripcionAuditoria(object obj)
        {
            try
            {
                Type objeto = null;
                PropertyInfo[] propiedades = null;

                objeto = obj.GetType();
                propiedades = objeto.GetProperties();

                var stringBuilder = new StringBuilder();

                stringBuilder.Append("<p>");

                //Recorre el objeto para sacar  los elementos y armar el codigo HTML que se muestra despues. (Titulos)
                for (int i = 1; i < propiedades.Count()-1; i++)
                {
                    //CAAM System.Nullable
                    if (!obj.GetType().GetProperty(propiedades[i].Name).ToString().Contains("System.Nullable"))
                    {
                        if (!obj.GetType().GetProperty(propiedades[i].Name).ToString().ToUpper().Contains("COLPATRIA"))
                            stringBuilder.Append(propiedades[i].Name + ": " + obj.GetType().GetProperty(propiedades[i].Name).GetValue(obj, null) + "<br/>");
                        //else
                        //{

                        //    var objClass = obj.GetType().GetProperty(propiedades[i].Name).GetValue(obj, null).GetType();
                        //    PropertyInfo[] objClassprop = objClass.GetProperties();
                        //    foreach (var item in objClassprop)
                        //    {
                        //        if (item.Name != "Count" && item.Name != "Item")
                        //        {
                        //            if (item.Name.ToUpper().Equals("ID"))
                        //            {
                        //                stringBuilder.AppendFormat(@"{0}.ID:{1}</br>", objClass.Name, objClass.GetType().GetProperty(item.Name).GetValue(objClass, null));
                        //                break;
                        //            }
                        //            else
                        //            {
                        //                var objClass2 = obj.GetType().GetProperty(item.Name).GetValue(obj, null).GetType();
                        //                PropertyInfo[] objClassprop2 = objClass.GetProperties();
                        //                foreach (var item2 in objClassprop2)
                        //                {
                        //                    if (item2.Name.ToUpper().Equals("ID"))
                        //                    {
                        //                        stringBuilder.AppendFormat(@"{0}.ID:{1}</br>", objClass2.Name, objClass2.GetType().GetProperty(item2.Name).GetValue(objClass2, null));
                        //                        break;
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    }
                }
                

                stringBuilder.Append("</p>");

                return stringBuilder.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evento"></param>
        /// <param name="FechaEvento"></param>
        /// <param name="UserName"></param>
        /// <param name="NombreTabla"></param>
        /// <param name="ObjetoAntiguo"></param>
        /// <param name="ObjetoNuevo"></param>
        /// <returns></returns>
        public ResultadoOperacionBD InsertarAuditoria(Entidades.tipoEventoTabla evento, DateTime FechaEvento, string UserName, Entidades.tablasAuditadas Tabla, Entidades.SegmentosInsercion Segmento, object ObjetoAntiguo = null, object ObjetoNuevo = null)
        {
            ResultadoOperacionBD result = new ResultadoOperacionBD();
            try
            {
                string lDescripcionNueva = string.Empty;
                string lDescripcionAntigua = string.Empty;



                 if (ObjetoAntiguo != null && ObjetoAntiguo.GetType().ToString() == "System.String")
                {
                    lDescripcionAntigua = ObjetoAntiguo.ToString();
                }
                else if (ObjetoAntiguo != null )
                {
                    lDescripcionAntigua = CrearDescripcionAuditoria(ObjetoAntiguo);
                }
               

                if (ObjetoNuevo != null && ObjetoNuevo.GetType().ToString() == "System.String")
                {
                    lDescripcionNueva = ObjetoNuevo.ToString();
                }
                else if (ObjetoNuevo != null)
                {
                    lDescripcionNueva = CrearDescripcionAuditoria(ObjetoNuevo);
                }

                Entidades.Auditoria auditoria = new Entidades.Auditoria()
                {
                    Fecha = DateTime.Now,
                    id_EventoTabla = (int)evento,
                    id_TablaAuditada = (int)Tabla,
                    Usuario = UserName,
                    id_Segmento = (int)Segmento,
                    Version_Anterior = lDescripcionAntigua,
                    Version_Nueva = lDescripcionNueva
                };

                _dbcontext.Auditorias.AddObject(auditoria);
                result.RegistrosAfectados = 1;
                result.Resultado = ResultadoOperacion.Exito;
                _dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                result.RegistrosAfectados = 0;
                result.Resultado = ResultadoOperacion.Error;
                result.MensajeError = ex.Message;
            }
            return result;
        }
    }
}
