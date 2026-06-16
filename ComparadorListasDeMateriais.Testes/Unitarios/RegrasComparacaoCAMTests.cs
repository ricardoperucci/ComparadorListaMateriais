using ComparadorListasDeMateriais.ComparadorCAM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelagemTorre;
using System.Collections.Generic;
using System.Linq;

namespace ComparadorListasDeMateriais.Testes.Unitarios
{
    /// <summary>
    /// Testes das REGRAS PURAS da comparação CAM. Dependem apenas do enum FabricanteEnum
    /// (ModelagemTorre) e dos diâmetros padrão de <see cref="EstaticosComparacaoCAM"/> — não
    /// carregam a HefestoCAM nem leem arquivos.
    /// </summary>
    [TestClass]
    public class RegrasComparacaoCAMTests
    {
        [TestMethod]
        public void FurosForaDoPadrao_TodosNoPadrao_RetornaVazio()
        {
            // Padrão (não-Brametal): 14.3, 17.5, 20.6
            var fora = RegrasComparacaoCAM.FurosForaDoPadrao(new[] { 14.3, 17.5 }, FabricanteEnum.CAW);
            Assert.AreEqual(0, fora.Count);
        }

        [TestMethod]
        public void FurosForaDoPadrao_DiametroDiferente_RetornaApenasOForaDoPadrao()
        {
            var fora = RegrasComparacaoCAM.FurosForaDoPadrao(new[] { 14.3, 99.9 }, FabricanteEnum.CAW);
            CollectionAssert.AreEquivalent(new List<double> { 99.9 }, fora);
        }

        [TestMethod]
        public void FurosForaDoPadrao_RemoveDuplicados()
        {
            var fora = RegrasComparacaoCAM.FurosForaDoPadrao(new[] { 99.9, 99.9 }, FabricanteEnum.CAW);
            Assert.AreEqual(1, fora.Count);
        }

        [TestMethod]
        public void FurosForaDoPadrao_Brametal_UsaPadraoProprio()
        {
            // Brametal: 14.7, 18, 21
            Assert.AreEqual(0, RegrasComparacaoCAM.FurosForaDoPadrao(new[] { 14.7, 18.0 }, FabricanteEnum.Brametal).Count);
            CollectionAssert.AreEquivalent(new List<double> { 13.0 },
                RegrasComparacaoCAM.FurosForaDoPadrao(new[] { 14.7, 13.0 }, FabricanteEnum.Brametal));
        }

        [TestMethod]
        public void CantoneiraSemAcoG_ComGR60_False()
        {
            Assert.IsFalse(RegrasComparacaoCAM.CantoneiraSemAcoG("ASTM A572 GR60"));
        }

        [TestMethod]
        public void CantoneiraSemAcoG_SemGR60_True()
        {
            Assert.IsTrue(RegrasComparacaoCAM.CantoneiraSemAcoG("ASTM A36"));
            Assert.IsTrue(RegrasComparacaoCAM.CantoneiraSemAcoG(null));
        }

        [TestMethod]
        public void PosicoesFaltando_CalculaAusentesEmCadaPasta()
        {
            RegrasComparacaoCAM.PosicoesFaltando(
                new[] { "1", "2" },
                new[] { "2", "3" },
                out List<string> faltandoNaA,
                out List<string> faltandoNaB);

            CollectionAssert.AreEquivalent(new List<string> { "3" }, faltandoNaA);
            CollectionAssert.AreEquivalent(new List<string> { "1" }, faltandoNaB);
        }
    }
}
