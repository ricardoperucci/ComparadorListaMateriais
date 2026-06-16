using ComparadorListasDeMateriais;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ComparadorListasDeMateriais.Testes.Unitarios
{
    /// <summary>
    /// Testes das funções utilitárias puras (normalização de nomes/posições, etc.).
    /// Não precisam de Excel, HefestoCAM nem arquivos — rodam sempre.
    /// </summary>
    [TestClass]
    public class FuncoesUteisTests
    {
        [TestMethod]
        public void ComparaPosicoes_IgnoraCaixaEEspacos()
        {
            Assert.IsTrue(FuncoesUteis.ComparaPosicoes("1 A", "1a"));
            Assert.IsFalse(FuncoesUteis.ComparaPosicoes("1A", "2A"));
        }

        [TestMethod]
        public void ComparaNomeEstruturas_MesmoNomeComCaixaDiferente_True()
        {
            Assert.IsTrue(FuncoesUteis.ComparaNomeEstruturas("PERNA 12", "perna 12"));
        }

        [TestMethod]
        public void ComparaNomeEstruturas_NomesDiferentes_False()
        {
            Assert.IsFalse(FuncoesUteis.ComparaNomeEstruturas("PERNA 12", "CABECA 5"));
        }

        [TestMethod]
        public void NomeEstruturaMaiusculo_DeixaMMinusculoAposDigito()
        {
            // "abc1m2" -> tudo maiúsculo, mas o 'M' logo após um dígito vira 'm' (ex.: 12m de altura).
            Assert.AreEqual("ABC1m2", FuncoesUteis.NomeEstruturaMaiusculo("abc1m2"));
        }

        [TestMethod]
        public void RemoveAcentos_RemoveAcentuacao()
        {
            Assert.AreEqual("cao", FuncoesUteis.RemoveAcentos("ção"));
        }

        [TestMethod]
        public void RemoveAcentos_ComRemocaoDeHifenEEspacos()
        {
            Assert.AreEqual("abc", FuncoesUteis.RemoveAcentos("a-b c", true));
        }

        [TestMethod]
        public void VerificaMelhoria_UmaDivergenciaDeUmMilimetro_True()
        {
            Assert.IsTrue(FuncoesUteis.VerificaMelhoria(new List<string> { "Comprimento 1mm" }));
        }

        [TestMethod]
        public void VerificaMelhoria_DivergenciaQualquer_False()
        {
            Assert.IsFalse(FuncoesUteis.VerificaMelhoria(new List<string> { "Material alterado" }));
        }

        [TestMethod]
        public void DivideTextoPorLinhas_QuebraPorCRLF_e_LF()
        {
            List<string> linhas = FuncoesUteis.DivideTextoPorLinhas("a\r\nb\nc");
            Assert.AreEqual(3, linhas.Count);
            Assert.AreEqual("a", linhas[0]);
            Assert.AreEqual("b", linhas[1]);
            Assert.AreEqual("c", linhas[2]);
        }
    }
}
