using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ComparadorListasDeMateriais.Utils
{
    /// <summary>
    /// Resultado de uma operação de compactação.
    /// </summary>
    public class ResultadoZip
    {
        public int Sucesso { get; set; }
        public int Erros { get; set; }
        public List<string> ArquivosGerados { get; set; } = new List<string>();
    }

    /// <summary>
    /// Serviço de compactação de arquivos, extraído do code-behind do formulário para ser testável.
    /// <para>Opera apenas sobre o sistema de arquivos (sem UI) — pode ser testado com pastas temporárias.</para>
    /// </summary>
    public static class ZipadorArquivos
    {
        /// <summary>
        /// Caminho do <c>.zip</c> correspondente a um arquivo (troca apenas a última extensão por <c>.zip</c>,
        /// preservando os pontos do nome). Ex.: <c>C:\a.b.cam</c> -> <c>C:\a.b.zip</c>.
        /// </summary>
        public static string CaminhoZipPara(string pArquivo)
        {
            return Path.ChangeExtension(pArquivo, ".zip");
        }

        /// <summary>
        /// Compacta TODOS os arquivos em um único <c>.zip</c>, nomeado a partir do primeiro arquivo.
        /// </summary>
        public static ResultadoZip CriarZipUnico(IEnumerable<string> pArquivos)
        {
            var resultado = new ResultadoZip();

            List<string> lista = (pArquivos ?? Enumerable.Empty<string>()).ToList();
            if (lista.Count == 0)
                return resultado;

            string caminhoZip = CaminhoZipPara(lista.First());

            using (ZipArchive zip = ZipFile.Open(caminhoZip, ZipArchiveMode.Create))
            {
                foreach (string arquivo in lista)
                {
                    try
                    {
                        zip.CreateEntryFromFile(arquivo, Path.GetFileName(arquivo));
                        resultado.Sucesso++;
                    }
                    catch
                    {
                        resultado.Erros++;
                    }
                }
            }

            resultado.ArquivosGerados.Add(caminhoZip);
            return resultado;
        }

        /// <summary>
        /// Compacta cada arquivo em seu próprio <c>.zip</c>.
        /// </summary>
        public static ResultadoZip CriarZipPorArquivo(IEnumerable<string> pArquivos)
        {
            var resultado = new ResultadoZip();

            foreach (string arquivo in pArquivos ?? Enumerable.Empty<string>())
            {
                try
                {
                    string caminhoZip = CaminhoZipPara(arquivo);

                    using (ZipArchive zip = ZipFile.Open(caminhoZip, ZipArchiveMode.Create))
                    {
                        zip.CreateEntryFromFile(arquivo, Path.GetFileName(arquivo));
                    }

                    resultado.Sucesso++;
                    resultado.ArquivosGerados.Add(caminhoZip);
                }
                catch
                {
                    resultado.Erros++;
                }
            }

            return resultado;
        }
    }
}
