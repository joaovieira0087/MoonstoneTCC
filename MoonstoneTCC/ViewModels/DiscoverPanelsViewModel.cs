using System.Collections.Generic;

namespace MoonstoneTCC.ViewModels
{
    public class DiscoverCardDTO
    {
        public int JogoId { get; set; }
        public string Nome { get; set; } = "";
        public string ImagemUrl { get; set; } = "/img/placeholder.jpg"; // ajuste se quiser
        public decimal Preco { get; set; }
        public decimal? PrecoPromocional { get; set; }
        public string? Link { get; set; } // se vier null, front usa /Jogo/Details?jogoId=...
        public bool IsUtility { get; set; } = false; // para “Veja favoritos”, “Meios de pagamento”, “Minha Carteira”
        public string? UtilitySubtitle { get; set; }  // descrição curta nos utilitários
    }

    public class DiscoverPanelVM
    {
        public string Key { get; set; } = "";
        public string Titulo { get; set; } = "";
        public List<DiscoverCardDTO> Itens { get; set; } = new();
    }

    public class DiscoverRowVM
    {
        public string RowId { get; set; } = "row-descoberta";
        public List<DiscoverPanelVM> Colunas { get; set; } = new();
        public int ItensPorFileira { get; set; } = 5; // fixo: 5
    }
}
