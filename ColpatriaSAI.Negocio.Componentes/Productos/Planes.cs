using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;


namespace ColpatriaSAI.Negocio.Componentes.Productos
{
    public class Planes
    {
        private SAI_Entities tabla = new SAI_Entities();

        #region Planes
        public List<Plan> ListarPlans()
        {
            //Relacion de campos de tablas respecto a las variables de navegación en el Modelo de datos x tabla

            return tabla.Plans.Include("Producto.Ramo.Compania").Where(Plan => Plan.id > 0).OrderBy(e => e.nombre).ToList();

            //EXAMPLE:
            //Company company = context.Companies
            //             .Include("Employee.Employee_Car")
            //             .Include("Employee.Employee_Country")
            //             .FirstOrDefault(c => c.Id == companyID);
        }

        public List<Plan> ListarPlansPorId(int idPlan)
        {
            return tabla.Plans.Include("Producto").Where(Plan => Plan.id == idPlan && Plan.id > 0).ToList();
        }

        public List<Plan> ListarPlansPorProducto(int idProducto)
        {
            return tabla.Plans.Include("Producto").Where(Plan => Plan.producto_id == idProducto && Plan.id > 0).ToList();
        }

        public List<Plan> ListarPlanPorProducto(int idProducto)
        {
            return tabla.Plans.Where(Plan => Plan.producto_id == idProducto && Plan.id > 0).OrderBy(e => e.nombre).ToList();
        }

        public int InsertarPlan(Plan plan, string Username)
        {
            int resultado = 0;
            if (plan.id == 0)
            {
                if (tabla.Plans.Where(Plan => Plan.nombre == plan.nombre).ToList().Count() == 0)
                {
                    tabla.Plans.AddObject(plan);
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.Plan,
    SegmentosInsercion.Personas_Y_Pymes, null, plan);
                    tabla.SaveChanges();
                    resultado = plan.id;
                }
            }
            else
            {
                if (tabla.Plans.Where(Plan => Plan.nombre == plan.nombre && Plan.id != plan.id).ToList().Count() == 0)
                {
                    Plan planActual = tabla.Plans.Single(p => p.id == plan.id);
                    var pValorAntiguo = planActual;
                    planActual.nombre = plan.nombre;
                    planActual.producto_id = plan.producto_id;
                    new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Plan,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, planActual);
                    tabla.SaveChanges();
                    resultado = plan.id;
                }
            }
            return resultado;
        }

        public string EliminarPlan(int id, string Username)
        {
            var detalleActual = tabla.PlanDetalles.Where(p => p.plan_id == id);
            foreach (var item in detalleActual) item.plan_id = 0;

            var planActual = tabla.Plans.Single(p => p.id == id);
            tabla.Plans.DeleteObject(planActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.Plan,
    SegmentosInsercion.Personas_Y_Pymes, planActual, null);
                tabla.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
        #endregion

        #region Planes Detalle

        public List<PlanDetalle> ListarPlanDetalles(int id)
        {
            if (id != 0)
                return tabla.PlanDetalles.Include("ProductoDetalle.RamoDetalle.Compania").Include("Plan").Where(p => p.id > 0 && (p.plan_id == id || p.plan_id == 0)).OrderBy(p => p.nombre).ToList();
            else
                return tabla.PlanDetalles.Include("ProductoDetalle.RamoDetalle.Compania").Include("Plan").Where(p => p.id > 0).OrderBy(p => p.nombre).ToList();
        }

        public int AgruparPlanDetalle(int plan_id, string planesTrue, string planesFalse)
        {
            List<int> idsTrue = new List<int>();
            List<int> idsFalse = new List<int>();
            foreach (string id in planesTrue.Split(','))
            {
                if (id != "") idsTrue.Add(Convert.ToInt32(id));
            }
            foreach (string id in planesFalse.Split(','))
            {
                if (id != "") idsFalse.Add(Convert.ToInt32(id));
            }

            var planDetalle = tabla.PlanDetalles;

            foreach (int id in idsTrue) planDetalle.Single(rd => rd.id == id).plan_id = plan_id;
            foreach (int id in idsFalse) planDetalle.Single(rd => rd.id == id).plan_id = 0;
            return tabla.SaveChanges();
        }

        public List<PlanDetalle> ListarPlanDetalleXProductoDetalle(int productoDetalleId)
        {
            return tabla.PlanDetalles
                .Include("ProductoDetalle.RamoDetalle.Compania")
                .Include("Plan")
                .Where(p => p.productoDetalle_id == productoDetalleId).OrderBy(p => p.nombre).ToList();
        }

        public List<PlanDetalle> ListarPlanDetalleActivosXProductoDetalle(int productoDetalleId)
        {
            return tabla.PlanDetalles
                .Include("ProductoDetalle.RamoDetalle.Compania")
                .Include("Plan")
                .Where(p => p.productoDetalle_id == productoDetalleId && (p.Activo.HasValue && p.Activo.Value)).OrderBy(p => p.nombre).ToList();
        }
        #endregion
    }

    public class Plazos
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<Plazo> ListarPlazoes()
        {
            return tabla.Plazoes.Where(Plazo => Plazo.id > 0).OrderBy(p => p.nombre).ToList();
        }

    }

    public class ModalidadPagos
    {
        private SAI_Entities tabla = new SAI_Entities();

        public List<ModalidadPago> ListarModalidadPagoes()
        {
            return tabla.ModalidadPagoes.Where(ModalidadPago => ModalidadPago.id > 0).OrderBy(p => p.nombre).ToList();
        }
    }
}
