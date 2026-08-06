using ComparadorListasDeMateriais.ComparadorV2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ComparadorListasDeMateriais.Testes.Unitarios
{
    /// <summary>
    /// Testes da regra de sigla de material na numeração da posição, portada do Hefesto
    /// (<c>FuncoesHefesto.RemoveSiglaMaterialDaPosicao</c>): só G, H e F são siglas.
    /// </summary>
    [TestClass]
    public class SiglaMaterialPosicaoTests
    {
        [DataTestMethod]
        [DataRow("65H", "65")]
        [DataRow("105G", "105")]
        [DataRow("70F", "70")]
        [DataRow("65h", "65")]
        [DataRow("105g", "105")]
        public void RemoveSigla_ComSiglaDeMaterial_TiraAUltimaLetra(string posicao, string esperado)
        {
            Assert.AreEqual(esperado, SiglaMaterialPosicao.RemoveSigla(posicao));
        }

        [DataTestMethod]
        [DataRow("65A")]
        [DataRow("65B")]
        [DataRow("65")]
        [DataRow("1050")]
        public void RemoveSigla_SemSiglaDeMaterial_NaoMexeNaNumeracao(string posicao)
        {
            Assert.AreEqual(posicao, SiglaMaterialPosicao.RemoveSigla(posicao));
        }

        [DataTestMethod]
        [DataRow("H")]
        [DataRow("G")]
        [DataRow("F")]
        public void RemoveSigla_UmCaractereSo_NaoViraStringVazia(string posicao)
        {
            // A guarda de tamanho existe no Hefesto justamente para não transformar a posição em nada.
            Assert.AreEqual(posicao, SiglaMaterialPosicao.RemoveSigla(posicao));
        }

        [DataTestMethod]
        [DataRow("65H", "H")]
        [DataRow("105g", "G")]
        [DataRow("70F", "F")]
        [DataRow("65A", "")]
        [DataRow("65", "")]
        public void Sigla_DevolveASiglaEmMaiuscula(string posicao, string esperado)
        {
            Assert.AreEqual(esperado, SiglaMaterialPosicao.Sigla(posicao));
        }

        [TestMethod]
        public void ChaveBase_IgnoraSiglaCaixaEEspacos()
        {
            Assert.AreEqual(SiglaMaterialPosicao.ChaveBase("65H"), SiglaMaterialPosicao.ChaveBase(" 65 g "));
        }

        [TestMethod]
        public void ChaveBase_PosicoesDiferentesNaoColidem()
        {
            Assert.AreNotEqual(SiglaMaterialPosicao.ChaveBase("65H"), SiglaMaterialPosicao.ChaveBase("650H"));
        }
    }
}
