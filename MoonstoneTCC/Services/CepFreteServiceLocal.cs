using System.Linq;

namespace MoonstoneTCC.Services
{
    /// Frete local por região (macro) baseada na 1ª casa do CEP.
    /// 0-1: SP | 2: RJ/ES | 3: MG | 4: BA/SE | 5: PE/AL/PB/RN
    /// 6: Norte/Nordeste (CE/PI/MA/PA/AP/AM/RO/AC/RR)
    /// 7: Centro-Oeste (DF/GO/TO/MT/MS)
    /// 8: Sul (PR/SC) | 9: RS
    public class CepFreteServiceLocal : ICepFreteService
    {
        public CotacaoFrete Calcular(string cep, decimal subtotal, int quantidadeItens)
        {
            var digits = SomenteDigitos(cep);
            if (digits.Length < 8) return new CotacaoFrete(39.90m, 7, "Nacional", "CEP inválido – usando padrão.");

            int d = digits[0] - '0';
            decimal baseValor; int prazo; string regiao;

            switch (d)
            {
                case 0:
                case 1:
                    regiao = "SP"; baseValor = 19.90m; prazo = 2; break;
                case 2:
                    regiao = "RJ/ES"; baseValor = 24.90m; prazo = 3; break;
                case 3:
                    regiao = "MG"; baseValor = 24.90m; prazo = 3; break;
                case 4:
                    regiao = "BA/SE"; baseValor = 29.90m; prazo = 4; break;
                case 5:
                    regiao = "PE/AL/PB/RN"; baseValor = 29.90m; prazo = 4; break;
                case 6:
                    regiao = "Norte/Nordeste"; baseValor = 39.90m; prazo = 6; break;
                case 7:
                    regiao = "Centro-Oeste"; baseValor = 34.90m; prazo = 5; break;
                case 8:
                    regiao = "Sul (PR/SC)"; baseValor = 26.90m; prazo = 3; break;
                case 9:
                    regiao = "RS"; baseValor = 26.90m; prazo = 3; break;
                default:
                    regiao = "Nacional"; baseValor = 39.90m; prazo = 7; break;
            }

            // Regras simples (ajuste à vontade):
            // + R$2,50 por item adicional
            var adicional = Math.Max(0, quantidadeItens - 1) * 2.50m;
            var valor = baseValor + adicional;

            // Frete grátis acima de R$ 250
            if (subtotal >= 250m) valor = 0m;

            return new CotacaoFrete(decimal.Round(valor, 2), prazo, regiao);
        }

        private static string SomenteDigitos(string s) => new string(s.Where(char.IsDigit).ToArray());
    }
}
