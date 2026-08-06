using ComparadorListasDeMateriais.ObjetosResultados;
using ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias;
using System.Globalization;

namespace ComparadorListasDeMateriais.ComparadorV2
{
    /// <summary>Uma divergência já traduzida para rótulo e valores prontos para exibição.</summary>
    public class DivergenciaFormatada
    {
        public string Rotulo { get; set; } = string.Empty;

        public string ValorOriginal { get; set; } = string.Empty;
        public string ValorNovo { get; set; } = string.Empty;

        /// <summary>Variação, quando faz sentido (ex.: "+18,0 mm"). Vazio quando não se aplica.</summary>
        public string Delta { get; set; } = string.Empty;

        public bool TemValores => ValorOriginal.Length > 0 || ValorNovo.Length > 0;
        public bool TemDelta => Delta.Length > 0;
    }

    /// <summary>
    /// Normaliza a exibição das divergências.
    /// <para>
    /// As 17 classes de <see cref="ErroPosicao"/> escrevem cada uma no seu formato
    /// ("Comprimento: 18mm =&gt; 1250 / 1268" vs. "Espessura: 4,75/6,35"). Este formatador traduz
    /// todas para a mesma estrutura (rótulo, original, novo, delta), sem alterar as classes
    /// compartilhadas com a aba antiga.
    /// </para>
    /// </summary>
    public static class FormatadorDivergenciaV2
    {
        private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");

        public static DivergenciaFormatada Formatar(ErroPosicao pErro)
        {
            switch (pErro)
            {
                case DivergenciaComprimento e:
                    return Medida("Comprimento", e.ValorOriginal, e.ValorNovo);

                case DivergenciaLargura e:
                    return Medida("Largura", e.ValorOriginal, e.ValorNovo);

                case DivergenciaEspessura e:
                    return Medida("Espessura", e.ValorOriginal, e.ValorNovo);

                case DivergenciaDiametroCalota e:
                    return Medida("Diâmetro da calota", e.ValorOriginal, e.ValorNovo);

                case DivergenciaFuroCalota e:
                    return Medida("Furo da calota", e.ValorOriginal, e.ValorNovo);

                case DivergenciaRaioUsinagemCalota e:
                    return Medida("Raio de usinagem", e.ValorOriginal, e.ValorNovo);

                case DivergenciaQuantidadePecas e:
                    return Contagem("Quantidade", e.ValorOriginal, e.ValorNovo);

                case DivergenciaQuantidadeRecortes e:
                    return Contagem("Recortes", e.ValorOriginal, e.ValorNovo);

                case DivergenciaQuantidadeDobras e:
                    return Contagem("Dobras", e.ValorOriginal, e.ValorNovo);

                case DivergenciaQuantidadeChanfros e:
                    return Contagem("Chanfros", e.ValorOriginal, e.ValorNovo);

                case DivergenciaPerfilCantoneira e:
                    return Texto("Perfil", e.ValorOriginal, e.ValorNovo);

                case DivergenciaConjuntoSoldado e:
                    return Texto("Conjunto soldado", e.ValorOriginal, e.ValorNovo);

                case DivergenciaEsquerda e:
                    return Texto("Peça direita", e.NumeracaoDireitaOriginal, e.NumeracaoDireitaNova);

                case DivergenciaDegrau e:
                    return Texto("Parafuso degrau", SimNao(e.TemDegrauOriginal), SimNao(e.TemDegrauNovo));

                case DivergenciaMaterial _:
                    return new DivergenciaFormatada { Rotulo = "Material" };

                case DivergenciaPecasTipo _:
                    return new DivergenciaFormatada { Rotulo = "Tipo de peça", ValorOriginal = "cantoneira/chapa diferentes" };

                default:
                    return new DivergenciaFormatada { Rotulo = pErro.EscreveErroTxt() };
            }
        }

        /// <summary>Linha compacta de uma divergência: "Comprimento 1250,0 → 1268,0 (+18,0 mm)".</summary>
        public static string LinhaCompacta(ErroPosicao pErro)
        {
            DivergenciaFormatada f = Formatar(pErro);

            if (!f.TemValores)
                return f.Rotulo;

            string texto = f.Rotulo + " " + f.ValorOriginal + " → " + f.ValorNovo;

            return f.TemDelta ? texto + " (" + f.Delta + ")" : texto;
        }

        private static DivergenciaFormatada Medida(string pRotulo, double pOriginal, double pNovo)
        {
            return new DivergenciaFormatada
            {
                Rotulo = pRotulo,
                ValorOriginal = Numero(pOriginal),
                ValorNovo = Numero(pNovo),
                Delta = Delta(pNovo - pOriginal)
            };
        }

        private static DivergenciaFormatada Contagem(string pRotulo, int pOriginal, int pNovo)
        {
            return new DivergenciaFormatada
            {
                Rotulo = pRotulo,
                ValorOriginal = pOriginal.ToString(Cultura),
                ValorNovo = pNovo.ToString(Cultura)
            };
        }

        private static DivergenciaFormatada Texto(string pRotulo, string pOriginal, string pNovo)
        {
            return new DivergenciaFormatada
            {
                Rotulo = pRotulo,
                ValorOriginal = Vazio(pOriginal),
                ValorNovo = Vazio(pNovo)
            };
        }

        private static string Numero(double pValor)
        {
            return pValor.ToString("0.0##", Cultura);
        }

        private static string Delta(double pDelta)
        {
            if (System.Math.Abs(pDelta) < 0.001)
                return string.Empty;

            string sinal = pDelta > 0 ? "+" : "-";

            return sinal + Numero(System.Math.Abs(pDelta)) + " mm";
        }

        private static string SimNao(bool pValor)
        {
            return pValor ? "sim" : "não";
        }

        private static string Vazio(string pValor)
        {
            return string.IsNullOrWhiteSpace(pValor) ? "(vazio)" : pValor.Trim();
        }
    }
}
