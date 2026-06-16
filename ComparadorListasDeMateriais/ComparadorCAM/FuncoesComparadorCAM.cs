using ModelagemTorre;
using ModelagemTorre.ObjetosCore;
using System.Reflection;
using System.Windows.Forms;

namespace ComparadorListasDeMateriais.ComparadorCAM
{
    /// <summary>
    /// Fachada da comparação de arquivos CAM usada pela UI.
    /// <para>
    /// Faz apenas a "fiação" entre o leitor real (<see cref="LeitorPastaCAMHefesto"/>),
    /// a orquestração (<see cref="ComparadorCAMService"/>) e o relatório (<see cref="RelatorioCAMTxt"/>).
    /// Toda a regra de negócio testável vive nessas classes; aqui só montamos o pipeline.
    /// </para>
    /// </summary>
    public static class FuncoesComparadorCAM
    {
        /// <summary>
        /// Compara os arquivos CAM de duas pastas e devolve o texto do relatório.
        /// </summary>
        public static string ComparaArquivosCAM(string pCaminhoPastaA, string pCaminhoPastaB, OpcoesComparacaoCAM pOpcoes)
        {
            // Configura o ParafusoController para o fabricante/tipo informado (mapeamento de furos Engetower -> fábrica).
            ConfigurarParafusoController(pOpcoes.ParafusoTipo, pOpcoes.FabricanteA);

            var service = new ComparadorCAMService(new LeitorPastaCAMHefesto());

            ResultadoComparacaoCAM resultado = service.Comparar(pCaminhoPastaA, pCaminhoPastaB, pOpcoes);

            return RelatorioCAMTxt.Gerar(resultado);
        }

        /// <summary>
        /// Atualiza o ParafusoController (singleton) para o tipo/fabricante informado.
        /// <para>
        /// OBS (refat2026 / ponto de integração #1): <c>AtualizaParafusoController</c> é declarado
        /// <c>internal</c> em ModelagemTorre.ObjetosCore. Para o build NÃO depender de
        /// <c>[InternalsVisibleTo("ComparadorListasDeMateriais")]</c> existir na DLL, a chamada é feita
        /// por reflexão (compila sempre). Se/quando o método virar público, troque por:
        /// <c>ParafusoController.Instance.AtualizaParafusoController(pTipo, pFabricante);</c>
        /// </para>
        /// </summary>
        private static void ConfigurarParafusoController(ParafusoTipoEnum pTipo, FabricanteEnum pFabricante)
        {
            ParafusoController instancia = ParafusoController.Instance;

            MethodInfo metodo = instancia.GetType().GetMethod(
                "AtualizaParafusoController",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            metodo?.Invoke(instancia, new object[] { pTipo, pFabricante });
        }

        /// <summary>
        /// Pede a seleção de uma pasta ao usuário (helper de UI).
        /// </summary>
        public static string SelecionaPasta(string pTexto)
        {
            using (var fbd = new FolderBrowserDialog { Description = $"Selecione a pasta com os {pTexto}" })
            {
                DialogResult result = fbd.ShowDialog();

                return (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    ? fbd.SelectedPath
                    : "erro";
            }
        }
    }
}
