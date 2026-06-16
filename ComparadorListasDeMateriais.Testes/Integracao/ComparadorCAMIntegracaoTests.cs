using ComparadorListasDeMateriais.ComparadorCAM;
using ComparadorListasDeMateriais.Testes.Infra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelagemTorre;

namespace ComparadorListasDeMateriais.Testes.Integracao
{
    /// <summary>
    /// Teste ponta a ponta da comparação CAM, lendo arquivos .CAM reais via a HefestoCAM.
    /// <para>
    /// Coloque os arquivos de exemplo em <c>DadosTeste\CAM\PastaA</c> e <c>DadosTeste\CAM\PastaB</c>.
    /// Enquanto não houver arquivos, o teste fica inconclusivo (não falha).
    /// </para>
    /// </summary>
    [TestClass]
    public class ComparadorCAMIntegracaoTests
    {
        [TestMethod]
        [TestCategory("Integracao")]
        public void CompararCAM_DuasPastas_GeraRelatorio()
        {
            string pastaA = CaminhosTeste.Pasta("CAM", "PastaA");
            string pastaB = CaminhosTeste.Pasta("CAM", "PastaB");

            if (!CaminhosTeste.TemDados(pastaA, "*.CAM") || !CaminhosTeste.TemDados(pastaB, "*.CAM"))
                Assert.Inconclusive(@"Coloque arquivos .CAM em DadosTeste\CAM\PastaA e DadosTeste\CAM\PastaB (ver LEIAME.md).");

            var opcoes = new OpcoesComparacaoCAM(
                FabricanteEnum.CAW, FabricanteEnum.CAW, ParafusoTipoEnum.A394CM, considerarDiametroFuros: false);

            // Passa pela fachada (que configura o ParafusoController internamente).
            string relatorio = FuncoesComparadorCAM.ComparaArquivosCAM(pastaA, pastaB, opcoes);

            Assert.IsFalse(string.IsNullOrWhiteSpace(relatorio));
            StringAssert.Contains(relatorio, "RELATÓRIO COMPARAÇÂO ARQUIVOS CAM");
        }
    }
}
