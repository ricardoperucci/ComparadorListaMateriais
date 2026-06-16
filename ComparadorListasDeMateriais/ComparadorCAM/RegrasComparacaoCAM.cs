using ModelagemTorre;
using System.Collections.Generic;
using System.Linq;

namespace ComparadorListasDeMateriais.ComparadorCAM
{
    /// <summary>
    /// Regras puras de verificação de arquivos CAM, <b>sem dependência de UI, Office ou da leitura de arquivos</b>.
    /// <para>
    /// São o coração testável da comparação CAM (TDD): recebem dados simples (diâmetros, materiais, chaves)
    /// e devolvem resultados determinísticos, fáceis de cobrir com testes unitários.
    /// </para>
    /// </summary>
    public static class RegrasComparacaoCAM
    {
        /// <summary>
        /// Retorna a lista (distinta) de diâmetros de furo que estão FORA do padrão da fábrica informada.
        /// Lista vazia significa que todos os furos estão dentro do padrão.
        /// </summary>
        public static List<double> FurosForaDoPadrao(IEnumerable<double> pDiametrosFuros, FabricanteEnum pFabricante)
        {
            List<double> diametrosPadrao = EstaticosComparacaoCAM.GetListaDiametrosPadrao(pFabricante);

            return (pDiametrosFuros ?? Enumerable.Empty<double>())
                .Where(diametro => !diametrosPadrao.Contains(diametro))
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Indica se a cantoneira está SEM aço grau 60, ou seja, o material não contém "GR60".
        /// </summary>
        public static bool CantoneiraSemAcoG(string pMaterialCAM)
        {
            return string.IsNullOrEmpty(pMaterialCAM) || !pMaterialCAM.Contains("GR60");
        }

        /// <summary>
        /// Dadas as chaves (posições) de cada pasta, calcula quais posições estão ausentes em cada uma.
        /// </summary>
        /// <param name="pChavesA">Posições presentes na pasta A.</param>
        /// <param name="pChavesB">Posições presentes na pasta B.</param>
        /// <param name="pFaltandoNaA">Saída: posições que existem em B mas não em A.</param>
        /// <param name="pFaltandoNaB">Saída: posições que existem em A mas não em B.</param>
        public static void PosicoesFaltando(
            IEnumerable<string> pChavesA,
            IEnumerable<string> pChavesB,
            out List<string> pFaltandoNaA,
            out List<string> pFaltandoNaB)
        {
            var conjuntoA = new HashSet<string>(pChavesA ?? Enumerable.Empty<string>());
            var conjuntoB = new HashSet<string>(pChavesB ?? Enumerable.Empty<string>());

            pFaltandoNaA = conjuntoB.Where(x => !conjuntoA.Contains(x)).ToList();
            pFaltandoNaB = conjuntoA.Where(x => !conjuntoB.Contains(x)).ToList();
        }
    }
}
