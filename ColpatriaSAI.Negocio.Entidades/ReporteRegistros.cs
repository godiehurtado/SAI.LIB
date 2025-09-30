using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColpatriaSAI.Negocio.Entidades
{
    public class ReporteRegistros
    {
        private Int32 _IdCompania;
        private String _Compania;
        private Int32 _TotalRegistrosMes;
        private Int32 _PromedioRegistros;

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
        public Int32 TotalRegistrosMes
        {
            get
            {
                return _TotalRegistrosMes;
            }
            set
            {
                _TotalRegistrosMes = value;
            }
        }
        public Int32 PromedioRegistros
        {
            get
            {
                return _PromedioRegistros;
            }
            set
            {
                _PromedioRegistros = value;
            }
        }
    }
}
