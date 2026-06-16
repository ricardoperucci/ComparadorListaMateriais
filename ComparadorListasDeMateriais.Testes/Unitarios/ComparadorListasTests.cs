using ComparadorListasDeMateriais;
using ComparadorListasDeMateriais.ObjetosLista;
using ComparadorListasDeMateriais.ObjetosResultados;
using ComparadorListasDeMateriais.Testes.Infra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace ComparadorListasDeMateriais.Testes.Unitarios
{
    /// <summary>
    /// Testes da orquestração de comparação de listas usando a sobrecarga testável
    /// <c>CompararListas(dic1, dic2, cabecalho, out resultado)</c> — em memória, sem Excel/txt.
    /// </summary>
    [TestClass]
    public class ComparadorListasTests
    {
        private static ObjetoCabecalho Cabecalho() => new ObjetoCabecalho("orig.txt", "nova.txt");

        [TestMethod]
        public void DuasListas_ComprimentoDiferente_GeraDivergenciaNaPosicao()
        {
            var lista1 = ConstrutoresTeste.Estrutura("ESTR1", ConstrutoresTeste.Chapa("1", 100, 50, 6));
            var lista2 = ConstrutoresTeste.Estrutura("ESTR1", ConstrutoresTeste.Chapa("1", 110, 50, 6));

            var comparador = new Comparador();
            comparador.CompararListas(lista1, lista2, Cabecalho(), out ObjetoResultadoComparacao resultado);

            Assert.AreEqual(1, resultado.ListaEstruturasComparadas.Count);
            EstruturaComparacao estrutura = resultado.ListaEstruturasComparadas[0];
            Assert.AreEqual(1, estrutura.ListaPosicoesComErros.Count);
            Assert.IsTrue(estrutura.ListaPosicoesComErros[0].ListaErrosPosicao
                .Any(e => e.TipoErroEnum == EnumErrosPosicao.Comprimento));
        }

        [TestMethod]
        public void PosicaoSomenteNaOriginal_ListadaComoSomenteOriginal()
        {
            var lista1 = ConstrutoresTeste.Estrutura("ESTR1",
                ConstrutoresTeste.Chapa("1", 100, 50, 6),
                ConstrutoresTeste.Chapa("2", 100, 50, 6));
            var lista2 = ConstrutoresTeste.Estrutura("ESTR1",
                ConstrutoresTeste.Chapa("1", 100, 50, 6));

            var comparador = new Comparador();
            comparador.CompararListas(lista1, lista2, Cabecalho(), out ObjetoResultadoComparacao resultado);

            EstruturaComparacao estrutura = resultado.ListaEstruturasComparadas[0];
            CollectionAssert.Contains(estrutura.PosicoesSomenteListaOriginal, "2");
        }

        [TestMethod]
        public void EstruturaSomenteNaNova_DetectadaComoNaoEncontrada()
        {
            var lista1 = ConstrutoresTeste.Estrutura("ESTR1", ConstrutoresTeste.Chapa("1", 100, 50, 6));

            var lista2 = ConstrutoresTeste.Estrutura("ESTR1", ConstrutoresTeste.Chapa("1", 100, 50, 6));
            lista2.Add("ESTR2", new List<ObjetoComparacaoLista> { ConstrutoresTeste.Chapa("9", 100, 50, 6) });

            var comparador = new Comparador();
            comparador.CompararListas(lista1, lista2, Cabecalho(), out ObjetoResultadoComparacao resultado);

            Assert.IsTrue(resultado.EstruturasSomenteListaNova.Any(e => e.Contains("ESTR2")));
        }

        [TestMethod]
        public void ListasIdenticas_NaoGeramErrosDePosicao()
        {
            var lista1 = ConstrutoresTeste.Estrutura("ESTR1", ConstrutoresTeste.Chapa("1", 100, 50, 6));
            var lista2 = ConstrutoresTeste.Estrutura("ESTR1", ConstrutoresTeste.Chapa("1", 100, 50, 6));

            var comparador = new Comparador();
            comparador.CompararListas(lista1, lista2, Cabecalho(), out ObjetoResultadoComparacao resultado);

            Assert.AreEqual(0, resultado.ListaEstruturasComparadas[0].ListaPosicoesComErros.Count);
        }
    }
}
