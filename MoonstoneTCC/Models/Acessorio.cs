using System.ComponentModel.DataAnnotations;

namespace MoonstoneTCC.Models
{
    public class Acessorio
    {
        public int AcessorioId { get; set; }

        [Required, StringLength(100)]
        public string Nome { get; set; }

        [StringLength(300)]
        public string DescricaoCurta { get; set; }

        public string DescricaoDetalhada { get; set; }

        [Required, DataType(DataType.Currency)]
        public decimal Preco { get; set; }

        public decimal? PrecoPromocional { get; set; }

        public string ImagemUrl { get; set; }
        public string ImagemThumbnailUrl { get; set; }
        public string ImagensAdicionais { get; set; }
        public string ImagensAdicionais2 { get; set; }
        public string ImagensAdicionais3 { get; set; }

        // Especificações Técnicas
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Cor { get; set; }
        public string Tipo { get; set; } // Fone, Controle, etc
        public string Conectividade { get; set; } // Bluetooth, USB, etc
        public string Compatibilidade { get; set; } // PC, PS5, Xbox, etc
        public string Material { get; set; } // Plástico ABS, Alumínio...
        public string Peso { get; set; } // "200g"
        public string Dimensoes { get; set; } // "10x5x3 cm"
        public string Bateria { get; set; } // "10h", "Sem Bateria"

        public string Garantia { get; set; } // "1 ano de garantia"
        public string ItensInclusos { get; set; } // "Cabo, Manual, Estojo"

        public string InformacoesTecnicas { get; set; }
        public string InformacoesExtras { get; set; }
        public string ClassificacaoUso { get; set; } // Gamer, Profissional, Casual

        public int QuantidadeEstoque { get; set; }
        public int EstoqueMinimoAlerta { get; set; }
        public bool EmEstoque => QuantidadeEstoque > 0;

        public bool IsAcessorioDestaque { get; set; }
        public bool ProdutoNovo { get; set; } // Exibir selo de "Novo"
        public bool ProdutoMaisVendido { get; set; } // Selo "Top Vendas"

        public double? AvaliacaoMedia { get; set; } // calculada por reviews
        public int TotalAvaliacoes { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Categoria
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
    }


}
