using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Threading.Tasks;

namespace ColpatriaSAI.Negocio.Componentes.LiqFranquicias
{

    // este archivo unicamente sirve para hacer cargue en la parametrizacion de liquidacion franquicias( cargue de exepciones , detpartifranquicia y rangos)
    public class CargueDatosLiqFranquicias
    {
        /// <summary>
        /// Metodo que devuelve un string con la informacion del proceso de liquidacion franquicias
        /// </summary>
        /// <param name="fechaini"> fecha inicial a liquidar </param>
        /// <param name="fechafinal">fecha final a liquidar </param>
        /// <returns></returns>
        public string IniciarHiloLiquidacionFranquicia2(DateTime fechaini, DateTime fechafinal)
        {
           //obiene los parametros ( rango de fechas) a consultar
            object[] parameters = new object[] {fechaini, fechafinal};
         
            StringBuilder mensaje = new StringBuilder();
            try
            {
                //ejecucion del metodo IniciarLiquidacionFranquicia enviado los parametros
                mensaje.Append(IniciarLiquidacionFranquicia(parameters));
            }
            catch (AggregateException ae)
            {
              // recorrre las excepciones internas que se generaron en el proceso de liquiodación
                foreach (var ex in ae.InnerExceptions)
                {
                    if (ex is ArgumentException)
                        mensaje.Append(" Errores:" + ex.Message);
                       
                    else
                        throw ex;
                }
            }

            // retorna el mensaje sobre el proceso de liquidación
            return mensaje.ToString();
           


        }

