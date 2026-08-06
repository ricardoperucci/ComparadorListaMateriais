using ComparadorListasDeMateriais.ComparadorV2;
using ComparadorListasDeMateriais.ObjetosLista;
using ComparadorListasDeMateriais.Testes.Infra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace ComparadorListasDeMateriais.Testes.Unitarios
{
    /// <summary>
    /// Testes do pareamento de posições — o coração da correção do caso "65H virou 65G".
    /// </summary>
    [TestClass]
    public class PareadorPosicoesTests
    {
        private static List<ObjetoComparacaoLista> Lista(params ObjetoComparacaoLista[] objetos)
        {
            return new List<ObjetoComparacaoLista>(objetos);
        }

        [TestMethod]
        public void Parear_MesmaNumeracao_FormaParExato()
        {
            ResultadoPareamento resultado = PareadorPosicoes.Parear(
                Lista(ConstrutoresTeste.Chapa("65H", 1000, 60, 4.75, "H")),
                Lista(ConstrutoresTeste.Chapa("65H", 1000, 60, 4.75, "H")));

            Assert.AreEqual(1, resultado.Pares.Count);
            Assert.AreEqual(TipoPar.Exato, resultado.Pares[0].Tipo);
            Assert.AreEqual(0, resultado.SomenteOriginal.Count);
            Assert.AreEqual(0, resultado.SomenteNova.Count);
        }

        [TestMethod]
        public void Parear_SoMudouOMaterial_FormaParDeMaterialTrocado()
        {
            ResultadoPareamento resultado = PareadorPosicoes.Parear(
                Lista(ConstrutoresTeste.Chapa("65H", 1000, 60, 4.75, "H")),
                Lista(ConstrutoresTeste.Chapa("65G", 1000, 60, 4.75, "G")));

            Assert.AreEqual(1, resultado.Pares.Count);
            Assert.AreEqual(TipoPar.MaterialTrocado, resultado.Pares[0].Tipo);
            Assert.AreEqual("65H", resultado.Pares[0].Original.NumeracaoComMaterial);
            Assert.AreEqual("65G", resultado.Pares[0].Nova.NumeracaoComMaterial);

            // O ponto do requisito: nada de "posição eliminada" mais "posição nova".
            Assert.AreEqual(0, resultado.SomenteOriginal.Count);
            Assert.AreEqual(0, resultado.SomenteNova.Count);
        }

        [TestMethod]
        public void Parear_PosicaoSemLetraContraPosicaoComSigla_FormaParDeMaterialTrocado()
        {
            ResultadoPareamento resultado = PareadorPosicoes.Parear(
                Lista(ConstrutoresTeste.Chapa("65", 1000, 60, 4.75, "S")),
                Lista(ConstrutoresTeste.Chapa("65H", 1000, 60, 4.75, "H")));

            Assert.AreEqual(1, resultado.Pares.Count);
            Assert.AreEqual(TipoPar.MaterialTrocado, resultado.Pares[0].Tipo);
        }

        [TestMethod]
        public void Parear_DoisCandidatosDeUmLado_NaoPareiaEAvisa()
        {
            ResultadoPareamento resultado = PareadorPosicoes.Parear(
                Lista(ConstrutoresTeste.Chapa("65H", 1000, 60, 4.75, "H"),
                      ConstrutoresTeste.Chapa("65F", 1000, 60, 4.75, "F")),
                Lista(ConstrutoresTeste.Chapa("65G", 1000, 60, 4.75, "G")));

            Assert.AreEqual(0, resultado.Pares.Count);
            Assert.AreEqual(1, resultado.Ambiguos.Count);

            GrupoAmbiguo grupo = resultado.Ambiguos[0];
            Assert.AreEqual("65", grupo.ChaveBase);
            CollectionAssert.AreEquivalent(new[] { "65H", "65F" }, grupo.CandidatosOriginal);
            CollectionAssert.AreEquivalent(new[] { "65G" }, grupo.CandidatosNova);

            // Os candidatos continuam aparecendo como eliminadas/novas, porque é o que eles são.
            Assert.AreEqual(2, resultado.SomenteOriginal.Count);
            Assert.AreEqual(1, resultado.SomenteNova.Count);
        }

        [TestMethod]
        public void Parear_LetraFinalQueNaoESigla_NaoPareia()
        {
            // "65A" e "65B" são posições distintas: A e B não são siglas de material.
            ResultadoPareamento resultado = PareadorPosicoes.Parear(
                Lista(ConstrutoresTeste.Chapa("65A", 1000, 60, 4.75)),
                Lista(ConstrutoresTeste.Chapa("65B", 1000, 60, 4.75)));

            Assert.AreEqual(0, resultado.Pares.Count);
            Assert.AreEqual(0, resultado.Ambiguos.Count);
            Assert.AreEqual("65A", resultado.SomenteOriginal.Single().NumeracaoComMaterial);
            Assert.AreEqual("65B", resultado.SomenteNova.Single().NumeracaoComMaterial);
        }

        [TestMethod]
        public void Parear_PareamentoExatoTemPrioridadeSobreABase()
        {
            // Com 65H nas duas listas, o 65G novo não pode roubar o par do 65H.
            ResultadoPareamento resultado = PareadorPosicoes.Parear(
                Lista(ConstrutoresTeste.Chapa("65H", 1000, 60, 4.75, "H")),
                Lista(ConstrutoresTeste.Chapa("65H", 1000, 60, 4.75, "H"),
                      ConstrutoresTeste.Chapa("65G", 900, 60, 3.0, "G")));

            Assert.AreEqual(1, resultado.Pares.Count);
            Assert.AreEqual(TipoPar.Exato, resultado.Pares[0].Tipo);
            Assert.AreEqual("65G", resultado.SomenteNova.Single().NumeracaoComMaterial);
        }

        [TestMethod]
        public void Parear_SemCorrespondente_ViraEliminadaOuNova()
        {
            ResultadoPareamento resultado = PareadorPosicoes.Parear(
                Lista(ConstrutoresTeste.Chapa("10H", 500, 40, 3.0, "H")),
                Lista(ConstrutoresTeste.Chapa("999G", 500, 40, 3.0, "G")));

            Assert.AreEqual(0, resultado.Pares.Count);
            Assert.AreEqual("10H", resultado.SomenteOriginal.Single().NumeracaoComMaterial);
            Assert.AreEqual("999G", resultado.SomenteNova.Single().NumeracaoComMaterial);
        }

        [TestMethod]
        public void Parear_ListasVazias_NaoQuebra()
        {
            ResultadoPareamento resultado = PareadorPosicoes.Parear(null, null);

            Assert.AreEqual(0, resultado.Pares.Count);
            Assert.AreEqual(0, resultado.SomenteOriginal.Count);
            Assert.AreEqual(0, resultado.SomenteNova.Count);
        }
    }
}
