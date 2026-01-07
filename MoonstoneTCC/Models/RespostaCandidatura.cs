namespace MoonstoneTCC.Models
{
    public class RespostaCandidatura
    {
        public int Id { get; set; }
        public int CandidaturaId { get; set; }
        public Candidatura Candidatura { get; set; }

        public int PerguntaVagaId { get; set; }
        public PerguntaVaga Pergunta { get; set; }

        public string RespostaTexto { get; set; }
    }

}