        public static string IniciarLiquidacionFranquicia(object parameters) //DateTime fechaini, DateTime fechafinal)
        {
            SAI_Entities contexto = new SAI_Entities();

            int countexceptions = 0;
            int countdetpartfra = 0;
            int countdetpartfrara = 0;

            var exceptions = new ConcurrentQueue<Exception>();
            object[] parameterArray = (object[]) parameters;
            DateTime fechaini = DateTime.Parse(parameterArray[0].ToString());
            DateTime fechafinal = DateTime.Parse(parameterArray[1].ToString());
            StringBuilder mensaje = new StringBuilder();
            int Regliquidados = 0;



            //using (var contexto2 = new SAI_Entities())
            //{
            //List<ColpatriaSAI.Negocio.Entidades.Recaudo> recaudos =
            //    contexto.TraerRecaudosPorFranquicias(fechaini, fechafinal).AsParallel().ToList();

            //var recaudos = from recaudo in contexto.Recaudoes.OfType<ColpatriaSAI.Negocio.Entidades.Recaudo>()
                           //where (recaudo.fechaCobranza >= fechaini && recaudo.fechaCobranza <= fechafinal)
            //               select recaudo;





            //List<ColpatriaSAI.Negocio.Entidades.Excepcion> excepciones = contexto.Excepcions.AsParallel().ToList();

            //    from exception in contexto2.Excepcions.OfType<ColpatriaSAI.Negocio.Entidades.Excepcion>()
            //    select exception;



            //List<ColpatriaSAI.Negocio.Entidades.DetallePartFranquicia> detallepartfranq =
                contexto.DetallePartFranquicias.AsParallel().ToList();


            //    from detpa in
            //        contexto2.DetallePartFranquicias.OfType<ColpatriaSAI.Negocio.Entidades.DetallePartFranquicia>()
            //    select detpa;

            List<ColpatriaSAI.Negocio.Entidades.DetalleLiquiRangosFranq> detliqrangfra =
                contexto.DetalleLiquiRangosFranqs.AsParallel().ToList();
            //    from detra in
            //        contexto2.DetalleLiquiRangosFranqs.OfType
            //        <ColpatriaSAI.Negocio.Entidades.DetalleLiquiRangosFranq>()
            //    select detra;

            LiquidacionFranquicia liquidacionFranquicia = new LiquidacionFranquicia();
            liquidacionFranquicia.fechaLiquidacion = DateTime.Now;
            liquidacionFranquicia.periodoLiquidacionIni = fechaini;

            liquidacionFranquicia.periodoLiquidacionFin = fechafinal;

            mensaje.Append("Procesados:" + 14.ToString());


            int distribucion = 0;

            distribucion = (14/14);


            if (14 > 0)
            {


                //try
                //{


                //Parallel.ForEach(recaudos, recaudo =>
                //                               {




                int i = 0;
                List<ColpatriaSAI.Negocio.Entidades.Recaudo> recaudoses = new List<Recaudo>();

              
                {
                    
                }

                for (int j = 0; j < 14; j++)
                {
                    DetalleLiquidacionFranquicia detalleLiquidacionFranquicia =
                        new DetalleLiquidacionFranquicia();





                    // Console.WriteLine("procesando:" + recaudo.id.ToString());

                    //	Compañía – Línea de negocio – detalleLiquidacionFranquicias.ramo_id– detalleLiquidacionFranquicias.producto_id– detalleLiquidacionFranquicias.zona_id– detalleLiquidacionFranquicias.localidad_id– detalleLiquidacionFranquicias.claveParticipante– Negocio  
                    switch (j)
                    {
                        case 0:
                            
                                
                                detalleLiquidacionFranquicia.porcentajeParticipacion = 10;

                                detalleLiquidacionFranquicia.localidad_id = 61;
                                detalleLiquidacionFranquicia.zona_id = 1;
                                detalleLiquidacionFranquicia.compania_id = 1;
                                detalleLiquidacionFranquicia.ramo_id = 1;
                                detalleLiquidacionFranquicia.producto_id = 1;
                                detalleLiquidacionFranquicia.numeroNegocio = 112193610000.ToString();

                                detalleLiquidacionFranquicia.claveParticipante = 28.ToString();
                                detalleLiquidacionFranquicia.lineaNegocio_id = 1;
                                detalleLiquidacionFranquicia.valorRecaudo = 100000;
                            break;
                            
                        case 1:

                            //	Compañía – Línea  de negocio – detalleLiquidacionFranquicias.ramo_id– detalleLiquidacionFranquicias.producto_id– Negocio
                                detalleLiquidacionFranquicia.compania_id = 1;
                                detalleLiquidacionFranquicia.lineaNegocio_id = 1;
                                detalleLiquidacionFranquicia.ramo_id = 1;
                                detalleLiquidacionFranquicia.producto_id = 1;
                               
                                detalleLiquidacionFranquicia.numeroNegocio = 112193610000.ToString();
                                detalleLiquidacionFranquicia.porcentajeParticipacion = 10;
                                detalleLiquidacionFranquicia.valorRecaudo = 100000;

                                break;


                            

                        case 2:
                                //Compañía – Línea  de negocio – detalleLiquidacionFranquicias.ramo_id– detalleLiquidacionFranquicias.producto_id– detalleLiquidacionFranquicias.claveParticipante 
                             detalleLiquidacionFranquicia.compania_id = 1;
                                detalleLiquidacionFranquicia.lineaNegocio_id = 1;
                                detalleLiquidacionFranquicia.ramo_id = 1;
                                detalleLiquidacionFranquicia.producto_id = 1;
                                detalleLiquidacionFranquicia.claveParticipante = 28.ToString();
                                detalleLiquidacionFranquicia.porcentajeParticipacion = 10;
                                detalleLiquidacionFranquicia.valorRecaudo = 100000;
                            break;
                                
                        case 3:
                            //	Compañía – Línea  de negocio – detalleLiquidacionFranquicias.ramo_id– detalleLiquidacionFranquicias.producto_id– detalleLiquidacionFranquicias.zona_id – detalleLiquidacionFranquicias.localidad_id
                            detalleLiquidacionFranquicia.compania_id = 1;
                            detalleLiquidacionFranquicia.ramo_id = 1;
                            detalleLiquidacionFranquicia.producto_id = 1;
                            detalleLiquidacionFranquicia.zona_id = 1;
                            detalleLiquidacionFranquicia.localidad_id = 61;
                             detalleLiquidacionFranquicia.porcentajeParticipacion = 10;
                             detalleLiquidacionFranquicia.valorRecaudo = 100000;
                            
                            break;
                        case 4:
                        //	Compañía – Línea  de negocio – detalleLiquidacionFranquicias.ramo_id– Producto

                            detalleLiquidacionFranquicia.compania_id = 1;
                            detalleLiquidacionFranquicia.lineaNegocio_id = 1;
                            detalleLiquidacionFranquicia.ramo_id = 1;
                            detalleLiquidacionFranquicia.producto_id = 1;

                            	  detalleLiquidacionFranquicia.porcentajeParticipacion = 10;
                                  detalleLiquidacionFranquicia.valorRecaudo = 100000;

                                break;

                        case 5:
                        //	Compañía – Línea  de negocio – Agrupador
                              
                                 detalleLiquidacionFranquicia.compania_id = 1;
                            detalleLiquidacionFranquicia.lineaNegocio_id = 1;
                            detalleLiquidacionFranquicia.codigo_agrupador = "1";
                          

                            	  detalleLiquidacionFranquicia.porcentajeParticipacion = 10;
                                  detalleLiquidacionFranquicia.valorRecaudo = 100000;

                                break;

                        case 6:
                        //	Compañía – Línea de negocio – Ramo

                                 detalleLiquidacionFranquicia.compania_id = 1;
                            detalleLiquidacionFranquicia.lineaNegocio_id = 1;
                            detalleLiquidacionFranquicia.ramo_id = 1;
                          

                            	  detalleLiquidacionFranquicia.porcentajeParticipacion = 10;
                                  detalleLiquidacionFranquicia.valorRecaudo = 100000;
                       

                            break;

                        case 7:
                            //	Compañía – Línea de negocio – detalleLiquidacionFranquicias.zona_id– detalleLiquidacionFranquicias.localidad_id
                            detalleLiquidacionFranquicia.compania_id = 1;
                            detalleLiquidacionFranquicia.lineaNegocio_id = 1;
                            detalleLiquidacionFranquicia.zona_id = 1;
                            detalleLiquidacionFranquicia.localidad_id = 61;


                            detalleLiquidacionFranquicia.porcentajeParticipacion = 10;
                            detalleLiquidacionFranquicia.valorRecaudo = 100000;
                            break;


                        case 8:
                        //	Compañía – Línea de negocio – detalleLiquidacionFranquicias.claveParticipante
                              detalleLiquidacionFranquicia.compania_id = 1;
                            detalleLiquidacionFranquicia.lineaNegocio_id = 1;
                            detalleLiquidacionFranquicia.claveParticipante = 28.ToString();

                            detalleLiquidacionFranquicia.porcentajeParticipacion = 10;
                            detalleLiquidacionFranquicia.valorRecaudo = 100000;
                            break;

                        case 9:
                        //	Compañía – Línea de negocio
                              detalleLiquidacionFranquicia.compania_id = 1;
                            detalleLiquidacionFranquicia.lineaNegocio_id = 1;
                            detalleLiquidacionFranquicia.valorRecaudo = 100000;
                            break;


                        case 10:
                        //Compañia-Ramo-Producto-TipoVehiculo -DETPARTFRA
                            detalleLiquidacionFranquicia.compania_id = 1;
                                detalleLiquidacionFranquicia.ramo_id = 1;
                                detalleLiquidacionFranquicia.producto_id = 1;
                            detalleLiquidacionFranquicia.tipoVehiculo = "1";
                            detalleLiquidacionFranquicia.porcentajeParticipacion = 15;
                            detalleLiquidacionFranquicia.valorRecaudo = 200000;
                            break;

                        case 11:

                            //Compañia-Ramo-Producto-DETPARTFRA
                               detalleLiquidacionFranquicia.compania_id = 1;
                                detalleLiquidacionFranquicia.ramo_id = 1;
                                detalleLiquidacionFranquicia.producto_id = 1;
                                detalleLiquidacionFranquicia.porcentajeParticipacion = 15;
                                detalleLiquidacionFranquicia.valorRecaudo = 200000;
                            break;
                        case 12:

                        //  //Compañia-Ramo-Producto-TipoVehiculo-- RANGOS
                             detalleLiquidacionFranquicia.compania_id = 5;
                                detalleLiquidacionFranquicia.ramo_id = 1;
                                detalleLiquidacionFranquicia.producto_id = 1;
                            detalleLiquidacionFranquicia.tipoVehiculo = "1";
                            detalleLiquidacionFranquicia.porcentajeParticipacion = 16;
                            detalleLiquidacionFranquicia.valorRecaudo = 300000;
                            break;
                        case 13:
                        //Compañia-Ramo-Producto--- RANGOS
                             detalleLiquidacionFranquicia.compania_id = 5;
                                detalleLiquidacionFranquicia.ramo_id = 1;
                                detalleLiquidacionFranquicia.producto_id = 1;
                                detalleLiquidacionFranquicia.porcentajeParticipacion = 16;
                                detalleLiquidacionFranquicia.valorRecaudo = 300000;
                            break;
                    }
                    


                 



               



                   
                  

                 

                    // Query a la tbla de Negocios con recaudo.compania_id && recaudo.ramo_id && recaudo.numeroNegocio




                    //detalleLiquidacionFranquicia.tipoVehiculo = traerTipovehi(recaudo.compania_id, recaudo.ramo_id, recaudo.numeroNegocio);//negocios.Last().tipoVehiculo;


                  
                  


                    // se instancia esta clase para para crear lo hilos 
                    List<int> opciones = null;

                    //var OpcionesThread =
                    //    new Thread(delegate()
                    //                   {
                    //                       opciones =
                    //                           opcionesThread.VerificarOpciones(detalleLiquidacionFranquicia,
                    //                                                             excepciones, detallepartfranq);
                    //                   });
                    //OpcionesThread.Start();
                    int exep = 0;
                    int detpar = 0;
                    int detpparrang = 0;

                    if (countexceptions == 10)
                    {
                        exep = 0;
                    }

                    if (countexceptions < (10))
                    {
                        List<ColpatriaSAI.Negocio.Entidades.Excepcion> excepcioneses = contexto.Excepcions.AsParallel().ToList();
                        exep = verificarException(detalleLiquidacionFranquicia,

                                                  excepcioneses, recaudoses, contexto);
                        countexceptions = countexceptions + exep;

                    }

                    if (countdetpartfra == 2)
                    {
                        detpar = 0;
                    }

                    if (countdetpartfra < (2) && exep == 0)
                    {
                      
                            List<ColpatriaSAI.Negocio.Entidades.DetallePartFranquicia> detallepartfranqs =
                                contexto.DetallePartFranquicias.AsParallel().ToList();

                            detpar = verificardetallePartFranquicia(
                                detalleLiquidacionFranquicia,
                                detallepartfranqs, recaudoses, contexto);
                            countdetpartfra = countdetpartfra + detpar;
                     


                    }
                    

                    if (countdetpartfrara < 2 && detpar == 0 && exep == 0)
                    {
                        List<ColpatriaSAI.Negocio.Entidades.DetallePartFranquicia> detallepartfranqs =
            contexto.DetallePartFranquicias.AsParallel().ToList();
                        detpparrang = verificardetallePartFranquicia(
                            detalleLiquidacionFranquicia,
                            detallepartfranqs, recaudoses, contexto);
                        countdetpartfrara = countdetpartfrara + detpparrang;

                    }






                    //});
                }
                contexto.LiquidacionFranquicias.AddObject(liquidacionFranquicia);

                lock (contexto)
                {
                    contexto.SaveChanges();
                }

                mensaje.Append(" cargados: Exepciones" + countexceptions.ToString() + " detparfranquic:" +
                               countdetpartfra.ToString() + "detparfranquicrang" + countdetpartfrara.ToString());


                //}

            }



            return mensaje.ToString();

        }

