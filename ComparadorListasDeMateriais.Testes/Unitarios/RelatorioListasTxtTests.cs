using ComparadorListasDeMateriais.ObjetosResultados;
using ComparadorListasDeMateriais.ObjetosResultados.ObjetosDivergencias;
using ComparadorListasDeMateriais.Relatorios;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ComparadorListasDeMateriais.Testes.Unitarios
{
    /// <summary>
    /// Testes do renderizador de texto do relatório de listas (função pura, sem Excel/arquivos).
    /// </summary>
    [TestClass]
    public class RelatorioListasTxtTests
    {
        [TestMethod]
        public void Gerar_EstruturaSemErros_TextoComCabecalhoEMensagemDeIgualdade()
        {
            var cabecalho = new ObjetoCabecalho("orig.txt", "nova.txt");
            var resultado = new ObjetoResultadoComparacao(
                cabecalho,
                new List<string>(),
                new List<string>(),
                new List<DivergenciaEntreEstruturaLista>());

            resultado.ListaEstruturasComparadas.Add(new EstruturaComparacao { NomeEstruturaSaida = "ESTR1" });

            string texto = RelatorioListasTxt.Gerar(resultado);

            StringAssert.Contains(texto, "Comparação listas de materiais");
            StringAssert.Contains(texto, "ESTR1");
            StringAssert.Contains(texto, "Todas as posições na estrutura estão iguais entre as listas");
        }

        [TestMethod]
        public void Gerar_ComEstruturasSomenteEmUmaLista_ListaNasSecoesCertas()
        {
            var cabecalho = new ObjetoCabecalho("orig.txt", "nova.txt");
            var resultado = new ObjetoResultadoComparacao(
                cabecalho,
                new List<string> { "PERNA 1" },
                new List<string> { "MENSULA 2" },
                new List<DivergenciaEntreEstruturaLista>());

            string texto = RelatorioListasTxt.Gerar(resultado);

            StringAssert.Contains(texto, "Estruturas que estão na Lista Original e não foram encontradas na Lista Nova:");
            StringAssert.Contains(texto, "PERNA 1");
            StringAssert.Contains(texto, "Estruturas que estão na Lista Nova e não foram encontradas na Lista Original:");
            StringAssert.Contains(texto, "MENSULA 2");
        }
    }
}
