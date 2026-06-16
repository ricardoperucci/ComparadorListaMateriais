using System;
using System.IO;
using System.Linq;

namespace ComparadorListasDeMateriais.Testes.Infra
{
    /// <summary>
    /// Localiza a pasta <c>DadosTeste</c> (copiada para a saída do build) e ajuda os testes de
    /// integração a decidir se há instâncias de exemplo para rodar ou se devem ser marcados como
    /// inconclusivos (Assert.Inconclusive) — assim a suíte fica "verde" mesmo antes de você
    /// colocar os arquivos de exemplo.
    /// </summary>
    public static class CaminhosTeste
    {
        public static string RaizDados => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DadosTeste");

        public static string Pasta(params string[] subpastas)
        {
            string caminho = RaizDados;
            foreach (string parte in subpastas)
                caminho = Path.Combine(caminho, parte);
            return caminho;
        }

        /// <summary>Arquivos (ignorando os LEIAME) existentes na pasta indicada.</summary>
        public static string[] Arquivos(string pasta, string padrao = "*.*")
        {
            if (!Directory.Exists(pasta))
                return new string[0];

            return Directory.GetFiles(pasta, padrao)
                .Where(arquivo => !Path.GetFileName(arquivo).StartsWith("LEIAME", StringComparison.OrdinalIgnoreCase))
                .OrderBy(arquivo => arquivo)
                .ToArray();
        }

        public static bool TemDados(string pasta, string padrao = "*.*") => Arquivos(pasta, padrao).Length > 0;
    }
}