        //private static SAI_Entities contexto = new SAI_Entities();

        public List<int> VerificarOpciones(DetalleLiquidacionFranquicia detalleLiquidacionFranquicias,
                                           List<Excepcion> excepciones, List<DetallePartFranquicia> detallepartfranq,
                                           List<Recaudo> recaudos, SAI_Entities contexto)
        {
            try
            {


                List<int> opciones = new List<int>();
                Excepcion excepcion = new Excepcion();
                DetallePartFranquicia detallePartFranquicia = new DetallePartFranquicia();
                List<DetallePartFranquicia> detallePartFranquicias =
                    new List<DetallePartFranquicia>();
                int countexc = 0;
                int countdetparfra = 0;
                int countdetparfrarang = 0;

                countexc = verificarException(detalleLiquidacionFranquicias, excepciones, recaudos, contexto);
                countdetparfra = verificardetallePartFranquicia(detalleLiquidacionFranquicias,
                                                                detallepartfranq, recaudos, contexto);
                countdetparfrarang = verificarRangosPartFranquicia(detalleLiquidacionFranquicias, detallepartfranq,
                                                                   recaudos, contexto);
                opciones.Add(countexc);
                opciones.Add(countdetparfra);
                opciones.Add(countdetparfrarang);
                return opciones;


            }
            catch (Exception er)
            {

                throw er;
            }
        }

