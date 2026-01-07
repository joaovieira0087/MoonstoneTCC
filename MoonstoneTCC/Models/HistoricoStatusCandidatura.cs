namespace MoonstoneTCC.Models
{
    public class HistoricoStatusCandidatura
    {
        public int Id { get; set; }
        public int CandidaturaId { get; set; }
        public Candidatura Candidatura { get; set; }

        public string DeStatus { get; set; }
        public string ParaStatus { get; set; }

        public DateTime DataHora { get; set; }


    }

}
