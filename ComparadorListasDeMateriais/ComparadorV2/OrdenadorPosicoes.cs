using System.Collections.Generic;
using System.Linq;

namespace ComparadorListasDeMateriais.ComparadorV2
{
    /// <summary>
    /// Ordena numerações de posição de forma natural: 2, 10, 65G, 65H, 107F — e não 10, 107F, 2, 65G,
    /// que é o que a ordenação alfabética faria.
    /// </summary>
    public static class OrdenadorPosicoes
    {
        public static List<string> Ordenar(IEnumerable<string> pPosicoes)
        {
            return (pPosicoes ?? Enumerable.Empty<string>())
                .OrderBy(ParteNumerica)
                .ThenBy(x => x, System.StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static List<T> Ordenar<T>(IEnumerable<T> pItens, System.Func<T, string> pNumeracao)
        {
            return (pItens ?? Enumerable.Empty<T>())
                .OrderBy(x => ParteNumerica(pNumeracao(x)))
                .ThenBy(x => pNumeracao(x), System.StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <summary>Prefixo numérico da posição, usado como chave primária de ordenação.</summary>
        private static int ParteNumerica(string pPosicao)
        {
            if (string.IsNullOrWhiteSpace(pPosicao))
                return int.MaxValue;

            string digitos = new string(pPosicao.TrimStart().TakeWhile(char.IsDigit).ToArray());

            return digitos.Length > 0 && int.TryParse(digitos, out int valor) ? valor : int.MaxValue;
        }
    }
}