        public static int verificarException(DetalleLiquidacionFranquicia detalleLiquidacionFranquicias,
                                             List<Excepcion> Excepcions, List<Recaudo> recaudos, SAI_Entities contexto)
        {
            

            Excepcion excepcion = new Excepcion();
            
            Recaudo recaudo = new Recaudo();
            //1	Compañía – Línea de negocio – detalleLiquidacionFranquicias.ramo_id– detalleLiquidacionFranquicias.producto_id– detalleLiquidacionFranquicias.zona_id– detalleLiquidacionFranquicias.localidad_id– detalleLiquidacionFranquicias.claveParticipante– Negocio  
            
            if (detalleLiquidacionFranquicias.compania_id != null &&
                detalleLiquidacionFranquicias.lineaNegocio_id != null &&
                detalleLiquidacionFranquicias.ramo_id != null && detalleLiquidacionFranquicias.producto_id != null &&
                detalleLiquidacionFranquicias.zona_id != null &&
                detalleLiquidacionFranquicias.localidad_id != null &&
                detalleLiquidacionFranquicias.claveParticipante != "" &&
                detalleLiquidacionFranquicias.numeroNegocio != null)
            {
                
                    excepcion = new Excepcion();
                    excepcion.compania_id = detalleLiquidacionFranquicias.compania_id;
                    excepcion.lineaNegocio_id = detalleLiquidacionFranquicias.lineaNegocio_id;
                    excepcion.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                    excepcion.producto_id = detalleLiquidacionFranquicias.producto_id;
                    excepcion.zona_id = detalleLiquidacionFranquicias.zona_id;
                    excepcion.Localidad_id = int.Parse(detalleLiquidacionFranquicias.localidad_id.ToString());
                    excepcion.clave = detalleLiquidacionFranquicias.claveParticipante;
                    excepcion.negocio_id = detalleLiquidacionFranquicias.numeroNegocio;
                excepcion.Porcentaje = detalleLiquidacionFranquicias.porcentajeParticipacion;
                    
                    contexto.Excepcions.AddObject(excepcion);

                    recaudo.compania_id = detalleLiquidacionFranquicias.compania_id;
                    recaudo.lineaNegocio_id = detalleLiquidacionFranquicias.lineaNegocio_id;
                    recaudo.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                    recaudo.producto_id = detalleLiquidacionFranquicias.producto_id;
                    recaudo.zona_id = detalleLiquidacionFranquicias.zona_id;
                    recaudo.localidad_id = int.Parse(detalleLiquidacionFranquicias.localidad_id.ToString());
                    recaudo.clave  = detalleLiquidacionFranquicias.claveParticipante;
                    recaudo.numeroNegocio = detalleLiquidacionFranquicias.numeroNegocio;
                    contexto.Recaudoes.AddObject(recaudo);



                    contexto.SaveChanges();


               // }

            }

            else
            {
                //2	Compañía – Línea  de negocio – detalleLiquidacionFranquicias.ramo_id– detalleLiquidacionFranquicias.producto_id– Negocio

                if (detalleLiquidacionFranquicias.compania_id != null &&
                    detalleLiquidacionFranquicias.lineaNegocio_id != null &&
                    detalleLiquidacionFranquicias.ramo_id != null &&
                    detalleLiquidacionFranquicias.producto_id != null &&
                   detalleLiquidacionFranquicias.numeroNegocio != null)
                {
                    
                        excepcion = new Excepcion();
                        excepcion.compania_id = detalleLiquidacionFranquicias.compania_id;
                        excepcion.lineaNegocio_id = detalleLiquidacionFranquicias.lineaNegocio_id;
                        excepcion.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                        excepcion.producto_id = detalleLiquidacionFranquicias.producto_id;
                        excepcion.Porcentaje = detalleLiquidacionFranquicias.porcentajeParticipacion;
                    excepcion.Localidad_id = 61;
                    
                       
                        excepcion.negocio_id = detalleLiquidacionFranquicias.numeroNegocio;
                        contexto.Excepcions.AddObject(excepcion);

                        recaudo.compania_id = detalleLiquidacionFranquicias.compania_id;
                        recaudo.lineaNegocio_id = detalleLiquidacionFranquicias.lineaNegocio_id;
                        recaudo.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                        recaudo.producto_id = detalleLiquidacionFranquicias.producto_id;
                      
                        recaudo.numeroNegocio = detalleLiquidacionFranquicias.numeroNegocio;
                        contexto.Recaudoes.AddObject(recaudo);



                        contexto.SaveChanges();


                 //   }
                }
                else
                {

                    //3	Compañía – Línea  de negocio – detalleLiquidacionFranquicias.ramo_id– detalleLiquidacionFranquicias.producto_id– detalleLiquidacionFranquicias.claveParticipante 
                    if (detalleLiquidacionFranquicias.compania_id != null &&
                        detalleLiquidacionFranquicias.lineaNegocio_id != null &&
                        detalleLiquidacionFranquicias.ramo_id != null &&
                        detalleLiquidacionFranquicias.producto_id != null &&
                        detalleLiquidacionFranquicias.claveParticipante != null  && detalleLiquidacionFranquicias.claveParticipante!= "")
                    {
                        
                            excepcion = new Excepcion();
                            excepcion.compania_id = detalleLiquidacionFranquicias.compania_id;
                            excepcion.lineaNegocio_id = detalleLiquidacionFranquicias.lineaNegocio_id;
                            excepcion.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                            excepcion.producto_id = detalleLiquidacionFranquicias.producto_id;
                            excepcion.clave=detalleLiquidacionFranquicias.claveParticipante;
                            excepcion.Porcentaje = detalleLiquidacionFranquicias.porcentajeParticipacion;
                        excepcion.Localidad_id = 61;
                    

                            contexto.Excepcions.AddObject(excepcion);

                            recaudo.compania_id = detalleLiquidacionFranquicias.compania_id;
                            recaudo.lineaNegocio_id = detalleLiquidacionFranquicias.lineaNegocio_id;
                            recaudo.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                            recaudo.producto_id = detalleLiquidacionFranquicias.producto_id;
                            recaudo.clave = detalleLiquidacionFranquicias.claveParticipante;

                            contexto.Recaudoes.AddObject(recaudo);



                            contexto.SaveChanges();


                        //}

                    }
                    else
                    {
                      
                        //4	Compañía – Línea  de negocio – detalleLiquidacionFranquicias.ramo_id– detalleLiquidacionFranquicias.producto_id– detalleLiquidacionFranquicias.zona_id – detalleLiquidacionFranquicias.localidad_id
                        if (detalleLiquidacionFranquicias.compania_id != null &&
                            detalleLiquidacionFranquicias.lineaNegocio_id != null &&
                            detalleLiquidacionFranquicias.ramo_id != null &&
                            detalleLiquidacionFranquicias.producto_id != null &&
                            detalleLiquidacionFranquicias.zona_id != null &&
                            detalleLiquidacionFranquicias.localidad_id != null)
                        {
                            
                                excepcion = new Excepcion();
                                excepcion.compania_id = detalleLiquidacionFranquicias.compania_id;
                                excepcion.lineaNegocio_id = detalleLiquidacionFranquicias.lineaNegocio_id;
                                excepcion.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                                excepcion.producto_id = detalleLiquidacionFranquicias.producto_id;
                                excepcion.zona_id = detalleLiquidacionFranquicias.zona_id;
                                excepcion.Localidad_id = int.Parse(detalleLiquidacionFranquicias.localidad_id.ToString());
                                excepcion.Porcentaje = detalleLiquidacionFranquicias.porcentajeParticipacion;
                    

                                contexto.Excepcions.AddObject(excepcion);

                                recaudo.compania_id = detalleLiquidacionFranquicias.compania_id;
                                recaudo.lineaNegocio_id = detalleLiquidacionFranquicias.lineaNegocio_id;
                                recaudo.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                                recaudo.producto_id = detalleLiquidacionFranquicias.producto_id;
                                recaudo.zona_id = detalleLiquidacionFranquicias.zona_id;
                                recaudo.localidad_id = int.Parse(detalleLiquidacionFranquicias.localidad_id.ToString());

                                contexto.Recaudoes.AddObject(recaudo);



                                contexto.SaveChanges();


                            //}

                        }
                        else
                        {
                            //5	Compañía – Línea  de negocio – detalleLiquidacionFranquicias.ramo_id– Producto
                            if (detalleLiquidacionFranquicias.compania_id != null &&
                                detalleLiquidacionFranquicias.lineaNegocio_id != null &&
                                detalleLiquidacionFranquicias.ramo_id != null &&
                                detalleLiquidacionFranquicias.producto_id != null)
                            {
                                
                                    excepcion = new Excepcion();
                                    excepcion.compania_id = detalleLiquidacionFranquicias.compania_id;
                                    excepcion.lineaNegocio_id = detalleLiquidacionFranquicias.lineaNegocio_id;
                                    excepcion.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                                    excepcion.producto_id = detalleLiquidacionFranquicias.producto_id;
                                    excepcion.Porcentaje = detalleLiquidacionFranquicias.porcentajeParticipacion;
                                    excepcion.Localidad_id = 61;

                                    contexto.Excepcions.AddObject(excepcion);

                                    recaudo.compania_id = detalleLiquidacionFranquicias.compania_id;
                                    recaudo.lineaNegocio_id = detalleLiquidacionFranquicias.lineaNegocio_id;
                                    recaudo.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                                    recaudo.producto_id = detalleLiquidacionFranquicias.producto_id;

                                    contexto.Recaudoes.AddObject(recaudo);



                                    contexto.SaveChanges();


                                //}

                            }
                            else
                            {
                                //6	Compañía – Línea  de negocio – Agrupador
                                if (detalleLiquidacionFranquicias.compania_id != null &&
                                    detalleLiquidacionFranquicias.lineaNegocio_id != null &&
                                    detalleLiquidacionFranquicias.codigo_agrupador != null)
                                {
                                    
                                        excepcion = new Excepcion();
                                        excepcion.compania_id = detalleLiquidacionFranquicias.compania_id;
                                        excepcion.lineaNegocio_id = detalleLiquidacionFranquicias.lineaNegocio_id;
                                        excepcion.codigoAgrupador = detalleLiquidacionFranquicias.codigo_agrupador;
                                        excepcion.Porcentaje = detalleLiquidacionFranquicias.porcentajeParticipacion;
                                        excepcion.Localidad_id = 61;

                                        contexto.Excepcions.AddObject(excepcion);

                                        //recaudo.compania_id = detalleLiquidacionFranquicias.compania_id;
                                        //recaudo.lineaNegocio_id = detalleLiquidacionFranquicias.lineaNegocio_id;
                                        //recaudo.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                                        //recaudo.producto_id = detalleLiquidacionFranquicias.producto_id;
                                        //recaudo.zona_id = detalleLiquidacionFranquicias.zona_id;
                                        //recaudo.localidad_id = int.Parse(detalleLiquidacionFranquicias.localidad_id.ToString());
                                        //recaudo.participante_id = int.Parse(detalleLiquidacionFranquicias.claveParticipante);
                                        //recaudo.numeroNegocio = detalleLiquidacionFranquicias.numeroNegocio;
                                        contexto.Recaudoes.AddObject(recaudo);



                                        contexto.SaveChanges();


                                    //}



                                }
                                else
                                {
                                    //7	Compañía – Línea de negocio – Ramo
                                    if (detalleLiquidacionFranquicias.compania_id != null &&
                                        detalleLiquidacionFranquicias.lineaNegocio_id != null &&
                                        detalleLiquidacionFranquicias.ramo_id != null)
                                    {
                                       
                                            excepcion = new Excepcion();
                                            excepcion.compania_id = detalleLiquidacionFranquicias.compania_id;
                                            excepcion.lineaNegocio_id = detalleLiquidacionFranquicias.lineaNegocio_id;
                                            excepcion.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                                            excepcion.Porcentaje = detalleLiquidacionFranquicias.porcentajeParticipacion;
                                            excepcion.Localidad_id = 61;
                                            contexto.Excepcions.AddObject(excepcion);

                                            recaudo.compania_id = detalleLiquidacionFranquicias.compania_id;
                                            recaudo.lineaNegocio_id = detalleLiquidacionFranquicias.lineaNegocio_id;
                                            recaudo.ramo_id = detalleLiquidacionFranquicias.ramo_id;

                                            contexto.Recaudoes.AddObject(recaudo);



                                            contexto.SaveChanges();


                                       // }



                                    }
                                    else
                                        //8	Compañía – Línea de negocio – detalleLiquidacionFranquicias.zona_id– detalleLiquidacionFranquicias.localidad_id
                                        if (detalleLiquidacionFranquicias.compania_id != null &&
                                            detalleLiquidacionFranquicias.lineaNegocio_id != null &&
                                            detalleLiquidacionFranquicias.zona_id != null &&
                                            detalleLiquidacionFranquicias.localidad_id != null)
                                        {
                                          
                                                excepcion = new Excepcion();
                                                excepcion.compania_id = detalleLiquidacionFranquicias.compania_id;
                                                excepcion.lineaNegocio_id =
                                                    detalleLiquidacionFranquicias.lineaNegocio_id;
                                                excepcion.zona_id = detalleLiquidacionFranquicias.zona_id;
                                                excepcion.Localidad_id =
                                                    int.Parse(detalleLiquidacionFranquicias.localidad_id.ToString());
                                                excepcion.Porcentaje = detalleLiquidacionFranquicias.porcentajeParticipacion;
                    
                                                contexto.Excepcions.AddObject(excepcion);

                                                recaudo.compania_id = detalleLiquidacionFranquicias.compania_id;
                                                recaudo.lineaNegocio_id = detalleLiquidacionFranquicias.lineaNegocio_id;
                                                recaudo.zona_id = detalleLiquidacionFranquicias.zona_id;
                                                recaudo.localidad_id =
                                                    int.Parse(detalleLiquidacionFranquicias.localidad_id.ToString());

                                                contexto.Recaudoes.AddObject(recaudo);



                                                contexto.SaveChanges();


                                            //}
                                        }
                                        else
                                        {
                                            //9	Compañía – Línea de negocio – detalleLiquidacionFranquicias.claveParticipante
                                            if (detalleLiquidacionFranquicias.compania_id != null &&
                                                detalleLiquidacionFranquicias.lineaNegocio_id != null &&
                                                detalleLiquidacionFranquicias.claveParticipante != null && detalleLiquidacionFranquicias.claveParticipante!= "")
                                            {
                                                
                                                    excepcion = new Excepcion();
                                                    excepcion.compania_id = detalleLiquidacionFranquicias.compania_id;
                                                    excepcion.lineaNegocio_id =
                                                        detalleLiquidacionFranquicias.lineaNegocio_id;

                                                    excepcion.clave =
                                                      detalleLiquidacionFranquicias.claveParticipante;
                                                    excepcion.Porcentaje = detalleLiquidacionFranquicias.porcentajeParticipacion;
                                                    excepcion.Localidad_id = 61;

                                                    contexto.Excepcions.AddObject(excepcion);


                                                    recaudo.compania_id = detalleLiquidacionFranquicias.compania_id;
                                                    recaudo.lineaNegocio_id =
                                                        detalleLiquidacionFranquicias.lineaNegocio_id;

                                                    recaudo.clave =
                                                        detalleLiquidacionFranquicias.claveParticipante;

                                                    contexto.Recaudoes.AddObject(recaudo);



                                                    contexto.SaveChanges();


                                                //}

                                            }


                                            else
                                            {
                                                //10	Compañía – Línea de negocio


                                                if (detalleLiquidacionFranquicias.compania_id != null &&
                                                    detalleLiquidacionFranquicias.lineaNegocio_id != null)
                                                {
                                                    
                                                        excepcion = new Excepcion();
                                                        excepcion.compania_id =
                                                            detalleLiquidacionFranquicias.compania_id;
                                                        excepcion.lineaNegocio_id =
                                                            detalleLiquidacionFranquicias.lineaNegocio_id;
                                                        excepcion.Porcentaje = detalleLiquidacionFranquicias.porcentajeParticipacion;
                                                        excepcion.Localidad_id = 61;

                                                        contexto.Excepcions.AddObject(excepcion);


                                                        recaudo.compania_id = detalleLiquidacionFranquicias.compania_id;
                                                        recaudo.lineaNegocio_id =
                                                            detalleLiquidacionFranquicias.lineaNegocio_id;

                                                        contexto.Recaudoes.AddObject(recaudo);



                                                        contexto.SaveChanges();


                                                   // }




                                                }
                                            }

                                        }


                                }
                            }
                        }
                    }


                }



            }

            if (excepcion != null)
            {
                return 1;

            }
            else
            {
                return 0;
            }

        }

