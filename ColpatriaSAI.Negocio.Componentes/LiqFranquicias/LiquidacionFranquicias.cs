using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
namespace ColpatriaSAI.Negocio.Componentes.LiqFranquicias
{
    public class LiquidacionFranquicias
    {
        private SAI_Entities contexto = new SAI_Entities();

        public List<LiquidacionFranquicia> ListarLiquidacionFranquicias()
        {
            LiquidacionFranquicia liquidacionFranquicia = new LiquidacionFranquicia();

            return contexto.LiquidacionFranquicias.Include("EstadoLiquidacion").ToList();
        }

        public LiquidacionFranquicia TraerLiquidacionFranquicia(int idLiquidacionFranquicia)
        {
            LiquidacionFranquicia liquidacionFranquicia = new LiquidacionFranquicia();
            liquidacionFranquicia = contexto.LiquidacionFranquicias.Include("EstadoLiquidacion").Where(x => x.id == idLiquidacionFranquicia).FirstOrDefault();
            return liquidacionFranquicia;
        }

        public List<EstadoLiquidacion> ListarEstadosLiquidacionFranquicias()
        {
            return contexto.EstadoLiquidacions.ToList();
        }

        public int ActualizarLiquidacionFranquiciaReliquidacion(int idLiquidacion, int permiteReliquidar, string Username)
        {
            int result = 0;

            LiquidacionFranquicia liquidFranquicia = contexto.LiquidacionFranquicias.Where(e => e.id == idLiquidacion).FirstOrDefault();
            var pValorAntiguo = liquidFranquicia;
            liquidFranquicia.permite_reliquidar = permiteReliquidar;
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.LiquidacionFranquicia,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, liquidFranquicia);
            contexto.SaveChanges();

            return result;

        }

