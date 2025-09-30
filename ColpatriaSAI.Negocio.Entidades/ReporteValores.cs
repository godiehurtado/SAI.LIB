using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColpatriaSAI.Negocio.Entidades
{
    public class ReporteValores
    {
        private Int32 _IdCompania;
        private String _Compania;
        private Double _TotalValorMes;
        private Double _Promedio;

        public Int32 IdCompania
        {
            get
            {
                return _IdCompania;
            }
            set
            {
                _IdCompania = value;
            }
        }
        public String Compania
        {
            get
            {
                return _Compania;
            }
            set
            {
                _Compania = value;
            }
        }
        public Double TotalValorMes
        {
            get
            {
                return _TotalValorMes;
            }
            set
            {
                _TotalValorMes = value;
            }
        }
        public Double Promedio
        {
            get
            {
                return _Promedio;
            }
            set
            {
                _Promedio = value;
            }
        }
    }
}
