namespace MoonstoneTCC.Models
{
    public class MensagemCandidatura
    {
        public int Id { get; set; }

        public int CandidaturaId { get; set; }
        public Candidatura Candidatura { get; set; }

        public string Remetente { get; set; } 
        public string Texto { get; set; }
        public DateTime DataHora { get; set; }
    }

}
