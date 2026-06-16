using ComparadorListasDeMateriais.ComparadorCAM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ComparadorListasDeMateriais.Testes.Unitarios
{
    /// <summary>
    /// Testes do renderizador de texto do relatório CAM (função pura, sem HefestoCAM).
    /// </summary>
    [TestClass]
    public class RelatorioCAMTxtTests
    {
        [TestMethod]
        public void Gerar_TudoPresenteESemDivergencias_TextoPositivo()
        {
            var resultado = new ResultadoComparacaoCAM
            {
                CaminhoPastaA = @"C:\A",
                CaminhoPastaB = @"C:\B"
            };

            string texto = RelatorioCAMTxt.Gerar(resultado);

            StringAssert.Contains(texto, "RELATÓRIO COMPARAÇÂO ARQUIVOS CAM");
            StringAssert.Contains(texto, "Todas posições existem nas duas pastas.");
            StringAssert.Contains(texto, "Todas cantoneiras com aço G nos arquivos da Pasta A");
            StringAssert.Contains(texto, "Todas peças com furo dentro do padrão da fábrica nos arquivos da Pasta B");
        }

        [TestMethod]
        public void Gerar_ComDivergencias_ListaPosicaoEDiferencas()
        {
            var resultado = new ResultadoComparacaoCAM();
            resultado.Divergencias.Add(new DivergenciaPosicaoCAM("5", new List<string> { "Espessura", "Largura" }));

            string texto = RelatorioCAMTxt.Gerar(resultado);

            StringAssert.Contains(texto, "Divergências encontradas entre os arquivos:");
            StringAssert.Contains(texto, "POS. 5: Espessura / Largura");
        }

        [TestMethod]
        public void Gerar_ComPosicoesFaltando_ListaNosBlocosCertos()
        {
            var resultado = new ResultadoComparacaoCAM();
            resultado.PosicoesFaltandoNaA.Add("10");
            resultado.PosicoesFaltandoNaB.Add("20");

            string texto = RelatorioCAMTxt.Gerar(resultado);

            StringAssert.Contains(texto, "Posições não encontradas nos arquivos da Pasta A");
            StringAssert.Contains(texto, "Posições não encontradas nos arquivos da Pasta B");
        }

        [TestMethod]
        public void Gerar_ComFurosForaDoPadrao_ListaNaPastaCorreta()
        {
            var resultado = new ResultadoComparacaoCAM();
            resultado.FurosForaPadraoNaA.Add("12 (99,9)");
            resultado.CantoneirasSemAcoGNaB.Add("7");

            string texto = RelatorioCAMTxt.Gerar(resultado);

            StringAssert.Contains(texto, "Peças com furo diferente do padrão da fábrica nos arquivos da Pasta A");
            StringAssert.Contains(texto, "Cantoneiras sem aço G nos arquivos da Pasta B");
        }
    }
}
