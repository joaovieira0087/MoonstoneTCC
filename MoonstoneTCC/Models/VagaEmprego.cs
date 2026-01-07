namespace MoonstoneTCC.Models
{
    public class VagaEmprego
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Cargo { get; set; }
        public string Descricao { get; set; }
        public string Requisitos { get; set; }
        public decimal Salario { get; set; }
        public DateTime DataCriacao { get; set; }

        public ICollection<PerguntaVaga> Perguntas { get; set; }
        public ICollection<Candidatura> Candidaturas { get; set; }
        public DateTime? DataEncerramento { get; set; } // nullable para permitir vaga sem data

    }
}
