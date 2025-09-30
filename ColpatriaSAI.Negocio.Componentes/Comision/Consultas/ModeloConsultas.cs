using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColpatriaSAI.Negocio.Componentes.Comision.Consultas
{
    public class ListadoUsuarios
    {
        public string numExtraccion { get; set; }
        public long usuariosEntrantes { get; set; }
        public long UsuariosSalientes { get; set; }
    }

    public class DetalleFacturas {
        public string Descripcion_log { get; set; }
        public long conteo { get; set; }
    }

    public class DetalleUsuarios
    {
        public string Descripcion_log { get; set; }
        public long conteo { get; set; }
    }

    public class LogExtFacturacion {
        public string CTY_CNAME { get; set; }
      public string PRO_NCODE {get;set;}
      public string PRO_CNAME {get;set;}
      public string  PLA_NCODE {get;set;}
      public string PLA_CNAME {get;set;}
      public string  CONTRACT_CODE {get;set;}
      public string AIN_CLEGALCODE {get;set;}
      public DateTime  AIN_DINVOICEDATE {get;set;}
      public string   AIN_NTOTALVALUE {get;set;}
      public double  AIN_NTAXES {get;set;}
      public string  TOTAL_FACT {get;set;}
      public int INT_NCODE {get;set;}
      public string INT_CINTERMEDIARY_NAME {get;set;}
      public string INT_CKEY {get;set;}
      public string BRE_NCOMMISSIONPERCENT {get;set;}
      public int MEM_NCODE {get;set;}
      public string ITY_NCODE {get;set;}
      public string PER_CIDENTIFICATIONNUMBER {get;set;}
      public string PCO_CFARE_CODE {get;set;}
      public string MVA_NVALUE {get;set;}
      public int ANO_NCODE {get;set;}
      public string NOV_CDESCRIPTION {get;set;}
      public string  ANO_DCREATEDDATE {get;set;}
      public string  ANO_DSTARTINGDATE {get;set;}
      public string  ANO_DFINALDATE {get;set;}
      public string  ANO_NNOVELTYVALUE {get;set;}
      public string VALOR_USUARIO {get;set;}
      public string  VALOR_ASESOR {get;set;}
      public DateTime EXTRACT_DATE {get;set;}
      public int ESTADO_ID {get;set;}
      public string CodExtraccion {get;set;}
      public string ITY_CSHORTNAME {get;set;}
    }

    public class LogExtracionRecaudo { 
    
     public string CTY_NCODE {get;set;}
	 public string  CTY_CNAME {get;set;}
	 public string PRO_NCODE {get;set;}
	 public string PRO_CNAME {get;set;}
	 public string PLA_NCODE {get;set;}
	 public string PLA_CNAME {get;set;}
	 public string CONTRACT_CODE {get;set;}
  	 public string AIN_CLEGALCODE{get;set;}
	 public DateTime  AIN_DINVOICEDATE {get;set;}
	 public string AIN_NTOTALVALUE {get;set;}
	 public string AIN_NTAXES {get;set;}
	 public string TOTAL_FACT {get;set;}
	 public string INT_NCODE {get;set;}
	 public string INT_CINTERMEDIARY_NAME {get;set;}
	 public string INT_CKEY {get;set;}
	 public string BRE_NCOMMISSIONPERCENT {get;set;} 
	 public string ITY_NCODE {get;set;}
	 public string PER_CIDENTIFICATIONNUMBER {get;set;}
	 public string PCO_CFARE_CODE {get;set;}
	 public string MVA_NVALUE {get;set;}
	 public string PARTICIPATION {get;set;}
	 public string VALOR_USUARIO {get;set;}
	 public string VALOR_ASESOR {get;set;}
	 public DateTime EXTRACT_DATE {get;set;}
	 public string EXTRACT_STATE {get;set;}
	 public string CodExtraccion {get;set;}
	 public string ITY_CSHORTNAME {get;set;}
    }

    public class LogExtraccionBH
    {
        public string MEM_NCODE { get; set; }
        public string TID_NCODE { get; set; }
        public string ITY_CTYPEIDENTIFICATION { get; set; }
        public string PER_CIDENTIFICATIONNUMBER { get; set; }
        public string PER_CLASTNAME { get; set; }
        public string PER_CMOTHERNAME { get; set; }
        public string PER_CFIRSTNAME { get; set; }
        public string PER_CMIDDLENAME { get; set; }
        public string AGE { get; set; }
        public string ACO_CONTRACTCODE { get; set; }
        public string SUB_CONTRACT { get; set; }
        public string UTY_NCODE { get; set; }
        public string UTY_CNAME { get; set; }
        public string INT_NCODE { get; set; }
        public string INT_CINTERMEDIARY_NAME { get; set; }
        public string INT_CKEY { get; set; }
        public string BRE_NCOMMISSIONPERCENT { get; set; }
        public string DIR_NCODE { get; set; }
        public string DIR_CNAME { get; set; }
        public string CTY_NCODE { get; set; }
        public string CTY_CNAME { get; set; }
        public string PROPLA_NCODE { get; set; }
        public string MEM_DSTARTINGDATE { get; set; }
        public string MEM_DENDINGDATE { get; set; }
        public string PME_CDESCRIPTION { get; set; }
        public string FST_NCODE { get; set; }
        public string FST_CNAME { get; set; }
        public string PME_NCODE { get; set; }
        public string ESTADO { get; set; }
        public string TIPOLOG { get; set; }
        public string CodExtraccion { get; set; }

    }
}
