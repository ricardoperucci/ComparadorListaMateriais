using ComparadorListasDeMateriais.ComparadorCAM;
using ComparadorListasDeMateriais.Testes.Infra;
using Hefesto.CAM.ComparacaoArquivos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelagemTorre;
using System.Collections.Generic;

namespace ComparadorListasDeMateriais.Testes.Integracao
{
    /// <summary>
    /// Testa a orquestração de <see cref="ComparadorCAMService"/> através da porta
    /// <see cref="ILeitorPastaCAM"/> (com um leitor falso). Categoria "RequerHefestoCAM" porque
    /// a assinatura usa o tipo <c>ObjetoLeituraCAMTxt</c> (HefestoCAM) — a DLL precisa estar presente
    /// no bin (chega via o ProjectReference do projeto principal).
    /// </summary>
    [TestClass]
    public class ComparadorCAMServiceTests
    {
        [TestMethod]
        [TestCategory("RequerHefestoCAM")]
        public void Comparar_DicionariosVazios_SemDivergencias()
        {
            var service = new ComparadorCAMService(new FakeLeitorPastaCAM());

            var opcoes = new OpcoesComparacaoCAM(FabricanteEnum.CAW, FabricanteEnum.CAW, ParafusoTipoEnum.A394CM, false);

            ResultadoComparacaoCAM resultado = service.Comparar(
                new Dictionary<string, ObjetoLeituraCAMTxt>(),
                new Dictionary<string, ObjetoLeituraCAMTxt>(),
                opcoes,
                "A",
                "B");

            Assert.IsTrue(resultado.TodasPosicoesPresentes);
            Assert.AreEqual(0, resultado.Divergencias.Count);
        }
    }
}
