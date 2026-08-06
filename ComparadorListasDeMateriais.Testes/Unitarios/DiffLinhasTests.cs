using ComparadorListasDeMateriais.ComparadorV2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace ComparadorListasDeMateriais.Testes.Unitarios
{
    /// <summary>
    /// Testes do diff de linhas usado no lado a lado do relatório HTML.
    /// </summary>
    [TestClass]
    public class DiffLinhasTests
    {
        private static List<string> L(params string[] linhas) => new List<string>(linhas);

        [TestMethod]
        public void Comparar_TextosIguais_NaoAcusaDiferenca()
        {
            ResultadoDiffLinhas diff = DiffLinhas.Comparar(L("a", "b", "c"), L("a", "b", "c"));

            Assert.IsFalse(diff.TemDiferenca);
            Assert.AreEqual(3, diff.Iguais);
            Assert.AreEqual(3, diff.Linhas.Count);
        }

        [TestMethod]
        public void Comparar_LinhaAlterada_ApareceLadoALado()
        {
            ResultadoDiffLinhas diff = DiffLinhas.Comparar(L("a", "b", "c"), L("a", "X", "c"));

            Assert.AreEqual(1, diff.Alteradas);
            Assert.AreEqual(0, diff.Inseridas);
            Assert.AreEqual(0, diff.Removidas);

            LinhaDiff alterada = diff.Linhas.Single(x => x.Tipo == TipoLinhaDiff.Alterada);
            Assert.AreEqual("b", alterada.TextoOriginal);
            Assert.AreEqual("X", alterada.TextoNova);
            Assert.AreEqual(2, alterada.NumeroOriginal);
            Assert.AreEqual(2, alterada.NumeroNova);
        }

        [TestMethod]
        public void Comparar_InsercaoNoMeio_NaoDesalinhaOResto()
        {
            // É exatamente aqui que a comparação por índice erra: ela marcaria b, c e d como
            // alteradas em vez de reconhecer uma única inserção.
            ResultadoDiffLinhas diff = DiffLinhas.Comparar(L("a", "b", "c"), L("a", "NOVA", "b", "c"));

            Assert.AreEqual(1, diff.Inseridas);
            Assert.AreEqual(0, diff.Alteradas);
            Assert.AreEqual(0, diff.Removidas);
            Assert.AreEqual(3, diff.Iguais);

            LinhaDiff inserida = diff.Linhas.Single(x => x.Tipo == TipoLinhaDiff.Inserida);
            Assert.AreEqual("NOVA", inserida.TextoNova);
            Assert.AreEqual(0, inserida.NumeroOriginal);
        }

        [TestMethod]
        public void Comparar_RemocaoNoMeio_MarcaSomenteALinhaRemovida()
        {
            ResultadoDiffLinhas diff = DiffLinhas.Comparar(L("a", "b", "c", "d"), L("a", "c", "d"));

            Assert.AreEqual(1, diff.Removidas);
            Assert.AreEqual(0, diff.Alteradas);
            Assert.AreEqual(0, diff.Inseridas);

            LinhaDiff removida = diff.Linhas.Single(x => x.Tipo == TipoLinhaDiff.Removida);
            Assert.AreEqual("b", removida.TextoOriginal);
            Assert.AreEqual(0, removida.NumeroNova);
        }

        [TestMethod]
        public void Comparar_LadoVazio_ListaTudoComoInserido()
        {
            ResultadoDiffLinhas diff = DiffLinhas.Comparar(L(), L("a", "b"));

            Assert.AreEqual(2, diff.Inseridas);
            Assert.AreEqual(0, diff.Iguais);
        }

        [TestMethod]
        public void Comparar_AlteracaoNoFim_MantemNumeracaoCorreta()
        {
            ResultadoDiffLinhas diff = DiffLinhas.Comparar(L("a", "b", "c"), L("a", "b", "Z"));

            LinhaDiff alterada = diff.Linhas.Single(x => x.Tipo == TipoLinhaDiff.Alterada);
            Assert.AreEqual(3, alterada.NumeroOriginal);
            Assert.AreEqual(3, alterada.NumeroNova);
        }

        [TestMethod]
        public void Comparar_ListaGrandeComPoucasMudancas_NaoDegrada()
        {
            var original = new List<string>();
            for (int i = 0; i < 4000; i++)
                original.Add("linha " + i);

            var nova = new List<string>(original);
            nova[2000] = "linha alterada";
            nova.Insert(3000, "linha inserida");

            ResultadoDiffLinhas diff = DiffLinhas.Comparar(original, nova);

            Assert.IsFalse(diff.AlinhamentoDegradado);
            Assert.AreEqual(1, diff.Alteradas);
            Assert.AreEqual(1, diff.Inseridas);
            Assert.AreEqual(0, diff.Removidas);
        }

        [TestMethod]
        public void Comparar_NumeracaoDasLinhasIguaisSegueOsDoisLados()
        {
            ResultadoDiffLinhas diff = DiffLinhas.Comparar(L("a", "b"), L("NOVA", "a", "b"));

            LinhaDiff ultima = diff.Linhas.Last();
            Assert.AreEqual(TipoLinhaDiff.Igual, ultima.Tipo);
            Assert.AreEqual(2, ultima.NumeroOriginal);
            Assert.AreEqual(3, ultima.NumeroNova);
        }
    }
}
