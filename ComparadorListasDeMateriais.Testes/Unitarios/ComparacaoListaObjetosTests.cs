using ComparadorListasDeMateriais.ObjetosResultados;
using ComparadorListasDeMateriais.Testes.Infra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ComparadorListasDeMateriais.Testes.Unitarios
{
    /// <summary>
    /// Testes da comparação peça a peça (Chapa/Cantoneira), construindo os objetos por propriedade.
    /// </summary>
    [TestClass]
    public class ComparacaoListaObjetosTests
    {
        [TestMethod]
        public void Chapa_ComparadaComCantoneira_ApontaTipoDiferente()
        {
            var chapa = ConstrutoresTeste.Chapa("1", 100, 50, 6);
            var cantoneira = ConstrutoresTeste.Cantoneira("1", 100, 65, 6);

            var diferencas = chapa.CompararComObjeto(cantoneira);

            Assert.AreEqual(1, diferencas.Count);
            Assert.AreEqual(EnumErrosPosicao.PecasDiferentes, diferencas[0].TipoErroEnum);
        }

        [TestMethod]
        public void Chapas_Iguais_SemDivergencias()
        {
            var a = ConstrutoresTeste.Chapa("1", 100, 50, 6);
            var b = ConstrutoresTeste.Chapa("1", 100, 50, 6);

            Assert.AreEqual(0, a.CompararComObjeto(b).Count);
        }

        [TestMethod]
        public void Chapas_LarguraDiferente_GeraDivergenciaDeLargura()
        {
            var a = ConstrutoresTeste.Chapa("1", 100, 50, 6);
            var b = ConstrutoresTeste.Chapa("1", 100, 70, 6);

            var diferencas = a.CompararComObjeto(b);

            Assert.IsTrue(diferencas.Any(d => d.TipoErroEnum == EnumErrosPosicao.Largura));
        }

        [TestMethod]
        public void Chapas_ComprimentoMuitoDiferente_GeraDivergenciaDeComprimentoNC()
        {
            var a = ConstrutoresTeste.Chapa("1", 100, 50, 6);
            var b = ConstrutoresTeste.Chapa("1", 110, 50, 6);

            var diferencas = a.CompararComObjeto(b);

            var comprimento = diferencas.FirstOrDefault(d => d.TipoErroEnum == EnumErrosPosicao.Comprimento);
            Assert.IsNotNull(comprimento);
            Assert.IsTrue(comprimento.NcOuMelhoria, "Diferença > 2mm deve ser tratada como NC.");
        }

        [TestMethod]
        public void Cantoneira_ComparadaComChapa_ApontaTipoDiferente()
        {
            var cantoneira = ConstrutoresTeste.Cantoneira("1", 100, 65, 6);
            var chapa = ConstrutoresTeste.Chapa("1", 100, 50, 6);

            var diferencas = cantoneira.CompararComObjeto(chapa);

            Assert.AreEqual(1, diferencas.Count);
            Assert.AreEqual(EnumErrosPosicao.PecasDiferentes, diferencas[0].TipoErroEnum);
        }

        [TestMethod]
        public void Cantoneiras_AbaDiferente_GeraDivergenciaDePerfil()
        {
            var a = ConstrutoresTeste.Cantoneira("1", 100, 65, 6);
            var b = ConstrutoresTeste.Cantoneira("1", 100, 75, 6);

            var diferencas = a.CompararComObjeto(b);

            Assert.IsTrue(diferencas.Any(d => d.TipoErroEnum == EnumErrosPosicao.PerfilCantoneira));
        }
    }
}
