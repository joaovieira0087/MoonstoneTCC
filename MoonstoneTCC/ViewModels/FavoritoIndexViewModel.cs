namespace MoonstoneTCC.ViewModels
{
    public class FavoritoIndexViewModel
    {
        public List<FavoritoViewModel> Favoritos { get; set; } = new();
        public string? FiltroOrdem { get; set; }
        public string? FiltroVisibilidade { get; set; }
        public string? FiltroPlataforma { get; set; }
        public string? FiltroTag { get; set; }
        public List<string> PlataformasDisponiveis { get; set; } = new();
    }


}
