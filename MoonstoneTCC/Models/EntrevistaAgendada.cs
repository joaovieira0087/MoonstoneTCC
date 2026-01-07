namespace MoonstoneTCC.Models
{
    public class EntrevistaAgendada
    {
        public int Id { get; set; }

        public int CandidaturaId { get; set; }
        public Candidatura Candidatura { get; set; }

        public DateTime DataHoraEntrevista { get; set; }

        public string Observacoes { get; set; }
    }

}
