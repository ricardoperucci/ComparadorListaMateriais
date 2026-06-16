using ComparadorListasDeMateriais;
using ComparadorListasDeMateriais.ObjetosResultados;
using ComparadorListasDeMateriais.Testes.Infra;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ComparadorListasDeMateriais.Testes.Integracao
{
    /// <summary>
    /// Testes ponta a ponta da comparação de listas a partir de arquivos reais.
    /// <list type="bullet">
    ///   <item>Listas .txt: coloque 2 arquivos em <c>DadosTeste\Listas</c> (rodam sem Excel).</item>
    ///   <item>Listas .xlsx: coloque 2 arquivos em <c>DadosTeste\Excel</c> (precisam do Excel instalado).</item>
    /// </list>
    /// </summary>
    [TestClass]
    public class ComparadorListasArquivosIntegracaoTests
    {
        [TestMethod]
        [TestCategory("Integracao")]
        public void CompararListasTxt_GeraResultadoSemLancar()
        {
            var arquivos = CaminhosTeste.Arquivos(CaminhosTeste.Pasta("Listas"), "*.txt");
            if (arquivos.Length < 2)
                Assert.Inconclusive(@"Coloque 2 listas .txt em DadosTeste\Listas (ver LEIAME.md).");

            var comparador = new Comparador();
            bool ok = comparador.CompararListas(arquivos[0], arquivos[1], out ObjetoResultadoComparacao resultado);

            Assert.IsTrue(ok);
            Assert.IsNotNull(resultado);
            Assert.IsNotNull(resultado.ListaEstruturasComparadas);
        }

        [TestMethod]
        [TestCategory("Integracao")]
        [TestCategory("RequerExcel")]
        public void CompararListasExcel_GeraResultadoSemLancar()
        {
            var arquivos = CaminhosTeste.Arquivos(CaminhosTeste.Pasta("Excel"), "*.xls*");
            if (arquivos.Length < 2)
                Assert.Inconclusive(@"Coloque 2 listas .xls/.xlsx em DadosTeste\Excel (requer Excel instalado; ver LEIAME.md).");

            var comparador = new Comparador();
            bool ok = comparador.CompararListas(arquivos[0], arquivos[1], out ObjetoResultadoComparacao resultado);

            Assert.IsTrue(ok);
            Assert.IsNotNull(resultado);
        }
    }
}
