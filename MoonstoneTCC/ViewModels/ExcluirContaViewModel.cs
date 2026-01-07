namespace MoonstoneTCC.ViewModels
{
    public class ExcluirContaViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public int TotalPedidos { get; set; }
        public decimal TotalGasto { get; set; }
        public DateTime DataCadastro { get; set; }
        public TimeSpan TempoNaPlataforma { get; set; }
        public string MotivoExclusao { get; set; } // <- Usado apenas para exibir, POST não usa ele diretamente
    }
}