        public static int verificardetallePartFranquicia(DetalleLiquidacionFranquicia detalleLiquidacionFranquicias,
                                                         List<DetallePartFranquicia> detallepartfranq,
                                                         List<Recaudo> recaudos,
                                                         SAI_Entities contexto)
        {
            DetallePartFranquicia detallePartFranquicia = new DetallePartFranquicia();
            detallePartFranquicia.porcentaje = 40.ToString();
            
            Recaudo recaudo = new Recaudo();
            //Compañia-Ramo-Producto-TipoVehiculo
            if (detalleLiquidacionFranquicias.compania_id != null && detalleLiquidacionFranquicias.ramo_id != null &&
                detalleLiquidacionFranquicias.producto_id != null && detalleLiquidacionFranquicias.tipoVehiculo != null)
            {

                //detallePartFranquicia = detallepartfranq.FirstOrDefault(
                //    e =>
                //    e.compania_id == detalleLiquidacionFranquicias.compania_id &&
                //    e.ramo_id == detalleLiquidacionFranquicias.ramo_id &&
                //    e.producto_id == detalleLiquidacionFranquicias.producto_id &&
                //    e.tipoVehiculo_id == int.Parse(detalleLiquidacionFranquicias.tipoVehiculo) &&
                //    e.rangoinferior == null &&
                //    e.rangosuperior == null);

                //if (detallePartFranquicia == null)
                //{
                    detallePartFranquicia = new DetallePartFranquicia();
                    detallePartFranquicia.compania_id = detalleLiquidacionFranquicias.compania_id;
                    detallePartFranquicia.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                    detallePartFranquicia.producto_id = detalleLiquidacionFranquicias.producto_id;
                    detallePartFranquicia.tipoVehiculo_id = int.Parse(detalleLiquidacionFranquicias.tipoVehiculo);
                    detallePartFranquicia.rangoinferior = null;
                    detallePartFranquicia.rangosuperior = null;
                detallePartFranquicia.porcentaje = detalleLiquidacionFranquicias.porcentajeParticipacion.ToString();

                    contexto.DetallePartFranquicias.AddObject(detallePartFranquicia);



                    recaudo.compania_id = detalleLiquidacionFranquicias.compania_id;
                    recaudo.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                    recaudo.producto_id = detalleLiquidacionFranquicias.producto_id;
                    //recaudo.tipoVehiculo = int.Parse(detalleLiquidacionFranquicias.tipoVehiculo);

                    contexto.Recaudoes.AddObject(recaudo);
                    contexto.SaveChanges();
               // }
            }

            else
            {
                //Compañia-Ramo-Producto
                if (detalleLiquidacionFranquicias.compania_id != null && detalleLiquidacionFranquicias.ramo_id != null &&
                    detalleLiquidacionFranquicias.producto_id != null)
                {


                    //detallePartFranquicia = detallepartfranq.FirstOrDefault(
                    //    e =>
                    //    e.compania_id == detalleLiquidacionFranquicias.compania_id &&
                    //    e.ramo_id == detalleLiquidacionFranquicias.ramo_id &&
                    //    e.producto_id == detalleLiquidacionFranquicias.producto_id &&
                    //    e.rangoinferior == null &&
                    //    e.rangosuperior == null);

                    //if (detallePartFranquicia == null)
                    //{
                        detallePartFranquicia = new DetallePartFranquicia();
                        detallePartFranquicia.compania_id = detalleLiquidacionFranquicias.compania_id;
                        detallePartFranquicia.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                        detallePartFranquicia.producto_id = detalleLiquidacionFranquicias.producto_id;

                        detallePartFranquicia.rangoinferior = null;
                        detallePartFranquicia.rangosuperior = null;
                        detallePartFranquicia.porcentaje = detalleLiquidacionFranquicias.porcentajeParticipacion.ToString();

                        contexto.DetallePartFranquicias.AddObject(detallePartFranquicia);



                        recaudo.compania_id = detalleLiquidacionFranquicias.compania_id;
                        recaudo.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                        recaudo.producto_id = detalleLiquidacionFranquicias.producto_id;

                        contexto.Recaudoes.AddObject(recaudo);

                        contexto.SaveChanges();

                   // }


                }

            }

            if (detallePartFranquicia != null)
            {
                return 1;

            }
            else
            {
                return 0;
            }
        }