        public int ActualizarLiquidacionFranquiciaEstado(int idLiquidacion, int idEstado, string Username)
        {
            int result = 0;

            LiquidacionFranquicia liquidFranquicia = contexto.LiquidacionFranquicias.Where(e => e.id == idLiquidacion).FirstOrDefault();
            var pValorAntiguo = liquidFranquicia;
            liquidFranquicia.estado = idEstado;
            contexto.SaveChanges();

            //LLAMAMOS EL PROCEDIMIENTO PARA ACTUALIZAR LOS RANGOS ACUMULADOS SOLO CUANDO EL ESTADO SEA ANULADA
            if (idEstado == 3)
            {
                EntityConnection entityConnection = (EntityConnection)contexto.Connection;
                DbConnection storeConnection = entityConnection.StoreConnection;
                DbCommand command = storeConnection.CreateCommand();
                command.CommandText = "LiquidarAnularLiquidacion";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("liquidacionId", idLiquidacion));

                bool openingConnection = command.Connection.State == ConnectionState.Closed;

                if (openingConnection) { command.Connection.Open(); }

                try
                {
                    result = command.ExecuteNonQuery();
                }
                finally
                {
                    if (openingConnection && command.Connection.State == ConnectionState.Open) { command.Connection.Close(); }
                }
            }
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.LiquidacionFranquicia,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, liquidFranquicia);
            return result;
        }

        public int GenerarPagosLiquidacionFranquicia(int idLiquidacionFranquica, string usuario)
        {
            EntityConnection entityConnection = (EntityConnection)contexto.Connection;
            DbConnection storeConnection = entityConnection.StoreConnection;
            DbCommand command = storeConnection.CreateCommand();
            command.CommandText = "LiquidarFranquiciasPagos";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("idLiquidacion", idLiquidacionFranquica));
            command.Parameters.Add(new SqlParameter("usuario", usuario));

            bool openingConnection = command.Connection.State == ConnectionState.Closed;

            if (openingConnection) { command.Connection.Open(); }

            int result;

            try
            {
                result = command.ExecuteNonQuery();
            }
            finally
            {
                if (openingConnection && command.Connection.State == ConnectionState.Open) { command.Connection.Close(); }
            }

            return result;
        }

        public int InsertarLiquidacionFranquicia(LiquidacionFranquicia liquidacionFranquicia, string Username)
        {

            contexto.LiquidacionFranquicias.AddObject(liquidacionFranquicia);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.LiquidacionFranquicia,
    SegmentosInsercion.Personas_Y_Pymes, null, liquidacionFranquicia);
            contexto.SaveChanges();

            var idLiquidacion = (from lf in contexto.LiquidacionFranquicias
                                 select lf.id).Max();

            var liquidacionProceso = new LiquidacionFranquiciaProceso()
            {
                liquidacion_id = idLiquidacion,
                proceso = 1
            };
            contexto.LiquidacionFranquiciaProcesoes.AddObject(liquidacionProceso);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.LiquidacionFranquiciaProceso,
    SegmentosInsercion.Personas_Y_Pymes, null, liquidacionProceso);
            contexto.SaveChanges();

            return idLiquidacion;
        }

        public int LiquidarFranquiciasDetalleTotal(LiquidacionFranquicia liquidacionFranquicia)
        {
            var liquidacionProceso = contexto.LiquidacionFranquiciaProcesoes.Where(x => x.liquidacion_id == liquidacionFranquicia.id).FirstOrDefault();
            liquidacionProceso.proceso = 1;
            contexto.SaveChanges();

            EntityConnection entityConnection = (EntityConnection)contexto.Connection;
            DbConnection storeConnection = entityConnection.StoreConnection;
            DbCommand command = storeConnection.CreateCommand();
            command.CommandText = "LiquidarFranquiciasDetalleTotal";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("idLiquidacion", liquidacionFranquicia.id));
            command.Parameters.Add(new SqlParameter("fechaini", liquidacionFranquicia.periodoLiquidacionIni));
            command.Parameters.Add(new SqlParameter("fechafin", liquidacionFranquicia.periodoLiquidacionFin));

            bool openingConnection = command.Connection.State == ConnectionState.Closed;

            if (openingConnection) { command.Connection.Open(); command.CommandTimeout = 10000; }

            int result;

            try
            {
                result = command.ExecuteNonQuery();
            }
            finally
            {
                if (openingConnection && command.Connection.State == ConnectionState.Open) { command.Connection.Close(); }
            }
            return result;

        }

        public int LiquidarFranquiciasExcepciones(LiquidacionFranquicia liquidacionFranquicia)
        {
            var liquidacionProceso = contexto.LiquidacionFranquiciaProcesoes.Where(x => x.liquidacion_id == liquidacionFranquicia.id).FirstOrDefault();
            liquidacionProceso.proceso = 2;
            contexto.SaveChanges();

            EntityConnection entityConnection = (EntityConnection)contexto.Connection;
            DbConnection storeConnection = entityConnection.StoreConnection;
            DbCommand command = storeConnection.CreateCommand();
            command.CommandText = "LiquidarFranquiciasExcepciones";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("idLiquidacion", liquidacionFranquicia.id));
            command.Parameters.Add(new SqlParameter("fechaini", liquidacionFranquicia.periodoLiquidacionIni));
            command.Parameters.Add(new SqlParameter("fechafin", liquidacionFranquicia.periodoLiquidacionFin));

            bool openingConnection = command.Connection.State == ConnectionState.Closed;

            if (openingConnection) { command.Connection.Open(); command.CommandTimeout = 10000; }

            int result;

            try
            {
                result = command.ExecuteNonQuery();
            }
            finally
            {
                if (openingConnection && command.Connection.State == ConnectionState.Open) { command.Connection.Close(); }
            }

            return result;
        }

        public int LiquidarFranquiciasParticipaciones(LiquidacionFranquicia liquidacionFranquicia)
        {
            var liquidacionProceso = contexto.LiquidacionFranquiciaProcesoes.Where(x => x.liquidacion_id == liquidacionFranquicia.id).FirstOrDefault();
            liquidacionProceso.proceso = 3;
            contexto.SaveChanges();

            EntityConnection entityConnection = (EntityConnection)contexto.Connection;
            DbConnection storeConnection = entityConnection.StoreConnection;
            DbCommand command = storeConnection.CreateCommand();
            command.CommandText = "LiquidarFranquiciasParticipacion";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("idLiquidacion", liquidacionFranquicia.id));
            command.Parameters.Add(new SqlParameter("fechaini", liquidacionFranquicia.periodoLiquidacionIni));
            command.Parameters.Add(new SqlParameter("fechafin", liquidacionFranquicia.periodoLiquidacionFin));

            bool openingConnection = command.Connection.State == ConnectionState.Closed;

            if (openingConnection) { command.Connection.Open(); command.CommandTimeout = 10000; }

            int result;

            try
            {
                result = command.ExecuteNonQuery();
            }
            finally
            {
                if (openingConnection && command.Connection.State == ConnectionState.Open) { command.Connection.Close(); }
            }

            return result;
        }

        public int LiquidarFranquiciasPorRangos(LiquidacionFranquicia liquidacionFranquicia)
        {

            var liquidacionProceso = contexto.LiquidacionFranquiciaProcesoes.Where(x => x.liquidacion_id == liquidacionFranquicia.id).FirstOrDefault();
            liquidacionProceso.proceso = 4;
            contexto.SaveChanges();

            List<DetalleLiquidacionFranquicia> detalleLiquidacionFranquiciasList = new List<DetalleLiquidacionFranquicia>();
            DetalleLiquiRangosFranq acumuladosActualizar = new DetalleLiquiRangosFranq();
            List<DetalleLiquiRangosFranq> detalleAcumulados = new List<DetalleLiquiRangosFranq>();
            List<DetallePartFranquicia> rangos = new List<DetallePartFranquicia>();
            List<DetallePartFranquicia> rangosDetallePartFanquicia = new List<DetallePartFranquicia>();
            List<ParticipacionFranquicia> rangosPartFanquicia = new List<ParticipacionFranquicia>();
            List<double[]> liquidacionTemp = new List<double[]>();

            //TRAEMOS EL DETALLE A LIQUIDAR
            var detalleLiquidacionFranquiciasListTemp = contexto.ObtenerDetalleLiquidacionFranquicia(liquidacionFranquicia.id).ToList();

            detalleLiquidacionFranquiciasList = (from dlf in detalleLiquidacionFranquiciasListTemp.AsEnumerable()
                                                 select new DetalleLiquidacionFranquicia
                                                 {
                                                     id = dlf.id,
                                                     porcentajeParticipacion = dlf.porcentajeParticipacion,
                                                     totalParticipacion = dlf.totalParticipacion,
                                                     fechaContabl = dlf.fechaContabl,
                                                     fechaRecaudo = dlf.fechaRecaudo,
                                                     compania_id = dlf.compania_id,
                                                     ramo_id = dlf.ramo_id,
                                                     producto_id = dlf.producto_id,
                                                     numeroNegocio = dlf.numeroNegocio,
                                                     lineaNegocio_id = dlf.lineaNegocio_id,
                                                     amparo_Id = dlf.amparo_Id,
                                                     liquidacionFranquicia_id = dlf.liquidacionFranquicia_id,
                                                     liquidadoPor = dlf.liquidadoPor,
                                                     ramo_id_agrupado = dlf.ramo_id_agrupado,
                                                     producto_id_agrupado = dlf.producto_id_agrupado,
                                                     tipoVehiculo = dlf.tipoVehiculo,
                                                     valorRecaudo = dlf.valorRecaudo,
                                                     nivelDirector = dlf.nivelDirector,
                                                     claveParticipante = dlf.claveParticipante,
                                                     modalidadPagoId = dlf.modalidadPagoId,
                                                     numeroRecibo = dlf.numeroRecibo,
                                                     colquines = dlf.colquines,
                                                     zona_id = dlf.zona_id,
                                                     codigo_agrupador = dlf.codigo_agrupador,
                                                     localidad_id = dlf.localidad_id
                                                 }).ToList();


            //TRAEMOS LOS ACUMULADOS DE LOS DETALLES A LIQUIDAR
            var detalleAcumuladosTemp = (from dlrf in contexto.DetalleLiquiRangosFranqs
                                         join dlf in contexto.DetalleLiquidacionFranquicias
                                         on new { dlrf.compania_id, dlrf.ramo_id, dlrf.producto_id, dlrf.numeroNegocio } equals new { dlf.compania_id, dlf.ramo_id, dlf.producto_id, dlf.numeroNegocio }
                                         where dlf.liquidacionFranquicia_id == liquidacionFranquicia.id && dlf.fechaContabl >= liquidacionFranquicia.periodoLiquidacionIni && dlf.fechaContabl <= liquidacionFranquicia.periodoLiquidacionFin && (dlf.liquidadoPor == 2 || dlf.liquidadoPor == 0) && dlrf.anio == liquidacionFranquicia.periodoLiquidacionIni.Value.Year
                                         group dlrf by new { dlrf.compania_id, dlrf.ramo_id, dlrf.producto_id, dlrf.numeroNegocio } into dlrfData
                                         select new
                                         {
                                             acumuladoTotal = dlrfData.Max(d => d.acumuladoTotal),
                                             compania_id = dlrfData.Max(d => d.compania_id),
                                             ramo_id = dlrfData.Max(d => d.ramo_id),
                                             producto_id = dlrfData.Max(d => d.producto_id),
                                             numeroNegocio = dlrfData.Max(d => d.numeroNegocio),
                                             anio = dlrfData.Max(d => d.anio)
                                         });

            detalleAcumulados = (from da in detalleAcumuladosTemp.AsEnumerable()
                                 select new DetalleLiquiRangosFranq
                                 {
                                     id = 0,
                                     acumuladoTotal = da.acumuladoTotal,
                                     compania_id = da.compania_id,
                                     ramo_id = da.ramo_id,
                                     producto_id = da.producto_id,
                                     numeroNegocio = da.numeroNegocio,
                                     anio = da.anio,
                                     PorcentajeActual = 0,
                                     RangoActual = 0
                                 }).ToList();


            //TRAEMOS LOS RANGOS DE PORCENTAJES PARA LOS DETALLES A LIQUIDAR                   
            var rangosMaestroTemp = (from dlf in contexto.DetalleLiquidacionFranquicias
                                     join pf in contexto.ParticipacionFranquicias
                                     on dlf.localidad_id equals pf.Localidad_id
                                     where dlf.liquidacionFranquicia_id == liquidacionFranquicia.id && dlf.fechaContabl >= liquidacionFranquicia.periodoLiquidacionIni &&
                                           dlf.fechaContabl <= liquidacionFranquicia.periodoLiquidacionFin &&
                                           (dlf.liquidadoPor == 2 || dlf.liquidadoPor == 0) &&
                                           dlf.fechaContabl >= pf.fecha_ini &&
                                           dlf.fechaContabl <= pf.fecha_fin
                                     group pf by pf.id into pfData
                                     select new
                                     {
                                         id = pfData.Max(d => d.id),
                                         localidad_id = pfData.Max(d => d.Localidad_id),
                                         fecha_ini = pfData.Max(d => d.fecha_ini),
                                         fecha_fin = pfData.Max(d => d.fecha_fin)
                                     });


            rangosPartFanquicia = (from rpf in rangosMaestroTemp.AsEnumerable()
                                   select new ParticipacionFranquicia
                                   {
                                       id = rpf.id,
                                       Localidad_id = rpf.localidad_id,
                                       fecha_ini = rpf.fecha_ini,
                                       fecha_fin = rpf.fecha_fin
                                   }).ToList();

            var rangosTemp = contexto.ObtenerRangosLiquidacionFranquicia(liquidacionFranquicia.id).ToList();

            rangos = (from da in rangosTemp.AsEnumerable()
                      select new DetallePartFranquicia
                      {
                          id = da.id,
                          compania_id = da.compania_id,
                          ramo_id = da.ramo_id,
                          producto_id = da.producto_id,
                          porcentaje = da.porcentaje,
                          rangoinferior = da.rangoinferior,
                          rangosuperior = da.rangosuperior,
                          plan_id = da.plan_id,
                          tipoVehiculo_id = da.tipoVehiculo_id,
                          amparo_id = da.amparo_id,
                          lineaNegocio_id = da.lineaNegocio_id,
                          part_franquicia_id = da.part_franquicia_id
                      }).ToList();

            int anioLiquidacion = liquidacionFranquicia.periodoLiquidacionIni.Value.Year;
            string smlv = contexto.SalarioMinimoes.Where(z => z.anio == anioLiquidacion).First().smlv.ToString();

            liquidacionProceso = contexto.LiquidacionFranquiciaProcesoes.Where(x => x.liquidacion_id == liquidacionFranquicia.id).FirstOrDefault();
            liquidacionProceso.proceso = 5;
            contexto.SaveChanges();

            //foreach (DetalleLiquidacionFranquicia detalleLiquidacionFranquicias in detalleLiquidacionFranquiciasList)
            //{
               
            //    //TRAEMOS LOS RANGOS PARA LA LIQUIDACION DE FRANQUICIA              
            //    rangosDetallePartFanquicia = obtenerRangos(detalleLiquidacionFranquicias, rangos, rangosPartFanquicia);

            //    //DETERMINAMOS SI HAY RANGOS PARA EL RECAUDO Y SI EL VALOR ES MAYOR DE 0
            //    if (rangosDetallePartFanquicia.Count() > 0 && detalleLiquidacionFranquicias.valorRecaudo != 0)
            //    {

                  
            //        //OBTENEMOS EL ACUMULADO EN EL QUE SE ENCUENTRA POR (compania_id-ramo_id-producto_id-numeroNegocio)
            //        DetalleLiquiRangosFranq detalleRangoAcumulado = (from da in detalleAcumulados
            //                                                         where
            //                                                           da.compania_id == detalleLiquidacionFranquicias.compania_id &&
            //                                                           da.ramo_id == detalleLiquidacionFranquicias.ramo_id &&
            //                                                           da.producto_id == detalleLiquidacionFranquicias.producto_id &&
            //                                                           da.numeroNegocio == detalleLiquidacionFranquicias.numeroNegocio
            //                                                         select da
            //                                                         ).FirstOrDefault();

            //        #region NO BORRAR

            //        /*double acumulado = 0;
            //        if (detalleRangoAcumulado != null && detalleRangoAcumulado.acumuladoTotal != null)
            //        {
            //            acumulado = Convert.ToDouble(detalleRangoAcumulado.acumuladoTotal);
            //        }

            //        //double valorRecaudo = Convert.ToDouble(detalleLiquidacionFranquicias.valorRecaudo);

            //        //TRAEMOS EL ID DEL RANGO DE PARTICIPACION SEGUN EL ACUMULADO
            //        int idParticipacionFranquicia = traerIdParticipacionPorAcumulado(acumulado, rangosDetallePartFanquicia, smlv); */

            //        #endregion

            //        //Traifo el valor del recaudo
            //        double valorRecaudo = Convert.ToDouble(detalleLiquidacionFranquicias.valorRecaudo);

            //        //CALCULAMOS LA PARTICIPACION
            //        double totalParticipacion = CalcularRango(valorRecaudo, /*idParticipacionFranquicia,*/ rangosDetallePartFanquicia, smlv,/* acumulado,*/ false);

            //        //GUARDAMOS TEMPORALMENTE EL TOTAL DE PARTICIPACION Y EL ID DE LA LIQUIDACION PARA HACER LA ACTUALIZACION POSTERIORMENTE
            //        liquidacionTemp.Add(new double[]
	           //     {
		          //      detalleLiquidacionFranquicias.id,
		          //      totalParticipacion,
            //            valorRecaudo
	           //     });

            //        //ACTUALIZAMOS LOS VALORES DE ACUMULADOS DE LA LIQUIDACION
            //        if (detalleRangoAcumulado == null)
            //        {
            //            acumuladosActualizar = new DetalleLiquiRangosFranq();
            //            acumuladosActualizar.id = -1;
            //            acumuladosActualizar.acumuladoTotal = valorRecaudo;
            //            acumuladosActualizar.compania_id = detalleLiquidacionFranquicias.compania_id;
            //            acumuladosActualizar.ramo_id = detalleLiquidacionFranquicias.ramo_id;
            //            acumuladosActualizar.producto_id = detalleLiquidacionFranquicias.producto_id;
            //            acumuladosActualizar.numeroNegocio = detalleLiquidacionFranquicias.numeroNegocio;
            //            detalleAcumulados.Add(acumuladosActualizar);
            //        }
            //        else
            //        {
            //            detalleRangoAcumulado.acumuladoTotal = /*acumulado +*/ valorRecaudo;
            //        }
            //    }
            //}

            liquidacionProceso = contexto.LiquidacionFranquiciaProcesoes.Where(x => x.liquidacion_id == liquidacionFranquicia.id).FirstOrDefault();
            liquidacionProceso.proceso = 6;
            contexto.SaveChanges();

            //PARA EJECUTAR SP
            EntityConnection entityConnection = (EntityConnection)contexto.Connection;
            DbConnection storeConnection = entityConnection.StoreConnection;
            DbCommand command = storeConnection.CreateCommand();
            command.Connection.Open();
            int maxRegisterExecuteSQL = 900;

            //GUARDAMOS INFORMACION DE ACUMULADOS
            //foreach (DetalleLiquiRangosFranq acumuladosActualizar1 in detalleAcumulados)
            //{
            //    //DETERMINAMOS SI SE CREA O ACTUALIZA
            //    string spExecute = "LiquidarFranquiciasAcumuladosInsert";
            //    if (acumuladosActualizar1.id == 0)
            //    {
            //        spExecute = "LiquidarFranquiciasAcumulados";
            //    }

            //    //EJECUTAMOS SP PARA INSERTAR NUEVOS ACUMULADOS                
            //    command = storeConnection.CreateCommand();
            //    command.CommandText = spExecute;
            //    command.CommandType = CommandType.StoredProcedure;
            //    command.Parameters.Add(new SqlParameter("liquidacionId", liquidacionFranquicia.id));
            //    command.Parameters.Add(new SqlParameter("companiaId", acumuladosActualizar1.compania_id));
            //    command.Parameters.Add(new SqlParameter("ramoId", acumuladosActualizar1.ramo_id));
            //    command.Parameters.Add(new SqlParameter("productoId", acumuladosActualizar1.producto_id));
            //    command.Parameters.Add(new SqlParameter("numeroNegocio", acumuladosActualizar1.numeroNegocio));
            //    command.Parameters.Add(new SqlParameter("acumuladoTotal", acumuladosActualizar1.acumuladoTotal));
            //    command.ExecuteNonQuery();
            //}

            //GUARDAMOS INFORMACION DE LIQUIDACION POR RANGOS EN TABLA TEMPORAL
            //int totalRegister = 1;
            //string strSQL = string.Empty;
            //foreach (double[] dataLiquidacion in liquidacionTemp)
            //{
            //    Int64 id = Convert.ToInt64(dataLiquidacion[0]);
            //    Double totalParticipacion = dataLiquidacion[1];
            //    Double valorRecaudo = dataLiquidacion[2];
            //    Double porcentajeParticipacion = (totalParticipacion / valorRecaudo) * 100;

            //    //GUARDAMOS LA SENTENCIA SQL
            //    strSQL += "(" + id + "," + totalParticipacion.ToString().Replace(',', '.') + "," + porcentajeParticipacion.ToString().Replace(',', '.') + "),";

            //    //EJECUTAMOS LA CONSULTA
            //    if (totalRegister == maxRegisterExecuteSQL)
            //    {
            //        strSQL = strSQL.Substring(0, strSQL.Length - 1).Trim();
            //        strSQL = "INSERT INTO DetalleLiquidacionFranquiciaTemp VALUES " + strSQL;
            //        command = storeConnection.CreateCommand();
            //        command.CommandText = strSQL;
            //        command.ExecuteNonQuery();
            //        totalRegister = 0;
            //        strSQL = string.Empty;
            //    }

            //    totalRegister++;
            //}

            //if (totalRegister > 0 && !string.IsNullOrEmpty(strSQL))
            //{
            //    strSQL = strSQL.Substring(0, strSQL.Length - 1).Trim();
            //    strSQL = "INSERT INTO DetalleLiquidacionFranquiciaTemp VALUES " + strSQL;
            //    command = storeConnection.CreateCommand();
            //    command.CommandText = strSQL;
            //    command.ExecuteNonQuery();
            //}

            //EJECUTAMOS SP PARA ACTUALIZAR TABLA REAL CON TABLA TEMP DE LIQUIDACION DE RANGOS
            command = storeConnection.CreateCommand();
            command.CommandText = "LiquidarFranquiciasRangos";
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();

            command.Connection.Close();

            return 1;
        }

        private int traerIdParticipacionPorAcumulado(double acumulado, List<DetallePartFranquicia> detallePartFranquicias, string smlv)
        {

            int idParticipacionPorAcumulado = 0;
            if (acumulado == 0)
                idParticipacionPorAcumulado = detallePartFranquicias.First().id;
            else
            {
                foreach (DetallePartFranquicia detalle in detallePartFranquicias)
                {
                    double? valorMinimo = (detalle.rangoinferior > 0) ? detalle.rangoinferior * Convert.ToDouble(smlv) : 0;
                    double? valorMaximo = (detalle.rangosuperior > 0) ? detalle.rangosuperior * Convert.ToDouble(smlv) : 0;

                    if (Math.Abs(acumulado) >= valorMinimo && Math.Abs(acumulado) <= valorMaximo)
                    {
                        idParticipacionPorAcumulado = detalle.id;
                        return idParticipacionPorAcumulado;
                    }
                }
            }

            return idParticipacionPorAcumulado;

        }

        /// <summary>
        /// Metodo que devuelve liquidacion desde cero o con acumulados
        /// </summary>
        /// <param name="valorRecaudo">valor del recaudo para hacer calculos</param>
        /// <param name="id">id de la participación de franquicia</param>
        /// <param name="detallePartFranquicias"> lista de detalle de participacion de franquicias usado para el calculo del rango</param>
        /// <returns></returns>
        public double CalcularRango(double valorRecaudo, /*int id,*/List<DetallePartFranquicia> detallePartFranquicias, string smlv, /*double acumulado,*/ Boolean invertido)
        {
            try
            {
                double result = 0;
                double valorMinimoLiquidar = 0;
                double valorMaximoLiquidar = 0;
                double totalLiquidado = 0;
                double nuevoValorLiquidar = 0;
                double nuevoSaldo = valorRecaudo;

                foreach (DetallePartFranquicia detalle in detallePartFranquicias)
                {
                    if (Math.Abs(nuevoSaldo) > valorMinimoLiquidar)
                    {
                        //Valor que se utiliza para verificar cuanto es en salarios minimos a liquidar
                        valorMinimoLiquidar = detalle.rangosuperior.Value * Convert.ToDouble(smlv);
                        valorMaximoLiquidar = detallePartFranquicias[2].rangoinferior.Value * Convert.ToDouble(smlv);

                        //Verifica si el recaudo es mayor al valor de valor minimo a liquidar para realizar la liquidacion especial
                        if (Math.Abs(valorRecaudo) > valorMaximoLiquidar)
                        {
                            #region Calculo para mayor el valor del recaudo mayor a el valor minimo a liquidar
                            //Calcula el valor de porcentaje a pagar cuando el recaudo es inferior al del valor minimo a liquidar
                            if (Math.Abs(valorRecaudo) < valorMinimoLiquidar)
                            {
                                //Cuando el valor del recaudo es superior al Valor minimo a liquidar
                                if (valorRecaudo > 0)
                                {
                                    double nuevoRecaudo = valorRecaudo - valorMaximoLiquidar;
                                    totalLiquidado += nuevoRecaudo * detalle.porcentaje.Value / 100;
                                }
                                //Cuando el valor del recaudo es 0 (cero), Se multiplica por -1.
                                else
                                {
                                    double nuevoRecaudo = valorRecaudo + valorMaximoLiquidar;
                                    totalLiquidado += nuevoRecaudo * detalle.porcentaje.Value / 100;
                                }
                            }
                            else
                            {
                                //Cuando el valor del recaudo es superior al Valor minimo a liquidar
                                if (valorRecaudo > 0)
                                {
                                    if (nuevoValorLiquidar != 0)
                                        valorMinimoLiquidar = nuevoValorLiquidar;

                                    totalLiquidado += valorMinimoLiquidar * detalle.porcentaje.Value / 100;
                                    nuevoValorLiquidar = valorMaximoLiquidar - valorMinimoLiquidar;
                                }
                                //Cuando el valor del recaudo es 0 (cero), Se multiplica por -1.
                                else
                                {
                                    if (nuevoValorLiquidar != 0)
                                        valorMinimoLiquidar = nuevoValorLiquidar;

                                    totalLiquidado += ((-1) * valorMinimoLiquidar) * detalle.porcentaje.Value / 100;
                                    nuevoValorLiquidar = valorMaximoLiquidar - valorMinimoLiquidar;
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region Calcula el valor de porcentaje a pagar cuando el recaudo es inferior al del valor minimo a liquidar
                            //Calcula el valor de porcentaje a pagar cuando el recaudo es inferior al del valor minimo a liquidar
                            if (Math.Abs(valorRecaudo) < valorMinimoLiquidar)
                            {
                                totalLiquidado += valorRecaudo * detalle.porcentaje.Value / 100;

                                if (valorRecaudo < 0)
                                    nuevoSaldo = valorRecaudo + valorMinimoLiquidar;
                                else
                                    nuevoSaldo = valorRecaudo - valorMinimoLiquidar;
                            }
                            else
                            {
                                //Cuando el valor del recaudo es superior al Valor minimo a liquidar
                                if (valorRecaudo > 0)
                                {
                                    totalLiquidado += valorMinimoLiquidar * detalle.porcentaje.Value / 100;
                                    valorRecaudo = valorRecaudo - valorMinimoLiquidar;
                                }
                                //Cuando el valor del recaudo es 0 (cero), Se multiplica por -1.
                                else
                                {
                                    totalLiquidado += ((-1) * valorMinimoLiquidar) * detalle.porcentaje.Value / 100;
                                    valorRecaudo = valorRecaudo - ((-1) * valorMinimoLiquidar);
                                }
                            }
                            #endregion
                        }
                    }
                }

                result = totalLiquidado;
                return result;

                #region NO BORRAR (Cogido Anterior, Liquida por acumulado)
                /*double result = 0;
            double rango = 0;

            #region NO BORRAR (Codigo Anterior, liquida por acumulado)

            //SE CONSULTA EL RANGO ACTUAL POR EL ID
            //DETERMINA SI EL VALOR DEL RECAUDO ES MENOR DE CERO PARA OBTENER EL RANGOINFERIOR DEL DETALLE PART FRANQUICIAS
            if (valorRecaudo < 0)
            {

                //HAY UNA EXCEPCION CUANDO EL VALOR DEL RECAUDO ES MENOR DE 0 Y ADEMAS EL ACUMULADO TAMBIEN LO ES.
                //EN ESTE CASO VUELVE Y SE TOMA EL RANGO SUPERIOR DE DETALLE PART FRANQUICIAS
                if (acumulado <= 0)
                    rango = (double)(Math.Abs(acumulado) - (detallePartFranquicias.FirstOrDefault(e => e.id == id).rangosuperior * Convert.ToDouble(smlv)));
                else
                    rango = (double)(acumulado - (detallePartFranquicias.FirstOrDefault(e => e.id == id).rangoinferior * Convert.ToDouble(smlv)));

            }
            else
            {

           

            //HAY UNA EXCEPCION CUANDO EL VALOR DEL RECAUDO ES MENOR DE 0 Y ADEMAS EL ACUMULADO TAMBIEN LO ES.
                //EN ESTE CASO VUELVE Y SE TOMA EL RANGO INFERIOR DE DETALLE PART FRANQUICIAS
                if (acumulado < 0)
                    rango = (double)(Math.Abs(acumulado) - (detallePartFranquicias.FirstOrDefault(e => e.id == id).rangoinferior * Convert.ToDouble(smlv)));
                else
                {
                    var rangoSuperior = detallePartFranquicias.FirstOrDefault(e => e.id == id).rangosuperior;
                    if (rangoSuperior != null)
                        rangoSuperior = Convert.ToDouble(rangoSuperior);

                    rango = (double)(acumulado - (rangoSuperior * Convert.ToDouble(smlv)));
                    rango = Math.Abs(rango);
                }

            }

            #endregion

            rango = detallePartFranquicias.FirstOrDefault().rangosuperior.Value * Convert.ToDouble(smlv);


            //SE CALCULA EL PORCENTAJE
            //double porcentaje = Convert.ToDouble(detallePartFranquicias.FirstOrDefault(e => e.id == id).porcentaje);
            double porcentaje = Convert.ToDouble(detallePartFranquicias.FirstOrDefault().porcentaje);

            int factor = 1;

            #region NO BORRAR (Codigo Anterior, liquida por acumulado)
            // si valor del recaudo es menor al rango indica  la liquidacion esta dentro del rango  
            // se hace la validacion con el valor absoluto cuando el valor del recaudo sea negativo
            if (Math.Abs(valorRecaudo) < Math.Abs(rango))
            {
                //EstaExcepcion se tiene en cuenta para cuando se esta en acumulado negativo y los recaudos son positivos en este caso la liquidacion va negativa y no positiva
                if (valorRecaudo > 0 && acumulado < 0)
                    factor = -1; 

                result = factor * valorRecaudo * (porcentaje / 100);
            }
            else
            {
                // si valor del recaudo es mayor al rango indica  la liquidacion no esta dentro del rango y se llama recursivamente esta funcion para recalcular porcentaje y valor  
                if ((valorRecaudo < 0 && acumulado > 0) || (valorRecaudo > 0 && acumulado < 0))
                    factor = -1; 
            #endregion

            result = factor * rango * (porcentaje / 100);


            #region NO BORRAR

            if (valorRecaudo > 0 && acumulado < 0)
            {
                valorRecaudo = valorRecaudo - (rango);
                acumulado += rango;
            }
            else
            {
                valorRecaudo = valorRecaudo - (rango * factor);
                acumulado += (rango * factor);
            } 

            #endregion

            if (valorRecaudo > 0)
                valorRecaudo = valorRecaudo - (rango);


            #region NO BORRAR
             int idSiguiente = 0;

            //SACAR EL SIGUIENTE ID DE LA PARAMETRIZACION POR RANGOS. SI TIENE.
            DetallePartFranquicia detallePartFranquiciaTemp = new DetallePartFranquicia();
            if (acumulado == 0)
                idSiguiente = id;

            if (idSiguiente == 0)
            {
                //OBTENEMOS EL RANGO INFERIOR DEL ID ACTUAL
                double rangoInferiorActual = Convert.ToDouble(detallePartFranquicias.Where(x => x.id == id).First().rangoinferior);

                if (valorRecaudo >= 0 || acumulado < 0)
                {
                    if (valorRecaudo >= 0 && acumulado < 0)
                        detallePartFranquiciaTemp = detallePartFranquicias.Where(x => x.rangoinferior < rangoInferiorActual).Last();
                    else
                        detallePartFranquiciaTemp = detallePartFranquicias.Where(x => x.rangoinferior > rangoInferiorActual).First();
                }
                else
                    detallePartFranquiciaTemp = detallePartFranquicias.Where(x => x.rangoinferior < rangoInferiorActual).Last();

                if (detallePartFranquiciaTemp != null)
                    idSiguiente = detallePartFranquiciaTemp.id;
            }

            if (idSiguiente != 0)  
            #endregion
            result += CalcularRango(valorRecaudo, idSiguiente, detallePartFranquicias, smlv, acumulado, invertido);
            }

            return result; */
                #endregion
            }
            catch
            { throw; }
        }

        private List<DetallePartFranquicia> obtenerRangos(DetalleLiquidacionFranquicia detalleLiquidacionFranquicia, List<DetallePartFranquicia> rangosParticipacionFranquicia, List<ParticipacionFranquicia> rangosParticipacionFranquiciaMaestro)
        {
            List<DetallePartFranquicia> rangosList = new List<DetallePartFranquicia>();

            Boolean tieneRango = false;


            //VALIDAMOS LA LIQUIDACION POR CADA UNA DE LAS DISTINTAS COMBINACIONES PARA LOS RANGOS		
            //Combinacion Compañia - Ramo - Producto - Linea de negocio - Tipo de vehículo - Amparo
            if (!tieneRango && detalleLiquidacionFranquicia.compania_id != null && detalleLiquidacionFranquicia.ramo_id != null && detalleLiquidacionFranquicia.producto_id != null && detalleLiquidacionFranquicia.lineaNegocio_id != null && detalleLiquidacionFranquicia.lineaNegocio_id != 0 && detalleLiquidacionFranquicia.tipoVehiculo != null && detalleLiquidacionFranquicia.tipoVehiculo != "0" && detalleLiquidacionFranquicia.amparo_Id != null && detalleLiquidacionFranquicia.amparo_Id != 0)
            {
                rangosList = (from dpf in rangosParticipacionFranquicia
                              join pf in rangosParticipacionFranquiciaMaestro
                              on dpf.part_franquicia_id equals pf.id
                              where
                                pf.Localidad_id == detalleLiquidacionFranquicia.localidad_id && detalleLiquidacionFranquicia.fechaContabl >= pf.fecha_ini && detalleLiquidacionFranquicia.fechaContabl <= pf.fecha_fin &&
                                dpf.compania_id == detalleLiquidacionFranquicia.compania_id &&
                                dpf.ramo_id == detalleLiquidacionFranquicia.ramo_id_agrupado &&
                                dpf.producto_id == detalleLiquidacionFranquicia.producto_id_agrupado &&
                                dpf.lineaNegocio_id == detalleLiquidacionFranquicia.lineaNegocio_id &&
                                dpf.tipoVehiculo_id == Convert.ToInt32(detalleLiquidacionFranquicia.tipoVehiculo) &&
                                dpf.amparo_id == detalleLiquidacionFranquicia.amparo_Id &&
                                dpf.rangoinferior != null &&
                                dpf.rangosuperior != null
                              select dpf
                     ).ToList();

                if (rangosList.Count() > 0)
                    tieneRango = true;
            }

            //Combinacion Compañia - Ramo - Producto - Linea de negocio - Tipo de vehículo
            if (!tieneRango && detalleLiquidacionFranquicia.compania_id != null && detalleLiquidacionFranquicia.ramo_id != null && detalleLiquidacionFranquicia.producto_id != null && detalleLiquidacionFranquicia.lineaNegocio_id != null && detalleLiquidacionFranquicia.lineaNegocio_id != 0 && detalleLiquidacionFranquicia.tipoVehiculo != null && detalleLiquidacionFranquicia.tipoVehiculo != "0")
            {
                rangosList = (from dpf in rangosParticipacionFranquicia
                              join pf in rangosParticipacionFranquiciaMaestro
                              on dpf.part_franquicia_id equals pf.id
                              where
                                pf.Localidad_id == detalleLiquidacionFranquicia.localidad_id && detalleLiquidacionFranquicia.fechaContabl >= pf.fecha_ini && detalleLiquidacionFranquicia.fechaContabl <= pf.fecha_fin &&
                                dpf.compania_id == detalleLiquidacionFranquicia.compania_id &&
                                dpf.ramo_id == detalleLiquidacionFranquicia.ramo_id_agrupado &&
                                dpf.producto_id == detalleLiquidacionFranquicia.producto_id_agrupado &&
                                dpf.lineaNegocio_id == detalleLiquidacionFranquicia.lineaNegocio_id &&
                                dpf.tipoVehiculo_id == Convert.ToInt32(detalleLiquidacionFranquicia.tipoVehiculo) &&
                                dpf.rangoinferior != null &&
                                dpf.rangosuperior != null
                              select dpf
                     ).ToList();

                if (rangosList.Count() > 0)
                    tieneRango = true;
            }

            //Combinacion Compañia - Ramo - Producto - Linea de negocio - Amparo
            if (!tieneRango && detalleLiquidacionFranquicia.compania_id != null && detalleLiquidacionFranquicia.ramo_id != null && detalleLiquidacionFranquicia.producto_id != null && detalleLiquidacionFranquicia.lineaNegocio_id != null && detalleLiquidacionFranquicia.lineaNegocio_id != 0 && detalleLiquidacionFranquicia.amparo_Id != null && detalleLiquidacionFranquicia.amparo_Id != 0)
            {
                rangosList = (from dpf in rangosParticipacionFranquicia
                              join pf in rangosParticipacionFranquiciaMaestro
                              on dpf.part_franquicia_id equals pf.id
                              where
                                pf.Localidad_id == detalleLiquidacionFranquicia.localidad_id && detalleLiquidacionFranquicia.fechaContabl >= pf.fecha_ini && detalleLiquidacionFranquicia.fechaContabl <= pf.fecha_fin &&
                                dpf.compania_id == detalleLiquidacionFranquicia.compania_id &&
                                dpf.ramo_id == detalleLiquidacionFranquicia.ramo_id_agrupado &&
                                dpf.producto_id == detalleLiquidacionFranquicia.producto_id_agrupado &&
                                dpf.lineaNegocio_id == detalleLiquidacionFranquicia.lineaNegocio_id &&
                                dpf.amparo_id == detalleLiquidacionFranquicia.amparo_Id &&
                                dpf.rangoinferior != null &&
                                dpf.rangosuperior != null
                              select dpf
                     ).ToList();

                if (rangosList.Count() > 0)
                    tieneRango = true;
            }

            //Combinacion Compañia - Ramo - Producto - Linea de negocio
            if (!tieneRango && detalleLiquidacionFranquicia.compania_id != null && detalleLiquidacionFranquicia.ramo_id != null && detalleLiquidacionFranquicia.producto_id != null && detalleLiquidacionFranquicia.lineaNegocio_id != null && detalleLiquidacionFranquicia.lineaNegocio_id != 0)
            {
                rangosList = (from dpf in rangosParticipacionFranquicia
                              join pf in rangosParticipacionFranquiciaMaestro
                              on dpf.part_franquicia_id equals pf.id
                              where
                                pf.Localidad_id == detalleLiquidacionFranquicia.localidad_id && detalleLiquidacionFranquicia.fechaContabl >= pf.fecha_ini && detalleLiquidacionFranquicia.fechaContabl <= pf.fecha_fin &&
                                dpf.compania_id == detalleLiquidacionFranquicia.compania_id &&
                                dpf.ramo_id == detalleLiquidacionFranquicia.ramo_id_agrupado &&
                                dpf.producto_id == detalleLiquidacionFranquicia.producto_id_agrupado &&
                                dpf.lineaNegocio_id == detalleLiquidacionFranquicia.lineaNegocio_id &&
                                dpf.rangoinferior != null &&
                                dpf.rangosuperior != null
                              select dpf
                     ).ToList();

                if (rangosList.Count() > 0)
                    tieneRango = true;
            }

            //Combinacion Compañia - Ramo - Producto - Tipo de vehículo
            if (!tieneRango && detalleLiquidacionFranquicia.compania_id != null && detalleLiquidacionFranquicia.ramo_id != null && detalleLiquidacionFranquicia.producto_id != null && detalleLiquidacionFranquicia.tipoVehiculo != null && detalleLiquidacionFranquicia.tipoVehiculo != "0")
            {
                rangosList = (from dpf in rangosParticipacionFranquicia
                              join pf in rangosParticipacionFranquiciaMaestro
                              on dpf.part_franquicia_id equals pf.id
                              where
                                pf.Localidad_id == detalleLiquidacionFranquicia.localidad_id && detalleLiquidacionFranquicia.fechaContabl >= pf.fecha_ini && detalleLiquidacionFranquicia.fechaContabl <= pf.fecha_fin &&
                                dpf.compania_id == detalleLiquidacionFranquicia.compania_id &&
                                dpf.ramo_id == detalleLiquidacionFranquicia.ramo_id_agrupado &&
                                dpf.producto_id == detalleLiquidacionFranquicia.producto_id_agrupado &&
                                dpf.tipoVehiculo_id == Convert.ToInt32(detalleLiquidacionFranquicia.tipoVehiculo) &&
                                dpf.rangoinferior != null &&
                                dpf.rangosuperior != null
                              select dpf
                     ).ToList();

                if (rangosList.Count() > 0)
                    tieneRango = true;
            }

            //Combinacion Compañia - Ramo - Producto - Amparo
            if (!tieneRango && detalleLiquidacionFranquicia.compania_id != null && detalleLiquidacionFranquicia.ramo_id != null && detalleLiquidacionFranquicia.producto_id != null && detalleLiquidacionFranquicia.amparo_Id != null && detalleLiquidacionFranquicia.amparo_Id != 0)
            {
                rangosList = (from dpf in rangosParticipacionFranquicia
                              join pf in rangosParticipacionFranquiciaMaestro
                              on dpf.part_franquicia_id equals pf.id
                              where
                                pf.Localidad_id == detalleLiquidacionFranquicia.localidad_id && detalleLiquidacionFranquicia.fechaContabl >= pf.fecha_ini && detalleLiquidacionFranquicia.fechaContabl <= pf.fecha_fin &&
                                dpf.compania_id == detalleLiquidacionFranquicia.compania_id &&
                                dpf.ramo_id == detalleLiquidacionFranquicia.ramo_id_agrupado &&
                                dpf.producto_id == detalleLiquidacionFranquicia.producto_id_agrupado &&
                                dpf.amparo_id == Convert.ToInt32(detalleLiquidacionFranquicia.amparo_Id) &&
                                dpf.rangoinferior != null &&
                                dpf.rangosuperior != null
                              select dpf
                     ).ToList();

                if (rangosList.Count() > 0)
                    tieneRango = true;
            }

            //Combinacion Compania + Ramo + Producto
            if (!tieneRango && detalleLiquidacionFranquicia.compania_id != null && detalleLiquidacionFranquicia.ramo_id != null && detalleLiquidacionFranquicia.producto_id != null)
            {
                rangosList = (from dpf in rangosParticipacionFranquicia
                              join pf in rangosParticipacionFranquiciaMaestro
                              on dpf.part_franquicia_id equals pf.id
                              where
                                pf.Localidad_id == detalleLiquidacionFranquicia.localidad_id && detalleLiquidacionFranquicia.fechaContabl >= pf.fecha_ini && detalleLiquidacionFranquicia.fechaContabl <= pf.fecha_fin &&
                                dpf.compania_id == detalleLiquidacionFranquicia.compania_id &&
                                dpf.ramo_id == detalleLiquidacionFranquicia.ramo_id_agrupado &&
                                dpf.producto_id == detalleLiquidacionFranquicia.producto_id_agrupado &&
                                dpf.rangoinferior != null &&
                                dpf.rangosuperior != null
                              select dpf
                     ).ToList();

                if (rangosList.Count() > 0)
                    tieneRango = true;
            }

            if (rangosList != null && rangosList.Count() > 0)
                rangosList = rangosList.OrderBy(e => e.rangoinferior).ToList();

            return rangosList;
        }

        public int ObtenerProcesoLiquidacion()
        {
            int idProceso = 0;

            SAI_Entities contextoTemp = new SAI_Entities();

            var liquidacionTemp = (from lfp in contextoTemp.LiquidacionFranquiciaProcesoes
                                   select lfp).ToList();

            if (liquidacionTemp != null && liquidacionTemp.Count() > 0)
            {
                idProceso = Convert.ToInt32(liquidacionTemp.First().proceso);
            }

            return idProceso;
        }

        public int EliminarLiquidacionProceso(int idLiquidacion, string Username)
        {

            Entidades.LiquidacionFranquiciaProceso liquidacionProcesos = contexto.LiquidacionFranquiciaProcesoes.Where(e => e.liquidacion_id == idLiquidacion).FirstOrDefault();
            contexto.LiquidacionFranquiciaProcesoes.DeleteObject(liquidacionProcesos);
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.LiquidacionFranquiciaProceso,
    SegmentosInsercion.Personas_Y_Pymes, liquidacionProcesos, null);
            return contexto.SaveChanges();
        }

    }

}
