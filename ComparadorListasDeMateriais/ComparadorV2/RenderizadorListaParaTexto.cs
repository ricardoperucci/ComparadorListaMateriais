using System.Collections.Generic;
using System.Linq;

namespace ComparadorListasDeMateriais.ComparadorV2
{
    /// <summary>Texto de uma lista pronto para o lado a lado, junto com a origem dele.</summary>
    public class TextoLista
    {
        public List<string> Linhas { get; set; } = new List<string>();

        /// <summary>
        /// True quando o texto não é o arquivo original e sim uma renderização (caso do Excel,
        /// que não tem "texto cru"). O relatório avisa quando isso acontece.
        /// </summary>
        public bool Renderizado { get; set; }
    }

    /// <summary>
    /// Obtém o texto de uma lista para exibição lado a lado.
    /// <para>
    /// Para .txt é o conteúdo do arquivo, lido com o mesmo tratamento de encoding do resto do
    /// programa. Para .xls/.xlsx não existe texto cru, então as linhas da aba MATERIAIS são
    /// renderizadas como texto tabulado.
    /// </para>
    /// </summary>
    public static class RenderizadorListaParaTexto
    {
        public static TextoLista Obter(string pCaminho)
        {
            string caminho = pCaminho ?? string.Empty;

            if (caminho.ToLower().EndsWith(".txt"))
            {
                return new TextoLista
                {
                    Linhas = FuncoesUteis.DivideTextoPorLinhas(FuncoesUteis.LerTxtLisaMateriais(caminho)),
                    Renderizado = false
                };
            }

            if (caminho.ToLower().Contains(".xls"))
            {
                List<List<string>>[] abas = FuncoesUteis.LerExcelVariasAbas(caminho, new[] { "MATERIAIS" });

                return new TextoLista
                {
                    Linhas = RenderizarLinhasExcel(abas[0]),
                    Renderizado = true
                };
            }

            return new TextoLista();
        }

        /// <summary>Junta as células de cada linha do Excel numa string estável, para o diff comparar.</summary>
        private static List<string> RenderizarLinhasExcel(List<List<string>> pLinhas)
        {
            if (pLinhas == null)
                return new List<string>();

            return pLinhas
                .Select(linha => string.Join(" | ", (linha ?? new List<string>()).Select(c => (c ?? string.Empty).Trim())))
                .ToList();
        }
    }
}