        public int verificarRangosPartFranquicia(DetalleLiquidacionFranquicia detalleLiquidacionFranquicias,
                                                 List<DetallePartFranquicia> detallepartfranq, List<Recaudo> recaudoses,
                                                 SAI_Entities contexto)
        {
            List<DetallePartFranquicia> detallePartFranquicias = null;

            DetallePartFranquicia detallePartFranquicia = new DetallePartFranquicia();
            detallePartFranquicia.porcentaje = 50.ToString();
            
            Recaudo recaudo = new Recaudo();
            List<Recaudo> recaudos = new List<Recaudo>();
            //Compañia-Ramo-Producto-TipoVehiculo
            if (detalleLiquidacionFranquicias.compania_id != null && detalleLiquidacionFranquicias.ramo_id != null &&
                detalleLiquidacionFranquicias.producto_id != null && detalleLiquidacionFranquicias.tipoVehiculo != null)
            {
            //    detallePartFranquicias =
            //        detallepartfranq.AsParallel().Where(
            //            e => e.compania_id == detalleLiquidacionFranquicias.compania_id &&
            //                 e.ramo_id == detalleLiquidacionFranquicias.ramo_id &&
            //                 e.producto_id == detalleLiquidacionFranquicias.producto_id &&
            //                 e.tipo_vehiculo_id == detalleLiquidacionFranquicias.tipoVehiculo &&
            //                 e.rangoinferior != null &&
            //                 e.rangosuperior != null
            //            ).OrderBy(e => e.rangoinferior).ToList();

            //    if (detallePartFranquicias.Count == 0)
            //    {

                    detallePartFranquicia.compania_id = detalleLiquidacionFranquicias.compania_id;
                    detallePartFranquicia.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                    detallePartFranquicia.producto_id = detalleLiquidacionFranquicias.producto_id;
                    detallePartFranquicia.tipoVehiculo_id = Convert.ToInt32(detalleLiquidacionFranquicias.tipoVehiculo);
                    detallePartFranquicia.rangoinferior = 0;
                    detallePartFranquicia.rangosuperior = 2000000;
                    detallePartFranquicia.porcentaje = detalleLiquidacionFranquicias.porcentajeParticipacion.ToString();


                    contexto.DetallePartFranquicias.AddObject(detallePartFranquicia);

                    recaudo.compania_id = detalleLiquidacionFranquicias.compania_id;
                    recaudo.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                    recaudo.producto_id = detalleLiquidacionFranquicias.producto_id;
                    //recaudo.tipoVehiculo = int.Parse(detalleLiquidacionFranquicias.tipoVehiculo);

                    contexto.Recaudoes.AddObject(recaudo);

                    contexto.SaveChanges();

               // }




            }

            else
            {
                //Compañia-Ramo-Producto
                if (detalleLiquidacionFranquicias.compania_id != null && detalleLiquidacionFranquicias.ramo_id != null &&
                    detalleLiquidacionFranquicias.producto_id != null)
                {
                    //detallePartFranquicias =
                    //    detallepartfranq.AsParallel().Where(
                    //        e => e.compania_id == detalleLiquidacionFranquicias.compania_id &&
                    //             e.ramo_id == detalleLiquidacionFranquicias.ramo_id &&
                    //             e.producto_id == detalleLiquidacionFranquicias.producto_id &&
                    //             e.rangoinferior != null &&
                    //             e.rangosuperior != null
                    //        ).OrderBy(e => e.rangoinferior).ToList();




                    //if (detallePartFranquicias.Count == 0)
                    //{

                        detallePartFranquicia.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                        detallePartFranquicia.producto_id = detalleLiquidacionFranquicias.producto_id;
                        detallePartFranquicia.rangoinferior = 0;
                        detallePartFranquicia.rangosuperior = 2000000;
                        detallePartFranquicia.porcentaje = detalleLiquidacionFranquicias.porcentajeParticipacion.ToString();


                        contexto.DetallePartFranquicias.AddObject(detallePartFranquicia);


                        recaudo.ramo_id = detalleLiquidacionFranquicias.ramo_id;
                        recaudo.producto_id = detalleLiquidacionFranquicias.producto_id;

                        contexto.Recaudoes.AddObject(recaudo);

                        contexto.SaveChanges();


                   // }

                }

            }

            if (detallePartFranquicias != null)
            {
                return 1;

            }
            else
            {
                return 0;
            }




        }

