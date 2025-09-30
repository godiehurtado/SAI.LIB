using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColpatriaSAI.Datos;
using ColpatriaSAI.Negocio.Entidades;
using System.Data.SqlClient;

namespace ColpatriaSAI.Negocio.Componentes.Concursos
{
    public class ParticipanteConcursos
    {
        private SAI_Entities tabla = new SAI_Entities();


        public List<Participante> ListarParticipantes()
        {
            return tabla.Participantes.ToList();
        }

        public List<ParticipanteConcurso> ListarParticipanteConcursoes()
        {
            return tabla.ParticipanteConcursoes.Include("Concurso").Include("Segmento").Include("Canal").Include("Nivel").Include("Localidad").Include("Zona").Include("Participante").Include("Categoria").Include("Compania").Include("JerarquiaDetalle").Where(ParticipanteConcurso => ParticipanteConcurso.id > 0 && ParticipanteConcurso.canal_id >= 0 && ParticipanteConcurso.zona_id >= 0 && ParticipanteConcurso.localidad_id >= 0 && ParticipanteConcurso.categoria_id >= 0 && ParticipanteConcurso.nivel_id >= 0 && ParticipanteConcurso.segmento_id >= 0).ToList();
        }
        
        public List<ParticipanteConcurso> ListarParticipanteConcursoesPorId(int id)
        {
            return tabla.ParticipanteConcursoes.Include("Concurso").Include("Segmento").Include("Canal").Include("Nivel").Include("Localidad").Include("Zona").Include("Participante").Include("Categoria").Include("Compania").Include("JerarquiaDetalle").Where(ParticipanteConcurso => ParticipanteConcurso.id == id && ParticipanteConcurso.id > 0 && ParticipanteConcurso.canal_id >= 0 && ParticipanteConcurso.zona_id >= 0 && ParticipanteConcurso.localidad_id >= 0 && ParticipanteConcurso.categoria_id >= 0 && ParticipanteConcurso.nivel_id >= 0 && ParticipanteConcurso.segmento_id >= 0).ToList();
        }


        public int InsertarParticipanteConcurso(ParticipanteConcurso participanteconcurso, string Username)
        {
            int resultado = 0;
            
            var tipoConcurso = (from c in tabla.Concursoes                                
                                where (c.id == participanteconcurso.concurso_id)
                                select c.tipoConcurso_id).First();            
            
            if (tipoConcurso == 1)
            {
                if (tabla.ParticipanteConcursoes.Where(ParticipanteConcurso => ParticipanteConcurso.categoria_id == participanteconcurso.categoria_id && ParticipanteConcurso.participante_id == participanteconcurso.participante_id &&
                    ParticipanteConcurso.segmento_id == participanteconcurso.segmento_id && ParticipanteConcurso.canal_id == participanteconcurso.canal_id && ParticipanteConcurso.nivel_id == participanteconcurso.nivel_id && ParticipanteConcurso.localidad_id == participanteconcurso.localidad_id
                    && ParticipanteConcurso.zona_id == participanteconcurso.zona_id && ParticipanteConcurso.compania_id == participanteconcurso.compania_id && ParticipanteConcurso.concurso_id == participanteconcurso.concurso_id).ToList().Count() == 0)
                {
                    tabla.ParticipanteConcursoes.AddObject(participanteconcurso);
                    tabla.SaveChanges();
                    resultado = participanteconcurso.id;
                }
            }

            else if (tipoConcurso == 2)
            {
                if (tabla.ParticipanteConcursoes.Where(ParticipanteConcurso => ParticipanteConcurso.categoria_id == participanteconcurso.categoria_id && ParticipanteConcurso.jerarquiaDetalle_id == participanteconcurso.jerarquiaDetalle_id &&
                    ParticipanteConcurso.segmento_id == participanteconcurso.segmento_id && ParticipanteConcurso.canal_id == participanteconcurso.canal_id && ParticipanteConcurso.nivel_id == participanteconcurso.nivel_id && ParticipanteConcurso.localidad_id == participanteconcurso.localidad_id
                    && ParticipanteConcurso.zona_id == participanteconcurso.zona_id && ParticipanteConcurso.compania_id == participanteconcurso.compania_id && ParticipanteConcurso.concurso_id == participanteconcurso.concurso_id).ToList().Count() == 0)
                {
                    tabla.ParticipanteConcursoes.AddObject(participanteconcurso);
                    tabla.SaveChanges();
                    resultado = participanteconcurso.id;
                } 
            }
            new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Creacion, DateTime.Now, Username, tablasAuditadas.ParticipanteConcurso,
    SegmentosInsercion.Personas_Y_Pymes, null, tipoConcurso);
            return resultado;
        }

        public int ActualizarParticipanteConcurso(int id, ParticipanteConcurso participanteconcurso, string Username)
        {
            var participanteconcursoActual = this.tabla.ParticipanteConcursoes.Where(ParticipanteConcurso => ParticipanteConcurso.id == id && ParticipanteConcurso.id > 0 && ParticipanteConcurso.canal_id >= 0 && ParticipanteConcurso.zona_id >= 0 && ParticipanteConcurso.localidad_id >= 0 && ParticipanteConcurso.categoria_id >= 0 && ParticipanteConcurso.nivel_id >= 0 && ParticipanteConcurso.segmento_id >= 0).First();
            var pValorAntiguo = participanteconcursoActual;
            participanteconcursoActual.segmento_id = participanteconcurso.segmento_id;
            participanteconcursoActual.canal_id = participanteconcurso.canal_id;
            participanteconcursoActual.nivel_id = participanteconcurso.nivel_id;
            participanteconcursoActual.localidad_id = participanteconcurso.localidad_id;
            participanteconcursoActual.zona_id = participanteconcurso.zona_id;
            participanteconcursoActual.participante_id = participanteconcurso.participante_id;
            participanteconcursoActual.categoria_id = participanteconcurso.categoria_id;
            participanteconcursoActual.compania_id = participanteconcurso.compania_id;
           new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Modificacion, DateTime.Now, Username, tablasAuditadas.ParticipanteConcurso,
    SegmentosInsercion.Personas_Y_Pymes, pValorAntiguo, participanteconcursoActual);

            return tabla.SaveChanges();
        }

        public string EliminarParticipanteConcurso(int id, ParticipanteConcurso participanteconcurso, string Username)
        {
            var participanteconcursoActual = this.tabla.ParticipanteConcursoes.Where(ParticipanteConcurso => ParticipanteConcurso.id == id && ParticipanteConcurso.id > 0).First();
            tabla.DeleteObject(participanteconcursoActual);
            try
            {
                new Componentes.Utilidades.Auditoria().InsertarAuditoria(tipoEventoTabla.Eliminacion, DateTime.Now, Username, tablasAuditadas.ParticipanteConcurso,
    SegmentosInsercion.Personas_Y_Pymes, participanteconcursoActual, null);
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




