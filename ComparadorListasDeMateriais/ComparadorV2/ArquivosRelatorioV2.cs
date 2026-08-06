using System.IO;
using System.Text;

namespace ComparadorListasDeMateriais.ComparadorV2
{
    /// <summary>
    /// Grava os relatórios da v2 em disco.
    /// <para>
    /// Sempre em UTF-8 <b>com BOM</b>: o relatório usa ═ ─ ■ → ≤, que não existem em Windows-1252,
    /// e o BOM garante que Bloco de Notas e Excel abram o arquivo com os acentos certos.
    /// </para>
    /// </summary>
    public static class ArquivosRelatorioV2
    {
        private static readonly UTF8Encoding Utf8ComBom = new UTF8Encoding(true);

        /// <summary>Grava o relatório .txt e devolve o caminho gravado.</summary>
        public static string EscreverTxt(string pCaminhoSemExtensao, ResultadoComparacaoV2 pResultado)
        {
            string caminho = pCaminhoSemExtensao + ".txt";

            File.WriteAllText(caminho, RelatorioV2Txt.Gerar(pResultado), Utf8ComBom);

            return caminho;
        }

        /// <summary>Grava o relatório .html e devolve o caminho gravado.</summary>
        public static string EscreverHtml(
            string pCaminhoSemExtensao,
            ResultadoComparacaoV2 pResultado,
            TextoLista pTextoOriginal,
            TextoLista pTextoNova)
        {
            string caminho = pCaminhoSemExtensao + ".html";

            File.WriteAllText(caminho, RelatorioV2Html.Gerar(pResultado, pTextoOriginal, pTextoNova), Utf8ComBom);

            return caminho;
        }
    }
}