        //Metodo que devuelve liquidacion desde cero o con acumulados
        public double CalcularRango(double valorRecaudo, int id, List<DetallePartFranquicia> detallePartFranquicias)
        {

            double result = 0;
            double rango = 0;

            // se consulta el rango actual por el id 
            rango = (double) (detallePartFranquicias.FirstOrDefault(e => e.id == id).rangosuperior -
                              detallePartFranquicias.FirstOrDefault(e => e.id == id).rangoinferior);
            double porcentaje = Convert.ToDouble(detallePartFranquicias.FirstOrDefault(e => e.id == id).porcentaje);

            // si valor del recaudo es menor al rango indica  la liquidacion esta dentro del rango  
            if (valorRecaudo < rango)
            {
                result = valorRecaudo*(porcentaje/100);

            }
            else
            {
                // si valor del recaudo es mayor al rango indica  la liquidacion no esta dentro del rango y se llama recursivamente esta funcion para recalcular porcentaje y valor  
                result = rango*(porcentaje/100);
                valorRecaudo = valorRecaudo - rango;
                int idSiguiente = 0;

                for (int i = 0; i < detallePartFranquicias.Count(); i++)
                {
                    if (detallePartFranquicias.ElementAt(i).id == id)
                    {
                        if (i + 1 < detallePartFranquicias.Count())
                        {
                            //if(detallePartFranquicias.ElementAt(i + 1) != null)
                            //{
                            if (detallePartFranquicias.ElementAt(i + 1).rangosuperior != null)
                            {
                                // aqui se trae el siguiente id de parametrizacion
                                idSiguiente = detallePartFranquicias.ElementAt(i + 1).id;
                            }
                            else
                            {
                                idSiguiente = 0;
                            }



                        }


                    }
                }

                if (idSiguiente != 0)
                {
                    result += CalcularRango(valorRecaudo, idSiguiente, detallePartFranquicias);
                }

            }


            return result;
        }


    }
}


