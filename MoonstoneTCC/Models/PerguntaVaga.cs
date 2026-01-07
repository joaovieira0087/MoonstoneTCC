namespace MoonstoneTCC.Models
{
    public class PerguntaVaga
    {
        public int Id { get; set; }
        public string Texto { get; set; }
        public int VagaEmpregoId { get; set; }
        public VagaEmprego Vaga { get; set; }
    }

}
