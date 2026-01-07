namespace MoonstoneTCC.Models
{
    public class EtapaProcessoSeletivo
    {
        public int Id { get; set; }

        public int CandidaturaId { get; set; }
        public Candidatura Candidatura { get; set; }

        public string NomeEtapa { get; set; } // Ex: "Entrevista agendada"
        public string Observacoes { get; set; } // Comentários do admin (opcional)
        public DateTime Data { get; set; } // Quando foi adicionada a etapa
    }
}
